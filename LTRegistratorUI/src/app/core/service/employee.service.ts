import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs/Observable';
import { map } from 'rxjs/operators';
import { environment } from '../../../environments/environment';

import { Task } from '../models/task.model';

export interface NoIdTaskNote {
  Day: string;
  Hours: number;
}

@Injectable({
  providedIn: 'root'
})
export class EmployeeService {

  constructor(private http: HttpClient) {
  }

  // get
  public getTasks(userId: number, projectId: number, startDate: any, endDate: any): Observable<Task[]> {
    return this.http.get<Task>(`${this.getUrl(userId, projectId)}/?startDate=${startDate}&EndDate=${endDate}
    `).pipe(
      map((data: any) =>
        data.map((task: any) => {
          var result = new Task();
          result.id = task.id;
          result.name = task.name;
          result.taskNotes = task.taskNotes;
          result.vacation = task.leave
          return result;
        }))
      // map((data: any) => { console.log(data); return new Task(data.id, data.name, data.taskNotes, data.leave); })
    );
  }

  // post
  // это первый запрос, если таск пустой
  public addTask(userId: number, projectId: number, task: Task): Observable<any> {

    const noId: NoIdTaskNote[] = [];
    for (let i = 0; i < task.taskNotes.length; i++) {
      noId.push({Day: task.taskNotes[i].day, Hours: task.taskNotes[i].hours});
    }

    const body = {
      Name: `${task.name}`,
      TaskNotes: noId
    };

    return this.http.post(this.getUrl(userId, projectId), body);
  }

  // put
  public editTask(userId: number, taskId: number, task: Task): Observable<any> {
    const taskNotes: NoIdTaskNote[] = [];
    for (let i = 0; i < task.taskNotes.length; i++) {
      taskNotes.push({Day: task.taskNotes[i].day, Hours: task.taskNotes[i].hours});
    }
    const body = {
      Name: `${task.name}`,
      Id: `${taskId}`,
      TaskNotes: taskNotes
    };
    return this.http.put(environment.apiBaseUrl + `api/Task/employee/${userId}`, body);
  }

  // delete
  // api/timesheet-edit/{EmployeeID}/leaves?leaveID=2
  public deleteTask(taskId: number): Observable<any> {
    return this.http.delete<Task>(environment.apiBaseUrl + `api/task/${taskId}`);
  }

  public getSubordinateEmployees() {
    let url = environment.apiBaseUrl + `api/employee`;
    return this.http.get(url);
  }

  private getUrl(userId: number, projectId: number) {
    return environment.apiBaseUrl + `api/task/project/${projectId}/employee/${userId}`;
  }
}
