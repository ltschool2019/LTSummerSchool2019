import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppComponent } from './app.component';

import { EmployeeComponent } from './employee/employee.component';
import { SideMenuComponent } from './side-menu/side-menu.component';
import { HeaderComponent } from './header/header.component';
import { LoginComponent } from './login/login.component';
import { HttpClientModule } from '@angular/common/http';
import { LoginService } from 'src/app/core/service/login.service';


@NgModule({
  declarations: [
    AppComponent,
    EmployeeComponent,
    SideMenuComponent,
    HeaderComponent,
    LoginComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule
  ],
  providers: [
    LoginService,

  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
