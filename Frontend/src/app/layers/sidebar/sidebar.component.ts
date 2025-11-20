import { Component, viewChild } from '@angular/core';
import { MatDrawer } from '@angular/material/sidenav';
import { SidebarService } from './sidebar.service';
import { Output } from '@angular/core';
import { EventEmitter } from '@angular/core';

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
drawer = viewChild<MatDrawer>('drawer');

  
 constructor(private sidebarService:SidebarService) {}
 

ngAfterViewInit() {
  this.sidebarService.register(this.drawer()!);
}
isMobile = false;

ngOnInit() {
  this.checkScreen();
  window.addEventListener('resize', () => this.checkScreen());
}

checkScreen() {
  this.isMobile = window.innerWidth <= 768;
}
@Output() close = new EventEmitter();

closeSidebar() {
  this.close.emit();
}



}
