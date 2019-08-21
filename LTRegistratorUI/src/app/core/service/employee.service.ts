import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs/Observable';
import { map } from 'rxjs/operators';

import { Task } from '../models/task.model';
export interface noIdTaskNote {
  Day: string;
  Hours: number;
}
@Injectable({
  providedIn: 'root'
})
export class EmployeeService {

  constructor(private http: HttpClient) { }
  // get
  public getTasks(userId: any, projectId: any, startDate: any, endDate: any): Observable<Task[]> {
    return this.http.get<Task>(`${this.getUrl(userId, projectId)}/?startDate=${startDate}&EndDate=${endDate}
    `).pipe(
      map((data: any) =>
        data.map((task: any) => { console.log(task.name); return new Task(task.id, task.name, task.taskNotes, task.leave); }))
      // map((data: any) => { console.log(data); return new Task(data.id, data.name, data.taskNotes, data.leave); })
    )
  }
  //post 
  //это первый запрос, если таск пустой
  public addTask(userId: any, projectId: any, task: Task): Observable<any> {

    let noId: noIdTaskNote[] = [];
    for (let i = 0; i < 7; i++) {
      noId.push({ Day: task.taskNotes[i].day, Hours: task.taskNotes[i].hours });
    }

    let body = {
      Name: `${task.name}`,
      TaskNotes: noId
    }

    return this.http.post(this.getUrl(userId, projectId), body);
  }
  // put
  public editTask(userId: any, projectId: any, task: Task): Observable<any> {
    let noId: noIdTaskNote[] = [];
    for (let i = 0; i < 7; i++) {
      noId.push({ Day: task.taskNotes[i].day, Hours: task.taskNotes[i].hours });
    }
    let body = {
      Name: `${task.name}`,
      Id: `${projectId}`,
      TaskNotes: noId
    }
    return this.http.put(`http://localhost:5000/api/Task/employee/${userId}`, body);
  }
  // delete
  // api/employee/{EmployeeID}/leaves?leaveID=2
  public deleteTask(userId: number, projectId: number): Observable<any> {
    return this.http.delete<Task>(`http://localhost:5000/api/task/${projectId}/employee/${userId}`);
  }

  private getUrl(userId: any, projectId: any) {
    return `http://localhost:5000/api/task/project/${projectId}/employee/${userId}`;
  }
}
