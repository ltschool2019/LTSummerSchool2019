export class Day {
    date: number;
    weekday: string;

    days = ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun'];

    constructor(date: number, weekday: number) {
        this.date = date;
        this.weekday = this.days[weekday - 1];
    }
}