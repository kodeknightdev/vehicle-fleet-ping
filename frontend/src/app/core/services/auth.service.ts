import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { tap } from 'rxjs/operators';

import { ApiService } from './api.service';

export interface CurrentUser {
  username: string;
  displayName: string;
}

const TOKEN_KEY = 'fp.auth';
const USER_KEY = 'fp.user';

@Injectable({
  providedIn: 'root'
})
export class AuthService extends ApiService {
  private userSubject = new BehaviorSubject<CurrentUser | null>(this.readUser());
  readonly user$ = this.userSubject.asObservable();

  constructor(http: HttpClient) {
    super(http);
  }

  get token(): string | null {
    return sessionStorage.getItem(TOKEN_KEY);
  }

  get isLoggedIn(): boolean {
    return !!this.token;
  }

  login(username: string, password: string): Observable<CurrentUser> {
    const token = btoa(`${username}:${password}`);
    const headers = new HttpHeaders({ Authorization: `Basic ${token}` });

    return this.http.get<CurrentUser>(this.url('auth/me'), { headers }).pipe(
      tap(user => {
        sessionStorage.setItem(TOKEN_KEY, token);
        sessionStorage.setItem(USER_KEY, JSON.stringify(user));
        this.userSubject.next(user);
      })
    );
  }

  logout(): void {
    sessionStorage.removeItem(TOKEN_KEY);
    sessionStorage.removeItem(USER_KEY);
    this.userSubject.next(null);
  }

  private readUser(): CurrentUser | null {
    const raw = sessionStorage.getItem(USER_KEY);
    return raw ? JSON.parse(raw) : null;
  }
}
