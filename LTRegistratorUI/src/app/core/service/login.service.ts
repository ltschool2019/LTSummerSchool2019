import {Injectable} from "@angular/core";
import {HttpClient} from "@angular/common/http";
import {Observable} from 'rxjs';
import {User} from "../../shared/models/user.model";
import { map } from 'rxjs/operators';

@Injectable()
export class LoginService {
  public token = new Object(); 
  public token_Json = new String;
  constructor(private http: HttpClient) {}
  getUser() {
    return this.http.get('http://localhost:3000/token');
    
   // return JSON.parse(atob(this.token.token.split('.')[1]))
  }
}



//JSON.parse(atob(token.split('.')[1]))