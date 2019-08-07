import { Project } from './project.model';

export class User {
  private _id: number;
  private _name: string;
  private _surname: string;
  private _mail: string;
  private _role: any;
  private _projects: Project[];

  constructor(employeeId: number, firstName: string, secondName: string,
              mail: string, maxRole: string, projects: any[]) {
    this._id = employeeId;
    this._name = firstName;
    this._surname = secondName;
    this._mail = mail;
    this._role = maxRole;
    this._projects = projects.map((project: any) => new Project(project.projectId, project._name));
  }

  get id(): number {
    return this._id;
  }

  set id(value: number) {
    this._id = value;
  }

  get mail(): string {
    return this._mail;
  }

  set mail(value: string) {
    this._mail = value;
  }

  get projects(): Project[] {
    return this._projects;
  }

  set projects(value: Project[]) {
    this._projects = value;
  }

  get name(): string {
    return this._name;
  }

  set name(value: string) {
    this._name = value;
  }

  get surname(): string {
    return this._surname;
  }

  set surname(value: string) {
    this._surname = value;
  }

  get role(): any {
    return this._role;
  }

  set role(value: any) {
    this._role = value;
  }
}
