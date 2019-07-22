export class Project {
    name: string;
    hours: number;
    id: number;

    constructor(name: string, hours: number, id: number) {
        this.name = name;
        this.hours = hours;
        this.id = id;
    }
}