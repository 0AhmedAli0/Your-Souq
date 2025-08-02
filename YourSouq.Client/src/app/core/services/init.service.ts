import { inject, Injectable } from '@angular/core';
import { CartService } from './cart.service';
import { forkJoin, of, tap } from 'rxjs';
import { AccountService } from './account.service';
import { SignalrService } from './signalr.service';

@Injectable({
  providedIn: 'root',
})
export class InitService {
  private cartService = inject(CartService);
  private accountService = inject(AccountService);
  private signalrService = inject(SignalrService);

  // This service is used to initialize the application
  //app initializer will wait for this service to finish before loading the app so that this function should return an observable
  init() {
    const cartId = localStorage.getItem('cart_id');
    const cart$ = cartId ? this.cartService.getCart(cartId) : of(null); //use of function from rxjs to return an observable

    return forkJoin({
      //forkJoin is used to combine multiple observables into one observable and wait for all of them to finish
      cart: cart$,
      user: this.accountService.getUserInfo().pipe(
        //use tap to do side effects without changing the data
        tap((user) => {
          if (user) {
            this.signalrService.CreateHubConnection(); // Create SignalR connection if user is logged in
          }
        })
      ),
    });
  }
}
