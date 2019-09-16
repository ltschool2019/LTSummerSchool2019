import { Component, OnInit, ElementRef, ViewChild } from '@angular/core';
import { ManagerProjectsService } from 'src/app/core/service/manager_projects.service';
import { ManagerProjects } from 'src/app/shared/models/manager_projects.model';
import { FormGroup } from '@angular/forms';
import { MatTableDataSource } from '@angular/material/table';
import { AddProjectDialogComponent } from 'src/app/add-project-dialog/add-project-dialog.component';
import { MatDialog } from '@angular/material/dialog';
import { OverlayService } from "../shared/overlay/overlay.service";

@Component({
  selector: 'app-manager-projects',
  templateUrl: './manager-projects.component.html',
  styleUrls: ['./manager-projects.component.scss'],
})
export class ManagerProjectsComponent implements OnInit {
  public manProject: MatTableDataSource<ManagerProjects>;
  manProjectForm: FormGroup;
  displayedColumns: string[] = ['name', 'delete'];
  @ViewChild('datePicker') datePicker: ElementRef;
  
  constructor(
    public dialog: MatDialog,
    private managerProjectsService: ManagerProjectsService,
    private overlayService: OverlayService) {}

  ngOnInit() {
    this.getManagerProjects();
	this.reportDateFC = new FormControl(new Date());
  }

  openDialogAddProj(): void {
    const dialogRef = this.dialog.open(AddProjectDialogComponent, {
      width: '350px'
    });
    dialogRef.afterClosed().subscribe(result => {
      if (!result) {
        return;
      }
      this.managerProjectsService.addManagerProject(result)
        .subscribe((project) => {
          this.manProject.data = [...this.manProject.data, project];
        }, () => {
          this.overlayService.danger('Ошибка создания');
        });
    });
  }

  getMonthlyReport(): void {
    this.managerProjectsService.getMonthlyReport(this.reportDateFC.value);
  }
  
  datePickerClose($event) {
    this.reportDateFC.setValue($event);
	this.datePicker.close();
  }	  

  getManagerProjects(): void {
    this.managerProjectsService.getManagerProjects()
      .subscribe((data: []) => {
        this.manProject = new MatTableDataSource(data);
      });
  }

  deleteProject(id: number): void {
    this.managerProjectsService.deleteProject(id)
      .subscribe((project) => {
        this.manProject.data = this.manProject.data.filter((x) => {
          return x.id !== id;
        });
      }, () => {
        this.overlayService.danger('Ошибка удаления');
      });
  }
}
