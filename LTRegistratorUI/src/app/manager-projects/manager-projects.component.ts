import { Component, OnInit } from '@angular/core';
import { ManagerProjectsService } from 'src/app/core/service/manager_projects.service';
import { ManagerProjects } from 'src/app/shared/models/manager_projects.model';
import { FormGroup, FormControl, FormBuilder, Validators } from '@angular/forms';
import { MatTableDataSource } from '@angular/material/table';
import { AddProjectDialogComponent } from 'src/app/add-project-dialog/add-project-dialog.component'
import {MatDialog } from '@angular/material/dialog';

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
    const dialogRef = this.dialog.open(AddProjectDialogComponent, {
      width: '350px'
    });

    
    dialogRef.afterClosed().subscribe(result => {
      this.managerProjectsService.addManagerProject(result)
      .subscribe((project) =>{
        this.man_project.data = [...this.man_project.data, project];
      });
    });
  }
  
  getManagerProjects(): void {
    this.managerProjectsService.getManagerProjects()
    .subscribe((data:[]) =>{
      this.man_project =  new MatTableDataSource(data)});
   }
   
   deleteProj(id:number):void{
     this.managerProjectsService.deleteProj(id)
     .subscribe((project)=>{
      this.man_project.data = this.man_project.data.filter((x)=>{
        return x.id !== id;
       })
     });
   }
   addManagerProject(projectName): void {
    this.managerProjectsService.addManagerProject(projectName)
    .subscribe((data) =>{this.man_project = data})
  }
}
