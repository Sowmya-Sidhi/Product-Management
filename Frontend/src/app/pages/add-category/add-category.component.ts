import { Component, EventEmitter, Output, Input, OnChanges } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { CategoriesService } from '../categoriesview/categories.service';

@Component({
  selector: 'app-add-category',
  standalone: false,
  templateUrl: './add-category.component.html',
  styleUrl: './add-category.component.scss'
})
export class AddCategoryComponent implements OnChanges {
  @Output() formSubmit = new EventEmitter<any>();
  @Output() closeForm = new EventEmitter<void>();
  @Input() category: any | null = null;
  @Input() existingCategories: any[] = [];

  categoryForm: FormGroup;
  isDuplicate = false;

  constructor(private fb: FormBuilder, private categoriesService: CategoriesService) {
    this.categoryForm = this.fb.group({
      name: ['', Validators.required]
    });
  }

  ngOnChanges(): void {
    if (this.category) {
      this.categoryForm.patchValue({
        name: this.category.name
      });
    } else {
      this.categoryForm.reset();
    }
  }

  ngOnInit() {
    this.categoryForm.get('name')?.valueChanges.subscribe(() => {
      this.checkDuplicateName();
    });
  }

  checkDuplicateName() {
    const enteredName = this.categoryForm.get('name')?.value?.trim();
    if (!enteredName) {
      this.isDuplicate = false;
      return;
    }

    const isEditing = !!this.category;
    this.isDuplicate = this.existingCategories.some(c =>
      c.name.toLowerCase() === enteredName.toLowerCase() &&
      (!isEditing || c.id !== this.category?.id)
    );
  }

  onSubmit() {
    if (this.isDuplicate) {
      alert('Category name already exists!');
      return;
    }

    if (this.categoryForm.valid) {
      const categoryData = { ...this.categoryForm.value };
      if (this.category) {
        categoryData.id = this.category.id; // Include id for edit
      }
      this.formSubmit.emit(categoryData); // emit to parent
      this.categoryForm.reset();
      this.closeForm.emit();
    } else {
      alert('Please enter a valid category name.');
    }
  }
}
