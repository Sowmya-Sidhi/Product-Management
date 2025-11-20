import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  standalone: false,
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  title = 'product-management';
   constructor(public router: Router) {}
    isAuthPage(): boolean {
    const route = this.router.url;
    return route === '/login' || route === '/register'|| route ==='/resetpassword';
  }
}
