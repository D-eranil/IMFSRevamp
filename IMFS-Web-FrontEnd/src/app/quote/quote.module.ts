import { CommonModule } from '@angular/common';
import { KeyFilterModule } from 'primeng/keyfilter';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SharedModule as PrimeNGSharedModule } from 'primeng/api';
import { AutoCompleteModule } from 'primeng/autocomplete';
import { ButtonModule } from 'primeng/button';
import { CalendarModule } from 'primeng/calendar';
import { CheckboxModule } from 'primeng/checkbox';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { DialogModule } from 'primeng/dialog';
import { DividerModule } from 'primeng/divider';
import { DropdownModule } from 'primeng/dropdown';
import { EditorModule } from 'primeng/editor';
import { FileUploadModule } from 'primeng/fileupload';
import { InputTextModule } from 'primeng/inputtext';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { MessagesModule } from 'primeng/messages';
import { PanelModule } from 'primeng/panel';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { RadioButtonModule } from 'primeng/radiobutton';
import { SplitButtonModule } from 'primeng/splitbutton';
import { SplitterModule } from 'primeng/splitter';
import { TableModule } from 'primeng/table';
import { TabViewModule } from 'primeng/tabview';
import { TooltipModule } from 'primeng/tooltip';
import { IMFSSharedModule } from '../shared/imfs-shared.module';
import { QuoteAcceptanceComponent } from './quote-acceptance/quote-acceptance.component';
import { QuoteDetailsComponent } from './quote-details/quote-details.component';
import { QuoteEmailHistoryModalComponent } from './quote-email-history-modal/quote-email-history-modal.component';
import { QuoteRoutingModule } from './quote-routing.module';
import { QuoteSearchComponent } from './quote-search/quote-search.component';
import { AuSearchTemplateComponent } from '../company-search/au-search-template/au-search-template.component';
import { DocumentUploadPopupComponent } from 'src/app/document-upload-popup/document-upload-popup.component';
import { StepsModule } from 'primeng/steps'


@NgModule({
    imports: [CommonModule, DialogModule, ConfirmDialogModule, FormsModule, ReactiveFormsModule, InputTextModule, InputTextareaModule,
        DropdownModule, PrimeNGSharedModule, CheckboxModule, ButtonModule, TooltipModule, TabViewModule, SplitterModule, SplitButtonModule,
        ProgressSpinnerModule, EditorModule, KeyFilterModule,
        CalendarModule, PanelModule, MessagesModule, IMFSSharedModule, TableModule, FormsModule, RadioButtonModule,
        DividerModule, FileUploadModule, StepsModule, AutoCompleteModule, QuoteRoutingModule],
    declarations: [QuoteDetailsComponent, QuoteSearchComponent, QuoteEmailHistoryModalComponent, QuoteAcceptanceComponent, AuSearchTemplateComponent, DocumentUploadPopupComponent],
    exports: [],
})
export class QuoteModule { }
