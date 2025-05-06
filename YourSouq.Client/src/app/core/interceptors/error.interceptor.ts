import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject, Inject } from '@angular/core';
import { NavigationExtras, Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';
import { SnackbarService } from '../services/snackbar.service';

// This interceptor handles errors that occur during HTTP requests
// we need to tell angular to use this interceptor in the app.config file ( provideHttpClient(withInterceptors([errorInterceptor])))
export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router); // Inject the Router service to send the user to different page
  const snackbar = inject(SnackbarService); // Inject the SnackbarService to show error messages

  return next(req).pipe(
    //use pipe not subscribe because i need to Intercept the request and handle errors
    //subscribe is used to execute the observable and get the response

    // rxjs operator to Catch any errors that occur during the request
    catchError((error: HttpErrorResponse) => {
      // Check if the error is an instance of HttpErrorResponse
      if (error.status === 400) {
        if (error.error.errors) {
          const modelStateErrors = [];
          for (const key in error.error.errors) {
            if (error.error.errors[key]) {
              modelStateErrors.push(error.error.errors[key]);
            }
          }
          throw modelStateErrors.flat(); // flatten the array of errors
        } else {
          snackbar.error(error.error.title || error.error);
        }
      }
      if (error.status === 401) {
        // Redirect to login page or show a message
        // router.navigate(['/auth/login']);
        snackbar.error(error.error.title || error.error);
      }
      if (error.status === 404) {
        // Handle not found error
        router.navigateByUrl('/not-found');
      }
      if (error.status === 500) {
        // Handle server error
        const navigationExtras: NavigationExtras = {
          state: { error: error.error }, // pass the error to the server error page with url
        };
        router.navigateByUrl('/server-error', navigationExtras);
      }
      return throwError(() => error); // will throw the error to the next interceptor or to the component that called the request
      // and will navigate to the error page
    })
  );
};
