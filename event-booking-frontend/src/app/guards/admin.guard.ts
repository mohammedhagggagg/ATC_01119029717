import { inject } from "@angular/core";
import { CanActivateFn, Router } from "@angular/router";
import { CookieService } from "ngx-cookie-service";
import { map, Observable, of } from "rxjs";
import { AuthService } from "../services/auth.service";

export const adminGuard: CanActivateFn = (route, state): Observable<boolean> => {
  const router = inject(Router);
  const cookieService = inject(CookieService);
  const authService = inject(AuthService);

  if (typeof window === 'undefined') {
    return of(false);
  }

  const storedData = cookieService.get('userData');
  if (storedData) {
    const parsedData = JSON.parse(storedData);
    if (parsedData.token) {
      return authService.userData.pipe(
        map(user => {
          console.log('User in Guard:', user);
          console.log('Role in Guard:', user?.role);
          if (user && user.role && user.role === 'Admin') {
            console.log('Access Granted to Admin Panel');
            return true;
          } else {
            console.log('Access Denied: Not an Admin');
            setTimeout(() => {
              router.navigate(['/home']);
            }, 0);
            return false;
          }
        })
      );
    }
  }

  console.log('No Token Found, Redirecting to Home');
  setTimeout(() => {
    router.navigate(['/home']);
  }, 0);
  return of(false);
};