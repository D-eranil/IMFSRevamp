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
// import { FinanceTypeDetailsModalComponent } from './finance-type-details-modal/finance-type-details-modal.component';
import { UserListComponent } from './user-list/user-list.component';
import { UserRoutingModule } from './user-routing.module';
import { UserDetailsModalComponent } from './user-details-modal/user-details-modal.component';

@NgModule({
    imports: [CommonModule, DialogModule, ConfirmDialogModule, InputTextModule, InputTextareaModule,
        DropdownModule, PrimeNGSharedModule, CheckboxModule, ButtonModule, TooltipModule,
        CalendarModule, PanelModule, MessagesModule, IMFSSharedModule, TableModule, FormsModule, RadioButtonModule,
        FileUploadModule, UserRoutingModule],
    declarations: [UserListComponent, UserDetailsModalComponent],
    exports: [],
})
export class UserModule { }
