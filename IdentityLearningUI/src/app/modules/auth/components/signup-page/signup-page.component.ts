import { Component } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthResponse } from 'src/app/modules/auth/models/authResponse';
import { SignUp } from 'src/app/modules/auth/models/sign-up/signUp';
import { SignUpApiService } from 'src/app/modules/auth/apiServices/signup.service';
import { AuthenticationService } from 'src/app/modules/auth/services/authServices/auth.service';
import { EmptyFormValidator } from 'src/app/modules/auth/services/customValidators/emptyFormValidator';
import { GetValidationErrorMessage } from 'src/app/modules/auth/services/customValidators/getValidationErrorMessage';
import { ContactNumberValidator, EmailValidator, MatchPasswordValidator, NameValidator, PasswordValidator } from 'src/app/modules/auth/services/customValidators/signUpFormValidators';

@Component({
  selector: 'app-signup-page',
  templateUrl: './signup-page.component.html',
  styleUrls: ['./signup-page.component.scss']
})

export class SignupPageComponent {

  hidePw: boolean = true;
  hideCfPw: boolean = true;
  signUpForm: FormGroup;

  constructor(
    private signUpApiService: SignUpApiService,
    private authService: AuthenticationService,
    private router: Router
  ){}

  ngOnInit(){ 
    this.signUpForm = new FormGroup({
      name : new FormControl(null, NameValidator()),
      emailAddress: new FormControl(null, EmailValidator()),
      contactNumber: new FormControl(null, ContactNumberValidator()),
      password: new FormControl(null, PasswordValidator()), 
      confirmPassword: new FormControl(null, Validators.required)
    }, MatchPasswordValidator('password', 'confirmPassword'));
  }

  public SignUp(){
    if(this.signUpForm.invalid){
      EmptyFormValidator(this.signUpForm);
    }else{
      let signUpData:SignUp = new SignUp();
      signUpData.name = this.signUpForm.controls['name'].value;
      signUpData.email = this.signUpForm.controls['emailAddress'].value;
      signUpData.phoneNumber = this.signUpForm.controls['contactNumber'].value;
      signUpData.password = this.signUpForm.controls['password'].value;

      this.signUpApiService.CheckUniqueEmail(signUpData.email).subscribe({
        next: () =>{
          this.signUpApiService.SignUp(signUpData).subscribe({
            next: (token: AuthResponse) =>{
              this.authService.OnSignUp(token);
              this.router.navigate(['Main/Dashboard']);
            },
            error: () => {
              this.signUpForm.get('emailAddress')?.setErrors({msg: "Email address already used."});
            }
          });
        },
        error: () =>{
          this.signUpForm.get('emailAddress')?.setErrors({msg: "Email address already used."});
        }
      })
    }
  }

  public GetErrorMessage(formControlName: string){
    return GetValidationErrorMessage(this.signUpForm.get(formControlName));
  }

  ToogleEyePw(){
    this.hidePw = this.ToogleEye(this.hidePw);
  }

  ToogleEyeCfPw(){
    this.hideCfPw = this.ToogleEye(this.hideCfPw);
  }

  ToogleEye(hide:boolean): boolean{
    if(hide){
      hide = false;
    }else{
      hide = true;
    }
    return hide;
  }
}
