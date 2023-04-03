import { Component } from '@angular/core';
import { ActivatedRoute, ActivatedRouteSnapshot, Router } from '@angular/router';
import { ForgotPassworApiServices } from 'src/app/modules/auth/apiServices/forgotPassword.service';

@Component({
  selector: 'app-check-your-mail',
  templateUrl: './check-your-mail.component.html',
  styleUrls: ['./check-your-mail.component.scss']
})
export class CheckYourMailComponent {

  stateValues:{[key: string] : string} | undefined;
  email:string;
  errorMesssage:string;

  constructor(
    private router: Router,
    private forgotPasswordApiService: ForgotPassworApiServices
    ){

    this.stateValues = this.router.getCurrentNavigation()?.extras.state;
    if(!this.stateValues){
      this.router.navigate(['/SignIn']);
    }else{
      this.email = this.stateValues['email'];
    }
  }

  Resend(){
    this.forgotPasswordApiService.ForgotPasswordApi(this.email).subscribe({
      next: () =>{
        this.router.navigate(['/CheckYourMail'], {state: {email: this.email}});
      },
      error: () =>{
        this.errorMesssage = "Something went wrong. Try Again!";
      } 
    });
  }

  OpenMail(){
    window.open("https://mail.google.com/mail");
  }        
}
