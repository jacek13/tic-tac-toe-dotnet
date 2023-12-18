import {Component, OnInit} from '@angular/core';
import {FormBuilder, FormsModule, ReactiveFormsModule, Validators} from "@angular/forms";
import { Router, RouterModule } from '@angular/router';
import { catchError, map, tap } from 'rxjs';
import { AuthService } from '../auth.service';
import { NgFor, NgIf } from '@angular/common';

const TOKEN_NAME = 'ACCESS_TOKEN'

@Component({
  selector: 'app-signin',
  templateUrl: './signup-confirm.component.html',
  standalone: true,
  imports: [NgIf, NgFor, FormsModule, ReactiveFormsModule, RouterModule],
})
export class SignUpConfirmComponent implements OnInit{

  constructor(
    private readonly formBuilder: FormBuilder,
    private readonly authService: AuthService,
    private readonly router: Router) {
  }

  loginForm = this.formBuilder.group({
      username: ['', {validators: [Validators.required], updateOn: 'blur'}],
      code: ['', {validators: [Validators.required], updateOn: 'blur'}]
    }
  )

  ngOnInit(): void {
  }

  onLogin(): void {
    this.authService.signUpConfirm({
      Email: this.loginForm.value.username!,
      ConfirmationCode: this.loginForm.value.code!
    }).pipe(
      catchError(error => { throw error }),
      tap((respone: any) => {
        if(respone) {
            this.router.navigate(['/auth/sign-in']);
        }
        else {
            this.router.navigate(['/auth/sign-up/confirm']);
        }
      })).subscribe();
  }
}