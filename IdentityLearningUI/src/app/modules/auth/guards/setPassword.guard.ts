import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot} from "@angular/router";
import { localHostUrl } from "../../../constant";

@Injectable({
    providedIn: 'root'
})

export class SetPasswordGuard implements CanActivate{

    email:string | null;
    token:string | null;

    verifyUserTokenApiUrl: string = localHostUrl + "Authentication/verifyUserToken";
    constructor(
        private router: Router,
        private httpClient: HttpClient
    ){}

    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot):boolean{
        debugger
        this.email= route.queryParams['email'];
        this.token= route.queryParams['token'];
        console.log('asdfasdfsdfsdafdsf')
        console.log(this.email, this.token)
        if(!this.email || !this.token){
            this.router.navigate(['/SignIn']);
            return false;
        }
        
        this.httpClient.get(this.verifyUserTokenApiUrl, { params: {email: this.email, token: this.token, tokenType: "ForgotPassword"}}).subscribe({
            next: () => {
                return true;
            },
            error: () => { 
                this.router.navigate(['/SignIn']);
                return false;
            }
        })
        return true;
    }
}