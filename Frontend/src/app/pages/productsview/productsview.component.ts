import { Component } from '@angular/core';
import { ProductsviewService } from './productsview.service';
import { AuthService } from '../../services/auth.service'; // Import AuthService

@Component({
  selector: 'app-productsview',
  standalone: false,
  templateUrl: './productsview.component.html',
  styleUrls: ['./productsview.component.scss']
})
export class ProductsviewComponent {
  products: any[] = [];
  categories: any[] = [];
  showForm = false;
  message = '';
  editingProduct: any = null;

  role: string | null = null; // store logged-in user's role

  constructor(
    private productsviewService: ProductsviewService,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.loadProducts();
    this.loadCategories();

    // Get user role from AuthService
    this.role = this.authService.getRole();
    console.log('User role:', this.role);
  }

  // Load products
  loadProducts() {
    this.productsviewService.getProducts().subscribe({
      next: (data) => (this.products = data),
      error: (err) => console.error('Error loading products:', err)
    });
  }

  // Load categories
  loadCategories() {
    this.productsviewService.getCategories().subscribe({
      next: (data) => (this.categories = data),
      error: (err) => console.error('Error loading categories:', err)
    });
  }

  // Open form for adding a product
  onButtonClick() {
    if (this.role !== 'Admin' && this.role !== 'Manager') {
      alert('⚠️ You do not have permission to add products.');
      return;
    }
    this.editingProduct = null;
    this.showForm = true;
    this.message = '';
  }

  //  Open form for editing
  onEdit(product: any) {
    if (this.role !== 'Admin' && this.role !== 'Manager') {
      alert('⚠️ You do not have permission to edit products.');
      return;
    }
    this.editingProduct = { ...product };
    this.showForm = true;
  }

  //  Handle form submission
  onFormSubmit(product: any) {
    if (this.editingProduct) {
      // Editing existing product
      this.productsviewService.updateProduct(this.editingProduct.productCode, product).subscribe({
        next: (updated) => {
          const index = this.products.findIndex(p => p.productCode === updated.productCode);
          if (index !== -1) this.products[index] = updated;
          this.loadProducts();
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

  // Delete product (Admin only)
  onDelete(product: any) {
    if (this.role !== 'Admin') {
      alert('⚠️ Only Admins can delete products.');
      return;
    }

    if (!confirm(`Are you sure you want to delete ${product.productName}?`)) return;

    this.productsviewService.deleteProduct(product.productCode).subscribe({
      next: () => {
        this.products = this.products.filter(p => p.productCode !== product.productCode);
        this.message = `${product.productName} deleted successfully!`;
      },
      error: (err) => console.error('Error deleting product:', err)
    });
  }

  // Close form
  onFormClose() {
    this.showForm = false;
    this.editingProduct = null;
  }
}
