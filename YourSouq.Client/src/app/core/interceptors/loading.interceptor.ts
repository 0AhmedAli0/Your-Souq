import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { delay, finalize, identity } from 'rxjs';
import { BusyService } from '../services/busy.service';
import { environment } from '../../../environments/environment';

export const loadingInterceptor: HttpInterceptorFn = (req, next) => {
  const busyService = inject(BusyService);
  busyService.busy();
  return next(req).pipe(
    //use pipe to provide a rxjs operators
    environment.production ? identity : delay(500), // Simulate a delay of 1 second for loading effect
    finalize(
      // finalize is used to execute some code after the observable completes, regardless of whether it was successful or errored
      () => busyService.idle()
    )
  );
};
