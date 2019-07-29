import { Component, OnInit } from '@angular/core';

export class Employee {
  name: string;
  email: string;

  constructor(name: string, email: string) {
    this.name = name;
    this.email = email;
  }
}

export class EmployeeItem {
  employee: Employee;
  selected: boolean;

  constructor(name: string, email: string) {
    this.employee = new Employee(name, email);
  }
}

@Component({
  selector: 'app-admin',
  templateUrl: './admin.component.html',
  styleUrls: ['./admin.component.scss']
})
export class AdminComponent implements OnInit {

  ngOnInit() {
  }

  areAllSelected: boolean;
  employees: EmployeeItem[];
  searchTerm: string;

 constructor() {
   this.employees = [
     new EmployeeItem('Ann', 'a@b'),
     new EmployeeItem('Bob', 'b@c'),
     new EmployeeItem('Elle', 'c@d')
   ]
 }


 selectAll() {
   for (let emp of this.employees) {
     emp.selected = this.areAllSelected;
   }
 }
 checkIfAllSelected() {
   this.areAllSelected = this.employees.every(function (item: any) {
     return item.selected == true;
   })
 }

}
