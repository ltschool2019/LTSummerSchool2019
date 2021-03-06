import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import * as moment from 'moment';
import { catchError, tap } from 'rxjs/operators';
import { _throw } from 'rxjs-compat/observable/throw';
import { environment } from '../../../environments/environment';

import { UserService } from './user.service';


@Injectable()
export class LoginService {
  private token = {};
  private token_Json = String;

  constructor(private http: HttpClient, private userService: UserService) {
  }


  public signIn(email: string, password: string) {
    return this.http.post<any>(environment.apiBaseUrl + 'api/account/login', { email, password })
      .pipe(
        tap((token) => this.setSession(token)),
        catchError(err => {
          console.error(err);
          return _throw(err);
        })
      );
  }

  public logout() {
    localStorage.removeItem('id_token');
    localStorage.removeItem('expires_at');
    localStorage.removeItem('userId');
    this.userService.clearUserInfo();
  }

  private setSession(authResult) {
    this.token_Json = (JSON.parse(atob(authResult.token.split('.')[1])));

    localStorage.setItem('userId', this.token_Json['EmployeeID']);
    const expiresAt = moment().add(authResult.exp, 'second');
    localStorage.setItem('id_token', authResult.token);
    localStorage.setItem('expires_at', JSON.stringify(expiresAt.valueOf()));
  }
}
