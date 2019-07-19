import { Component, OnInit } from '@angular/core';
//import { Days } from 'src/app/days';
//import { DAYS } from 'src/app/days-of-week';

@Component({
  selector: 'app-employee',
  templateUrl: './employee.component.html',
  styleUrls: ['./employee.component.scss']
})
export class EmployeeComponent implements OnInit {

  constructor() { }

  ngOnInit() {
  }
  days = [ "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun"];
 // days = DAYS;
  
}
