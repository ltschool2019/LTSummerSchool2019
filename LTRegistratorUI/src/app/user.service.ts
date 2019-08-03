import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { User } from './user.model';
import { Project } from './project.model';
import { ReplaySubject } from 'rxjs/ReplaySubject';
import { shareReplay, map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  id = 2;
  private userUrl = `http://localhost:52029/api/employee/${this.id}/info`;

  getUser() {
    return this.http.get<User>(this.userUrl).pipe(
      map((user: any) => {
        return new User(user.employeeId, user.firstName, user.secondName,
          user.mail, user.maxRole, user["projects"]);
      }),
      shareReplay(1)
    );
  }

  constructor(private http: HttpClient) { }
}
