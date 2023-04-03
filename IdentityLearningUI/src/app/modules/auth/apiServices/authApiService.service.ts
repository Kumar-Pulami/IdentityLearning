import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs/internal/Observable";
import { localHostUrl } from "src/app/constant";
import { AuthResponse } from "src/app/modules/auth/models/authResponse";
import { ForgotPasswordDTO } from "src/app/modules/auth/models/forgot-password/forgot-password";

@Injectable({
    providedIn: 'root'
})

export class AuthenticationApiServices{
    newAuthTokenApiUrl: string = localHostUrl + "Authentication/getNewJwtToken";

    constructor(
        private httpClient: HttpClient,
    ){}

    RequestNewToken(authToken: AuthResponse): Observable<AuthResponse>{
        return this.httpClient.post<AuthResponse>(this.newAuthTokenApiUrl, authToken);
    }

    async RequestNewTokenSync(authToken: AuthResponse) {
        return await this.RequestNewToken(authToken);
    }
}