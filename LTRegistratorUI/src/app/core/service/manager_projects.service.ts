import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpRequest, HttpErrorResponse } from '@angular/common/http';
import { catchError, tap } from 'rxjs/operators';
import { _throw } from 'rxjs-compat/observable/throw';

@Injectable({
  providedIn: 'root'
})

export class ManagerProjectsService {
  private projectPostUrl = `http://localhost:5000/api/manager/project/`;
  constructor(
    private http: HttpClient
  ) { }

  getManagerProjects(): any {
      return this.http.get<any>(this.getManagerUrlGet());
  }
  addManagerProject(projectName: any): any {
    return this.http.post<any>(this.projectPostUrl, {name: projectName})
    .pipe(
      catchError(this.handleError)
    );
  }
  getManagerUrlGet() {
    return `http://localhost:5000/api/manager/` + localStorage.getItem('userId') + `/projects`;
  }
  deleteProject(id: number ) {
    return this.http.delete<any>(this.deleteProjectGerUrl(id));
  }

  deleteProjectGerUrl(id:number){
    return `http://localhost:5000/api/manager/project/` + id;
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
