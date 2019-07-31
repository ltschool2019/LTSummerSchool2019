import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpRequest } from '@angular/common/http';

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


  //get

  getVacations(): Observable<Vacation[]> {
    /*return this.http.get<Vacation[]>(this.vacationsUrl).pipe(map(data => {
      let vacationsList = data[null];
      return vacationsList.map((vacation: any) =>
        new Vacation(vacation.LeaveId, vacation.TypeLeave, vacation.StartDate, vacation.EndDate));
    }));*/
    return this.http.get<Vacation[]>(this.vacationsUrl);
  }

  //post
  addVacation(vacation: Vacation): Observable<any> {
    let vacationTypes = ['Больничный', 'Отпуск', 'Обучение', 'Простой'];
    const body = {
      TypeLeave: vacationTypes.indexOf(vacation.type),
      StartDate: `${vacation.start}T00:00:00`,
      EndDate: `${vacation.end}T00:00:00`
      //.toISOString()
    };
    console.log(body);
    return this.http.post(this.vacationsUrl, [body]);
  }
  //put
  editVacation(vacation: Vacation): Observable<any> {
    let vacationTypes = ['Больничный', 'Отпуск', 'Обучение', 'Простой'];
    const body = {
      LeaveId: vacation.id,
      TypeLeave: vacationTypes.indexOf(vacation.type),
      StartDate: `${vacation.start}T00:00:00`,
      EndDate: `${vacation.end}T00:00:00`
      //.toISOString()
    };
    return this.http.put(this.vacationsUrl, [body]);
  }
  //delete

  deleteVacation(vacation: Vacation): Observable<Vacation> {
    //FIXME: заставь меня работать
    return; //this.http.request<Vacation>(req);
  }
}
