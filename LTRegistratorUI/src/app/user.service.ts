import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { User } from './user.model';
import { Observable, of } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private projectUrl = 'http://localhost:52029/api/employee/info';

  constructor(private http: HttpClient) { }

  getProjects(): Observable<User> {
    return this.http.get<User>(this.projectUrl);
  }
}
