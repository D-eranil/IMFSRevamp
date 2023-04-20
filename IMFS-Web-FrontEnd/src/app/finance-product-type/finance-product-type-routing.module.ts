import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { FinanceProductTypeListComponent } from './finance-product-type-list/finance-product-type-list.component';


@NgModule({
    imports: [
        RouterModule.forChild([
            { path: '', component: FinanceProductTypeListComponent },
        ])
    ],
    exports: [RouterModule]
})
export class FinanceProductTypeRoutingModule { }
