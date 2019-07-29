export class User {
    id: number;
    name: string;
    surname: string;
    mail: string
    role: any;
    projects: {projectId:number, name:string};

    constructor(employeeId: number, firstName: string, secondName: string, mail: string, maxRole: number) {
        this.id = employeeId;
        this.name = firstName;
        this.surname = secondName;
        this.mail = mail;
        //this.role = maxRole;
        switch (maxRole){
            case(0):{
                this.role='';
                break;
            }
            case (1):{
                this.role='(Менеджер)';
                break;
            }
            case(2):{
                this.role='(Администратор)';
                break;
            }
        }
    }
}