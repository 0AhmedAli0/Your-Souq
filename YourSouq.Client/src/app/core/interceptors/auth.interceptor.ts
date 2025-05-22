import { HttpInterceptorFn } from '@angular/common/http';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  //this interceptor to sent cookie with each request to server if we have it
  const clonedRequest = req.clone({
    withCredentials: true,
  });
  return next(clonedRequest);
};
