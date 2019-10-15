import { Component, OnInit, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import * as moment from 'moment';

import { VacationEditDialogComponent } from './vacation-edit-dialog/vacation-edit-dialog.component';
import { Vacation } from '../core/models/vacation.model';
import { VacationService } from '../core/service/vacation.service';
import { UserService } from '../core/service/user.service';
import { OverlayService } from "../shared/overlay/overlay.service";
import { ApiError } from '../core/models/apiError.model';

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
    {value: 'SickLeave', viewValue: 'SickLeave'},
    {value: 'Vacation', viewValue: 'Vacation'},
    {value: 'Training', viewValue: 'Training'},
    {value: 'Idle', viewValue: 'Idle'}
  ];

  vacations: Vacation[] = [];
  private userId: number;

  @ViewChild('LoaderComponent', { static: true }) LoaderComponent;

  constructor(
    private fb: FormBuilder,
    private vacationService: VacationService,
    private userService: UserService,
    public dialog: MatDialog,
    private overlayService: OverlayService
  ) {
  }

  ngOnInit() {
    this.userId = this.userService.getUserId();
    this.initForm();
    this.getVacations();

    // todo add manager's userId variety
  }

  // get
  getVacations(): void {
    this.LoaderComponent.showLoader();
    this.vacationService.getVacations(this.userId).subscribe(
      vacations => {
        this.vacations = vacations
      },
      err => {
        let apiError = <ApiError>err.error;
        this.overlayService.danger(apiError.message);
      }
    ).add(() => this.LoaderComponent.hideLoader());
  }

  // post
  onSubmit() {
    const newVacation = new Vacation(
      0,
      this.vacationForm.get('typeLeave').value,
      moment(this.vacationForm.get('startDate').value).toISOString(true),
      moment(this.vacationForm.get('endDate').value).toISOString(true)
    );

    this.vacationService.addVacation(this.userId, newVacation).subscribe(
      (vocation: Vacation) => {
        this.vacations.push(vocation);
        this.overlayService.success("Отпуск успешно создан")
      },
      err => {
        let apiError = <ApiError>err.error;
        this.overlayService.danger(apiError.message);
      }
    );
  }

  // delete
  delete(vacation: Vacation): void {
    this.vacationService.deleteVacation(+this.userId, +vacation.id).subscribe(
      () => {
        this.vacations = this.vacations.filter(v => v !== vacation);
        this.overlayService.success("Отпуск упешно удалено");
      },
      err => {
        let apiError = <ApiError>err.error;
        this.overlayService.danger(apiError.message);
      });
  }


  openEditModal(vacation: Vacation) {
    const dialogRef = this.dialog.open(VacationEditDialogComponent,
      {data: {id: vacation.id, type: vacation.typeLeave, start: vacation.startDate, end: vacation.endDate}});
    dialogRef.afterClosed().subscribe(result => {
      if (result !== undefined && result !== 'false') {
        this.editVacation(result);
      }
    });
  }

  // put
  editVacation(value) {
    const newVacation = new Vacation(value.id,
      value.type,
      moment(value.start).toISOString(true),
      moment(value.end).toISOString(true)
    );
    this.vacationService.editVacation(this.userId, newVacation).subscribe(
      (vacation: Vacation) => {
        this.vacations = this.vacations.filter(v => v.id !== vacation.id);
        this.vacations.push(vacation);
        this.overlayService.success("Отпуск успешно обновлен")
      }, 
      err => {
        let apiError = <ApiError>err.error;
        this.overlayService.danger(apiError.message);
      }
    );
  }

  private initForm(): void {
    this.vacationForm = this.fb.group({
      typeLeave: 'SickLeave',
      startDate: [new Date(), [Validators.required]],
      endDate: [new Date(), [Validators.required]],
    });
  }
}
