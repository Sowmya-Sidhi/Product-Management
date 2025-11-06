import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ReportsviewComponent } from './reportsview.component';

describe('ReportsviewComponent', () => {
  let component: ReportsviewComponent;
  let fixture: ComponentFixture<ReportsviewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ReportsviewComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ReportsviewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
