import { inject, Injectable } from '@angular/core';
import { CartService } from './cart.service';
import { of } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class InitService {
  private cartService = inject(CartService);

  // This service is used to initialize the application
  //app initializer will wait for this service to finish before loading the app so that this function should return an observable
  init() {
    const cartId = localStorage.getItem('cart_id');
    const cart$ = cartId ? this.cartService.getCart(cartId) : of(null); //use of function from rxjs to return an observable

    return cart$;
  }
}
