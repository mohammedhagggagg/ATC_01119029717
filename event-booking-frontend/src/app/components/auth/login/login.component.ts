import { Component } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthService } from '../../../services/auth.service';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-login',
  imports: [ReactiveFormsModule, CommonModule, RouterLink],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {

  loginForm: FormGroup = new FormGroup(
    {
      emailOrUserName: new FormControl(null, { validators: [Validators.required, Validators.email] }),
      password: new FormControl(null, { validators: [Validators.required] }),
    },
  );

  errorMessage: string = '';
  isLoading: boolean = false;
  passwordVisible: boolean = false;

  constructor(private _authService: AuthService, private _router: Router) {
  }
  ngOnInit(): void {}

  get f() {
    return this.loginForm.controls;
  }

  togglePasswordVisibility(): void {
    this.passwordVisible = !this.passwordVisible;
  }

  onSubmit(): void {
    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched();
      return;
    }
    this.loginUser();
  }

  private loginUser(): void {
    this.isLoading = true;
    const loginData = this.loginForm.value;
    console.log('Login data being sent:', loginData);
    this._authService.login(loginData).subscribe({
      next: (response) => {
        console.log('Login response:', response);
        this._authService.saveUserData(response);
        this.isLoading = false;
        this._router.navigate(['/home']);
      },
      error: (error) => {
        this.isLoading = false;
        this.errorMessage = error.error.message;
        console.log('Login error:', error);
      }
    });
  }
}
