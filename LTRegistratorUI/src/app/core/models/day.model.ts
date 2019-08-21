export class Day {
    date: string;
    weekday: string;

    days = ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun'];

    constructor(date: string, weekday: number) {
        this.date = date;
        this.weekday = this.days[weekday - 1];
    }
}