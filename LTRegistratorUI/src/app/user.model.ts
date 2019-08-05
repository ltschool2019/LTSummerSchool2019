import { Project } from './project.model';
export class User {
    id: number;
    name: string;
    surname: string;
    mail: string;
    role: any;
    projects: Project[];

    constructor(employeeId: number, firstName: string, secondName: string,
        mail: string, maxRole: string, projects: any[]) {
        this.id = employeeId;
        this.name = firstName;
        this.surname = secondName;
        this.mail = mail;
        this.role = maxRole;
        this.projects = projects.map((project: any) => new Project(project.projectId, project.name));
    }
}