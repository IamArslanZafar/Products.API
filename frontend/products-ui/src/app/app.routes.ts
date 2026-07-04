import { Routes } from '@angular/router';
import { authGuard } from './core/auth.guard';
import { LoginComponent } from './pages/login/login.component';
import { ProductListComponent } from './pages/product-list/product-list.component';

export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'products' },
  { path: 'login', component: LoginComponent },
  { path: 'products', component: ProductListComponent, canActivate: [authGuard] },
  { path: '**', redirectTo: 'products' }
];
