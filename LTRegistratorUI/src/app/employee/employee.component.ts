import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
//import { DateAdapter } from '@angular/material';

import { UserService } from '../core/service/user.service';
import { Project } from '../core/models/project.model';
import { Day } from '../core/models/day.model';

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
    console.log('curr=' + curr);
    for (let i = 1; i <= 7; i++) {//getDate() - Получить число месяца, от 1 до 31.
      let first = curr.getDate() - curr.getDay() + i;//getDay() - Получить номер дня в неделе.(от 0(вс) до 6 (сб)). В итоге получаем дни с пн по вс

      console.log('getDate=' + curr.getDate());
      console.log('getDay' + curr.getDay());
      curr.setDate(first);//преобразуем и получаем только число
      let day = curr.getDate();
      console.log('curr=' + curr);
      let newDay = new Day(+day, i);
      this.week.push(newDay);//добавить в массив дней недели
    }
    console.log('___________________________________________-');
  }
  projects: Project[];
  constructor(
    private fb: FormBuilder,
    private userService: UserService,
  ///  private dateAdapter: DateAdapter<any>
  ) { }
  previousWeek() {//выбранный день -7 назад
    this.week = [];
    let curr = new Date(this.taskForm.get('currentWeek').value);
    let first = curr.getDate() - 7;
    let day = new Date(curr.setDate(first));
    this.taskForm.patchValue({
      currentWeek: day
    })
    this.getWeek();
  }
  nextWeek() {//выбранный +7 вперед
    this.week = [];
    let curr = new Date(this.taskForm.get('currentWeek').value);
    let first = curr.getDate() + 7;
    let day = new Date(curr.setDate(first));
    this.taskForm.patchValue({
      currentWeek: day
    })
    this.getWeek();

  }
  ngOnInit() {
    this.getProject();
    this.initForm();
    //this.dateAdapter.setLocale('ru-Latn');
    this.getWeek();
  }
  getProject() {
    this.userService.getUserInfo().subscribe(user => this.projects = user.projects);
  }
  private initForm(): void {
    this.taskForm = this.fb.group({
      type: '',
      currentWeek: new Date(),
      total: '',
    });
  }
}
