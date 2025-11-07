import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';


@Component({
  selector: 'app-login',
  standalone: false,
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent {
  email:string='';
  password:string='';
  errorMessage: string = '';  // for showing backend error if login fails

  constructor(private authService: AuthService, private router: Router) {}

  onLogin() {
    const credentials = { email: this.email, password: this.password };

    this.authService.login(credentials).subscribe({
      next: (response) => {
        console.log('Login successful:', response);

        this.authService.saveToken(response.token);
        this.router.navigate(['/dashboard']);
        sessionStorage.getItem('token')
        sessionStorage.getItem('role')

      },
      error: (err) => {
        console.error('Login failed:', err);
        this.errorMessage = err.error || 'Invalid credentials. Please try again.';
      }
    });
  }
}
