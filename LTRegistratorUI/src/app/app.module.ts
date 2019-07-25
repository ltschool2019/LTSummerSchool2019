import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { AppComponent } from './app.component';

import { EmployeeComponent } from './employee/employee.component';
import { SideMenuComponent } from './side-menu/side-menu.component';
import { HeaderComponent } from './header/header.component';
import { LoginComponent } from './login/login.component';
import { VacationComponent } from './vacation/vacation.component';
import { NotFoundComponent } from './not-found/not-found.component';
import { TimesheetComponent } from './timesheet/timesheet.component';
import { UserComponent } from './user/user.component';

import { AppRoutingModule } from './app-routing.module';
import { HttpClientModule } from '@angular/common/http';
import { VacationService } from './vacation.service';


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
    UserComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    ReactiveFormsModule,
    HttpClientModule
  ],
  providers: [VacationService],
  bootstrap: [AppComponent]
})
export class AppModule { }
