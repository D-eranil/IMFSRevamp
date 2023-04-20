import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { AutoCompleteModule } from 'primeng/autocomplete';
import { ButtonModule } from 'primeng/button';
import { CalendarModule } from 'primeng/calendar';
import { CardModule } from 'primeng/card';
import { CheckboxModule } from 'primeng/checkbox';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { DialogModule } from 'primeng/dialog';
import { DropdownModule } from 'primeng/dropdown';
import { InputTextModule } from 'primeng/inputtext';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { ListboxModule } from 'primeng/listbox';
import { MessagesModule } from 'primeng/messages';
import { MultiSelectModule } from 'primeng/multiselect';
import { OverlayPanelModule } from 'primeng/overlaypanel';
import { PanelModule } from 'primeng/panel';
import { ProgressBarModule } from 'primeng/progressbar';
import { ScrollPanelModule } from 'primeng/scrollpanel';
import { SelectButtonModule } from 'primeng/selectbutton';
import { SidebarModule } from 'primeng/sidebar';
import { SplitButtonModule } from 'primeng/splitbutton';
import { TableModule } from 'primeng/table';
import { TabViewModule } from 'primeng/tabview';
import { ToastModule } from 'primeng/toast';
import { TooltipModule } from 'primeng/tooltip';
import { CurrencyInputDirective } from './custom-directive/currency-input.directive';
import { YesNoPipe } from './custom-pipe/boolean-pipe';
import { LoadingComponent } from './loading-component/loading.component';
import { ToastComponent } from './toast-component/toast.component';
import { ToolbarComponent } from './toolbar/imfs-toolbar.component';
import { FooterComponent } from './footer/imfs-footer.component';
import { BannerComponent } from './banner/imfs-banner.component';
import { TopNavComponent } from './top-nav/top-nav.component';
import { TopNavEvent } from './top-nav/topnav.event';
import { MenubarModule } from 'primeng/menubar';
import { MenuItem } from 'primeng/api';
import { BreadcrumbModule } from 'primeng/breadcrumb';
import { BreadcrumbComponent } from './breadcrumb/imfs-breadcrumb.component';
import { DisableControlDirective } from './custom-directive/disable-control.directive';

@NgModule({
    // tslint:disable-next-line: max-line-length
    declarations: [ToastComponent, LoadingComponent, CurrencyInputDirective, YesNoPipe, ToolbarComponent, TopNavComponent,
        FooterComponent, BannerComponent, BreadcrumbComponent, DisableControlDirective],
    imports: [CommonModule, ToastModule, ButtonModule, ReactiveFormsModule,
        AutoCompleteModule, FormsModule, MessagesModule, TooltipModule, DropdownModule, ListboxModule,
        RouterModule, DialogModule, ConfirmDialogModule, InputTextModule, TableModule, SidebarModule, MenubarModule,
        ScrollPanelModule, CalendarModule, PanelModule, MultiSelectModule, SplitButtonModule, CardModule, InputTextareaModule,
        ProgressBarModule, SelectButtonModule, CheckboxModule, OverlayPanelModule, TabViewModule, BreadcrumbModule ],
    exports: [ToastComponent, LoadingComponent, DisableControlDirective,
        CurrencyInputDirective, YesNoPipe, ToolbarComponent, TopNavComponent, FooterComponent, BannerComponent, BreadcrumbComponent],
    providers: [TopNavEvent],
})
export class IMFSSharedModule { }
