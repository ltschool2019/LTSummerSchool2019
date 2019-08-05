import { Component, OnInit } from '@angular/core';
import { Vacation } from '../core/models/vacation.model';
import { VacationService } from '../core/service/vacation.service';
import { FormGroup, FormControl, FormBuilder, Validators } from '@angular/forms';

@Component({
  selector: 'app-vacation',
  templateUrl: './vacation.component.html',
  styleUrls: ['./vacation.component.scss']
})

export class VacationComponent implements OnInit {
  vacationForm: FormGroup;
  vacationTypes: string[];

  //TODO: из бд подгружать
  vacations: Vacation[] = [

  ];

  constructor(private fb: FormBuilder) { }

  ngOnInit() {
    this.vacationTypes = ['Отпуск', 'Больничный'];
    this.initForm();
  }
  private initForm(): void {
    this.vacationForm = this.fb.group({
      type: 'Отпуск',
      start: [null, [Validators.required]],
      end: [null, [Validators.required]],
    });
  }

  delete(vacation: Vacation): void {
    //TODO: метод удаления под сервис
    this.vacations.splice(this.vacations.indexOf(vacation), 1);
  }

  onSubmit() {
    const controls = this.vacationForm.controls;

    /** Проверяем форму на валидность */
    if (this.vacationForm.invalid) {
      /** Если форма не валидна, то помечаем все контролы как touched*/
      Object.keys(controls)
        .forEach(controlName => controls[controlName].markAsTouched());

      /** Прерываем выполнение метода*/
      return;
    }

    /** TODO: Обработка данных формы */
    this.vacations.push(this.vacationForm.value)
  }
}
