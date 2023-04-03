import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { localHostUrl } from "src/app/constant";
import { ForgotPasswordDTO } from "src/app/modules/auth/models/forgot-password/forgot-password";

@Injectable({
    providedIn: 'root'
})

export class ForgotPassworApiServices{
    forgotPasswordApiUrl: string = localHostUrl + "Authentication/forgotPassword";

    constructor(
        private httpClient: HttpClient,
    ){}

    ForgotPasswordApi(data: string){
        return this.httpClient.get<ForgotPasswordDTO>(this.forgotPasswordApiUrl, {params: {email: data}});
    }
}