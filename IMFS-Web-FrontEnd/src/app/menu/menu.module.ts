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
import { TabMenuModule } from 'primeng/tabmenu';
import { MenuComponent } from './menu.component';
import { MenuRoutingModule } from './menu-routing.module';

@NgModule({
    imports: [CommonModule, DialogModule, ConfirmDialogModule, InputTextModule, InputTextareaModule,
        DropdownModule, PrimeNGSharedModule, CheckboxModule, ButtonModule, TooltipModule,
        CalendarModule, PanelModule, MessagesModule, IMFSSharedModule, TableModule, FormsModule, RadioButtonModule,
        FileUploadModule, TabMenuModule, MenuRoutingModule],
    declarations: [MenuComponent],
    exports: [],
})
export class MenuModule { }
