import jwt_decode from 'jwt-decode';
import { Injectable } from "@angular/core";

@Injectable({
    providedIn: 'root'
})

export class JwtTokenService {

  GetDecodeToken(jwtToken: string):{[key: string]: string} {
    return jwt_decode(jwtToken);
  }

}