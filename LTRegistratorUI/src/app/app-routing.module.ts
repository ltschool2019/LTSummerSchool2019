import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

//import путей
import { LoginComponent } from './login/login.component';
import { UserComponent } from './user/user.component';
import { VacationComponent } from './vacation/vacation.component';
import { NotFoundComponent } from './not-found/not-found.component';
import { TimesheetComponent } from './timesheet/timesheet.component';
import { EmployeeComponent } from './employee/employee.component';
import { EmployeesTable } from './admin/admin.component';
import { TimesheetResolverService } from './timesheet/timesheet-resolver.service';

const userRoutes: Routes = [
  {path: 'timesheet', component: TimesheetComponent, resolve: {user: TimesheetResolverService}},
  {path: '', redirectTo: 'timesheet', pathMatch: 'full'},
  {path: 'vacation', component: VacationComponent},
  {path: 'em_table', component: EmployeesTable},
  {path: 'timesheet/edit', component: EmployeeComponent} // FIXME:  сделать children
];

const appRoutes: Routes = [
  {path: 'login', component: LoginComponent},
  {path: '', redirectTo: '/login', pathMatch: 'full'},
  {path: 'user', component: UserComponent, children: userRoutes},
  {path: '**', component: NotFoundComponent}
];

@NgModule({
  imports: [RouterModule.forRoot(appRoutes)],
  exports: [RouterModule]
})
export class AppRoutingModule {
}
