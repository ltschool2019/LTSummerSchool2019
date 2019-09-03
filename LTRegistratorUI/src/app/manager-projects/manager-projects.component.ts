import { Component, OnInit } from '@angular/core';
import { ManagerProjectsService } from 'src/app/core/service/manager_projects.service';
import { ManagerProjects } from 'src/app/shared/models/manager_projects.model';
import { FormGroup, FormControl, FormBuilder, Validators } from '@angular/forms';
import { MaterialModule } from "src/app/material.module";
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatIconModule } from '@angular/material/icon';
import { AddProjectDialogComponent } from 'src/app/add-project-dialog/add-project-dialog.component'
import {MatDialog, MatDialogRef, MAT_DIALOG_DATA} from '@angular/material/dialog';
import { from } from 'rxjs';

@Component({
  selector: 'app-manager-projects',
  templateUrl: './manager-projects.component.html',
  styleUrls: ['./manager-projects.component.scss'],
  
})
export class ManagerProjectsComponent implements OnInit {
  public man_project: MatTableDataSource<ManagerProjects>;
  manProjectForm: FormGroup;
  displayedColumns: string[] = ['name', 'status', 'export','delete'];

  constructor(
    public dialog: MatDialog,
    private managerProjectsService: ManagerProjectsService) { 
    }

  ngOnInit() {
    this.getManagerProjects();
    

   
  }
  openDialogAddProj():void{
    console.log(this.man_project.data);
    console.log("gfd");
    const dialogRef = this.dialog.open(AddProjectDialogComponent, {
      width: '250px'
    });

    
    dialogRef.afterClosed().subscribe(result => {
      console.log(`The dialog was ${result}`);
      this.managerProjectsService.addManagerProject(result)
      .subscribe((project) =>{
        console.log(this.man_project.data);
        this.man_project.data = [...this.man_project.data, project];
      });
    });
  }
  
  getManagerProjects(): void {
    this.managerProjectsService.getManagerProjects()
    .subscribe((data:[]) =>{
      console.log(data);
      this.man_project =  new MatTableDataSource(data)});
   }
   deleteProj(id:number):void{
     this.managerProjectsService.deleteProj(id)
     .subscribe((project)=>{
      this.man_project.data = this.man_project.data.filter((x)=>{
        return x.id !== id;
       })
     });
    console.log(this.man_project.data);
   }
   addManagerProject(projectName): void {
    this.managerProjectsService.addManagerProject(projectName)
    .subscribe((data) =>{this.man_project = data})
  }
   

  //  editVacation(value) {
  //   this.vacationService.editVacation(this.userId, newVacation)
  //     .subscribe(() => {
  //       this.vacations = this.vacations.filter(v => v.id !== newVacation.id);
  //       this.vacations.push(newVacation);
  //     }
  //     );
  // }

}
