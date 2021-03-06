import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable } from 'rxjs/Observable';

import { LoginService } from 'src/app/core/service/login.service';


@Injectable()
export class JwtInterceptor implements HttpInterceptor {
  constructor(
    private loginService: LoginService,
    private router: Router
  ) {
  }

  // todo fix
  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    // add authorization header with jwt token if available
    const token = localStorage.getItem('id_token');
    // if (!token) {
    //     alert('User is not authorized!');
    // } else {
    request = request.clone({
      setHeaders: {
        Authorization: `Bearer ` + token
      }
    });
    // }
    return next.handle(request);
  }
}

