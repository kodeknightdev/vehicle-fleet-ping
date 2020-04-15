import { Injectable } from '@angular/core';
import { HttpErrorResponse, HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

import { environment } from '../../environments/environment';
import { AuthService } from '../services/auth.service';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  constructor(private auth: AuthService, private router: Router) { }

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    if (!req.url.startsWith(environment.apiUrl)) {
      return next.handle(req);
    }

    const token = this.auth.token;
    const isLoginCheck = req.headers.has('Authorization');
    if (token && !isLoginCheck) {
      req = req.clone({ setHeaders: { Authorization: `Basic ${token}` } });
    }

    return next.handle(req).pipe(
      catchError((err: HttpErrorResponse) => {
        // stored credentials stopped working (password changed, user removed)
        if (err.status === 401 && !isLoginCheck) {
          this.auth.logout();
          this.router.navigate(['/login'], { queryParams: { returnUrl: this.router.url } });
        }
        return throwError(err);
      })
    );
  }
}
