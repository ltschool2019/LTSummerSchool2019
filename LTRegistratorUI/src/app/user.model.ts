import { Project } from './project.model';
export class User {
    id: number=0;
    name: string='';
    surname: string='';
    mail: string='';
    role: any='';
    projects: Project[];
    roleTypes = ['', '(Manager)', '(Admin)'];

    constructor(employeeId: number, firstName: string, secondName: string,
        mail: string, maxRole: number, projects: Project[]) {
        this.id = employeeId;
        this.name = firstName;
        this.surname = secondName;
        this.mail = mail;
        //this.role = maxRole;
        this.role = this.roleTypes[maxRole];
        this.projects = projects;
    }
}