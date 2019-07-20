import { Component, OnInit } from '@angular/core';
import { Project } from '../project.model';

@Component({
  selector: 'app-timesheet',
  templateUrl: './timesheet.component.html',
  styleUrls: ['./timesheet.component.scss']
})
export class TimesheetComponent implements OnInit {
  projects: Project[] = [
    { name: 'Project A', hours: 45, id: 1 },
    { name: 'Project B', hours: 65, id: 2 }
  ];
  constructor() { }

  ngOnInit() {
  }

}
