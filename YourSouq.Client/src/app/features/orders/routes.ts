import { Routes } from '@angular/router';
import { authGuard } from '../../core/guards/auth.guard';
import { OrderDetailedComponent } from './order-detailed/order-detailed.component';
import { OrderComponent } from './order.component';

export const ordersRoutes: Routes = [
  {
    path: '',
    component: OrderComponent,
    canActivate: [authGuard],
  },
  {
    path: ':id',
    component: OrderDetailedComponent,
    canActivate: [authGuard],
  },
];
