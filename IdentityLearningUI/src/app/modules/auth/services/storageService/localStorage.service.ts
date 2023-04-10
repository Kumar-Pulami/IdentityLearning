import { Injectable } from "@angular/core";

@Injectable({
    providedIn: 'root'
})

export class LocalStorageService {

    Set(key: string, value: string) {
        localStorage.setItem(key, value);
    }

    Get(key: string) {
        return localStorage.getItem(key);
    }

    Remove(key: string) {
        localStorage.removeItem(key);
    }

    RemoveAll(){
        localStorage.clear();
    }

    GetUserId(){
        return this.Get('userId');
    }
}