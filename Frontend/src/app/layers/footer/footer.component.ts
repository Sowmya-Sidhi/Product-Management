import { Component } from '@angular/core';
import { interval } from 'rxjs';

@Component({
  selector: 'app-footer',
  standalone: false,
  templateUrl: './footer.component.html',
  styleUrl: './footer.component.scss'
})


 // LiveUpdateDateTime
 //UsingPipes
export class FooterComponent {
  currentDateTime!:Date;
  private intervalId:any;

  ngOnInit(){
    this.updateTime();
    this.intervalId=setInterval(()=> this.updateTime(),1000);
  }
  updateTime(){
    this.currentDateTime=new Date();
  }
  ngDestory(){
    clearInterval(this.intervalId);
  }

  }


