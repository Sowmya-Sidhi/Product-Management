import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CategoriesviewComponent } from './categoriesview.component';

describe('CategoriesviewComponent', () => {
  let component: CategoriesviewComponent;
  let fixture: ComponentFixture<CategoriesviewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [CategoriesviewComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CategoriesviewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
