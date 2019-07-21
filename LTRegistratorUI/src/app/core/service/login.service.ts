import {Injectable} from "@angular/core";
import {HttpClient} from "@angular/common/http";
import {Observable} from 'rxjs';
import {User} from "../../shared/models/user.model";
import { map } from 'rxjs/operators';
import * as moment from "moment";


@Injectable()
export class LoginService {
  public token = new Object(); 
  public token_Json = new String;
  constructor(private http: HttpClient) {}

 
  getUser(email:string, password:string) {
    return this.http.post<any>('http://localhost:53635/api/account/login', {email, password}).subscribe((token) => this.setSession(token));
  }
  private setSession(authResult) {
    this.token_Json = (JSON.parse(atob(authResult.token.split('.')[1])));
    console.log(this.token_Json);
    
    const expiresAt = moment().add(authResult.exp,'second');
    localStorage.setItem('id_token', authResult.token);
    localStorage.setItem("expires_at", JSON.stringify(expiresAt.valueOf()) );
}          
}