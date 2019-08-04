import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpRequest } from '@angular/common/http';

import { Observable, of } from 'rxjs';
import { catchError, map, tap, first } from 'rxjs/operators';
import { ManagerProjects } from 'src/app/shared/models/manager_projects.model';

@Injectable({
  providedIn: 'root'
})

export class ManagerProjectsService {
  private id = 2;
  private manager_projectsUrl = `http://localhost:53635/api/manager/${this.id}/projects`;
  public man_project: ManagerProjects[] ;

  constructor(
    private http: HttpClient
  ) { }


  // getManagerProjects() {
  //   return this.http.get<any>('http://localhost:53635/api/manager/2/projects')
  //   .subscribe((model: ManagerProjects[]) => this.setModelManagerProjets(model));
  // }
  // setModelManagerProjets(model) {
  //   this.man_project = model;
  // }
  getManagerProjects():any {
    // return this.http.get<ManagerProjects[]>(this.manager_projectsUrl).pipe(map(data => {
    //   debugger;
    //   return data;
    // }));
    return this.http.get<any>(this.manager_projectsUrl);

  }
}
