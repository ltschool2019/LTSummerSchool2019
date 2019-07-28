import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, FormControl, Validators } from '@angular/forms';

import { User } from 'src/app/shared/models/user.model';
import { LoginService } from 'src/app/core/service/login.service'

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent {
  
  loginForm: FormGroup;

  constructor(private fb: FormBuilder, private loginService: LoginService) { }

  ngOnInit() {
    this.initForm();
    
  //   this.api.getUser().subscribe((x:any) => {
  //     this.data = x;
  //     this.user = (JSON.parse(atob(this.data.token.split('.')[1])));
  //     console.log(this.user);
  //   });
  }

  initForm() {
    this.loginForm = this.fb.group({
      email: ['', [
        Validators.required, Validators.email
      ]],
      password: ['', [
        Validators.required
      ]]
    });
  }
  
  onSubmit() {
    const controls = this.loginForm.controls;
    /** Проверяем форму на валидность */
    if (this.loginForm.invalid) {
      /** Если форма не валидна, то помечаем все контролы как touched*/
      Object.keys(controls)
        .forEach(controlName => controls[controlName].markAsTouched());

      /** Прерываем выполнение метода*/
      return;
    }

    /** TODO: Обработка данных формы */    
    this.loginService.getUser(this.loginForm.get('email').value, this.loginForm.get('password').value);
    // console.log(this.loginForm.value);
  }
}
