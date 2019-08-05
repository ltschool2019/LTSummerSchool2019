import { isNumber } from 'util';

export class Vacation {
    id: any;
    type: any;
    //FIXME: Сделать тип - date 
    start: string;
    end: string;

  //  vacationTypes = ['SickLeave', 'Vacation', 'Training', 'Idleger'];

    constructor(leaveId: any, typeLeave: string, startDate: string, endDate: string) {
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