import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpRequest } from '@angular/common/http';

import { Observable, of } from 'rxjs';
import { catchError, map, tap, first } from 'rxjs/operators';
import { ManagerProjects } from 'src/app/shared/models/manager_projects.model';

@Injectable({
  providedIn: 'root'
})

export class ManagerProjectsService {
  private id = 2;
  private managerProjectsUrl = `http://localhost:53635/api/manager/${this.id}/projects`;

  constructor(
    private http: HttpClient
  ) { }

  getManagerProjects():any {
    return this.http.get<any>(this.managerProjectsUrl)
  }
  addManagerProject(projectName:any):any {
    return this.http.post<any>(this.managerProjectsUrl, {name: projectName});
  }

  // addVacation(userId: any, vacation: Vacation): Observable<any> {
  //   const body = {
  //     TypeLeave: `${vacation.type}`,
  //     StartDate: `${vacation.start}`,
  //     EndDate: `${vacation.end}`
  //   };
  //   return this.http.post(this.getUrl(userId), [body]);
  // }

}
