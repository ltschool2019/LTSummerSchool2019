import { Resolve } from '@angular/router';
import { Observable } from 'rxjs/Observable';
import { Injectable } from '@angular/core';
import { UserService } from '../core/service/user.service';
import { User } from '../core/models/user.model';


@Injectable({
  providedIn: 'root'
})
export class TimesheetResolverService implements Resolve<User> {
  constructor(private userService: UserService) {
  }

  resolve(): Observable<User> | Promise<User> | User {
    return this.userService.getUserInfo();
  }
}
