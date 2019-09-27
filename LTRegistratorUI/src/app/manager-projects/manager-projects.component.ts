import { Component, OnInit } from '@angular/core';
import { ManagerProjectsService } from 'src/app/core/service/manager_projects.service';
import { ManagerProjects } from 'src/app/shared/models/manager_projects.model';
import { FormGroup, FormControl, FormBuilder, Validators } from '@angular/forms';
import { MatTableDataSource } from '@angular/material/table';
import { AddProjectDialogComponent } from 'src/app/add-project-dialog/add-project-dialog.component';
import { MatDialog } from '@angular/material/dialog';
import { Router } from "@angular/router";

@Component({
  selector: 'app-manager-projects',
  templateUrl: './manager-projects.component.html',
  styleUrls: ['./manager-projects.component.scss'],
})
export class ManagerProjectsComponent implements OnInit {
  public manProject: MatTableDataSource<ManagerProjects>;
  manProjectForm: FormGroup;
  displayedColumns: string[] = ['name', 'delete'];

  constructor(
    public dialog: MatDialog,
    private managerProjectsService: ManagerProjectsService, 
    private router: Router) {}

  ngOnInit() {
    this.getManagerProjects();
  }
  openDialogAddProj(): void {
    this.router.navigateByUrl('user/create_project');
  }

  getMonthlyReport(): void {
    this.managerProjectsService.getMonthlyReport(new Date());
  }

  getManagerProjects(): void {
    this.managerProjectsService.getManagerProjects()
    .subscribe((data: []) => {
      this.manProject =  new MatTableDataSource(data); });
   }
   deleteProject(id: number): void {
     this.managerProjectsService.deleteProject(id)
     .subscribe((project) => {
      this.manProject.data = this.manProject.data.filter((x) => {
        return x.id !== id;
       });
     });
   }
}
