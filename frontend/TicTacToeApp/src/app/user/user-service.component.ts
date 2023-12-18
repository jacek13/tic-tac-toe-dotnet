import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

const prefix = '/user';
const host = environment.apiUrl;

@Injectable({
  providedIn: 'root'
})
export class UserService {

  constructor(private readonly http: HttpClient) { }

  getGlobalScoreBoard = (): Observable<any> => this.http.get<any>(`${host}${prefix}/score-board`);
}