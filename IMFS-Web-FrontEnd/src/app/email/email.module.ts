import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SharedModule as PrimeNGSharedModule } from 'primeng/api';
import { ButtonModule } from 'primeng/button';
import { CalendarModule } from 'primeng/calendar';
import { CheckboxModule } from 'primeng/checkbox';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { DialogModule } from 'primeng/dialog';
import { DividerModule } from 'primeng/divider';
import { DropdownModule } from 'primeng/dropdown';
import { FileUploadModule } from 'primeng/fileupload';
import { InputTextModule } from 'primeng/inputtext';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { MessagesModule } from 'primeng/messages';
import { PanelModule } from 'primeng/panel';
import { RadioButtonModule } from 'primeng/radiobutton';
import { SplitterModule } from 'primeng/splitter';
import { TableModule } from 'primeng/table';
import { TabViewModule } from 'primeng/tabview';
import { TooltipModule } from 'primeng/tooltip';
import { IMFSSharedModule } from '../shared/imfs-shared.module';
import { AutoCompleteModule } from 'primeng/autocomplete';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { EmailComponent } from './email/email.component';
import { EditorModule } from 'primeng/editor';
import { EmailUploadModalComponent } from './email-upload-modal/email-upload-modal.component';
import { EmailRoutingModule } from './email-routing.module';

@NgModule({
    imports: [CommonModule, DialogModule, ConfirmDialogModule, FormsModule, ReactiveFormsModule, InputTextModule, InputTextareaModule,
        DropdownModule, PrimeNGSharedModule, CheckboxModule, ButtonModule, TooltipModule, TabViewModule, SplitterModule,
        ProgressSpinnerModule, EditorModule,
        CalendarModule, PanelModule, MessagesModule, IMFSSharedModule, TableModule, FormsModule, RadioButtonModule,
        DividerModule, FileUploadModule, AutoCompleteModule, EmailRoutingModule],
    declarations: [EmailComponent, EmailUploadModalComponent],
    exports: [],
})
export class EmailModule { }
