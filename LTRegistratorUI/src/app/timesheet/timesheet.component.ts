import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';

import { Project } from '../core/models/project.model';

@Component({
  selector: 'app-timesheet',
  templateUrl: './timesheet.component.html',
  styleUrls: ['./timesheet.component.scss']
})
export class TimesheetComponent implements OnInit {
  projects: Project[] = [];

  @ViewChild('LoaderComponent', { static: true }) LoaderComponent;

  constructor(
    private route: ActivatedRoute, 
    private router: Router) {
  }

  ngOnInit() {
    this.LoaderComponent.showLoader();
    this.projects = this.route.snapshot.data.user.projects;
    this.LoaderComponent.hideLoader();
  }

  openProjectDetails(id: number): void {
    this.router.navigateByUrl(`user/timesheet/${id}/tasks`);
  }

}
