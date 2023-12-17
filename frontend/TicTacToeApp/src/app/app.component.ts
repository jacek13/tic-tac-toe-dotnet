import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { AuthService } from './auth/auth.service';
import { catchError, tap } from 'rxjs';
import { Router } from '@angular/router';

const TOKEN_NAME = 'ACCESS_TOKEN'

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit  {
  private accessToken: string | null;
  public userInfo: any | null;
  public isLoggedIn: boolean;

  constructor(private authService: AuthService, private cdr: ChangeDetectorRef, private readonly router: Router) {
    this.isLoggedIn = false;
    this.accessToken = this.authService.getAuthorizationToken();
  }

  ngOnInit(): void {
    this.accessToken = this.authService.getAuthorizationToken();
    if (this.accessToken) {
      this.authService.meInfo().pipe(
        catchError(error => { 
          console.log("user is not logged");
          this.isLoggedIn = false;
          throw error 
        }),
        tap((respone: any) => {
          this.userInfo = respone;
          console.log(this.userInfo);
          console.log("user is logged");
          this.isLoggedIn = true;
          this.cdr.detectChanges();
        })).subscribe();
    }
    else {
      console.log("user is not logged");
      this.isLoggedIn = false;
    }
  }

  signOut(): void {
    this.accessToken = this.authService.getAuthorizationToken();
    if (this.accessToken) {
      this.authService.signOut().pipe(
        catchError(error => { 
          throw error 
        }),
        tap((respone: any) => {
          this.userInfo = null;
          console.log("user signed out");
          this.isLoggedIn = false;
          this.cdr.detectChanges();
        })).subscribe();
    }
    else {
      console.log("user is already signed out");
      this.isLoggedIn = false;
    }
  }

  title = 'TicTacToeApp';
}
