import { Component, OnInit } from '@angular/core';
import { LoginService } from './core/service/login.service';
import { User } from './shared/models/user.model';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  constructor(private api: LoginService) {};
  title = 'registrator-ui';
  public data: any = [];
  public user:User;
  ngOnInit(): void {
    this.api.getUser().subscribe((x:any) => {
      this.data = x;
      this.user = (JSON.parse(atob(this.data.token.split('.')[1])));
      console.log(this.user);
    });
    
  };
}
