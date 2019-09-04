export class TaskNote {
    id: number;
    day: string;
    hours: number;

    constructor(id: number, day: string, hours: number) {
        this.id = id;
        this.day = day;
        this.hours = hours;
    }
}