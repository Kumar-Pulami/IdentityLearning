import { Component } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { SetNewPasswordApiServices } from 'src/app/modules/auth/apiServices/setNewPassword.service';
import { CustomPasswordValidator } from 'src/app/modules/auth/services/customValidators/customPasswordValidator';
import { EmptyFormValidator } from 'src/app/modules/auth/services/customValidators/emptyFormValidator';
import { GetValidationErrorMessage } from 'src/app/modules/auth/services/customValidators/getValidationErrorMessage';
import { MatchPasswordValidator } from 'src/app/modules/auth/services/customValidators/signUpFormValidators';

@Component({
  selector: 'app-set-password',
  templateUrl: './set-password.component.html',
  styleUrls: ['./set-password.component.scss']
})
export class SetPasswordComponent {
  hidePw: boolean = true;
  hideCfPw: boolean = true;
  email:string;
  token:string;
  setPasswordForm: FormGroup;
  errorMessage:boolean = false;

  constructor(
    private router: Router,
    private setNewPasswordApiServices: SetNewPasswordApiServices,
    private activatedRoute: ActivatedRoute
  ){
    this.email = this.activatedRoute.snapshot.queryParams['email'];
    this.token = this.activatedRoute.snapshot.queryParams['token'];
  }


  ngOnInit(){
    this.setPasswordForm = new FormGroup({
      password: new FormControl (null, CustomPasswordValidator()),
      confirmPassword: new FormControl (null, Validators.required)
    }, MatchPasswordValidator('password', 'confirmPassword'))
  }

  ResetPassword(){
    if(this.setPasswordForm.invalid){
      EmptyFormValidator(this.setPasswordForm);
    }else{
      this.setNewPasswordApiServices.SetNewPassword(this.email, this.token, this.setPasswordForm.controls['password'].value).subscribe({
        next: () => {
          this.router.navigate(['/SignIn']);
        },
        error: () =>{
          this.errorMessage = true;
        }
      });
    }
  }

  GetErrorMessage(formContorlName: string){
    return GetValidationErrorMessage(this.setPasswordForm.get(formContorlName));
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
