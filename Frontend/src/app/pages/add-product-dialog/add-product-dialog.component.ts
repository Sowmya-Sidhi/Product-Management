import { Component, EventEmitter, Output, Input } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-add-product-dialog',
  standalone: false,
  templateUrl: './add-product-dialog.component.html',
  styleUrl: './add-product-dialog.component.scss'
})
export class AddProductDialogComponent {
  @Output() closeForm = new EventEmitter<void>();
  @Output() submitProduct = new EventEmitter<any>();

  private _product: any = null;

  @Input()
  set product(value: any) {
    this._product = value;

    if (this._product) {
      // Patch the form with existing values for editing
      this.productForm.patchValue({
        productCode: this._product.productCode,
        productName: this._product.productName,
        price: this._product.price,
        isActive: this._product.isActive,
        categoryName: this._product.categoryName
      });
    } else {
      // Reset form for adding new product
      this.productForm.reset({ isActive: false });
    }
  }
  get product() {
    return this._product;
  }

  productForm: FormGroup;

  constructor(private fb: FormBuilder) {
    this.productForm = this.fb.group({
      productCode: ['', Validators.required],
      productName: ['', Validators.required],
      price: [null, [Validators.required, Validators.min(0)]],
      isActive: [false],
      categoryName: ['', Validators.required]
    });
  }

  onSubmit() {
    if (this.productForm.valid) {
      const formData = { ...this.productForm.value };
      
      // remove _id if editing to avoid MongoDB immutable _id error
      if (this.product && this.product._id) {
        delete formData._id;
      }

      this.submitProduct.emit(formData); // send to parent component
      this.productForm.reset({ isActive: false });
      this.closeForm.emit();
    } else {
      alert('Please fill all required fields correctly!');
    }
  }
}
