import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
// import { DateAdapter } from '@angular/material';

import { UserService } from '../core/service/user.service';
import { EmployeeService } from '../core/service/employee.service';
import { Project } from '../core/models/project.model';
import { Day } from '../core/models/day.model';
import { Task } from '../core/models/task.model';
import { TaskNote } from '../core/models/taskNote.model';

@Component({
  selector: 'app-timesheet-edit',
  templateUrl: './timesheet-edit.component.html',
  styleUrls: ['./timesheet-edit.component.scss']
})
export class TimesheetEditComponent implements OnInit {
  taskForm: FormGroup;
  // curr: Date = new Date();//текущая дата в формате "Tue Aug 13 2019 21:30:08 GMT+0300 (Москва, стандартное время)"
  week: Day[] = [];
  task: Task[];
  projects: Project[];
  userId: number;
  projectId: number;
  startDate: any;
  endDate: any;
  canPut = true; // post или пут запрос
  constructor(
    private fb: FormBuilder,
    private userService: UserService,
    private employeeService: EmployeeService
    ///  private dateAdapter: DateAdapter<any>
  ) {
  }

  ngOnInit() {
    this.userId = this.userService.getUserId();
    this.initForm();
    this.getProject();
    // this.dateAdapter.setLocale('ru-Latn');
    this.getWeek();
  }

  clear() {// очистить инпуты формы
    for (let i = 0; i < 7; i++) {
      this.taskForm.controls[`day${i}`].setValue('');
    }

    this.getTotalHours();
  }

  previousWeek() {// выбранный день -7 назад
    this.week = [];
    this.clear();
    const curr = new Date(this.taskForm.get('currentWeek').value);
    const first = curr.getDate() - 7;
    const day = new Date(curr.setDate(first));
    this.taskForm.patchValue({
      currentWeek: day
    }, {onlySelf: true});
    this.getWeek();
  }

  getWeek() {// получить текущую неделю
    this.week = [];
    this.clear();
    const curr = new Date(this.taskForm.get('currentWeek').value);
    for (let i = 1; i <= 7; i++) {// getDate() - Получить число месяца, от 1 до 31.
      const first = curr.getDate() - curr.getDay() + i;
      // getDay() - Получить номер дня в неделе.(от 0(вс) до 6 (сб)). В итоге получаем дни с пн по вс
      curr.setDate(first); // преобразуем и получаем только число
      const day = curr.toISOString().slice(0, 10);
      const newDay = new Day(day, i);
      this.week.push(newDay); // добавить в массив дней недели
    }

    this.getTasks();
  }

  nextWeek() {// выбранный +7 вперед
    this.week = [];
    this.clear();
    const curr = new Date(this.taskForm.get('currentWeek').value);
    const first = curr.getDate() + 7;
    const day = new Date(curr.setDate(first));
    this.taskForm.patchValue({
      currentWeek: day
    }, {onlySelf: true});
    this.getWeek();
  }

  getTotalHours() {// часы за неделю
    let sum = +0;
    for (let i = 0; i < 7; i++) {
      sum += +this.taskForm.controls[`day${i}`].value;
    }
    this.taskForm.controls[`total`].setValue(`${sum}`);
  }

  // get
  getTasks(): void {
    this.employeeService.getTasks(
      +localStorage.getItem('userId'),
      this.taskForm.controls['type'].value.id,
      this.week[0].date, this.week[6].date
    )
      .subscribe(
        tasks => {
          this.task = tasks;
          tasks.map(
            (task: any) => {
              task.taskNotes.map((taskNote: any) => {
                  const index = this.week.findIndex(item => item.date === taskNote.day.slice(0, 10));
                  this.taskForm.controls[`day${index}`].setValue(taskNote.hours);
                }
              );
              task.vacation.map((leave: any) => {
                const startIndex = this.week.findIndex(item => item.date === leave.start.slice(0, 10));
                const endIndex = this.week.findIndex(item => item.date === leave.end.slice(0, 10));
                const element = document.querySelectorAll(`.task__days__day__container__hours`);
                for (let i = 0; i <= 6; i++) {
                  if (i >= startIndex && i <= endIndex) {
                    (<HTMLElement>element[i]).style.backgroundColor = 'rgba(255, 194, 0, 0.3)';
                  } else {
                    (<HTMLElement>element[i]).style.backgroundColor = '#FFFFFF';
                  }
                }
              });
            }
          );
          this.canPut = true; // если прошло, то делаем put запросы
          this.getTotalHours();
        },
        error => {
          error.status === 404 ? this.canPut = false : console.log(error);
        }
      );
  }

  save() {
    this.canPut ? this.editTask() : this.addTask();
  }

  // post
  addTask() {
    const newTaskNotes: TaskNote[] = [];
    for (let i = 0; i < 7; i++) {
      if (this.taskForm.controls[`day${i}`].value !== '') {
        newTaskNotes.push(new TaskNote(0, this.week[i].date, this.taskForm.controls[`day${i}`].value));
      }
    }
    const newTask = new Task(+localStorage.getItem('userId'), this.taskForm.controls['type'].value.name, newTaskNotes, []);

    this.employeeService.addTask(+localStorage.getItem('userId'), this.taskForm.controls['type'].value.id, newTask)
      .subscribe(() => {
        this.canPut = true;
      });
  }

  // put
  editTask() {
    const newTaskNotes: TaskNote[] = [];
    for (let i = 0; i < 7; i++) {
      if (this.taskForm.controls[`day${i}`].value !== '') {
        newTaskNotes.push(new TaskNote(0, this.week[i].date, this.taskForm.controls[`day${i}`].value));
      }
    }
    const newTask = new Task(+localStorage.getItem('userId'), this.taskForm.controls['type'].value.name, newTaskNotes, []);
    this.employeeService.editTask(+localStorage.getItem('userId'), this.task[0].id, newTask)
      .subscribe(() => {
      });
  }

  // delete
  delete(): void {
    this.employeeService.deleteTask(+this.userId, this.task[0].id).subscribe();
  }

  private getProject() {
    this.userService.getUserInfo().subscribe(user => {
      this.projects = user.projects;
    });
  }

  private initForm(): void {
    this.taskForm = this.fb.group({
      type: '',
      currentWeek: new Date(),
      day0: '',
      day1: '',
      day2: '',
      day3: '',
      day4: '',
      day5: '',
      day6: '',
      total: '',
    });
  }
}