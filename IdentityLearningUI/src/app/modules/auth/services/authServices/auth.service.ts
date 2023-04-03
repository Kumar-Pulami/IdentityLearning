import { Injectable } from "@angular/core";
import { AuthResponse } from "src/app/modules/auth/models/authResponse";
import { LocalStorageService } from "../storageService/localStorage.service";
import { SessionStorageService } from "../storageService/sessionStorage.service";
import { JwtTokenService } from "./jwtToken.service";

@Injectable({
    providedIn: 'root'
})

export class AuthenticationService{

    constructor(
        private sessionStorageService: SessionStorageService,
        private localStorageService: LocalStorageService,
        private jwtTokenService: JwtTokenService
    ){}
    
    IsAuthenticated(){ 
        const localStorageJwtToken:string | null = this.localStorageService.Get('jwtToken');
        const localStorageRefreshToken:string | null = this.localStorageService.Get('refreshToken');
        const sessionStorageJwtToken:string | null = this.sessionStorageService.Get('jwtToken');
        const sessionStorageRefreshToken:string | null = this.sessionStorageService.Get('refreshToken');

        if(localStorageJwtToken && localStorageRefreshToken){
            return true
        }else if(sessionStorageJwtToken && sessionStorageRefreshToken){
            return true;
        }else{
            return false;
        }
    }

    OnSignIn(token: AuthResponse, rememberMe:boolean){
        this.ClearAllUserData();
        if(rememberMe){
            this.SetAuthTokenInLocalStorage(token);
        }else{
            this.SetAuthTokenInSession(token);
        }
    }

    OnSignUp(token: AuthResponse){
       this.ClearAllUserData();
       this.SetAuthTokenInLocalStorage(token);
    }

    onSignOut(){
       this.ClearAllUserData();
    }

    private ClearAllUserData(){
        this.localStorageService.RemoveAll();
        this.sessionStorageService.RemoveAll();
    }

    private SetAuthTokenInSession(token: AuthResponse){
        this.sessionStorageService.Set("jwtToken", token.jwtToken);
        this.sessionStorageService.Set("refreshToken", token.refreshToken);
        let decodedToken: {[key: string]: string} = this.jwtTokenService.GetDecodeToken(token.jwtToken);
        Object.entries(decodedToken).forEach(([key, value]) => {
            if(key === "sub" || key === "email" || key ===  "userId" || key === "name"){
                this.sessionStorageService.Set(key, value);
            }
        });
    }

    private SetAuthTokenInLocalStorage(token: AuthResponse){
        this.localStorageService.Set("jwtToken", token.jwtToken);
        this.localStorageService.Set("refreshToken", token.refreshToken);
        let decodedToken: {[key: string]: string} = this.jwtTokenService.GetDecodeToken(token.jwtToken);
        Object.entries(decodedToken).forEach(([key, value]) => {
            if(key === "sub" || key === "email" || key ===  "userId" || key === "name"){
                this.localStorageService.Set(key, value);
            }
        });
    }
}