import { CanActivateFn, Router } from '@angular/router';
import { SnackbarService } from '../services/snackbar.service';
import { CartService } from '../services/cart.service';
import { inject } from '@angular/core';

export const emptyCartGuard: CanActivateFn = (route, state) => {
  const cartService = inject(CartService);
  const Snackbar = inject(SnackbarService);
  const router = inject(Router);

  if (!cartService.cart() || cartService.cart()?.items.length === 0) {
    Snackbar.error(
      'Your cart is empty. Please add items to your cart before check out.'
    );
    router.navigateByUrl('/cart');
    return false; // If the cart is empty, block access
  }
  return true;
};
