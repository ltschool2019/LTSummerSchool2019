export class Vacation {
    LeaveId: number;
    TypeLeave: string;
    StartDate: string;
    EndDate: string;

    constructor(LeaveId: number, TypeLeave: number, StartDate: string, EndDate: string) {
        this.LeaveId = LeaveId;
        this.TypeLeave = TypeLeave ? 'Болезнь' : 'Отпуск';
        this.StartDate = StartDate;
        this.EndDate = EndDate;
    }
}