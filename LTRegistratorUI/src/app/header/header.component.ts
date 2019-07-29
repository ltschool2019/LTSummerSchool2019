import { Component, OnInit } from '@angular/core';
import { User } from '../user.model';
import { UserService } from '../user.service';

import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss']
})
export class HeaderComponent implements OnInit {
  user: User;
  private userUrl = 'http://localhost:52029/api/employee/info';

  constructor(private http: HttpClient) { }

  ngOnInit() {
    this.getUser();
  }
  getUser() {
    this.http.get(this.userUrl).subscribe(
      (data: any) => this.user =
        new User(data.employeeId, data.firstName, data.secondName, data.mail, data.maxRole));
  }
}
