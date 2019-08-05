import { Component, OnInit } from '@angular/core';
import { EmployeesTable } from 'src/app/employee-table/employee-table.component';
import { AddManagerComponent } from 'src/app/add-manager/add-manager.component';

@Component({
  selector: 'app-admin',
  templateUrl: './admin.component.html',
  styleUrls: ['./admin.component.scss']
})
export class AdminComponent implements OnInit {

  constructor() { }

  ngOnInit() {
  }

}
