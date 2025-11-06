import { NgModule } from '@angular/core';
import { BrowserModule, provideClientHydration, withEventReplay } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { NavbarComponent } from './layers/navbar/navbar.component';
import { SidebarComponent } from './layers/sidebar/sidebar.component';
import { FooterComponent } from './layers/footer/footer.component';
import { ProductsviewComponent } from './pages/productsview/productsview.component';
import { CategoriesviewComponent } from './pages/categoriesview/categoriesview.component';
import { ReportsviewComponent } from './pages/reportsview/reportsview.component';
import { MainContentComponent } from './layers/main-content/main-content.component';
import { LoginComponent } from './pages/login/login.component';
import { RegisterComponent } from './pages/register/register.component';
import { MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { AddProductDialogComponent } from './pages/add-product-dialog/add-product-dialog.component';
import { ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { authInterceptor } from './auth.interceptor';


@NgModule({
  declarations: [
    AppComponent,
    NavbarComponent,
    SidebarComponent,
    FooterComponent,
    ProductsviewComponent,
    
    CategoriesviewComponent,
    ReportsviewComponent,
    MainContentComponent,
    LoginComponent,
    RegisterComponent,
    AddProductDialogComponent
  ],
  imports: [
    BrowserModule,
    FormsModule,
    HttpClientModule,
    MatDialogModule,
    MatButtonModule,
    AppRoutingModule,
    ReactiveFormsModule
    
  ],
  providers: [
  provideClientHydration(withEventReplay()),
  provideHttpClient(withInterceptors([authInterceptor]))
],

  bootstrap: [AppComponent]
})
export class AppModule { }
