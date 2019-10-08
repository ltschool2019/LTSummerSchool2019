import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpRequest, HttpErrorResponse } from '@angular/common/http';
import { _throw } from 'rxjs-compat/observable/throw';
import { environment } from '../../../environments/environment';
import * as moment from 'moment/moment';
import { Project } from '../models/project.model';

@Injectable({
  providedIn: 'root'
})

export class ManagerProjectsService {
  private projectPostUrl = environment.apiBaseUrl + `api/manager/project/`;
  constructor(
    private http: HttpClient
  ) { }

  getManagerProjects(): any {
      return this.http.get<any>(this.getManagerUrlGet());
  }

  addManagerProject(project: Project): any {
    return this.http.post<any>(this.projectPostUrl, project);
  }

  updateManagerProject(project: Project) {
    return this.http.put<any>(this.projectPostUrl, project);
  }

  getManagerUrlGet() {
    return environment.apiBaseUrl + `api/manager/` + localStorage.getItem('userId') + `/projects`;
  }

  getMonthlyReport(date: Date) {
    return this.http.get(environment.apiBaseUrl + 'api/reports/monthly/' + moment(date).format('YYYY-MM-DD') + '/manager/' + localStorage.getItem('userId'),
    { responseType: 'arraybuffer' });
  }

  deleteProject(id: number ) {
    return this.http.delete<any>(this.deleteProjectGerUrl(id));
  }

  deleteProjectGerUrl(id:number){
    return environment.apiBaseUrl + `api/manager/project/` + id;
  }
  private handleError(error: HttpErrorResponse) {
    if (error.error instanceof ErrorEvent) {
      // A client-side or network error occurred. Handle it accordingly.
      console.error('An error occurred:', error.error.message);
    } else {
      // The backend returned an unsuccessful response code.
      // The response body may contain clues as to what went wrong,
      console.error(
        `Backend returned code ${error.status}, ` );
        alert(`Project with this name already exist, try again!`);
    }
    // return an observable with a user-facing error message
    return _throw(
      'Something bad happened; please try again later.');
  };
}
