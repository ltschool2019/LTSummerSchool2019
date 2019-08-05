import { Component, OnInit, Inject } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material';
import { Vacation } from '../../vacation.model';

export interface vacationType {
  value: string;
  viewValue: string;
}
@Component({
  selector: 'app-vacation-edit-dialog',
  templateUrl: './vacation-edit-dialog.component.html',
  styleUrls: ['./vacation-edit-dialog.component.scss']
})
export class VacationEditDialogComponent implements OnInit {

  vacationTypes: vacationType[] = [
    { value: 'sickLeave', viewValue: 'SickLeave' },
    { value: 'vacation', viewValue: 'Vacation' },
    { value: 'training', viewValue: 'Training' },
    { value: 'idle', viewValue: 'Idle' }
  ];
  constructor(@Inject(MAT_DIALOG_DATA) public data: any) { }

  ngOnInit() {
  }

}
