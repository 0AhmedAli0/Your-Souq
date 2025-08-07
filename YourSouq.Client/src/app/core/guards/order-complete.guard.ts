import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { OrderService } from '../services/order.service';

export const orderCompleteGuard: CanActivateFn = (route, state) => {
  const orderService = inject(OrderService);
  const router = inject(Router);
  if (orderService.orderComplete) {
    return true; // Allow navigation if order is complete
  } else {
    router.navigate(['/shop']); // Redirect to checkout if order is not complete
    return false; // Prevent navigation
  }
};
