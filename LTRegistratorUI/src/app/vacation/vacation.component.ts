import { Component, OnInit } from '@angular/core';
import { Vacation } from '../vacation.model';
import { VacationService } from '../vacation.service';
import { UserService } from '../user.service'
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { VacationEditDialogComponent } from './vacation-edit-dialog/vacation-edit-dialog.component';

export interface vacationType {
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

  vacationTypes: vacationType[] = [
    { value: 'SickLeave', viewValue: 'SickLeave' },
    { value: 'Vacation', viewValue: 'Vacation' },
    { value: 'Training', viewValue: 'Training' },
    { value: 'Idle', viewValue: 'Idle' }
  ];
  minDate = new Date();
  maxDate = new Date(2020, 0, 1);

  vacations: Vacation[] = [];

  constructor(
    private fb: FormBuilder,
    private vacationService: VacationService,
    private userService: UserService,
    public dialog: MatDialog) { }

  ngOnInit() {
    this.initForm();
    this.getVacations();
  }
  getUserId() {
    let id: number;
    this.userService.getUser().subscribe(user => id = user.id);
    return id;
  }
  private initForm(): void {
    this.vacationForm = this.fb.group({
      type: '',
      start: [new Date(), [Validators.required]],
      end: [new Date(), [Validators.required]],
    });
  }
  //get
  getVacations(): void {
    this.vacationService.getVacations(this.getUserId())
      .subscribe(vacations => this.vacations = vacations);
  }
  //post
  onSubmit(value) {

    /** Проверяем форму на валидность */
    if (this.vacationForm.invalid) {
      /** Прерываем выполнение метода*/
      return;
    }

    let newVacation = new Vacation(0,
      value.type,
      value.start.toISOString(),
      value.end.toISOString());
    this.vacationService.addVacation(this.getUserId(), newVacation)
      .subscribe(() => { }, (error) => {
        if (error.status == 200) {
          this.vacations.push(newVacation);
        }
      });
  }
  //delete
  //TODO: дождаться back
  delete(vacation: Vacation): void {
    this.vacations = this.vacations.filter(v => v !== vacation);
    this.vacationService.deleteVacation(this.getUserId(), vacation.id).subscribe();
  }


  openEditModal(vacation: Vacation) {
    let dialogRef = this.dialog.open(VacationEditDialogComponent,
      { data: { id: vacation.id, type: vacation.type, start: vacation.start, end: vacation.end } });
    dialogRef.afterClosed().subscribe(result => {
      this.editVacation(result);
    });
  }
  //put
  editVacation(value) {
    let newVacation = new Vacation(value.id,
      value.type,
      value.start,
      value.end);
    this.vacationService.editVacation(this.getUserId(), newVacation)
      .subscribe(() => { },
        (error) => {
          if (error.status == 200) {
            this.vacations = this.vacations.filter(v => v.id !== newVacation.id);
            this.vacations.push(newVacation);
          }
        });
  }
}
