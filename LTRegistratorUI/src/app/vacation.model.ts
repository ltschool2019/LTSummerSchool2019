export class Vacation {
    id: number;
    type: string;
    start: string;
    end: string;

    constructor(leaveId: number, typeLeave: number, startDate: string, endDate: string) {
        this.id = leaveId;
        this.type = typeLeave ? 'Болезнь' : 'Отпуск';
        this.start = startDate.substring(0,10);
        this.end = endDate.substring(0,10);
    }
}