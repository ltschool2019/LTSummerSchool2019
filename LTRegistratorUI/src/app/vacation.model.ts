
export class Vacation {
    id: any;
    type: any;
    //FIXME: Сделать тип - date 
    start: any;
    end: any;


    constructor(leaveId: any, typeLeave: string, startDate: any, endDate: any) {
        this.id = leaveId;
        this.type = typeLeave;
        this.start = startDate;
        this.end = endDate;
    }

}