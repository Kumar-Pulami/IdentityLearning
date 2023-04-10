import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { GetHttpOptions, localHostUrl } from 'src/app/constant';
import { AuthResponse } from 'src/app/modules/auth/models/authResponse';
import { SignUp } from 'src/app/modules/auth/models/sign-up/signUp';
import { InvitedSignUp } from '../models/sign-up/invitedSignUp';

@Injectable({
  providedIn: 'root'
})

export class SignUpApiService {
  constructor(
    private httpClient: HttpClient
  ) { }
  
  private readonly signUpApiUrl: string = localHostUrl + 'Authentication/registerUser';
  private readonly checkUniqueEmailApiUrl: string = localHostUrl + 'Authentication/checkUniqueEmail';
  private readonly invitedSignUpApiUrl: string = localHostUrl + 'Authentication/registerInvitedUser';

  SignUp(signUpData: SignUp): Observable<AuthResponse>{
    return this.httpClient.post<AuthResponse>(this.signUpApiUrl, signUpData);
  }

  CheckUniqueEmail(emailAddress: string): Observable<any>{
    return this.httpClient.get(this.checkUniqueEmailApiUrl, {params: {email: emailAddress}});
  }

  InvitedSignUp(invitedSignUpData: InvitedSignUp): Observable<AuthResponse>{
    return this.httpClient.post<AuthResponse>(this.invitedSignUpApiUrl, invitedSignUpData);
  }
}
