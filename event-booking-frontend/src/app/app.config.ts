import { ApplicationConfig, importProvidersFrom, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import { HttpClientModule, provideHttpClient } from '@angular/common/http';
import { CookieService } from 'ngx-cookie-service';
import { AuthService } from './services/auth.service';
 import { provideToastr } from 'ngx-toastr'; 
 import { provideAnimations } from '@angular/platform-browser/animations';
export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }), provideRouter(routes),
    provideHttpClient(),
    importProvidersFrom(HttpClientModule),
    CookieService,
    {
      provide: AuthService,
      useClass: AuthService,
    },
   provideAnimations(), provideToastr({ timeOut: 3000, positionClass: 'toast-top-right', preventDuplicates: true, progressBar: true }) 
  ]
};
