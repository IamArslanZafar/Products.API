import { HttpClient } from '@angular/common/http';
import { Injectable, computed, signal } from '@angular/core';
import { Observable, tap } from 'rxjs';
import { API_BASE_URL } from './api-config';

export interface AuthResponse {
  token: string;
  expiresAtUtc: string;
}

export interface UserResponse {
  id: string;
  email: string;
}

export interface Credentials {
  email: string;
  password: string;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  // Held in memory only (not localStorage/sessionStorage) so a successful XSS
  // injection can't read the token off disk — it disappears on full page reload.
  private readonly token = signal<string | null>(null);

  readonly isAuthenticated = computed(() => this.token() !== null);

  constructor(private readonly http: HttpClient) {}

  get currentToken(): string | null {
    return this.token();
  }

  // Registration only creates the account — it never returns a token and never
  // signs the user in. The caller should send them to the login form afterward.
  register(credentials: Credentials): Observable<UserResponse> {
    return this.http.post<UserResponse>(`${API_BASE_URL}/auth/register`, credentials);
  }

  login(credentials: Credentials): Observable<AuthResponse> {
    return this.http
      .post<AuthResponse>(`${API_BASE_URL}/auth/login`, credentials)
      .pipe(tap((response) => this.token.set(response.token)));
  }

  logout(): void {
    this.token.set(null);
  }
}
