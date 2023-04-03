import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { localHostUrl } from "src/app/constant";
import { ForgotPasswordDTO } from "src/app/modules/auth/models/forgot-password/forgot-password";

@Injectable({
    providedIn: 'root'
})

export class SetNewPasswordApiServices{
    setNewPasswordApiUrl: string = localHostUrl + "Authentication/setNewPassword";

    constructor(
        private httpClient: HttpClient,
    ){}

    SetNewPassword(email: string, token:string, password:string): Observable<any>{
        return this.httpClient.post(this.setNewPasswordApiUrl, {email: email, token: token, password: password});
    }
}