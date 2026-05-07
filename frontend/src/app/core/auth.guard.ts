import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from './auth.service';

export const authGuard: CanActivateFn = (route) => {
  const auth = inject(AuthService);
  const router = inject(Router);

  const token = auth.getAccessToken();
  if (!token) {
    router.navigate(['/auth/login']);
    return false;
  }

  const allowedRoles = route.data?.['roles'] as string[] | undefined;
  if (allowedRoles?.length) {
    const role = auth.getRole();
    if (!role || !allowedRoles.includes(role)) {
      router.navigate(['/dashboard']);
      return false;
    }
  }

  return true;
};
