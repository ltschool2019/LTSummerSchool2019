import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
//import путей
import { LoginComponent } from './login/login.component';
import { UserComponent } from './user/user.component';
import { VacationComponent } from './vacation/vacation.component';
import { NotFoundComponent } from './not-found/not-found.component';
import { TimesheetComponent } from './timesheet/timesheet.component';
import { EmployeeComponent } from './employee/employee.component';
import { AdminComponent } from './admin/admin.component';

const userRoutes: Routes = [
  { path: 'timesheet', component: TimesheetComponent },
  { path: '', redirectTo: 'timesheet', pathMatch: 'full' },
  { path: 'vacation', component: VacationComponent },
  { path: 'timesheet/edit', component: EmployeeComponent }//позже  сделать children
]
//не удается прописать перенапрвление с user/ на user/timesheet хз, почему
const appRoutes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: '', redirectTo: '/login', pathMatch: 'full' },
  { path: 'user', component: UserComponent, children: userRoutes },
  { path: 'em_table', component: AdminComponent },
  { path: '**', component: NotFoundComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(appRoutes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
