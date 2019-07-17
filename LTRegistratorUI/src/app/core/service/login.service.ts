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
  getUser(email:string, password:string) {
    return this.http.post<string>('http://localhost:53634/api/account/login', {email, password})
    // return this.http.get('http://localhost:3000/token')
    // .pipe(catchError(this.handleError('login', hero))
    // );
  }
}