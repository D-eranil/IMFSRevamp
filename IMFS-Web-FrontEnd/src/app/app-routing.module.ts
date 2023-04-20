import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';

import { IMFSRoutes } from './models/routes/imfs-routes';
import { SSOComponent } from './sso/sso.component';
import { TestComponent } from './test.component';

const routes: Routes = [
  { path: '', loadChildren: () => import('./login/login.module').then(m => m.LoginModule) },
  { path: 'home', component: HomeComponent },
  { path: 'sso/call-back', component: SSOComponent },
  {
    path: 'login',
    loadChildren: () => import('./login/login.module').then(m => m.LoginModule)
  },
  { path: 'test', component: TestComponent },
  {
    path: IMFSRoutes.QuoteRates,
    loadChildren: () => import('./quote-rate/quote-rate.module').then(m => m.QuoteRateModule)
  },
  {
    path: IMFSRoutes.Funders,
    loadChildren: () => import('./funder/funder.module').then(m => m.FunderModule)
  },
  {
    path: IMFSRoutes.Vendors,
    loadChildren: () => import('./vendor/vendor.module').then(m => m.VendorModule)
  },
  {
    path: IMFSRoutes.FinanceTypes,
    loadChildren: () => import('./finance-type/finance-type.module').then(m => m.FinanceTypeModule)
  },
  {
    path: IMFSRoutes.FinanceProductTypes,
    loadChildren: () => import('./finance-product-type/finance-product-type.module').then(m => m.FinanceProductTypeModule)
  },
  {
    path: 'quote',
    loadChildren: () => import('./quote/quote.module').then(m => m.QuoteModule)
  },
  {
    path: 'application',
    loadChildren: () => import('./application/application.module').then(m => m.ApplicationModule)
  },
  {
    path: 'email',
    loadChildren: () => import('./email/email.module').then(m => m.EmailModule)
  },
  {
    path: IMFSRoutes.Menu,
    loadChildren: () => import('./menu/menu.module').then(m => m.MenuModule)
  },
  {
    path: IMFSRoutes.QuoteBreakTotalRates,
    loadChildren: () => import('./quote-total-rate/quote-total-rate.module').then(m => m.QuoteTotalRateModule)
  },
  {
    path: IMFSRoutes.QuoteBreakPercentRates,
    loadChildren: () => import('./quote-percent-rate/quote-percent-rate.module').then(m => m.QuotePercentRateModule)
  },
  {
    path: IMFSRoutes.User,
    loadChildren: () => import('./user/user.module').then(m => m.UserModule)
  },

];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
