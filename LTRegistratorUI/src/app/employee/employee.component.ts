import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';

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
    this.week =[];
    let curr = new Date(this.taskForm.get('currentWeek').value);
    for (let i = 1; i <= 7; i++) {//getDate() - Получить число месяца, от 1 до 31.
      let first = curr.getDate() - curr.getDay() + i;//getDay() - Получить номер дня в неделе.(от 0(вс) до 6 (сб)). В итоге получаем дни с пн по вс
      let day = new Date(curr.setDate(first)).toISOString().slice(8, 10);//преобразуем и получаем только число
      let newDay = new Day(+day, i);
      this.week.push(newDay);//добавить в массив дней недели
    }
  }
  projects: Project[];
  constructor(
    private fb: FormBuilder,
    private userService: UserService
  ) { }
  previousWeek() {//выбранный день -7 назад

  }
  nextWeek() {//выбранный +7 вперед

  }
  ngOnInit() {
    this.getProject();
    this.initForm();
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
