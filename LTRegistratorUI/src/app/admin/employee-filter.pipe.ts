import { PipeTransform, Pipe } from '@angular/core';
import { Employee, EmployeeItem } from './employee.model';
import { EMPLOYEES } from './mock-employee';

@Pipe({
  name: 'employeeFilter'
})

export class EmployeeFilterPipe implements PipeTransform {
  transform(EMPLOYEES: EmployeeItem[], searchTerm: string): EmployeeItem[] {
    if (!EMPLOYEES || !searchTerm) {
      return EMPLOYEES;
    }
    return EMPLOYEES.filter(EmployeeItem =>
      EmployeeItem.name.toLowerCase().indexOf(searchTerm.toLowerCase()) !== -1);
  }
}

