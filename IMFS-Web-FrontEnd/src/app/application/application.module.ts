import { CommonModule } from '@angular/common';
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
import { ApplicationDetailsContactModalComponent } from './application-details-contact-modal/application-details-contact-modal.component';
import { ApplicationDetailsComponent } from './application-details/application-details.component';
import { ApplicationEmailHistoryModalComponent } from './application-email-history-modal/application-email-history-modal.component';
import { ApplicationRoutingModule } from './application-routing.module';
import { ApplicationSearchComponent } from './application-search/application-search.component';
import {StepsModule} from 'primeng/steps';
import { ApplicationDocUploadPopupComponent } from './application-doc-upload-popup/application-doc-upload-popup.component'

@NgModule({
    imports: [CommonModule, DialogModule, ConfirmDialogModule, FormsModule, ReactiveFormsModule, InputTextModule, InputTextareaModule,
        DropdownModule, PrimeNGSharedModule, CheckboxModule, ButtonModule, TooltipModule, TabViewModule, SplitterModule, SplitButtonModule,
        ProgressSpinnerModule, EditorModule,
        CalendarModule, PanelModule, MessagesModule, IMFSSharedModule, TableModule, FormsModule, RadioButtonModule,StepsModule,
        DividerModule, FileUploadModule, AutoCompleteModule, ApplicationRoutingModule],
    // tslint:disable-next-line: max-line-length
    declarations: [ ApplicationSearchComponent, ApplicationDetailsComponent, ApplicationDetailsContactModalComponent, ApplicationEmailHistoryModalComponent, ApplicationDocUploadPopupComponent ],
    exports: [],
})
export class ApplicationModule { }
