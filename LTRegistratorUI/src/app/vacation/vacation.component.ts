import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';

import { VacationEditDialogComponent } from './vacation-edit-dialog/vacation-edit-dialog.component';
import { Vacation } from '../core/models/vacation.model';
import { VacationService } from '../core/service/vacation.service';
import { UserService } from '../core/service/user.service';

export interface VacationType {
  value: string;
  viewValue: string;
}

@Component({
  selector: 'app-vacation',
  templateUrl: './vacation.component.html',
  styleUrls: ['./vacation.component.scss']
})

export class VacationComponent implements OnInit {

  vacationForm: FormGroup;

  vacationTypes: VacationType[] = [
    { value: 'SickLeave', viewValue: 'SickLeave' },
    { value: 'Vacation', viewValue: 'Vacation' },
    { value: 'Training', viewValue: 'Training' },
    { value: 'Idle', viewValue: 'Idle' }
  ];

  vacations: Vacation[] = [];
  private userId: number;

  constructor(
    private fb: FormBuilder,
    private vacationService: VacationService,
    private userService: UserService,
    public dialog: MatDialog) {
  }

  ngOnInit() {
    this.userId = this.userService.getUserId();
    this.initForm();
    this.getVacations();
  }

  // get
  getVacations(): void {
    this.vacationService.getVacations(this.userId)
      .subscribe(vacations => this.vacations = vacations);
  }

  // post
  onSubmit() {
    //let value = new Date(this.vacationForm.get('currentWeek').value);
    let start = new Date(this.vacationForm.get('start').value);
    let end = new Date(this.vacationForm.get('end').value);
    //new Date создает дату по мск времени, а это на 3 часа меньше, чем по UTC.
    //Соответственно start.toISOString() дает дату на сутки меньше
    //это костыль (возможно) т.к.
    start.setHours(3);
    end.setHours(3);
    /** Проверяем форму на валидность */
    if (this.vacationForm.invalid) {
      /** Прерываем выполнение метода*/
      return;
    }

    const newVacation = new Vacation(0,
      this.vacationForm.get('type').value,
      start.toISOString(),
      end.toISOString());
    console.log('newVacation: ', newVacation);
    this.vacationService.addVacation(this.userId, newVacation)
      .subscribe(() => {
        this.vacations.push(newVacation);
      }
      );
  }

  // delete
  delete(vacation: Vacation): void {
    this.vacations = this.vacations.filter(v => v !== vacation);
    this.vacationService.deleteVacation(+this.userId, +vacation.id).subscribe();
  }


  openEditModal(vacation: Vacation) {
    const dialogRef = this.dialog.open(VacationEditDialogComponent,
      { data: { id: vacation.id, type: vacation.type, start: vacation.start, end: vacation.end } });
    dialogRef.afterClosed().subscribe(result => {
      if (result != undefined && result != "false") {
        this.editVacation(result);
      }
    });
  }

  // put
  editVacation(value) {
    let start = new Date(value.start);
    let end = new Date(value.end);
    start.setHours(3);
    end.setHours(3);
    const newVacation = new Vacation(value.id,
      value.type,
      start,
      end);
    this.vacationService.editVacation(this.userId, newVacation)
      .subscribe(() => {
        this.vacations = this.vacations.filter(v => v.id !== newVacation.id);
        this.vacations.push(newVacation);
      }
      );
  }

  private initForm(): void {
    this.vacationForm = this.fb.group({
      type: '',
      start: [new Date(), [Validators.required]],
      end: [new Date(), [Validators.required]],
    });
  }
}
