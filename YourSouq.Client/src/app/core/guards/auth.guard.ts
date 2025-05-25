import { CanActivateFn, Router } from '@angular/router';
import { AccountService } from '../services/account.service';
import { inject } from '@angular/core';
import { map, of } from 'rxjs';

export const authGuard: CanActivateFn = (route, state) => {
  const accountService = inject(AccountService);
  const router = inject(Router);

  if (accountService.currentUser()) {
    // because currentUser() is a signal, it work Synchronous >> والبتالي لا يقوم بالانتظار حتي يتم تسجيل الدخول
    //so that we need to work with observable to make guard wait until the Async method is completed
    return of(true);
  }
  // If the user is not authenticated, redirect to the login page
  else {
    return accountService.getAuthState().pipe(
      //because we are inside a guard, we don't need to subscribe to the observable, it will be done automatically
      map((auth) => {
        if (auth.isAuthenticated) {
          return true; // If the user is authenticated, allow access
        }
        // If the user is not authenticated, redirect to the login page
        else {
          router.navigate(['/account/login'], {
            queryParams: { returnUrl: state.url },
          }); //بمجرد تسجيل الدخول، سيتم إعادة توجيه المستخدم إلى الصفحة التي حاول الوصول إليها
          return false;
        }
      })
    );
  }
};
