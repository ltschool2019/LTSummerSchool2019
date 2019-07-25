import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { Vacation } from './vacation.model';
import { HttpClient } from '@angular/common/http';
import { catchError, map, tap } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})

export class VacationService {
  private id = 2;
  response: any;
  private vacationsUrl = `http://localhost:52029/api/employee/${this.id}/leaves`; 

  constructor(
    private http: HttpClient
  ) { }

  //TODO: Получать данные с бека
  //get
  
  getVacations() {
    this.http.get(this.vacationsUrl).subscribe((response => {
      this.response = response;
      console.log(this.response);
    }))
    return this.http.get<Vacation[]>(this.vacationsUrl)
      .pipe(
        tap(_ => console.log('fetched Vacations')),
        catchError(this.handleError<Vacation[]>('getVacations', []))
      );
  }/*
  //put
  updateVacation(Vacation: Vacation): Observable<any> {
    return this.http.put(this.vacationsUrl, Vacation, this.httpOptions).pipe(
      tap(_ => console.log(`updated Vacation id=${Vacation.LeaveId}`)),
      catchError(this.handleError<any>('updateVacation'))
    );
  }
  //ловим ошибки
  private handleError<T>(operation = 'operation', result?: T) {
    return (error: any): Observable<T> => {

      // TODO: send the error to remote logging infrastructure
      console.error(error); // log to console instead

      // TODO: better job of transforming error for user consumption
      console.log(`${operation} failed: ${error.message}`);

      // Let the app keep running by returning an empty result.
      return of(result as T);
    };
  }
}*/
