import { Component, OnInit } from '@angular/core';
import { User } from '../user.model';
import { UserComponent } from '../user/user.component';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss']
})
export class HeaderComponent implements OnInit {
  user: User = new User(0, '', '', '', '', ['']);
  constructor(private userComponent: UserComponent) { }

  ngOnInit() {
    this.getUser();
  }

  getUser() {
    this.userComponent.user$.subscribe(user => this.user = user);
  }
}
