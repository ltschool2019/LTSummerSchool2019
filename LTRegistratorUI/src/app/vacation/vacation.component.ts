import { Component, OnInit } from '@angular/core';
import { Vacation } from '../vacation.model';
import { VacationService } from '../vacation.service';
import { FormGroup, FormControl, FormBuilder, Validators } from '@angular/forms';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-vacation',
  templateUrl: './vacation.component.html',
  styleUrls: ['./vacation.component.scss'],
  providers: [VacationService]
})

export class VacationComponent implements OnInit {
  vacationForm: FormGroup;
  vacationTypes: string[];

  // надо из бд подгружать
  vacations: Vacation[] = [];

  constructor(
    private fb: FormBuilder,
    private vacationService: VacationService,
    private http: HttpClient) { }

  ngOnInit() {
    this.vacationTypes = ['Отпуск', 'Больничный'];
    this.getVacations();
    this.initForm();
  }
  private initForm(): void {
    this.vacationForm = this.fb.group({
      type: 'Отпуск',
      start: [null, [Validators.required]],
      end: [null, [Validators.required]],
    });
  }

  response: any;
  private id = 2;
  private vacationsUrl = `http://localhost:52029/api/employee/${this.id}/leaves`;
  
  getVacations() {
    this.http.get(this.vacationsUrl).subscribe((response => {
      this.response = response;
      console.log(this.response);
    }))

    delete (vacation: Vacation): void {
      //написать метод удаления
      this.vacations.splice(this.vacations.indexOf(vacation), 1);
    }

    onSubmit(type: string, start: string, end: string) {
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
        console.log(this.vacationForm.value);
       this.vacations.push(this.vacationForm.value)

    }
  }
