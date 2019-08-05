import { Component, OnInit } from '@angular/core';
import { Vacation } from '../vacation.model';
import { VacationService } from '../vacation.service';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';
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
  selectedValue: string;

  oldVacation: Vacation;
  vacationForm: FormGroup;
  editForm: FormGroup;

  vacationTypes: vacationType[] = [
    { value: 'sickLeave', viewValue: 'SickLeave' },
    { value: 'vacation', viewValue: 'Vacation' },
    { value: 'training', viewValue: 'Training' },
    { value: 'idle', viewValue: 'Idle' }
  ];
  minDate = new Date();
  maxDate = new Date(2020, 0, 1);

  vacations: Vacation[] = [];

  constructor(
    private fb: FormBuilder,
    private vacationService: VacationService,
    public dialog: MatDialog) { }

  ngOnInit() {
    this.initForm();
    this.getVacations();
  }
  private initForm(): void {
    this.vacationForm = this.fb.group({
      type: 'Vacation',
      start: [new FormControl(new Date()), [Validators.required]],
      end: [null, [Validators.required]],
    });
    this.editForm = this.fb.group({
      editType: '',
      editStart: [null, [Validators.required]],
      editEnd: [null, [Validators.required]],
    })
  }
  //get
  getVacations(): void {
    this.vacationService.getVacations()
      .subscribe(vacations => this.vacations = vacations);
  }
  //post
  onSubmit() {

    /** Проверяем форму на валидность */
    if (this.vacationForm.invalid) {
      /** Прерываем выполнение метода*/
      return;
    }

    let newVacation = new Vacation(0,
      this.vacationForm.value.type,
      this.vacationForm.value.start,
      this.vacationForm.value.end);
    this.vacationService.addVacation(newVacation)
      .subscribe(() =>
        this.vacations.push(newVacation) //FIXME: опять обновляется только после обновления страницы (сабскрайб не работает???)
      );
  }
  //delete
  //FIXME: не работает удаление. Пока что...
  delete(vacation: Vacation): void {
    this.vacations = this.vacations.filter(v => v !== vacation);
    this.vacationService.deleteVacation(vacation).subscribe();
  }

  //FIXME: отпуск применяется только вторым запросом
  /*   edit(vacation: Vacation): void {
      this.isModalDialogVisible = true;
      this.editForm.setValue({
        editType: vacation.type,
        editStart: vacation.start.substring(0, 10),
        editEnd: vacation.end.substring(0, 10)
      });
      this.oldVacation = vacation;
    }
  
    closeModal(): void {
      this.isModalDialogVisible = false;
    } */
  openEditModal(vacation: Vacation) {
    let dialogRef = this.dialog.open(VacationEditDialogComponent,
      { data: { id: vacation.id, type: vacation.type, start: vacation.start, end: vacation.end } });
    dialogRef.afterClosed().subscribe(result => {
      console.log(`Dialog result: ${result}`);
    });
  }
  //put
  /*editVacation() {
    alert(this.editForm.value.editType);
    let newVacation = new Vacation(
      this.oldVacation.id,
      this.editForm.value.editType,
      this.editForm.value.editStart,
      this.editForm.value.editEnd)
    this.vacationService.editVacation(newVacation)
      .subscribe(() => {
        this.vacations = this.vacations.filter(v => v !== this.oldVacation);
        this.vacations.push(newVacation);
      });
  //  this.isModalDialogVisible = false;
  }*/
}
