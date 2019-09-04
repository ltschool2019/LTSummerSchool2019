import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs/Observable';
import { map } from 'rxjs/operators';

import { Task } from '../models/task.model';

export interface NoIdTaskNote {
  Day: string;
  Hours: number;
}
@Injectable({
  providedIn: 'root'
})
export class EmployeeService {

  constructor(private http: HttpClient) { }
  // get
  public getTasks(userId: number, projectId: number, startDate: any, endDate: any): Observable<Task[]> {
    return this.http.get<Task>(`${this.getUrl(userId, projectId)}/?startDate=${startDate}&EndDate=${endDate}
    `).pipe(
      map((data: any) =>
        data.map((task: any) => { return new Task(task.id, task.name, task.taskNotes, task.leave); }))
      // map((data: any) => { console.log(data); return new Task(data.id, data.name, data.taskNotes, data.leave); })
    )
  }
  //post 
  //это первый запрос, если таск пустой
  public addTask(userId: number, projectId: number, task: Task): Observable<any> {

    let noId: NoIdTaskNote[] = [];
    for (let i = 0; i < task.taskNotes.length; i++) {
      noId.push({ Day: task.taskNotes[i].day, Hours: task.taskNotes[i].hours });
    }

    let body = {
      Name: `${task.name}`,
      TaskNotes: noId
    }

    return this.http.post(this.getUrl(userId, projectId), body);
  }
  // put
  public editTask(userId: number, taskId: number, task: Task): Observable<any> {
    let taskNotes: NoIdTaskNote[] = [];
    for (let i = 0; i < task.taskNotes.length; i++) {
      taskNotes.push({ Day: task.taskNotes[i].day, Hours: task.taskNotes[i].hours });
    }
    let body = {
      Name: `${task.name}`,
      Id: `${taskId}`,
      TaskNotes: taskNotes
    }
    return this.http.put(`http://localhost:5000/api/Task/employee/${userId}`, body);
  }
  // delete
  // api/employee/{EmployeeID}/leaves?leaveID=2
  public deleteTask(userId: number, taskId: number): Observable<any> {
    return this.http.delete<Task>(`http://localhost:5000/api/task/${taskId}/employee/${userId}`);
  }

  private getUrl(userId: number, projectId: number) {
    return `http://localhost:5000/api/task/project/${projectId}/employee/${userId}`;
  }
}
