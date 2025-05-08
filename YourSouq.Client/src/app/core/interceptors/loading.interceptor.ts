import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { delay, finalize } from 'rxjs';
import { BusyService } from '../services/busy.service';

export const loadingInterceptor: HttpInterceptorFn = (req, next) => {
  const busyService = inject(BusyService);
  busyService.busy();
  return next(req).pipe(
    //use pipe to provide a rxjs operators
    delay(500), // Simulate a delay of 1 second for loading effect
    finalize(
      // finalize is used to execute some code after the observable completes, regardless of whether it was successful or errored
      () => busyService.idle()
    )
  );
};
