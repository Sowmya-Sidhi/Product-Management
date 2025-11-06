import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  standalone: false,
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss'
})
export class RegisterComponent {
 email:string='';
  password:string='';
  constructor(private router:Router){}
  onRegister(){
    console.log('Registering',this.email);
    this.router.navigate(['/login']);
  }
}
