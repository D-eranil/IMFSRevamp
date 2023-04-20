import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { QuoteTotalRateComponent } from './quote-total-rate-list/quote-total-rate.component';


@NgModule({
    imports: [
        RouterModule.forChild([
            { path: '', component: QuoteTotalRateComponent },
        ])
    ],
    exports: [RouterModule]
})
export class QuoteTotalRateRoutingModule { }
