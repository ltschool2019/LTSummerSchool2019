import { Component, OnInit } from '@angular/core';

import { User } from '../../core/models/user.model';
import { UserService } from '../../core/service/user.service';
import { LoginService } from '../../core/service/login.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss']
})
export class HeaderComponent implements OnInit {
  user: User = new User(0, '', '', '', '', ['']);

  constructor(private userService: UserService, private loginService: LoginService, private router: Router) {
  }

  ngOnInit() {
    this.getUser();
  }

  getUser() {
    this.userService.getUserInfo().subscribe(user => this.user = user);
  }

  signOut() {
    this.loginService.logout();
    this.router.navigateByUrl('login');
  }

}
