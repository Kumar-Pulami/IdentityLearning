import { Injectable } from "@angular/core";
import { LocalStorageService } from "./localStorage.service";
import { SessionStorageService } from "./sessionStorage.service";

@Injectable({
    providedIn: 'root'
})


export class StorageService{
    constructor(
        private localStorageService : LocalStorageService,
        private sessionStorageService : SessionStorageService,
    ){}


    GetUserId(): string{
        let localStorageId: string | null = this.localStorageService.GetUserId();
        let sessionStorageId: string | null = this.sessionStorageService.GetUserId();
        if(localStorageId){
            return localStorageId;
        }else if(sessionStorageId){
            return sessionStorageId;
        }
        return "";
    }
}