import { Component, inject, OnInit, signal } from '@angular/core';
import { CheckoutService } from '../../../core/services/checkout.service';
import { MatRadioModule } from '@angular/material/radio';
import { CurrencyPipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CartService } from '../../../core/services/cart.service';
import { DeliveryMethod } from '../../../shared/models/deliveryMethods';

@Component({
  selector: 'app-checkout-delivery',
  standalone: true,
  imports: [MatRadioModule, CurrencyPipe, FormsModule],
  templateUrl: './checkout-delivery.component.html',
  styleUrl: './checkout-delivery.component.scss',
})
export class CheckoutDeliveryComponent implements OnInit {
  checkoutService = inject(CheckoutService);
  cartService = inject(CartService);
  selectedDeliveryMethod: DeliveryMethod | null = null;

  ngOnInit(): void {
    this.checkoutService.getDeliveryMethods().subscribe({
      next: (methods) => {
        if (this.cartService.cart()?.deliveryMethodId) {
          this.selectedDeliveryMethod =
            methods.find(
              (m) => m.id === this.cartService.cart()?.deliveryMethodId
            ) ?? null;
          if (this.selectedDeliveryMethod) {
            this.cartService.selectedDelivery.set(this.selectedDeliveryMethod);
          }
        }
      },
    });
  }

  onDeliveryMethodChange(event: any) {
    this.cartService.selectedDelivery.set(this.selectedDeliveryMethod);
    // أو
    // this.cartService.totals()?.shipping.set(event.target.value.price ?? 0);
    const cart = this.cartService.cart();
    if (cart) {
      cart.deliveryMethodId = this.selectedDeliveryMethod?.id;
      this.cartService.setCart(cart);
    }
  }
}
