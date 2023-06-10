import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { BasketItemsComponent } from './basket/basket-items/basket-items.component';
import { BasketComponent } from './basket/basket/basket.component';
import { AdminGuard } from './core/guard/admin.guard';
import { CustomerGuard } from './core/guard/customer.guard';
import { LoggedInGuard } from './core/guard/loggedIn.guard';
import { LoginGuard } from './core/guard/login.guard';
import { OrderListComponent } from './orders/order-list/order-list.component';
import { OrderComponent } from './orders/order/order.component';
import { OrdersComponent } from './orders/orders/orders.component';
import { ProductListComponent } from './products/product-list/product-list.component';
import { SellersListComponent } from './user/sellers/sellers-list/sellers-list.component';
import { LoginComponent } from './user/login/login.component';
import { ProfileComponent } from './user/profile/profile.component';
import { RegisterComponent } from './user/register/register.component';

const routes: Routes = [
  { path: '', pathMatch: "full", redirectTo: 'home' },
  { path: 'home', component: ProductListComponent, canActivate: [LoggedInGuard] },
  { path: 'login', component: LoginComponent, canActivate: [LoginGuard] },
  { path: 'register', component: RegisterComponent, canActivate: [LoginGuard] },
  { path: 'sellers', component: SellersListComponent, canActivate: [AdminGuard] },
  { path: 'card', component: BasketComponent, canActivate: [CustomerGuard] },
  { path: 'orders', component: OrdersComponent, canActivate: [LoggedInGuard] },
  { path: 'profile', component: ProfileComponent, canActivate: [LoggedInGuard] }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
