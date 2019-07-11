export class User{
    id:number;
    name:string;
    role:string;
    login:string;
    password:string;

    constructor(){
        this.id = 1;
        this.name = "Kolya";
        this.role = 'admin';
        this.login = 'kolya@mail.ru';
        this.password = '1234';
    }

}