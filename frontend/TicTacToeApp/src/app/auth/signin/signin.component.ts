import {Component, OnInit} from '@angular/core';
import {FormBuilder, FormsModule, ReactiveFormsModule, Validators} from "@angular/forms";
import { Router, RouterModule } from '@angular/router';
import { catchError, map, tap } from 'rxjs';
import { AuthService } from '../auth.service';
import { NgFor, NgIf } from '@angular/common';

const TOKEN_NAME = 'ACCESS_TOKEN'

@Component({
  selector: 'app-signin',
  templateUrl: './signin.component.html',
  standalone: true,
  imports: [NgIf, NgFor, FormsModule, ReactiveFormsModule, RouterModule],
})
export class SignInComponent implements OnInit{

  constructor(
    private readonly formBuilder: FormBuilder,
    private readonly authService: AuthService,
    private readonly router: Router) {
  }

  loginForm = this.formBuilder.group({
      username: ['', {validators: [Validators.required], updateOn: 'blur'}],
      password: ['', {validators: [Validators.required], updateOn: 'blur'}]
    }
  )

  ngOnInit(): void {
  }

  onLogin(): void {
    this.authService.signIn({
      Email: this.loginForm.value.username!,
      Password: this.loginForm.value.password!
    }).pipe(
      catchError(error => { throw error }),
      tap((respone: any) => {
        localStorage.setItem(TOKEN_NAME, 'Bearer ' + respone.token)
        localStorage.setItem("userId", respone.userId)
        this.router.navigate(['/games']);
      })).subscribe();
  }
}