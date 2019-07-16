import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
//import путей
import { LoginComponent } from './login/login.component';
import { VacationComponent } from './vacation/vacation.component';
import { NotFoundComponent } from './not-found/not-found.component';
import { TimesheetComponent } from './timesheet/timesheet.component';
//import { EditprojectComponent} from './editproject/editproject.component';


const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'vacation', component: VacationComponent },
  { path: '', redirectTo: '/login', pathMatch: 'full' },
  { path: 'timesheet', component: TimesheetComponent },
  //{ path: 'editproject/:project.name', component: EditprojectComponent },
  /* Эти еще не добавленны
  {path:'projects', component:ProjectsComponent},
  {path: 'addempolyee', component:},
  */
  { path: '**', component: NotFoundComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
