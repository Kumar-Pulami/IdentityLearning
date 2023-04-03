import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { AuthRoutingModule } from './auth-routing.module';
import { SigninPageComponent } from './components/signin-page/signin-page.component';
import { SignupPageComponent } from './components/signup-page/signup-page.component';
import { ForgotPasswordComponent } from './components/forgot-password/forgot-password.component';
import { CheckYourMailComponent } from './components/check-your-mail/check-your-mail.component';
import { SetPasswordComponent } from './components/set-password/set-password.component';
import { PageNotFoundComponent } from './components/page-not-found/page-not-found.component';
import { ReactiveFormsModule } from '@angular/forms';

@NgModule({
  declarations: [
    SigninPageComponent,
    SignupPageComponent,
    ForgotPasswordComponent,
    CheckYourMailComponent,
    SetPasswordComponent,
    PageNotFoundComponent
  ],
  
  imports: [
    CommonModule,
    AuthRoutingModule,
    ReactiveFormsModule
  ]
})
export class AuthModule { }
