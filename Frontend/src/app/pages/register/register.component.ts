import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { RegisterService } from './register.service';

@Component({
  selector: 'app-register',
  standalone: false,
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss'
})
export class RegisterComponent {
  registerForm: FormGroup;
  message = '';

  constructor(private fb: FormBuilder, private registerService: RegisterService) {
    this.registerForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', Validators.required],
      confirmPassword: ['', Validators.required],
      role: ['User'] // optional
    });
  }

  onSubmit() {
    if (this.registerForm.invalid) {
      alert('Please fill all fields correctly!');
      return;
    }

    const { password, confirmPassword, email, role } = this.registerForm.value;
    if (password !== confirmPassword) {
      alert('Passwords do not match!');
      return;
    }

    this.registerService.register({ email, password, role }).subscribe({
      next: (res) => {
        alert('User registered successfully!');
        this.registerForm.reset();
      },
      error: (err) => {
        console.error(err);
        alert(err.error || 'Registration failed.');
      }
    });
  }
}
