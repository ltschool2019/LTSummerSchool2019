
export class Vacation {
    id: any;
    typeLeave: any;
    //FIXME: Сделать тип - date 
    startDate: any;
    endDate: any;


    constructor(leaveId: any, typeLeave: string, startDate: any, endDate: any) {
        this.id = leaveId;
        this.typeLeave = typeLeave;
        this.startDate = startDate;
        this.endDate = endDate;
    }

}