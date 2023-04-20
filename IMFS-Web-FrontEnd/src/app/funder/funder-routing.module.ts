import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { FunderListComponent } from './funder-list/funder-list.component';


@NgModule({
    imports: [
        RouterModule.forChild([
            { path: '', component: FunderListComponent },
        ])
    ],
    exports: [RouterModule]
})
export class FunderRoutingModule { }
