import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot} from "@angular/router";

@Injectable({
    providedIn: 'root'
})

export class CheckYourMailGuard implements CanActivate{

    stateValues:{[Key:string]: string} | undefined;

    constructor(
        private router: Router
    ){
       this.stateValues = this.router.getCurrentNavigation()?.extras.state;
    }

    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot):boolean{
        if(this.stateValues === undefined){
            this.router.navigate(['/SignIn']);
            return false;
        }else{
            let email:string =  this.stateValues['email'];
            if(!email){
                this.router.navigate(['/SignIn']);
                return false;
            }
            return true;
        }
    }
}