import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { QuoteAcceptanceComponent } from './quote-acceptance/quote-acceptance.component';
import { QuoteDetailsComponent } from './quote-details/quote-details.component';
import { QuoteSearchComponent } from './quote-search/quote-search.component';


@NgModule({
    imports: [
        RouterModule.forChild([
            { path: 'quote-details', component: QuoteDetailsComponent },
            { path: 'quote-details/:id', component: QuoteDetailsComponent },
            { path: 'quote-details/:id/:mode', component: QuoteDetailsComponent },
            { path: 'quote-search', component: QuoteSearchComponent },
            { path: 'quote-acceptance', component: QuoteAcceptanceComponent}
        ])
    ],
    exports: [RouterModule]
})
export class QuoteRoutingModule { }
