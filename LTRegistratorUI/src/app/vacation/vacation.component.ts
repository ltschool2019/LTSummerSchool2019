import { Component, OnInit } from '@angular/core';
import { Vacation } from '../vacation.model';
import { VacationService } from '../vacation.service';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';

@Component({
  selector: 'app-vacation',
  templateUrl: './vacation.component.html',
  styleUrls: ['./vacation.component.scss']
})

export class VacationComponent implements OnInit {
  isModalDialogVisible: boolean = false;
  oldVacation: Vacation;
  vacationForm: FormGroup;
  editForm: FormGroup;
  vacationTypes: string[];

  vacations: Vacation[] = [];

  constructor(
    private fb: FormBuilder,
    private vacationService: VacationService) { }

  ngOnInit() {
    this.vacationTypes = ['Отпуск', 'Больничный', 'Обучение', 'Простой'];
    this.initForm();
    this.getVacations();
  }
  private initForm(): void {
    this.vacationForm = this.fb.group({
      type: 'Отпуск',
      start: [null, [Validators.required]],
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
    this.vacationService.getVacations().subscribe((vacations: Vacation[]) => {
      this.vacations = vacations.map(
        (vacation: any) =>
          new Vacation(+vacation.leaveId, +vacation.typeLeave, vacation.startDate, vacation.endDate));
    })
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
      this.vacationForm.value.end)
    this.vacationService.addVacation(newVacation)
      .subscribe(() => {
        this.vacations.push(newVacation);
      });
  }
  //delete
  //FIXME: не работает удаление. Пока что.
  delete(vacation: Vacation): void {
    this.vacations = this.vacations.filter(v => v !== vacation);
    this.vacationService.deleteVacation(vacation).subscribe();
  }

  //FIXME: отпуск применяется только вторым запросом
  edit(vacation: Vacation): void {
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
  }
  //put
  editVacation() {
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
    this.isModalDialogVisible = false;
  }
}
