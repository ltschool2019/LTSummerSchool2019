import { CustomField } from './customField.model';
import { User } from './user.model';

export class Project {
    id: number;
    name: string;
    totalHours: number;
    customFields: Array<CustomField> = [];
    employees: Array<User> = [];

    constructor(id?: number, name?: string, totalHours?: number) {
        this.id = id;
        this.name = name;
        this.totalHours = totalHours;
        this.customFields = new Array<CustomField>();
        this.employees = new Array<User>();
    }
}