export class ManagerProjects {
    name: string;
    employeeId: number;
    projects: [];

    constructor(name: string, id: number, projects: []) {
        this.name = name;
        this.employeeId = id;
        this.projects = projects;
    }
}
