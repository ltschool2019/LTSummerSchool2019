import { Component, OnInit } from '@angular/core';
import { Project } from '../core/models/project.model';
import { UserService } from '../core/service/user.service';

@Component({
  selector: 'app-timesheet',
  templateUrl: './timesheet.component.html',
  styleUrls: ['./timesheet.component.scss']
})
export class TimesheetComponent implements OnInit {
  projects: Project[];

  constructor(private userService: UserService) { }

  ngOnInit() {
    this.getUser();
  }

  getUser() {
    this.userService.getUser().subscribe(user => this.projects = user.projects);
  }
}
