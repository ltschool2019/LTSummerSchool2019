import { isNumber } from 'util';

export class Vacation {
    id: any;
    type: any;
    //FIXME: Сделать тип - date 
    start: any;
    end: any;

  //  vacationTypes = ['SickLeave', 'Vacation', 'Training', 'Idleger'];

    constructor(leaveId: any, typeLeave: string, startDate: any, endDate: any) {
        this.id = leaveId;
        /* if (isNumber(typeLeave)) {
             this.type = this.vacationTypes[typeLeave];
         }*/
        // else this.type = typeLeave;
        this.type = typeLeave;
        this.start = startDate;
        this.end = endDate;
    }

}