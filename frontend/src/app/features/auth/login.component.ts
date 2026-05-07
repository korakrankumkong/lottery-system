import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { AuthService } from '../../core/auth.service';

@Component({
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, MatCardModule, MatFormFieldModule, MatInputModule, MatButtonModule],
  template: `<mat-card class="max-w-md mx-auto mt-16"><h2 class="text-xl font-semibold mb-4">Sign in</h2><form [formGroup]="form" (ngSubmit)="submit()" class="space-y-3"><mat-form-field class="w-full"><mat-label>Username</mat-label><input matInput formControlName="username"></mat-form-field><mat-form-field class="w-full"><mat-label>Password</mat-label><input matInput type="password" formControlName="password"></mat-form-field><button mat-flat-button color="primary" class="w-full" [disabled]="loading">{{ loading ? 'Signing in...' : 'Login' }}</button><p *ngIf="error" class="text-sm text-red-600">{{ error }}</p></form></mat-card>`
})
export class LoginComponent {
  form: FormGroup;
  loading = false;
  error = '';

  constructor(private fb: FormBuilder, private auth: AuthService, private router: Router) {
    this.form = this.fb.group({ username: ['', Validators.required], password: ['', Validators.required] });
  }

  submit() {
    if (this.form.invalid || this.loading) return;
    this.loading = true;
    this.error = '';

    this.auth.login(this.form.value.username, this.form.value.password).subscribe({
      next: () => {
        this.loading = false;
        this.router.navigate(['/dashboard']);
      },
      error: () => {
        this.loading = false;
        this.error = 'Invalid username or password';
      }
    });
  }
}
