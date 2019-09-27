import { Component, OnInit } from '@angular/core';
import { EmployeeService } from '../../core/service/employee.service';
import { UserService } from '../../core/service/user.service';
import { MatTableDataSource } from '@angular/material/table';
import { Task } from '../../core/models/task.model';
import { formatDate } from '@angular/common';
import { Router, ActivatedRoute } from "@angular/router";

@Component({
  selector: 'app-task-list',
  templateUrl: './task-list.component.html',
  styleUrls: ['./task-list.component.scss']
})
export class TaskListComponent implements OnInit {
  public projectTasks: MatTableDataSource<Task>;
  displayedColumns: string[] = ['name', 'delete'];

  constructor(
    private employeeService: EmployeeService,
    private userService: UserService,
    private router: Router,
    private route: ActivatedRoute
  ) { }

  ngOnInit() {
    this.getTasks();
  }

  getTasks(): void {
    let projectId = Number(this.route.snapshot.paramMap.get('id'));
    let currentDate = new Date();
    let startDate = new Date(currentDate.getFullYear(), currentDate.getMonth() - 1, currentDate.getDay());
    this.employeeService.getTasks(this.userService.getUserId(), projectId, this.FormatDate(startDate), this.FormatDate(currentDate))
    .subscribe((tasks: []) => {
      this.projectTasks = new MatTableDataSource(tasks);
    });
  }

  private FormatDate(date: Date): string {
    return formatDate(date, 'yyyy-MM-dd', 'en')
  }

  public addNewTask(): void {
    let projectId = Number(this.route.snapshot.paramMap.get('id'));
    this.router.navigateByUrl(`user/timesheet/${projectId}/tasks/task_details`);
  }
}
