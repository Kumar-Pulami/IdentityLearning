import { Injectable } from "@angular/core";

@Injectable({
    providedIn: 'root'
})

export class SessionStorageService {
    
    Set(key: string, value: string) {
        sessionStorage.setItem(key, value);
    }

    Get(key: string) {
        return sessionStorage.getItem(key);
    }

    Remove(key: string) {
        sessionStorage.removeItem(key);
    }

    RemoveAll(){
        sessionStorage.clear();
    }
}