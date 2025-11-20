import { Component } from '@angular/core';
import { SidebarService } from '../sidebar/sidebar.service';

@Component({
  selector: 'app-main-content',
  standalone: false,
  templateUrl: './main-content.component.html',
  styleUrls: ['./main-content.component.scss']
})
export class MainContentComponent {
  constructor(private sidebarService:SidebarService){}
  toggleSidebar(){
    this.sidebarService.toggle();
  }

}
