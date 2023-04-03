import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { GetHttpOptions, localHostUrl } from 'src/app/constant';
import { AuthResponse } from 'src/app/modules/auth/models/authResponse';
import { SignUp } from 'src/app/modules/auth/models/sign-up/signUp';

@Injectable({
  providedIn: 'root'
})

export class SignUpApiService {
  constructor(
    private httpClient: HttpClient
  ) { }
  
  signUpApiUrl: string = localHostUrl + 'Authentication/registerUser';
  checkUniqueEmailApiUrl: string = localHostUrl + 'Authentication/checkUniqueEmail';

  SignUp(signUpData: SignUp): Observable<AuthResponse>{
    return this.httpClient.post<AuthResponse>(this.signUpApiUrl, signUpData, GetHttpOptions());
  }

  CheckUniqueEmail(emailAddress: string): Observable<any>{
    return this.httpClient.get(this.checkUniqueEmailApiUrl, {params: {email: emailAddress}});
  }
}
