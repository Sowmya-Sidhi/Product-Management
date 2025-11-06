import { Component } from '@angular/core';
import { ProductsviewService } from '../productsview/productsview.service';
import { CategoriesService } from '../categoriesview/categories.service';

@Component({
  selector: 'app-reportsview',
  standalone: false,
  templateUrl: './reportsview.component.html',
  styleUrl: './reportsview.component.scss'
})
export class ReportsviewComponent {
totalproducts=0;
activeproducts=0;
totalcategories=0;

products:any[]=[];
categories:any[]=[];


constructor(private productsviewService : ProductsviewService,private categoriesService:CategoriesService){ }
 

ngOnInit(): void {
  this.productsviewService.getProducts().subscribe({
    next: (data) => {
      this.products = data;
      this.totalproducts = data.length;
      this.activeproducts = data.filter(p => p.isActive).length;
    },
    error: (err) => console.error('Error loading products:', err)
  });

  this.categoriesService.getCategories().subscribe({
    next: (data) => {
      this.categories = data;
      this.totalcategories = data.length; // total categories from backend
    },
    error: (err) => console.error('Error loading categories:', err)
  });
}


}
