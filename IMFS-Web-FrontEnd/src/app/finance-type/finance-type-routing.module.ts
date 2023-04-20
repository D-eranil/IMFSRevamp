import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { FinanceTypeListComponent } from './finance-type-list/finance-type-list.component';


@NgModule({
    imports: [
        RouterModule.forChild([
            { path: '', component: FinanceTypeListComponent },
        ])
    ],
    exports: [RouterModule]
})
export class FinanceTypeRoutingModule { }
