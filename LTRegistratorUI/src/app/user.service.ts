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
  private userUrl = 'http://localhost:52029/api/employee/info';

  shareWithReplay = new ReplaySubject();

  getUser = this.http.get<User>(this.userUrl).pipe(
    map((user: any) => {
      return new User(user.employeeId, user.firstName,
        user.secondName, user.mail, +user.maxRole,
        user["projects"].map(
          (project: any) => new Project(project.projectId, project.name)
        ));
    }),
    shareReplay(1)
  );
  constructor(private http: HttpClient) { }
}
