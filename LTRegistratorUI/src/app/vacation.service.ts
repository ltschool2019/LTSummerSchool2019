import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { Observable, of } from 'rxjs';
import { catchError, map, tap, first } from 'rxjs/operators';

import { Vacation } from './vacation.model';

@Injectable({
  providedIn: 'root'
})

export class VacationService {
  private id = 2;
  private vacationsUrl = `http://localhost:52029/api/employee/${this.id}/leaves`;

  constructor(
    private http: HttpClient
  ) { }

  //TODO: Получать данные с бека
  //get

  getVacations(): Observable<Vacation[]> {
    /*return this.http.get<Vacation[]>(this.vacationsUrl).pipe(map(data => {
      let vacationsList = data[null];
      return vacationsList.map((vacation: any) =>
        new Vacation(vacation.LeaveId, vacation.TypeLeave, vacation.StartDate, vacation.EndDate));
    }));*/
     return this.http.get<Vacation[]>(this.vacationsUrl);
  }
}
