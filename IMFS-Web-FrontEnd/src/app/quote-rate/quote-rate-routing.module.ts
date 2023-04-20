import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { QuoteRateComponent } from './quote-rate-list/quote-rate.component';


@NgModule({
    imports: [
        RouterModule.forChild([
            { path: '', component: QuoteRateComponent },
        ])
    ],
    exports: [RouterModule]
})
export class QuoteRateRoutingModule { }
