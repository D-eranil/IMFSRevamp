import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { QuotePercentRateComponent } from './quote-percent-rate-list/quote-percent-rate.component';


@NgModule({
    imports: [
        RouterModule.forChild([
            { path: '', component: QuotePercentRateComponent },
        ])
    ],
    exports: [RouterModule]
})
export class QuotePercentRateRoutingModule { }
