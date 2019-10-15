import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs/Observable';
import { map } from 'rxjs/operators';
import { environment } from '../../../environments/environment';

import { Vacation } from '../models/vacation.model';


@Injectable({
  providedIn: 'root'
})

export class VacationService {

  constructor(
    private http: HttpClient
  ) {
  }

  // get
  getVacations(userId: any): Observable<Vacation[]> {
    return this.http.get<Vacation[]>(this.getUrl(userId));
  }

  // post
  addVacation(userId: any, vacation: Vacation): Observable<any> {
    const body = {
      TypeLeave: `${vacation.typeLeave}`,
      StartDate: `${vacation.startDate}`,
      EndDate: `${vacation.endDate}`
    };
    return this.http.post(this.getUrl(userId), body);
  }

  // put
  editVacation(userId: any, vacation: Vacation): Observable<any> {
    const body = {
      id: vacation.id,
      TypeLeave: vacation.typeLeave,
      StartDate: vacation.startDate,
      EndDate: vacation.endDate
    };
    return this.http.put(this.getUrl(userId), body);
  }

  // delete
  deleteVacation(userId:number, vacationId: number): Observable<any> {
    return this.http.delete<Vacation>(this.getUrl(userId) + `?leaveId=${vacationId}`);
  }

  private getUrl(userId: any) {
    return environment.apiBaseUrl + `api/employee/${userId}/leaves`;
  }

}
