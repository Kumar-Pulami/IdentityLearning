import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CheckYourMailComponent } from './components/check-your-mail/check-your-mail.component';
import { ForgotPasswordComponent } from './components/forgot-password/forgot-password.component';
import { PageNotFoundComponent } from './components/page-not-found/page-not-found.component';
import { SetPasswordComponent } from './components/set-password/set-password.component';
import { SigninPageComponent } from './components/signin-page/signin-page.component';
import { SignupPageComponent } from './components/signup-page/signup-page.component';
import { CheckYourMailGuard } from './guards/checkYourMail.guard';
import { SetPasswordGuard } from './guards/setPassword.guard';

const routes: Routes = [
  {path: 'SignIn' , component: SigninPageComponent},
  {path: "SignUp" , component: SignupPageComponent},
  {path: "ForgotPassword" , component:  ForgotPasswordComponent}, 
  {path: "CheckYourMail" , component:  CheckYourMailComponent, canActivate: [CheckYourMailGuard]},
  {path: "SetPassword" , component:  SetPasswordComponent, canActivate: [SetPasswordGuard]},
  {path: "", redirectTo: 'SignIn', pathMatch: 'full'},
  {path: "**", component: PageNotFoundComponent}
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})

export class AuthRoutingModule { }