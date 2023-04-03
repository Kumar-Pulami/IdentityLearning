import { Component } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthResponse } from 'src/app/modules/auth/models/authResponse';
import { SignIn } from 'src/app/modules/auth/models/sign-in/sign-in';
import { SignInApiService } from 'src/app/modules/auth/apiServices/signIn.service';
import { AuthenticationService } from 'src/app/modules/auth/services/authServices/auth.service';
import { EmptyFormValidator } from 'src/app/modules/auth/services/customValidators/emptyFormValidator';

@Component({
  selector: 'app-signin-page',
  templateUrl: './signin-page.component.html',
  styleUrls: ['./signin-page.component.scss']
})

export class SigninPageComponent {
  
  signInFormGroup: FormGroup;
  invalidCredentials: boolean = false;

  constructor(
    private router: Router,
    private signApiService: SignInApiService,
    private authService: AuthenticationService
  ){}

  ngOnInit(){
    if(this.authService.IsAuthenticated()){
      this.router.navigate(['/Dashboard']);
    }
    
    this.signInFormGroup = new FormGroup({
      userName: new FormControl (null, [Validators.required]),
      password: new FormControl (null, [Validators.required]),
      rememberMe: new FormControl (false)
    });

    this.signInFormGroup.valueChanges.subscribe(value =>{
      this.invalidCredentials = false;
    });

  }

  public SignIn(){
    if(this.signInFormGroup.invalid)
    {  
      EmptyFormValidator(this.signInFormGroup);
    }
    else
    {
      let signIn: SignIn = new SignIn();
      signIn.userName = this.signInFormGroup.controls['userName'].value;
      signIn.password = this.signInFormGroup.controls['password'].value;
      let rememberMeValue: boolean = this.signInFormGroup.controls['rememberMe'].value;

      this.signApiService.SignIn(signIn).subscribe({
        next: (data: AuthResponse) => {
          this.authService.OnSignIn(data, rememberMeValue);
          this.router.navigateByUrl('/Dashboard');
        },
        error: () => {
          this.invalidCredentials = true;
        } 
      });
    }
  }
}