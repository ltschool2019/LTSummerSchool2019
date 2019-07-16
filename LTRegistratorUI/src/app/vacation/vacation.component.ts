import { Component, OnInit } from '@angular/core';
import { Vacation } from '../vacation';
import { VacationService } from '../vacation.service';

@Component({
  selector: 'app-vacation',
  templateUrl: './vacation.component.html',
  styleUrls: ['./vacation.component.scss']
})

export class VacationComponent implements OnInit {
  type="Отпуск";
  vacations: Vacation[]=[
    
  ];

  constructor() { }

  ngOnInit() {
  }
  
  add(type:string,start:string,end:string):void{
    this.vacations.push(new Vacation(type,start,end));
  }
  delete(vacation:Vacation):void{
    this.vacations.splice(this.vacations.indexOf(vacation),1)
  }
}
