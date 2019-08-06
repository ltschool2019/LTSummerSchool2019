import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { User } from './user.model';
import { Observable } from 'rxjs';
import { shareReplay, map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  id = 2;
  user$: Observable<User>;
  private userUrl = `http://localhost:52029/api/employee/${this.id}/info`;

  getObservableUser() {
    this.user$ = this.http.get<User>(this.userUrl).pipe(
      map((user: any) => {
        return new User(user.id, user.firstName, user.secondName,
          user.mail, user.maxRole, user["projects"]);
      }),
      shareReplay(1)
    );
  }
  getUser() {
    return this.user$;
  }
  constructor(private http: HttpClient) {
    this.getObservableUser();
  }
}
