import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { SharedModule as PrimeNGSharedModule } from 'primeng/api';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { FileUploadModule } from 'primeng/fileupload';
import { RadioButtonModule } from 'primeng/radiobutton';
import { TableModule } from 'primeng/table';
import { IMFSSharedModule } from '../shared/imfs-shared.module';
import { LoginRoutingModule } from './login-routing.module';
import { LoginComponent } from '../login/login.component';

@NgModule({
    imports: [CommonModule, PrimeNGSharedModule, ButtonModule,
        CardModule,
        IMFSSharedModule, TableModule, FormsModule, RadioButtonModule,
        FileUploadModule, LoginRoutingModule],
    declarations: [LoginComponent],
    exports: [],
})
export class LoginModule { }
