import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { User } from '../../shared/models/user.model';
import * as moment from 'moment';


@Injectable()
export class LoginService {
  private token = new Object();
  private  token_Json =  String;
  private user = new User;
  constructor(private http: HttpClient) {}


  getUser(email: string, password: string) {
    return this.http.post<any>('http://localhost:52029/api/account/login', {email, password})
    .subscribe((token) => this.setSession(token));
  }
  private setSession(authResult) {
    this.token_Json = (JSON.parse(atob(authResult.token.split('.')[1])));
    this.user.role = this.token_Json['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];
    const expiresAt = moment().add(authResult.exp, 'second');
    localStorage.setItem('id_token', authResult.token);
    localStorage.setItem('expires_at', JSON.stringify(expiresAt.valueOf()) );
}
}
