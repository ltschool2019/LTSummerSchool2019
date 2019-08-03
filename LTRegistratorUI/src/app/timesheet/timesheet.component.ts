import { Component, OnInit } from '@angular/core';
import { Project } from '../project.model';
import { UserComponent } from '../user/user.component';

@Component({
  selector: 'app-timesheet',
  templateUrl: './timesheet.component.html',
  styleUrls: ['./timesheet.component.scss']
})
export class TimesheetComponent implements OnInit {
  projects: Project[];

  constructor(private userComponent: UserComponent) { }

  ngOnInit() {
    this.getUser();
  }

  getUser() {
    this.userComponent.user$.subscribe(user => this.projects = user.projects);
  }
}
