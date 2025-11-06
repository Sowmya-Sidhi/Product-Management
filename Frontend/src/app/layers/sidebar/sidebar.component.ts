import { Component } from '@angular/core';

@Component({
  selector: 'app-sidebar',
  standalone: false,
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.scss'
})
export class SidebarComponent {
 views = [
    { name: 'Dashboard', route: '/dashboard' },
    { name: 'Products', route: '/products' },
    { name: 'Categories', route: '/categories' },
    
  ];


}
