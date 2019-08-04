import { Component, OnInit } from '@angular/core';
import { ManagerProjectsService } from 'src/app/core/service/manager_projects.service';
import { ManagerProjects } from 'src/app/shared/models/manager_projects.model';

@Component({
  selector: 'app-manager-projects',
  templateUrl: './manager-projects.component.html',
  styleUrls: ['./manager-projects.component.scss']
})
export class ManagerProjectsComponent implements OnInit {
  public man_project: ManagerProjects[] ;

  constructor(
    private managerProjectsService: ManagerProjectsService) { }

  ngOnInit() {
    this.getManagerProjects();
  }
  getManagerProjects(): void {
    this.managerProjectsService.getManagerProjects()
    .subscribe((data) =>{this.man_project = data});
    /* this.vacationService.getVacations().subscribe((vacations: Vacation[]) => {
       this.vacations = vacations.map(
         (vacation: any) =>
           new Vacation(+vacation.leaveId, +vacation.typeLeave, vacation.startDate, vacation.endDate));
     })*/
    //  this. .getVacations()
    //  .subscribe(vacations => this.vacations = vacations);
   }

}
