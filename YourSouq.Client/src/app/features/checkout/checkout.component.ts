import { Component, inject, OnDestroy, OnInit, signal } from '@angular/core';
import { OrderSummaryComponent } from '../../shared/components/order-summary/order-summary.component';
import { MatStepper, MatStepperModule } from '@angular/material/stepper';
import { MatButton } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { Router, RouterLink } from '@angular/router';
import { StripeService } from '../../core/services/stripe.service';
import {
  ConfirmationToken,
  StripeAddressElement,
  StripeAddressElementChangeEvent,
  StripePaymentElement,
  StripePaymentElementChangeEvent,
} from '@stripe/stripe-js';
import { SnackbarService } from '../../core/services/snackbar.service';
import {
  MatCheckboxChange,
  MatCheckboxModule,
} from '@angular/material/checkbox';
import { StepperSelectionEvent } from '@angular/cdk/stepper';
import { Address } from '../../shared/models/user';
import { AccountService } from '../../core/services/account.service';
import { firstValueFrom } from 'rxjs';
import { CheckoutDeliveryComponent } from './checkout-delivery/checkout-delivery.component';
import { CheckoutReviewComponent } from './checkout-review/checkout-review.component';
import { CartService } from '../../core/services/cart.service';
import { CurrencyPipe, JsonPipe } from '@angular/common';
import { OrderToCreate, ShippingAddress } from '../../shared/models/order';
import { OrderService } from '../../core/services/order.service';

@Component({
  selector: 'app-checkout',
  standalone: true,
  imports: [
    OrderSummaryComponent,
    MatStepperModule,
    MatButton,
    RouterLink,
    MatCheckboxModule,
    CheckoutDeliveryComponent,
    CheckoutReviewComponent,
    CurrencyPipe,
    MatProgressSpinnerModule,
  ],
  templateUrl: './checkout.component.html',
  styleUrl: './checkout.component.scss',
})
export class CheckoutComponent implements OnInit, OnDestroy {
  private stripeService = inject(StripeService);
  private snackbar = inject(SnackbarService);
  private router = inject(Router);
  private accountService = inject(AccountService);
  private orderService = inject(OrderService);
  cartService = inject(CartService);
  addressElement?: StripeAddressElement;
  paymentElement?: StripePaymentElement;
  saveAddress = false;
  completionStatus = signal<{
    address: boolean;
    card: boolean;
    delivery: boolean;
  }>({
    address: false,
    card: false,
    delivery: false,
    // delivery: this.cartService.cart()?.deliveryMethodId !== null,
  });

  ConfirmationToken?: ConfirmationToken; //لكي يتمكن المستخدم من الدفع في خطوه لا تحتوي علي عنصر الدفع
  loading = false; // Flag to indicate loading state

  async ngOnInit() {
    try {
      this.addressElement = await this.stripeService.createAddressElement();
      this.addressElement?.mount('#address-element'); // the address element to the DOM لتركيب
      this.addressElement?.on('change', this.handelAddressChange);

      this.paymentElement = await this.stripeService.createPaymentElement();
      this.paymentElement?.mount('#payment-element'); // the payment element to the DOM لتركيب
      this.paymentElement.on('change', this.handelPaymentChange);
    } catch (error: any) {
      this.snackbar.error(error.message);
    }
  }
  handelPaymentChange = (event: StripePaymentElementChangeEvent) => {
    this.completionStatus.update((state) => {
      state.card = event.complete;
      return state;
    });
  };

  handelAddressChange = (event: StripeAddressElementChangeEvent) => {
    //this sintax means that the function is binded to the class instance
    this.completionStatus.update((state) => {
      state.address = event.complete;
      return state;
    });
  };

  handelDeliveryChange(event: boolean) {
    this.completionStatus.update((state) => {
      state.delivery = event;
      return state;
    });
  }

  async getConfirmationToken() {
    try {
      if (
        Object.values(this.completionStatus()).every(
          (status) => status === true
        )
      ) {
        const result = await this.stripeService.createConfirmationToken();
        if (result.error) throw new Error(result.error.message);
        this.ConfirmationToken = result.confirmationToken;
        console.log('Confirmation Token:', this.ConfirmationToken);
      }
    } catch (error: any) {
      this.snackbar.error(error.message);
    }
  }

  async onStepChange(event: StepperSelectionEvent) {
    if (event.selectedIndex === 1) {
      if (this.saveAddress) {
        const address = (await this.getAddressFromStripeAddress()) as Address;
        address && firstValueFrom(this.accountService.updateAddress(address));
      }
    }
    if (event.selectedIndex === 2) {
      await firstValueFrom(this.stripeService.createOrUpdatePaymentIntent());
    }
    if (event.selectedIndex === 3) {
      await this.getConfirmationToken();
    }
  }

  async confirmPayment(stepper: MatStepper) {
    this.loading = true; // Set loading state to true
    try {
      if (this.ConfirmationToken) {
        const result = await this.stripeService.confirmPayment(
          this.ConfirmationToken
        );

        if (result.paymentIntent?.status === 'succeeded') {
          //if payment was successful create the order
          const order = await this.createOrderModel();
          const orderResult = await firstValueFrom(
            this.orderService.createOrder(order)
          );
          if (orderResult) {
            this.orderService.orderComplete = true; // Set order complete status
            this.cartService.deleteCart(); // Clear the cart after successful payment
            this.cartService.selectedDelivery.set(null); // Clear the selected delivery method
            this.router.navigateByUrl('/checkout/success'); // Navigate to success page
          } else {
            throw new Error('Failed to create order');
          }
        } else if (result.error) {
          throw new Error(
            result.error.message || 'Payment confirmation failed'
          );
        } else {
          throw new Error('something went wrong with the payment confirmation');
        }
      }
    } catch (error: any) {
      this.snackbar.error(error.message || 'Error confirming payment');
      stepper.previous(); // Go back to the previous step if there's an error
    } finally {
      this.loading = false; // Reset loading state
    }
  }

  private async createOrderModel(): Promise<OrderToCreate> {
    const cart = this.cartService.cart();
    const shippingAddress =
      (await this.getAddressFromStripeAddress()) as ShippingAddress;
    const card = this.ConfirmationToken?.payment_method_preview.card;

    if (!cart?.id || !cart.deliveryMethodId || !shippingAddress || !card) {
      throw new Error('Missing required information to create order');
    }

    return {
      cartId: cart.id,
      paymentSummary: {
        last4: +card.last4,
        brand: card.brand,
        expMonth: card.exp_month,
        expYear: card.exp_year,
      },
      deliveryMethodId: cart.deliveryMethodId,
      shippingAddress,
    };
  }

  private async getAddressFromStripeAddress(): Promise<
    Address | ShippingAddress | null
  > {
    const result = await this.addressElement?.getValue();
    const address = result?.value.address;

    if (address) {
      return {
        name: result?.value.name,
        line1: address.line1,
        line2: address.line2 || undefined,
        city: address.city,
        country: address.country,
        state: address.state,
        postalCode: address.postal_code,
      };
    } else return null;
  }

  onSaveAddressCheckboxChange(event: MatCheckboxChange) {
    this.saveAddress = event.checked;
  }

  ngOnDestroy(): void {
    this.stripeService.disposeElements(); // Dispose of the Stripe elements when the component is destroyed
  }
}
