import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { FormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { AppComponent } from './app.component';
import { EmployeeComponent } from './employee/employee.component';
import { SideMenuComponent } from './side-menu/side-menu.component';
import { HeaderComponent } from './header/header.component';
import { LoginComponent } from './login/login.component';
import { JwtInterceptor } from 'src/app/helpers/jwt_interceptors.service';
import { VacationComponent } from './vacation/vacation.component';
import { NotFoundComponent } from './not-found/not-found.component';
import { TimesheetComponent } from './timesheet/timesheet.component';
import { UserComponent } from './user/user.component';

import { VacationService } from './vacation.service';
import { LoginService } from 'src/app/core/service/login.service';

import { AppRoutingModule } from './app-routing.module';

import { EmployeesTable } from './admin/admin.component';
import { MaterialModule } from "./material.module";
import { VacationEditDialogComponent } from './vacation/vacation-edit-dialog/vacation-edit-dialog.component';

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
    EmployeesTable,
    VacationEditDialogComponent
  ],
  entryComponents:[VacationEditDialogComponent],
  imports: [
    BrowserModule,
    MaterialModule,
    AppRoutingModule,
    ReactiveFormsModule,
    FormsModule,
    HttpClientModule,
    BrowserAnimationsModule
  ],
  providers: [
    LoginService,
    VacationService,
    FormsModule,
    HttpClientModule,
    { provide: HTTP_INTERCEPTORS, useClass: JwtInterceptor, multi: true },
    MaterialModule,
    BrowserAnimationsModule,
    AppRoutingModule,
    ReactiveFormsModule
  ],
  bootstrap: [AppComponent]
})
export class AppModule {
}
