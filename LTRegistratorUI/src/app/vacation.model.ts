export class Vacation {
    id: number;
    type: any;
    //FIXME: Сделать тип - date 
    start: string;
    end: string;
    constructor(leaveId: number, typeLeave: any, startDate: string, endDate: string) {
        this.id = leaveId;
        this.type = typeLeave ? 'Больничный' : 'Отпуск';
        this.start = startDate;
        this.end = endDate;
    }

}