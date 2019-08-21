import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
//import { DateAdapter } from '@angular/material';

import { UserService } from '../core/service/user.service';
import { EmployeeService } from '../core/service/employee.service';
import { Project } from '../core/models/project.model';
import { Day } from '../core/models/day.model';
import { Task } from '../core/models/task.model';
import { TaskNote } from '../core/models/taskNote.model';

@Component({
  selector: 'app-employee',
  templateUrl: './employee.component.html',
  styleUrls: ['./employee.component.scss']
})
export class EmployeeComponent implements OnInit {
  // days = ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun'];
  //TODO: date получать из datepicker
  taskForm: FormGroup;
  //FIXME: заполнять выбранную неделю, вместо текущей
  // curr: Date = new Date();//текущая дата в формате "Tue Aug 13 2019 21:30:08 GMT+0300 (Москва, стандартное время)"
  week: Day[] = [];
  getWeek() {
    //FIXME: ночью неверные значения
    this.week = [];
    let curr = new Date(this.taskForm.get('currentWeek').value);
    for (let i = 1; i <= 7; i++) {//getDate() - Получить число месяца, от 1 до 31.
      let first = curr.getDate() - curr.getDay() + i;//getDay() - Получить номер дня в неделе.(от 0(вс) до 6 (сб)). В итоге получаем дни с пн по вс
      curr.setDate(first);//преобразуем и получаем только число
      let day = curr.toISOString().slice(0, 10);
      let newDay = new Day(day, i);
      this.week.push(newDay);//добавить в массив дней недели
    }
  }
  task: Task[];
  projects: Project[];
  userId: number;
  projectId: number;
  startDate: any;
  endDate: any;

  constructor(
    private fb: FormBuilder,
    private userService: UserService,
    private employeeService: EmployeeService
    ///  private dateAdapter: DateAdapter<any>
  ) { }
  ngOnInit() {
    this.userId = this.userService.getUserId();
    this.initForm();
    this.getProject();
    //this.dateAdapter.setLocale('ru-Latn');
    this.getWeek();
  }
  previousWeek() {//выбранный день -7 назад
    this.week = [];
    let curr = new Date(this.taskForm.get('currentWeek').value);
    let first = curr.getDate() - 7;
    let day = new Date(curr.setDate(first));
    this.taskForm.patchValue({
      currentWeek: day
    }, { onlySelf: true })
    this.getWeek();
  }
  nextWeek() {//выбранный +7 вперед
    this.week = [];
    let curr = new Date(this.taskForm.get('currentWeek').value);
    let first = curr.getDate() + 7;
    let day = new Date(curr.setDate(first));
    this.taskForm.patchValue({
      currentWeek: day
    }, { onlySelf: true })
    this.getWeek();
  }
  /*   getData() {
      let userId = +localStorage.getItem('userId');
      let projectId = +this.taskForm.get('type');
      let startDate = this.week[0].date;
      let endDate = this.week[6].date;
      return { userId, projectId, startDate, endDate };
    } */
  // get
  getTasks(): void {
    this.employeeService.getTasks(+localStorage.getItem('userId'), this.taskForm.controls['type'].value.id, this.week[0].date, this.week[6].date)
      .subscribe(task => {
        this.task = task;
      });
  }
  //post
  addTask() {
    let newTaskNotes: TaskNote[] = [];
    newTaskNotes.push(new TaskNote(0, this.week[0].date, this.taskForm.controls["Mon"].value));
    newTaskNotes.push(new TaskNote(0, this.week[1].date, this.taskForm.controls["Tue"].value));
    newTaskNotes.push(new TaskNote(0, this.week[2].date, this.taskForm.controls["Wed"].value));
    newTaskNotes.push(new TaskNote(0, this.week[3].date, this.taskForm.controls["Thu"].value));
    newTaskNotes.push(new TaskNote(0, this.week[4].date, this.taskForm.controls["Fri"].value));
    newTaskNotes.push(new TaskNote(0, this.week[5].date, this.taskForm.controls["Sat"].value));
    newTaskNotes.push(new TaskNote(0, this.week[6].date, this.taskForm.controls["Sun"].value));
    const newTask = new Task(+localStorage.getItem('userId'), this.taskForm.controls['type'].value.name, newTaskNotes, []);

    this.employeeService.addTask(+localStorage.getItem('userId'), this.taskForm.controls['type'].value.id, newTask)
      .subscribe(() => { });
  }
  // put
  editTask() {
    let newTaskNotes: TaskNote[] = [];
    newTaskNotes.push(new TaskNote(0, this.week[0].date, this.taskForm.controls["Mon"].value));
    newTaskNotes.push(new TaskNote(0, this.week[1].date, this.taskForm.controls["Tue"].value));
    newTaskNotes.push(new TaskNote(0, this.week[2].date, this.taskForm.controls["Wed"].value));
    newTaskNotes.push(new TaskNote(0, this.week[3].date, this.taskForm.controls["Thu"].value));
    newTaskNotes.push(new TaskNote(0, this.week[4].date, this.taskForm.controls["Fri"].value));
    newTaskNotes.push(new TaskNote(0, this.week[5].date, this.taskForm.controls["Sat"].value));
    newTaskNotes.push(new TaskNote(0, this.week[6].date, this.taskForm.controls["Sun"].value));
    const newTask = new Task(+localStorage.getItem('userId'), this.taskForm.controls['type'].value.name, newTaskNotes, []);
    this.employeeService.editTask(+localStorage.getItem('userId'), this.taskForm.controls['type'].value.id, newTask)
      .subscribe(() => { });
  }
  //delete
  delete(): void {
    // this.vacations = this.vacations.filter(v => v !== vacation);
    this.employeeService.deleteTask(+this.userId, this.taskForm.controls['type'].value.id).subscribe();
  }

  private getProject() {
    this.userService.getUserInfo().subscribe(user => {
      this.projects = user.projects;
    });
  }
  private initForm(): void {
    this.taskForm = this.fb.group({
      type: null,
      currentWeek: new Date(),
      Mon: 0,
      Tue: 0,
      Wed: 0,
      Thu: 0,
      Fri: 0,
      Sat: 0,
      Sun: 0,
      total: 0,
    });
  }
}
