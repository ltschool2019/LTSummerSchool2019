import { Component, OnInit } from '@angular/core';

import { Project } from '../project.model';

import { ProjectService } from '../project.service';

@Component({
  selector: 'app-timesheet',
  templateUrl: './timesheet.component.html',
  styleUrls: ['./timesheet.component.scss']
})
export class TimesheetComponent implements OnInit {

  projects: Project[];

  constructor(private projectService: ProjectService) { }

  ngOnInit() {
    this.getProjects();
  }/*
  getProjects(): void {
    this.projectService.getProjects().subscribe((projects: Project[]) => {
      this.projects = projects.map(
        (vacation: any) =>
          new Project(+vacation.typeLeave, vacation.startDate, vacation.endDate));
  }*/
  getProjects() {
    this.projectService.getProjects().subscribe(data => this.projects = data["projects"]);
  }
}
