import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpRequest } from '@angular/common/http';
@Injectable({
  providedIn: 'root'
})

export class ManagerProjectsService {
  private projectPostUrl = `http://localhost:5000/api/manager/project/`;
  constructor(
    private http: HttpClient
  ) { }

  getManagerProjects():any {
      return this.http.get<any>(this.getManagerUrlGet());
  }
  addManagerProject(projectName:any):any {
    return this.http.post<any>(this.projectPostUrl, {name: projectName});
  }
  getManagerUrlGet(){
    return `http://localhost:5000/api/manager/`+localStorage.getItem('userId')+`/projects`
  }
  deleteProj(id:number){
    return this.http.delete<any>(this.deleteProjectGerUrl(id));
  }

  deleteProjectGerUrl(id:number){
    return `http://localhost:5000/api/manager/project/`+id
  }
}
