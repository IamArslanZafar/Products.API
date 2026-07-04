import { CommonModule } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../core/auth.service';
import { extractErrorMessage } from '../../core/http-error.util';
import { ToastService } from '../../shared/toast/toast.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent {
  private readonly fb = inject(FormBuilder);
  private readonly authService = inject(AuthService);
  private readonly toastService = inject(ToastService);
  private readonly router = inject(Router);

  readonly mode = signal<'login' | 'register'>('login');
  readonly isSubmitting = signal(false);
  readonly showPassword = signal(false);

  readonly form = this.fb.nonNullable.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(6)]]
  });

  toggleMode(): void {
    this.mode.set(this.mode() === 'login' ? 'register' : 'login');
  }

  togglePasswordVisibility(): void {
    this.showPassword.update((value) => !value);
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.isSubmitting.set(true);
    const credentials = this.form.getRawValue();

    if (this.mode() === 'register') {
      this.submitRegister(credentials);
      return;
    }

    this.submitLogin(credentials);
  }

  private submitRegister(credentials: { email: string; password: string }): void {
    this.authService.register(credentials).subscribe({
      next: () => {
        this.isSubmitting.set(false);
        this.toastService.success('Account created! Please log in to continue.');
        this.mode.set('login');
        this.form.reset();
      },
      error: (err: HttpErrorResponse) => {
        this.isSubmitting.set(false);
        this.toastService.error(extractErrorMessage(err, 'Could not create your account. Please try again.'));
      }
    });
  }

  private submitLogin(credentials: { email: string; password: string }): void {
    this.authService.login(credentials).subscribe({
      next: () => {
        this.isSubmitting.set(false);
        this.router.navigate(['/products']);
      },
      error: (err: HttpErrorResponse) => {
        this.isSubmitting.set(false);
        this.toastService.error(extractErrorMessage(err, 'Something went wrong. Please try again.'));
      }
    });
  }
}
