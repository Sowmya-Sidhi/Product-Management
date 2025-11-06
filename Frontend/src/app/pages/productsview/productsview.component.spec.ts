import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProductsviewComponent } from './productsview.component';

describe('ProductsviewComponent', () => {
  let component: ProductsviewComponent;
  let fixture: ComponentFixture<ProductsviewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ProductsviewComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ProductsviewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
