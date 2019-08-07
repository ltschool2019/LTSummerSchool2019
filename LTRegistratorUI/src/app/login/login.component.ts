import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';

import { LoginService } from 'src/app/core/service/login.service'
import { Router } from "@angular/router";

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {

  loginForm: FormGroup;

  constructor(private fb: FormBuilder, private loginService: LoginService, private router: Router) {
  }

  ngOnInit() {
    this.initForm();
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


    this.loginService.signIn(this.loginForm.get('email').value, this.loginForm.get('password').value)
      .subscribe(() => {
        this.router.navigateByUrl('user/timesheet');
      }, err => {
        // todo показать ошибку пользователю
      });
  }
}
