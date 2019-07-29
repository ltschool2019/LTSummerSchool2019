import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { AppComponent } from './app.component';
import { FormsModule } from '@angular/forms';
import { EmployeeComponent } from './employee/employee.component';
import { SideMenuComponent } from './side-menu/side-menu.component';
import { HeaderComponent } from './header/header.component';
import { LoginComponent } from './login/login.component';
import { VacationComponent } from './vacation/vacation.component';
import { NotFoundComponent } from './not-found/not-found.component';
import { TimesheetComponent } from './timesheet/timesheet.component';
import { AppRoutingModule } from './app-routing.module';
import { UserComponent } from './user/user.component';
import { AdminComponent } from './admin/admin.component';
import { EmployeeFilterPipe } from './admin/employee-filter.pipe';

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
    AdminComponent,
    EmployeeFilterPipe,
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    ReactiveFormsModule,
    FormsModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
