import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';

@Injectable({
    providedIn: 'root'
  })
  
export class ProjectService {
    constructor(private http: HttpClient) {}

    getProjectDetails(id: number) {
        let url = environment.apiBaseUrl + `api/manager/project/${id}`;
        return this.http.get(url);
    }
}