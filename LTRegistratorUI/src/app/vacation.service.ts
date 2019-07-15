import { Injectable } from '@angular/core';
import {Observable, of} from 'rxjs';
import {Vacation} from './vacation';
import {VACATIONS} from './mock-vacations';

@Injectable({
  providedIn: 'root'
})

export class VacationService {

  constructor() { }
}
