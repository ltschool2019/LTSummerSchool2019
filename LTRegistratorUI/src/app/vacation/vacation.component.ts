import { Component, OnInit } from '@angular/core';
import { Vacation } from '../vacation.model';
import { VacationService } from '../vacation.service';
import { FormGroup, FormControl, Validators } from '@angular/forms';

@Component({
  selector: 'app-vacation',
  templateUrl: './vacation.component.html',
  styleUrls: ['./vacation.component.scss']
})

export class VacationComponent implements OnInit {
  vacationForm = new FormGroup({
    type: new FormControl('', Validators.required),
    start: new FormControl('', Validators.required),
    end: new FormControl('', Validators.required),
  });


  //надо из бд подгружать
  vacations: Vacation[] = [

  ];

  constructor() { }

  ngOnInit() {
  }

  add(type: string, start: string, end: string): void {
    //метод добавления
  }
  delete(vacation: Vacation): void {
    //написать метод удаления
  }
  onSubmit() {
    console.warn(this.vacationForm.value);
  }
}
