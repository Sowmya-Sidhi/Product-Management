import { Component, EventEmitter, Output, Input, OnChanges } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-add-product-dialog',
  standalone: false,
  templateUrl: './add-product-dialog.component.html',
  styleUrl: './add-product-dialog.component.scss'
})
export class AddProductDialogComponent implements OnChanges {
  @Output() closeForm = new EventEmitter<void>();
  @Output() formSubmit = new EventEmitter<any>();
  @Input() product: any | null = null;
  @Input() existingProducts:any[]=[];
  @Input() categories:any[]=[];

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

  ngOnChanges(): void {
    if (this.product) {
      this.productForm.patchValue({
        productCode: this.product.productCode,
        productName: this.product.productName,
        price: this.product.price,
        isActive: this.product.isActive,
        categoryName: this.product.categoryName
      });
    } else {
      this.productForm.reset({ isActive: false });
    }
  }
  isDuplicate = false;
  ngOnInit() {
    this.productForm.get('productCode')?.valueChanges.subscribe(() => {
      this.checkDuplicateCode();
    });
  }

checkDuplicateCode() {
  const enteredCode = this.productForm.get('productCode')?.value?.trim();
  if (!enteredCode) {
    this.isDuplicate = false;
    return;
  }

  // Ignore current product’s own code during edit
  const isEditing = !!this.product;
  const duplicate = this.existingProducts.some(p =>
    p.productCode === enteredCode &&
    (!isEditing || p.productCode !== this.product?.productCode)
  );

  this.isDuplicate = duplicate;
}


 onSubmit() {
  if (this.isDuplicate) {
    alert('Product code already exists!');
    return;
  }

  if (this.productForm.valid) {
    this.formSubmit.emit(this.productForm.value);
    this.productForm.reset({ isActive: false });
    this.closeForm.emit();
  } else {
    alert('Please fill all required fields correctly!');
  }
}

}
