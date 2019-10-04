import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './login/login.component';
import { UserComponent } from './user/user.component';
import { VacationComponent } from './vacation/vacation.component';
import { NotFoundComponent } from './not-found/not-found.component';
import { TimesheetComponent } from './timesheet/timesheet.component';
import { TimesheetEditComponent } from './timesheet-edit/timesheet-edit.component';
import { TimesheetResolverService } from './timesheet/timesheet-resolver.service';
import { ManagerProjectsComponent } from 'src/app/manager-projects/manager-projects.component';
import { AdminComponent } from './admin/admin.component';
import { CreateProjectComponent } from './manager-projects/create-project/create-project.component';
import { TaskListComponent } from './timesheet/task-list/task-list.component';
import { TaskDetailsComponent } from './timesheet/task-list/task-details/task-details.component';

const userRoutes: Routes = [
  {path: 'timesheet', component: TimesheetComponent, resolve: {user: TimesheetResolverService}},
  {path: '', redirectTo: 'timesheet', pathMatch: 'full'},
  {path: 'vacation', component: VacationComponent},
  {path: 'admin', component: AdminComponent},
  {path: 'manager_projects', component: ManagerProjectsComponent},
  {path: 'timesheet/edit/:id', component: TimesheetEditComponent},
  {path: 'project_details', component: CreateProjectComponent},
  {path: 'timesheet/:id/tasks', component: TaskListComponent},
  {path: 'timesheet/:id/tasks/task_details/:taskId', component: TaskDetailsComponent},
  {path: 'timesheet/:id/tasks/task_details', component: TaskDetailsComponent}
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
