import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
  HttpErrorResponse
} from '@angular/common/http';
import { BehaviorSubject, catchError, filter, firstValueFrom, Observable, switchMap, take, throwError } from 'rxjs';
import { SessionStorageService } from '../services/storageService/sessionStorage.service';
import { LocalStorageService } from '../services/storageService/localStorage.service';
import { AuthResponse } from '../models/authResponse';
import { Router } from '@angular/router';
import { AuthenticationService } from '../services/authServices/auth.service';
import { AuthenticationApiServices } from '../apiServices/authApiService.service';

@Injectable()

export class AuthInterceptor implements HttpInterceptor {

  newTokenApiResolved: boolean= false;

  constructor(
    private sessionStorageService: SessionStorageService,
    private localStorageService: LocalStorageService,
    private router: Router,
    private authService: AuthenticationService,
    private authApiService: AuthenticationApiServices
  ) {}

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    return next.handle(this.AddJwtToken(request)).pipe( 
      catchError( error => {
        if(error instanceof HttpErrorResponse && error.status === 401){
          return this.Handle401Error(request, next, error);
        }
        return throwError(() => error);
      })
    );
  }

  private Handle401Error(request: HttpRequest<any>, next: HttpHandler, error:Error): Observable<HttpEvent<unknown>> {
    let wasStoredInLocal:boolean = false;
    let localStorageJwtToken: string | null  = this.localStorageService.Get('jwtToken');
    let localStorageRefreshToken: string | null  = this.localStorageService.Get('refreshToken');
    let sessionStorageJwtToken: string | null  = this.sessionStorageService.Get('jwtToken');
    let sessionStorageRefreshToken: string | null  = this.sessionStorageService.Get('refreshToken');
    let authToken: AuthResponse = new AuthResponse();

    if(localStorageJwtToken && localStorageRefreshToken){
      authToken.jwtToken = localStorageJwtToken;
      authToken.refreshToken = localStorageRefreshToken;
      wasStoredInLocal = true;
    }else if(sessionStorageJwtToken && sessionStorageRefreshToken){
      authToken.jwtToken = sessionStorageJwtToken;
      authToken.refreshToken = sessionStorageRefreshToken;
      wasStoredInLocal = false;
    }else{
      this.router.navigate(['/SignIn']);
      return throwError(() => error);
    }

    this.authApiService.RequestNewToken(authToken).subscribe({
      next: (newAuthToken: AuthResponse) =>{
        this.authService.OnSignIn(newAuthToken, wasStoredInLocal);
        this.newTokenApiResolved = true;
      },
      error: (data) => { 
      this.authService.onSignOut();
        this.router.navigate(['/SignIn']);
        this.newTokenApiResolved = true;
      }
    });


    // let newTokenPromise: Promise<AuthResponse> = this.GetNewTokenApi(authToken);
    // newTokenPromise.then((newAuthToken: AuthResponse) =>{
    //   this.authService.OnSignIn(newAuthToken, wasStoredInLocal);
    // })

    // newTokenPromise.catch((error) => {
    //   this.authService.onSignOut();S
    //   this.router.navigate(['/SignIn']);
    // })

    return next.handle(this.AddJwtToken(request));
  }


  public AddJwtToken(request: HttpRequest<any>){
    let localStorageJwtToken: string | null  = this.localStorageService.Get('jwtToken');
    let sessionStorageJwtToken: string | null  = this.sessionStorageService.Get('jwtToken');

    if(localStorageJwtToken){
      return request.clone({setHeaders: { Authorization: `Bearer ${localStorageJwtToken}`}});
    }else if(sessionStorageJwtToken){
      return request.clone({setHeaders: { Authorization: `Bearer ${sessionStorageJwtToken}`}});
    }else{
      return request;
    }
  }

  private async GetNewTokenApi(authToken: AuthResponse): Promise<AuthResponse>{
    return await firstValueFrom(this.authApiService.RequestNewToken(authToken));
  }  
}
