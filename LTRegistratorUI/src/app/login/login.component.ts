import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Router } from "@angular/router";

import { LoginService } from 'src/app/core/service/login.service'
import { OverlayService } from "../shared/overlay/overlay.service";

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {

  loginForm: FormGroup;

  constructor(
    private fb: FormBuilder,
    private loginService: LoginService,
    private router: Router,
    private overlayService: OverlayService
  ) {}

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
    this.overlayService.clear();

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
        this.overlayService.danger(
          'try again'
        )
      });
  }
}
