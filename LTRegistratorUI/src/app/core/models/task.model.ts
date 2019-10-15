import { TaskNote } from './taskNote.model';
import { Vacation } from './vacation.model';
import { CustomValue } from './customValue.model';

export class Task {
    id: number;
    name: any;
    projectId: number;
    customValues: CustomValue[] = [];
    taskNotes: TaskNote[] = [];
    vacation: Vacation[] = [];
}