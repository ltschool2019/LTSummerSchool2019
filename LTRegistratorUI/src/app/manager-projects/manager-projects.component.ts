import { Component, OnInit } from '@angular/core';
import { ManagerProjectsService } from 'src/app/core/service/manager_projects.service';
import { ManagerProjects } from 'src/app/shared/models/manager_projects.model';
import { FormGroup, FormControl, FormBuilder, Validators } from '@angular/forms';
import { MaterialModule } from "src/app/material.module";
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import {MatIconModule} from '@angular/material/icon';

@Component({
  selector: 'app-manager-projects',
  templateUrl: './manager-projects.component.html',
  styleUrls: ['./manager-projects.component.scss']
})
export class ManagerProjectsComponent implements OnInit {
  public man_project: MatTableDataSource<ManagerProjects[]> ;
  manProjectForm: FormGroup;
  displayedColumns: string[] = ['name', 'status', 'export','delete'];

  constructor(
    private managerProjectsService: ManagerProjectsService) { 
    }

  ngOnInit() {
    this.getManagerProjects();
  }
  
  getManagerProjects(): void {
    this.managerProjectsService.getManagerProjects()
    .subscribe((data) =>{this.man_project =  data});
   }

}
