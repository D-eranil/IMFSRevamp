import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';

import { ApplicationDetailsComponent } from './application-details/application-details.component';
import { ApplicationSearchComponent } from './application-search/application-search.component';


@NgModule({
    imports: [
        RouterModule.forChild([
            { path: 'application-details', component: ApplicationDetailsComponent },
            { path: 'application-details/:id', component: ApplicationDetailsComponent },
            { path: 'application-details/:id/:mode', component: ApplicationDetailsComponent },
            { path: 'application-search', component: ApplicationSearchComponent },
        ])
    ],
    exports: [RouterModule]
})
export class ApplicationRoutingModule { }
