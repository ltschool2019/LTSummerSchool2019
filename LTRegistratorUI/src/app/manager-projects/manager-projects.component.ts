import { Component, OnInit, ViewChild } from '@angular/core';
import { ManagerProjectsService } from 'src/app/core/service/manager_projects.service';
import { ManagerProjects } from 'src/app/shared/models/manager_projects.model';
import { FormControl, FormGroup } from '@angular/forms';
import { MatTableDataSource } from '@angular/material/table';
import { AddProjectDialogComponent } from 'src/app/add-project-dialog/add-project-dialog.component';
import { MatDialog } from '@angular/material/dialog';
import { OverlayService } from '../shared/overlay/overlay.service';
import { MatDatepicker } from '@angular/material';
import { Router } from "@angular/router";
import * as moment from 'moment/moment';
import * as FileSaver from 'file-saver';
import { ApiError } from '../core/models/apiError.model';

@Component({
  selector: 'app-manager-projects',
  templateUrl: './manager-projects.component.html',
  styleUrls: ['./manager-projects.component.scss'],
})
export class ManagerProjectsComponent implements OnInit {
  public manProject: MatTableDataSource<ManagerProjects>;
  manProjectForm: FormGroup;
  reportDateFC: FormControl;

  displayedColumns: string[] = ['name', 'delete'];
  showSpinner: boolean = false;

  @ViewChild('LoaderComponent', { static: true }) LoaderComponent;
  @ViewChild('datePicker', { static: false }) datePicker: MatDatepicker<Date>;

  constructor(
    public dialog: MatDialog,
    private managerProjectsService: ManagerProjectsService,
    private router: Router,
    private overlayService: OverlayService) { }

  ngOnInit() {
    this.getManagerProjects();
    this.reportDateFC = new FormControl(new Date());
  }

  createProject(): void {
    window.localStorage.removeItem("projectEditId");
    this.router.navigateByUrl('user/project_details');
  }

  updateProject(id: number): void {
    window.localStorage.removeItem("projectEditId");
    window.localStorage.setItem("projectEditId", id.toString());
    this.router.navigateByUrl(`user/project_details`);
  }

  getMonthlyReport(): void {
    this.showSpinner = true;
    this.managerProjectsService.getMonthlyReport(this.reportDateFC.value).subscribe(
      (response) => {
        let blob = new Blob([response], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
        FileSaver.saveAs(blob, 'monthly_report_' + moment(this.reportDateFC.value).format('YYYY_MM') + '.xlsx')
      },
      error => { },
      () => this.showSpinner = false
    );
  }

  datePickerClose($event) {
    this.reportDateFC.setValue($event);
    this.datePicker.close();
  }

  getManagerProjects(): void {
    this.LoaderComponent.showLoader();
    this.managerProjectsService.getManagerProjects().subscribe(
      (data: []) => {
        this.manProject = new MatTableDataSource(data);
      },
      err => {
        let apiError = <ApiError>err.error;
        this.overlayService.danger(apiError.message);
      }
    ).add(() => this.LoaderComponent.hideLoader());
  }

  deleteProject(id: number): void {
    this.managerProjectsService.deleteProject(id).subscribe(
      () => {
        this.manProject.data = this.manProject.data.filter(
          (x) => {
            return x.id !== id;
          }
        );
        this.overlayService.success("Проект успешно удален");
      },
      err => {
        let apiError = <ApiError>err.error;
        this.overlayService.danger(apiError.message);
      }
    );
  }
}
