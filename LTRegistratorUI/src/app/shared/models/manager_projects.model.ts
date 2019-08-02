export class ManagerProjects {
    name: string;
    employeeId: number;
    projects: [];
    projects_id: [];

    constructor(name: string, id: number, projects: [], projects_id: []) {
        this.name = name;
        this.employeeId = id;
        this.projects = projects;
    }
}
