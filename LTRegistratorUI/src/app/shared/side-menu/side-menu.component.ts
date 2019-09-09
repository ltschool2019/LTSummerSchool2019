import { Component, OnInit } from '@angular/core';
import { User } from '../../core/models/user.model';
import { UserService } from '../../core/service/user.service';
import { LoginService } from '../../core/service/login.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-side-menu',
  templateUrl: './side-menu.component.html',
  styleUrls: ['./side-menu.component.scss']
})
export class SideMenuComponent implements OnInit {

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
