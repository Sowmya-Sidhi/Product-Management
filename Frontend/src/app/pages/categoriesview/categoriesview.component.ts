import { Component } from '@angular/core';
import { CategoriesService } from './categories.service';

@Component({
  selector: 'app-categoriesview',
  standalone: false,
  templateUrl: './categoriesview.component.html',
  styleUrl: './categoriesview.component.scss'
})
export class CategoriesviewComponent {
  categories :any[]=[];
  constructor(private Categoriesservice: CategoriesService){ }

  ngOnInit(): void {
    this.Categoriesservice.getCategories().subscribe({
      next: (data) => {
        this.categories = data;
        console.log('Categories loaded:', data);
      },
      error: (err) => console.error('Error loading categories:', err)
    });
  }
  
  onEdit(categories:any){
    alert(`Editing Categories ${categories}`);
  }
   message=" ";

  onAdd(){
    this.message="Category Added";
  }

}
