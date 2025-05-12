import {
  APP_INITIALIZER,
  ApplicationConfig,
  provideZoneChangeDetection,
} from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { errorInterceptor } from './core/interceptors/error.interceptor';
import { loadingInterceptor } from './core/interceptors/loading.interceptor';
import { InitService } from './core/services/init.service';
import { lastValueFrom } from 'rxjs';

function initializeApp(initService: InitService) {
  // this function will be called before the app is loaded
  // it will wait for the initService to finish before loading the app
  return () =>
    lastValueFrom(initService.init()).finally(() => {
      // this will return a promise that will be resolved when the initService is finished
      const splash = document.getElementById('initial-splash');
      if (splash) {
        splash.remove(); // remove the splash screen >> سيقوم بحذف الشاشة المؤقتة
      }
    });
}

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideAnimationsAsync(),
    provideHttpClient(withInterceptors([errorInterceptor, loadingInterceptor])),
    {
      //configuration for app initializer
      provide: APP_INITIALIZER,
      useFactory: initializeApp,
      multi: true,
      deps: [InitService], // this will inject the initService into the function
    },
  ],
};
