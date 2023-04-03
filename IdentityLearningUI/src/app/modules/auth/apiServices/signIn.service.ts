import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { GetHttpOptions, localHostUrl } from "src/app/constant";
import { AuthResponse } from "src/app/modules/auth/models/authResponse";
import { SignIn } from "src/app/modules/auth/models/sign-in/sign-in";

@Injectable({
    providedIn: 'root'
})

export class SignInApiService{
    
    signApiUrl:string = localHostUrl + "Authentication/signIn";
    
    constructor(
        private httpClient: HttpClient
    ){}

    public SignIn(data: SignIn): Observable<AuthResponse>{
        return this.httpClient.post<AuthResponse>(this.signApiUrl, data, GetHttpOptions());
    }
}