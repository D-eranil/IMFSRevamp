import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { SharedModule as PrimeNGSharedModule } from 'primeng/api';
import { ButtonModule } from 'primeng/button';
import { RadioButtonModule } from 'primeng/radiobutton';
import { CalendarModule } from 'primeng/calendar';
import { CheckboxModule } from 'primeng/checkbox';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { DialogModule } from 'primeng/dialog';
import { DropdownModule } from 'primeng/dropdown';
import { FileUploadModule } from 'primeng/fileupload';
import { InputTextModule } from 'primeng/inputtext';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { MessagesModule } from 'primeng/messages';
import { PanelModule } from 'primeng/panel';
import { TableModule } from 'primeng/table';
import { TooltipModule } from 'primeng/tooltip';
import { IMFSSharedModule } from '../shared/imfs-shared.module';
import { QuoteTotalRateComponent } from './quote-total-rate-list/quote-total-rate.component';
import { QuoteTotalRateRoutingModule } from './quote-total-rate-routing.module';
import { QuoteTotalRateDetailsModalComponent } from './quote-total-rate-details-modal/quote-total-rate-details-modal.component';
import { QuoteTotalRateUploadModalComponent } from './quote-total-rate-upload-modal/quote-total-rate-upload-modal.component';


@NgModule({
    imports: [CommonModule, DialogModule, ConfirmDialogModule, InputTextModule, InputTextareaModule,
        DropdownModule, PrimeNGSharedModule, CheckboxModule, ButtonModule, TooltipModule,
        CalendarModule, PanelModule, MessagesModule, IMFSSharedModule, TableModule, FormsModule,
        FileUploadModule, QuoteTotalRateRoutingModule],
    declarations: [QuoteTotalRateComponent, QuoteTotalRateDetailsModalComponent, QuoteTotalRateUploadModalComponent],
    exports: [],
})
export class QuoteTotalRateModule { }
