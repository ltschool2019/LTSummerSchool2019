import { isNumber } from 'util';

export class Vacation {
    id: number;
    type: any;
    //FIXME: Сделать тип - date 
    start: string;
    end: string;

    vacationTypes = ['Больничный', 'Отпуск', 'Обучение', 'Простой'];

    constructor(leaveId: number, typeLeave: any, startDate: string, endDate: string) {
        this.id = leaveId;
        if (isNumber(typeLeave)) {
            this.type = this.vacationTypes[typeLeave];
        }
        else this.type = typeLeave;
        this.start = startDate;
        this.end = endDate;
    }

}