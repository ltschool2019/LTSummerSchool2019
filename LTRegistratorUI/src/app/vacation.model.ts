export class Vacation {
    // id: number;
    type: any;
    start: Date;
    end: Date;
    constructor(typeLeave: any, startDate: Date, endDate: Date) {
        //  this.id = leaveId;
        this.type = typeLeave ? 'Больничный' : 'Отпуск';
        this.start = startDate;
        this.end = endDate;
    }

}