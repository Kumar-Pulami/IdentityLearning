import { HttpHeaders } from "@angular/common/http";

export const localHostUrl: string = "https://localhost:44393/api/";

export function GetHttpOptions(jwtToken?: string) {
    const httpOptions = {
        headers: new HttpHeaders({
            'Content-Type': 'application/json',
            'Authorization': "Bearer" + jwtToken
        })
    }

    return httpOptions;
}