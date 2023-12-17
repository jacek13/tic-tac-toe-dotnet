import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { ConfirmSignUpRequest, SignInRequest, SignUpRequest } from '../models/requests/auth';
import { SignInResponse, SignUpResponse } from '../models/responses/auth';

const prefix = '/auth';
const host = environment.apiUrl;
const TOKEN_NAME = 'ACCESS_TOKEN'

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  constructor(private readonly http: HttpClient) { }

  signIn = (requestBody: SignInRequest): Observable<SignInResponse> => this.http.post<SignInResponse>(`${host}${prefix}/sign-in`, requestBody);

  signUp = (requestBody: SignUpRequest): Observable<SignUpResponse> => this.http.post<SignUpResponse>(`${host}${prefix}/sign-up`, requestBody);

  signUpConfirm = (requestBody: ConfirmSignUpRequest): Observable<boolean> => this.http.post<boolean>(`${host}${prefix}/sign-up/confirm`, requestBody);

  signOut = (): Observable<any> => this.http.post<any>(`${host}${prefix}/sign-out`, null);

  meInfo = (): Observable<any> => this.http.get<any>(`${host}${prefix}/me`);

  getAuthorizationToken = () => localStorage.getItem(TOKEN_NAME);
}