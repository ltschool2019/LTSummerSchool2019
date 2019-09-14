import { Component, OnInit, Inject } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';

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

  editForm: FormGroup;

  vacationTypes: vacationType[] = [
    { value: 'SickLeave', viewValue: 'SickLeave' },
    { value: 'Vacation', viewValue: 'Vacation' },
    { value: 'Training', viewValue: 'Training' },
    { value: 'Idle', viewValue: 'Idle' }
  ];

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: any,
    private fb: FormBuilder) { }

  ngOnInit() {
    this.initForm();
  }
  private initForm(): void {
    this.editForm = this.fb.group({
      type: '',
      start: [`${this.data.start}`, [Validators.required]],
      end: [`${this.data.end}`, [Validators.required]],
    });
  }
  onSubmit(value) {
    return {
      id: this.data.id,
      type: value.type,
      start: value.start,
      end: value.end
    }
  }
}
