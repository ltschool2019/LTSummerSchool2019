import { EmployeesTable } from 'src/app/employee-table/employee-table.component';
import { Component, OnInit, ViewChild } from '@angular/core';
import { MaterialModule } from "src/app/material.module";
import { MatTableDataSource } from '@angular/material/table';
import { MatSort } from '@angular/material/sort';
import { MatPaginator } from '@angular/material/paginator';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

export interface DialogData {
  parameter: boolean;
}

export interface Project {
  position: number;
  name: string;
}

const PROJECT: Project[] = [
  { position: 1, name: 'Project A' },
  { position: 2, name: 'Project B' },
  { position: 3, name: 'Project C' },
]

@Component({
  selector: 'app-admin',
  templateUrl: './admin.component.html',
  styleUrls: ['./admin.component.scss']
})

export class AdminComponent implements OnInit {
  @ViewChild(MatSort, { static: true }) sort: MatSort;
  displayedColumns: string[] = [ 'name', 'button-add', 'delete'];
  dataSource = new MatTableDataSource(PROJECT);

  constructor(public dialog: MatDialog) { }

  openDialog(): void {
    const dialogRef = this.dialog.open(EmployeesTable, {
      data: { parameter: false }
    });

    dialogRef.afterClosed().subscribe(result => {
      console.log('The dialog was closed');
    });
  }

  ngOnInit() {
    this.dataSource.sort = this.sort;
  }
    applyFilter(filterValue: string) {
      this.dataSource.filter = filterValue.trim().toLowerCase();
    }


}
