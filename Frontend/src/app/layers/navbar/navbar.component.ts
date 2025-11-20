import { Component,HostListener } from '@angular/core';
import { Router,NavigationEnd } from '@angular/router';
import { filter } from 'rxjs/operators';

@Component({
  selector: 'app-navbar',
  standalone: false,
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.scss']
})
export class NavbarComponent {
menuOpen=false;
currentPage='';
  toggleMenu(){
    this.menuOpen=!this.menuOpen;
  }
  @HostListener('document:click', ['$event'])
  handleOutsideClick(event: Event) {
    const target = event.target as HTMLElement;
    const clickedInside = target.closest('.menu-container');
    
    if (!clickedInside) {
      this.menuOpen = false; // 👈 Close menu if clicked outside
    }
  }
  onFile(){
    alert('File Option clicked');
    this.menuOpen=false;
  }
  constructor(private router:Router){
     this.router.events
      .pipe(filter(event => event instanceof NavigationEnd))
      .subscribe((event: NavigationEnd) => {
        this.updatePageTitle(event.urlAfterRedirects);
      });
  }
  onExit(){
    this.router.navigate(['/login']);
  }
  
    updatePageTitle(url: string) {
    if (url.includes('dashboard')) this.currentPage = 'Dashboard';
    else if (url.includes('products')) this.currentPage = 'Products';
    else if (url.includes('categories')) this.currentPage = 'Categories';
    else if (url.includes('login')) this.currentPage = 'Login';
    else this.currentPage = '';
  }
}
