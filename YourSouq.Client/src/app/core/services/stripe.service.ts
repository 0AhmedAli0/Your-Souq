import { inject, Injectable } from '@angular/core';
import {
  ConfirmationToken,
  loadStripe,
  Stripe,
  StripeAddressElement,
  StripeAddressElementOptions,
  StripeElements,
  StripePaymentElement,
} from '@stripe/stripe-js';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { CartService } from './cart.service';
import { Cart } from '../../shared/models/cart';
import { firstValueFrom, map } from 'rxjs';
import { AccountService } from './account.service';
@Injectable({
  providedIn: 'root',
})
export class StripeService {
  // we need single instance of stripe to work with it to avoid any problem when use multiple instances
  private stripePromise?: Promise<Stripe | null>;
  private http = inject(HttpClient);
  baseUrl = environment.apiUrl;
  private cartService = inject(CartService);
  private accountService = inject(AccountService);
  private elements?: StripeElements;
  private addressElement?: StripeAddressElement;
  private paymentElement?: StripePaymentElement;

  constructor() {
    // Initialize Stripe with your publishable key
    this.stripePromise = loadStripe(environment.stripePublicKey);
  }

  getStripeInstance() {
    return this.stripePromise;
  }

  //نحصل علي وظائف معينه لا تظهر الا اذا كنا نستخدم نفس مثيل العناصر لاشياء مثل عنوانناوبطاقه الدفع الخاصه بنا
  // لذالك نحن بحاجه لأستخدام نفس المثيل للأخزاء المختلفه من عمليه الدفع
  async initializeStripeElements() {
    if (!this.elements) {
      const stripe = await this.getStripeInstance();
      if (stripe) {
        const cart = await firstValueFrom(this.createOrUpdatePaymentIntent());
        this.elements = stripe.elements({
          clientSecret: cart.clientSecret,
          appearance: { labels: 'floating' }, // Customize the appearance of the elements لتحديد مظهر العناصر في html
        });
      } else {
        throw new Error('Stripe instance is not available');
      }
    }
    return this.elements;
  }

  async createPaymentElement() {
    if (!this.paymentElement) {
      const elements = await this.initializeStripeElements();
      if (elements) {
        this.paymentElement = elements.create('payment');
      } else {
        throw new Error('Stripe elements are not initialized');
      }
    }
    return this.paymentElement;
  }

  async createAddressElement() {
    if (!this.addressElement) {
      const elements = await this.initializeStripeElements();
      if (elements) {
        const user = this.accountService.currentUser();
        let defaultValues: StripeAddressElementOptions['defaultValues'] = {};

        if (user) {
          defaultValues.name = user.firstName + ' ' + user.lastName;
        }

        if (user?.address) {
          defaultValues.address = {
            line1: user.address.line1,
            line2: user.address.line2,
            city: user.address.city,
            state: user.address.state,
            country: user.address.country,
            postal_code: user.address.postalCode,
          };
        }

        const options: StripeAddressElementOptions = {
          mode: 'shipping',
          defaultValues,
        };
        this.addressElement = elements.create('address', options);
      } else {
        throw new Error('Stripe elements are not initialized');
      }
    }
    return this.addressElement;
  }

  async createConfirmationToken() {
    const stripe = await this.getStripeInstance();
    const elements = await this.initializeStripeElements();
    if (!elements) throw new Error('Stripe elements are not initialized');
    const result = await elements.submit();
    if (result.error) {
      throw new Error(
        result.error.message || 'Error creating confirmation token'
      );
    }
    if (stripe) {
      return await stripe.createConfirmationToken({ elements });
    } else {
      throw new Error('Stripe instance is not available');
    }
  }

  async confirmPayment(confirmationToken: ConfirmationToken) {
    const stripe = await this.getStripeInstance();
    const elements = await this.initializeStripeElements();
    const result = await elements.submit();
    if (result.error) throw new Error(result.error.message);

    const clientSecret = this.cartService.cart()?.clientSecret;
    if (stripe && clientSecret) {
      return await stripe.confirmPayment({
        clientSecret: clientSecret,
        confirmParams: {
          confirmation_token: confirmationToken.id,
        },
        redirect: 'if_required', // Redirect if required by the payment method
      });
    } else {
      throw new Error('Stripe instance or client secret is not available');
    }
  }

  createOrUpdatePaymentIntent() {
    const cart = this.cartService.cart();
    if (!cart) throw new Error('problem with Cart');

    return this.http.post<Cart>(this.baseUrl + 'payments/' + cart.id, {}).pipe(
      map((cart) => {
        this.cartService.setCart(cart); // Update the cart signal with the new value
        return cart;
      })
    );
  }

  //تعيين هذه العناصر وأعادتها بشكل فعال الي كونها غير محدده
  disposeElements() {
    this.elements = undefined;
    this.addressElement = undefined;
    this.paymentElement = undefined;
  }
}
