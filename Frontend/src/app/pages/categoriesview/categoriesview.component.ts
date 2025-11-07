import { Component } from '@angular/core';
import { CategoriesService } from './categories.service';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-categoriesview',
  standalone: false,
  templateUrl: './categoriesview.component.html',
  styleUrl: './categoriesview.component.scss'
})
export class CategoriesviewComponent {
  categories: any[] = [];
  showForm = false;
  message = '';
  editingCategory: any = null;
  role: string | null = null;

  constructor(
    private categoriesService: CategoriesService,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.loadCategories();

    // ✅ Get user role from AuthService
    this.role = this.authService.getRole();
    console.log('User role:', this.role);
  }

  // 🔁 Load all categories
  loadCategories() {
    this.categoriesService.getCategories().subscribe({
      next: (data) => {
        this.categories = data;
        console.log('Categories loaded:', data);
      },
      error: (err) => console.error('Error loading categories:', err)
    });
  }

  // ➕ Add new category
  onAdd() {
    if (this.role !== 'Admin' && this.role !== 'Manager') {
      alert('⚠️ You do not have permission to add categories.');
      return;
    }

    this.editingCategory = null;
    this.showForm = true;
    this.message = '';
  }

  // ✏️ Edit existing category
  onEdit(category: any) {
    if (this.role !== 'Admin' && this.role !== 'Manager') {
      alert('⚠️ You do not have permission to edit categories.');
      return;
    }

    this.editingCategory = { ...category };
    this.showForm = true;
    this.message = '';
  }

  // 💾 Save (Add or Edit)
  onFormSubmit(category: any) {
    if (this.editingCategory) {
      // ✏️ Update category
      this.categoriesService.updateCategory(this.editingCategory.id, category).subscribe({
        next: () => {
          this.loadCategories();
          this.message = `Category "${category.name}" updated successfully!`;
          this.showForm = false;
          this.editingCategory = null;
        },
        error: (err) => console.error('Error updating category:', err)
      });
    } else {
      // ➕ Add new category
      this.categoriesService.addCategory(category).subscribe({
        next: () => {
          this.loadCategories();
          this.message = `Category "${category.name}" added successfully!`;
          this.showForm = false;
        },
        error: (err) => console.error('Error adding category:', err)
      });
    }
  }

  // 🗑️ Delete category (Admin only)
  onDelete(category: any) {
    if (this.role !== 'Admin') {
      alert('⚠️ Only Admins can delete categories.');
      return;
    }

    if (!confirm(`Are you sure you want to delete "${category.name}"?`)) return;

    this.categoriesService.deleteCategory(category.id).subscribe({
      next: () => {
        this.loadCategories();
        this.message = `Category "${category.name}" deleted successfully!`;
      },
      error: (err) =>{ if (err?.error?.message?.includes('cannot delete') || err?.status === 400) {
        alert(`⚠️ Category "${category.name}" cannot be deleted because it is assigned to one or more products.`);
      } else {
        console.error('Error deleting category:', err);
      }
  }
});
  }

  // 🚪 Close the form
  onFormClose() {
    this.showForm = false;
    this.editingCategory = null;
  }
}
