import { Component, OnInit } from '@angular/core';
import { Project } from '../core/models/project.model';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-timesheet',
  templateUrl: './timesheet.component.html',
  styleUrls: ['./timesheet.component.scss']
})
export class TimesheetComponent implements OnInit {
  projects: Project[] = [];

  constructor(private route: ActivatedRoute) {
  }

  ngOnInit() {
    this.projects = this.route.snapshot.data.user.projects;
  }

}
