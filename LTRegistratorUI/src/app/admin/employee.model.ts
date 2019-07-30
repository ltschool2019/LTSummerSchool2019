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
