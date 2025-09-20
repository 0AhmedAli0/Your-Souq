import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AccountService } from '../services/account.service';
import { SnackbarService } from '../services/snackbar.service';

export const adminGuard: CanActivateFn = (route, state) => {
  const accountService = inject(AccountService);
  const snackbarService = inject(SnackbarService);
  const router = inject(Router);
  if (!accountService.isAdmin()) {
    snackbarService.error('You do not have permission to access this area');
    router.navigateByUrl('/');
    return false;
  }
  return true;
};
