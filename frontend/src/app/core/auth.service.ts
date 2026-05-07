import { Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { environment } from '../../environments/environment';
import { tap } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly accessTokenKey = 'lottery_access_token';
  private readonly refreshTokenKey = 'lottery_refresh_token';
  private readonly roleKey = 'lottery_role';

  readonly isLoggedIn = signal(!!localStorage.getItem(this.accessTokenKey));

  constructor(private http: HttpClient, private router: Router) {}

  login(username: string, password: string) {
    return this.http.post<any>(`${environment.apiUrl}/auth/login`, { username, password }).pipe(
      tap((r) => this.persistTokens(r?.Data ?? r?.data))
    );
  }

  refreshToken() {
    const accessToken = this.getAccessToken();
    const refreshToken = this.getRefreshToken();
    if (!accessToken || !refreshToken) {
      throw new Error('Missing tokens');
    }

    return this.http.post<any>(`${environment.apiUrl}/auth/refresh`, { accessToken, refreshToken }).pipe(
      tap((r) => this.persistTokens(r?.Data ?? r?.data))
    );
  }

  logout() {
    this.http.post(`${environment.apiUrl}/auth/logout`, {}).subscribe({ error: () => undefined });
    localStorage.removeItem(this.accessTokenKey);
    localStorage.removeItem(this.refreshTokenKey);
    localStorage.removeItem(this.roleKey);
    this.isLoggedIn.set(false);
    this.router.navigate(['/auth/login']);
  }

  getAccessToken() {
    return localStorage.getItem(this.accessTokenKey);
  }

  getRefreshToken() {
    return localStorage.getItem(this.refreshTokenKey);
  }

  getRole() {
    return localStorage.getItem(this.roleKey);
  }

  private persistTokens(payload: any) {
    if (!payload) return;
    localStorage.setItem(this.accessTokenKey, payload.AccessToken ?? payload.accessToken);
    localStorage.setItem(this.refreshTokenKey, payload.RefreshToken ?? payload.refreshToken);
    localStorage.setItem(this.roleKey, payload.Role ?? payload.role ?? 'Staff');
    this.isLoggedIn.set(true);
  }
}
