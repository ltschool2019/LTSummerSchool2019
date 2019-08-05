import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { Vacation } from './vacation.model';

@Injectable({
  providedIn: 'root'
})

export class VacationService {
  //FIXME: получать нужный id
  private id = 2;
  private vacationsUrl = `http://localhost:52029/api/employee/${this.id}/leaves`;

  vacations: Vacation[];

  constructor(
    private http: HttpClient
  ) { }


  //get

  getVacations(): Observable<Vacation[]> {
    return this.http.get<Vacation[]>(this.vacationsUrl).pipe(
      map(data => {
        return data.map((vacation: any) =>
          new Vacation(vacation.id, vacation.typeLeave, vacation.startDate, vacation.endDate));

      }));
  }

  //post
  addVacation(vacation: Vacation): Observable<any> {
    const body = {
      TypeLeave: `${vacation.type}`,
      StartDate: `${vacation.start}`,
      EndDate: `${vacation.end}`
    };
    return this.http.post(this.vacationsUrl, [body]);
  }
  //put
  editVacation(vacation: Vacation): Observable<any> {
    const body = {
      id: vacation.id,
      TypeLeave: vacation.type,
      StartDate: vacation.start,
      EndDate: vacation.end
    };
    return this.http.put(this.vacationsUrl, [body]);
  }
  //delete

  deleteVacation(vacation: Vacation): Observable<Vacation> {
    //FIXME: заставь меня работать
    return; //this.http.request<Vacation>(req);
  }
}
