import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import * as moment from 'moment';
import { catchError, tap } from 'rxjs/operators';
import { _throw } from 'rxjs-compat/observable/throw';

import { UserService } from './user.service';


@Injectable()
export class LoginService {
  private token = {};
  private token_Json = String;

  constructor(private http: HttpClient, private userService: UserService) {
  }


  public signIn(email: string, password: string) {
    return this.http.post<any>('http://localhost:5000/api/account/login', { email, password })
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
  }

  private setSession(authResult) {
    this.token_Json = (JSON.parse(atob(authResult.token.split('.')[1])));
    console.log(this.token_Json);
    //this.userService.setUserId(Number(this.token_Json['EmployeeID']));

    localStorage.setItem('userId', this.token_Json['EmployeeID']);
    this.userService.setUserId(+localStorage.getItem('userId'));
    const expiresAt = moment().add(authResult.exp, 'second');
    localStorage.setItem('id_token', authResult.token);
    localStorage.setItem('expires_at', JSON.stringify(expiresAt.valueOf()));
  }
}
