import { Component } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { Router } from '@angular/router';
import { ForgotPassworApiServices } from 'src/app/modules/auth/apiServices/forgotPassword.service';
import { EmptyFormValidator } from 'src/app/modules/auth/services/customValidators/emptyFormValidator';
import { GetValidationErrorMessage } from 'src/app/modules/auth/services/customValidators/getValidationErrorMessage';
import { EmailValidator } from 'src/app/modules/auth/services/customValidators/signUpFormValidators';

@Component({
  selector: 'app-forgot-password',
  templateUrl: './forgot-password.component.html',
  styleUrls: ['./forgot-password.component.scss']
})
export class ForgotPasswordComponent {

  forgotPasswordForm:FormGroup;
  errorMesssage:string = "";

  constructor(
    private router: Router,
    private forgotPasswordApiService: ForgotPassworApiServices
  ){}

  ngOnInit(){
    this.forgotPasswordForm = new FormGroup({
      email: new FormControl(null, EmailValidator())
    })
  }

  ResetPassword(){
    if(this.forgotPasswordForm.invalid){
      EmptyFormValidator(this.forgotPasswordForm);
    }else{      
      let emailValue: string = this.forgotPasswordForm.get('email')?.value;
      this.forgotPasswordApiService.ForgotPasswordApi(emailValue).subscribe({
        next: () =>{
          this.router.navigate(['/CheckYourMail'], {state: {email: emailValue}});
        },
        error: () =>{
          this.errorMesssage = "Something went wrong. Try Again!";
        } 
      });

    }
  }

  GetErrorMessage(formControlName: string){
    return GetValidationErrorMessage(this.forgotPasswordForm.get(formControlName))
  }
}
