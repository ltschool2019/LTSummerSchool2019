export class Project {
    id: number;
    name: string;
    totalHours: number;
    
    constructor(id: number, name: string, totalHours: number) {
        this.id = id;
        this.name = name;
        this.totalHours = totalHours;
    }
}