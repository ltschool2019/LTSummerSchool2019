import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Task } from '../models/task.model';

@Injectable({
    providedIn: 'root'
})
export class TaskService {
    constructor(private http: HttpClient) {}

    addNewTask(task: Task): any {
        let url = environment.apiBaseUrl + `api/task/`;
        return this.http.post(url, task);
    }

    getById(id: number) {
        let url = environment.apiBaseUrl + `api/task/${id}`;
        return this.http.get(url);
    }

    updateTask(task: Task): any {
        let url = environment.apiBaseUrl + `api/task/`;
        return this.http.put(url, task);
    }
}