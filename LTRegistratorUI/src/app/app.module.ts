import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { FormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { AppComponent } from './app.component';
import { EmployeeComponent } from './employee/employee.component';
import { SideMenuComponent } from './shared/side-menu/side-menu.component';
import { HeaderComponent } from './shared/header/header.component';
import { LoginComponent } from './login/login.component';
import { JwtInterceptor } from 'src/app/core/service/interceptor.service';
import { VacationComponent } from './vacation/vacation.component';
import { NotFoundComponent } from './not-found/not-found.component';
import { TimesheetComponent } from './timesheet/timesheet.component';
import { AppRoutingModule } from './app-routing.module';
import { UserComponent } from './user/user.component';
import { EmployeesTableComponent } from './employee-table/employee-table.component';
import { LoginService } from 'src/app/core/service/login.service';
import { MaterialModule } from './material.module';
import { AdminComponent } from './admin/admin.component';
import { VacationEditDialogComponent } from './vacation/vacation-edit-dialog/vacation-edit-dialog.component';
import { ManagerProjectsComponent } from './manager-projects/manager-projects.component';
import { VacationService } from './core/service/vacation.service';
import { TimesheetResolverService } from './timesheet/timesheet-resolver.service';


@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    VacationComponent,
    NotFoundComponent,
    TimesheetComponent,
    EmployeeComponent,
    SideMenuComponent,
    HeaderComponent,
    UserComponent,
    VacationEditDialogComponent,
    EmployeesTable,
    EmployeesTableComponent,
    AdminComponent,
    ManagerProjectsComponent
  ],
  entryComponents: [VacationEditDialogComponent, EmployeesTableComponent],
  imports: [
    BrowserModule,
    MaterialModule,
    BrowserAnimationsModule,
    AppRoutingModule,
    ReactiveFormsModule,
    FormsModule,
    HttpClientModule,
    FormsModule,
  ],
  providers: [
    LoginService,
    VacationService,
    FormsModule,
    HttpClientModule,
    TimesheetResolverService,
    {provide: HTTP_INTERCEPTORS, useClass: JwtInterceptor, multi: true},
    MaterialModule,
    BrowserAnimationsModule,
    AppRoutingModule,
    ReactiveFormsModule
  ],
  bootstrap: [AppComponent],
})
export class AppModule {
}
