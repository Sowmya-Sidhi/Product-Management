import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CategoriesviewComponent } from './pages/categoriesview/categoriesview.component';
import { ProductsviewComponent } from './pages/productsview/productsview.component';
import { ReportsviewComponent } from './pages/reportsview/reportsview.component';
import { LoginComponent } from './pages/login/login.component';
import { RegisterComponent } from './pages/register/register.component';

const routes: Routes = [
  {path:'login',component:LoginComponent},
    {path:'register',component:RegisterComponent},
  {path:'',redirectTo:'/login',pathMatch:'full'},

 
  {path:'dashboard',component:ReportsviewComponent},
  {path:'products',component:ProductsviewComponent},
  {path:'categories',component:CategoriesviewComponent},
  {path:'**',redirectTo:'/login'}
];

@NgModule({
  imports: [RouterModule.forRoot(routes,{useHash:true})],
  exports: [RouterModule]
})
export class AppRoutingModule { }

