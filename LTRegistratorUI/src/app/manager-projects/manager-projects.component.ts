import { Component, OnInit } from '@angular/core';
import { ManagerProjectsService } from 'src/app/core/service/manager_projects.service';

@Component({
  selector: 'app-manager-projects',
  templateUrl: './manager-projects.component.html',
  styleUrls: ['./manager-projects.component.scss']
})
export class ManagerProjectsComponent implements OnInit {

  constructor(
    private managerProjectsService: ManagerProjectsService) { }

  ngOnInit() {
    this.managerProjectsService.getManagerProjects();
  }
  getManagerProjects(): void {
    /* this.vacationService.getVacations().subscribe((vacations: Vacation[]) => {
       this.vacations = vacations.map(
         (vacation: any) =>
           new Vacation(+vacation.leaveId, +vacation.typeLeave, vacation.startDate, vacation.endDate));
     })*/
    //  this. .getVacations()
    //  .subscribe(vacations => this.vacations = vacations);
   }

}
