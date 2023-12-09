// game.service.ts
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class GameService {
  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  getGames(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/games`);
  }

  findRandomGame(): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/games/find-random`);
  }

  createGame(): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/games`, null);
  }
}
