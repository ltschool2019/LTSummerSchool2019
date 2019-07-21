import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, FormControl, Validators } from '@angular/forms';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {

  loginForm: FormGroup;

  constructor(private fb: FormBuilder) { }

  ngOnInit() {
    this.initForm();
  }

  initForm() {
    this.loginForm = this.fb.group({
      email: ['',[
        Validators.required,Validators.email
      ]],
      password: ['',[
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
     console.log(this.loginForm.value);
    }
}
