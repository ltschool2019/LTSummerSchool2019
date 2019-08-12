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
import { AppRoutingModule } from './app-routing.module';
import { UserComponent } from './user/user.component';
import { EmployeesTableComponent } from './employee-table/employee-table.component';
import { LoginService } from 'src/app/core/service/login.service';
import { MaterialModule } from './material.module';
import { AdminComponent } from './admin/admin.component';

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
    LoginComponent,
    UserComponent,
    EmployeesTableComponent,
    AdminComponent,
  ],
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
    {provide: HTTP_INTERCEPTORS, useClass: JwtInterceptor, multi: true}
  ],
  bootstrap: [AppComponent],
  entryComponents: [EmployeesTableComponent]
})
export class AppModule {
}
