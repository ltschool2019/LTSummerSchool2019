import { Component, OnInit } from '@angular/core';

import { Project } from '../project.model';
import { User } from '../user.model';
import { UserService } from '../user.service';

@Component({
  selector: 'app-timesheet',
  templateUrl: './timesheet.component.html',
  styleUrls: ['./timesheet.component.scss']
})
export class TimesheetComponent implements OnInit {
  user: User;
  projects: Project[];

  constructor(private userService: UserService) { }

  ngOnInit() {
    this.getUser();
  }

  getUser() {
    this.userService.getUser().subscribe(user => this.projects = user.projects);
  }
}
