import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { User } from './user.model';
import { Project } from './project.model';
import { Observable, of } from 'rxjs';
import { catchError, map, tap, first } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private userUrl = 'http://localhost:52029/api/employee/info';

  projects: Project[];
  user: User;

  constructor(private http: HttpClient) { }

  getUser(): Observable<User> {
    return this.http.get<User>(this.userUrl).pipe(
      map((user: any) => {
        this.projects = user["projects"].map(
          (project: any) => new Project(project.projectId, project.name)
        );
        return new User(user.employeeId, user.firstName, user.secondName, user.mail, +user.maxRole, this.projects);
      })
    )
  }
}
