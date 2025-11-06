import { Component } from '@angular/core';
import { ProductsviewService } from './productsview.service';

@Component({
  selector: 'app-productsview',
  standalone: false,
  templateUrl: './productsview.component.html',
  styleUrl: './productsview.component.scss'
})
export class ProductsviewComponent {
  products: any[] = [];
  showForm = false; 
  message = '';
  editingProduct: any = null;

  constructor(private productsviewService: ProductsviewService) {}

  ngOnInit(): void {
    this.loadProducts();
  }

  loadProducts() {
    this.productsviewService.getProducts().subscribe({
      next: (data) => this.products = data,
      error: (err) => console.error('Error loading products:', err)
    });
  }

  onButtonClick() {
    this.editingProduct = null; // Adding new
    this.showForm = true;
    this.message = '';
  }

  onEdit(product: any) {
    this.editingProduct = { ...product }; // Copy existing product
    this.showForm = true;
  }

 onFormSubmit(product: any) {
  if (this.editingProduct) {
    // Include productCode or _id to identify, but DO NOT send _id in body
    const updatedData = { ...product };
    delete updatedData._id; // 👈 important line

    this.productsviewService
      .updateProduct(this.editingProduct.productCode, updatedData)
      .subscribe({
        next: (updated) => {
          const index = this.products.findIndex(p => p.productCode === updated.productCode);
          if (index !== -1) this.products[index] = updated;
          this.message = `${updated.productName} updated successfully!`;
          this.showForm = false;
          this.editingProduct = null;
        },
        error: (err) => console.error('Error updating product:', err)
      });
  } else {
    // Adding new product
    this.productsviewService.addProduct(product).subscribe({
      next: (added) => {
        this.products.push(added);
        this.message = `${added.productName} added successfully!`;
        this.showForm = false;
      },
      error: (err) => console.error('Error adding product:', err)
    });
  }
}


  onDelete(product: any) {
    if (!confirm(`Are you sure you want to delete ${product.productName}?`)) return;

    this.productsviewService.deleteProduct(product.productCode).subscribe({
      next: () => {
        this.products = this.products.filter(p => p.productCode !== product.productCode);
        this.message = `${product.productName} deleted successfully!`;
      },
      error: (err) => console.error('Error deleting product:', err)
    });
  }

  onFormClose() {
    this.showForm = false;
    this.editingProduct = null;
  }
}
