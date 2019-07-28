import { Component, OnInit } from '@angular/core';
import { Vacation } from '../vacation.model';
import { VacationService } from '../vacation.service';
import { FormGroup, FormControl, FormBuilder, Validators } from '@angular/forms';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-vacation',
  templateUrl: './vacation.component.html',
  styleUrls: ['./vacation.component.scss']
})

export class VacationComponent implements OnInit {
  vacationForm: FormGroup;
  vacationTypes: string[];

  vacations: Vacation[] = [];
  vacation: Vacation;
  constructor(
    private fb: FormBuilder,
    private vacationService: VacationService) { }

  ngOnInit() {
    this.vacationTypes = ['Отпуск', 'Больничный'];
    this.initForm();
    this.getVacations();
  }
  private initForm(): void {
    this.vacationForm = this.fb.group({
      type: 'Отпуск',
      start: [null, [Validators.required]],
      end: [null, [Validators.required]],
    });
  }

  getVacations(): void {
    this.vacationService.getVacations().subscribe((vacations: Vacation[]) => {
      this.vacations = vacations.map(
        (vacation: any) =>
          new Vacation(+vacation.typeLeave, vacation.startDate, vacation.endDate));
    })
    /* this.vacationService.getVacations().pipe(first()).subscribe((vacations: Vacation[]) => {
       this.vacations = vacations;
       console.log(this.vacations);
     })*/
    /*
        this.vacationService.getVacations().subscribe(vacations => {
          this.vacations = vacations;
          console.log(vacations);
        }
        );*/
  }

  onSubmit() {
    const controls = this.vacationForm.controls;

    /** Проверяем форму на валидность */
    if (this.vacationForm.invalid) {
      /** Прерываем выполнение метода*/
      return;
    }

    /** TODO: Обработка данных формы */
    console.log(this.vacationForm.value);
    this.vacationService.addVacation(
      new Vacation(
        this.vacationForm.value.type,
        this.vacationForm.value.start,
        this.vacationForm.value.end))
      .subscribe(vacation => {
        this.vacations.push(vacation);
        console.log(this.vacation);
      })
  }


}
