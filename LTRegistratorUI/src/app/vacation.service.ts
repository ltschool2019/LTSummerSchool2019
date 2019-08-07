import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { Vacation } from './vacation.model';
import { VacationComponent } from './vacation/vacation.component';

@Injectable({
  providedIn: 'root'
})

export class VacationService {
  //FIXME: получать нужный id
  private;

  vacations: Vacation[];

  constructor(
    private http: HttpClient
  ) { }
  private getUrl(id: any) {
    return `http://localhost:5000/api/employee/${id}/leaves`;
  }

  //get

  getVacations(userId: any): Observable<Vacation[]> {
    return this.http.get<Vacation[]>(this.getUrl(userId)).pipe(
      map(data => {
        return data.map((vacation: any) =>
          new Vacation(vacation.id, vacation.typeLeave, vacation.startDate, vacation.endDate));

      }));
  }

  //post
  addVacation(userId: any, vacation: Vacation): Observable<any> {
    const body = {
      TypeLeave: `${vacation.type}`,
      StartDate: `${vacation.start}`,
      EndDate: `${vacation.end}`
    };
    return this.http.post(this.getUrl(userId), [body]);
  }
  //put
  editVacation(userId: any, vacation: Vacation): Observable<any> {
    const body = {
      id: vacation.id,
      TypeLeave: vacation.type,
      StartDate: vacation.start,
      EndDate: vacation.end
    };
    return this.http.put(this.getUrl(userId), [body]);
  }
  //delete
  //api/employee/{EmployeeID}/leaves?leaveID=2
  deleteVacation(id: number, userId): Observable<any> {
    //FIXME: заставь меня работать
    return this.http.delete<Vacation>(this.getUrl(userId) + `?leaveID=${id}`);
  }
}
