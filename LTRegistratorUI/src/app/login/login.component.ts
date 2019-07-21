import { Component, OnInit } from '@angular/core';
import { User } from 'src/app/shared/models/user.model';
import {LoginService} from 'src/app/core/service/login.service'

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent {

  private email: string ='';
  private password : string ='';

  public data: any = [];
  public user:User;
  constructor(private login_service: LoginService) {};

  login(){
    this.login_service.getUser(this.email,this.password);
  }

  // ngOnInit(): void{
  //   this.api.getUser().subscribe((x:any) => {
  //     this.data = x;
  //     this.user = (JSON.parse(atob(this.data.token.split('.')[1])));
  //     console.log(this.user);
  //   });
  // }

}
