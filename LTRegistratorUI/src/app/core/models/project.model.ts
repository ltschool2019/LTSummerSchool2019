import { CustomField } from './customField.model';

export class Project {
    id: number;
    name: string;
    totalHours: number;
    customFields: Array<CustomField> = [];

    constructor(id?: number, name?: string, totalHours?: number) {
        this.id = id;
        this.name = name;
        this.totalHours = totalHours;
        this.customFields = new Array<CustomField>();
    }
}