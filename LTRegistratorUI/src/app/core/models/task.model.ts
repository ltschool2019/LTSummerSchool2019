import { TaskNote } from './taskNote.model';
import { Vacation } from './vacation.model';

export class Task {
    id: number;
    name: any;
    taskNotes: TaskNote[];
    vacation: Vacation[];

    constructor(id: number, name: any, taskNotes: any[], vacation: any[]) {
        this.id = id;
        this.name = name;
        this.taskNotes = taskNotes.map((task: any) => new TaskNote(task.id, task.day, task.hours));
        this.vacation = vacation.map((leave: any) => new Vacation(leave.leaveId, leave.typeLeave, leave.startDate, leave.endDate));
        
    }
}