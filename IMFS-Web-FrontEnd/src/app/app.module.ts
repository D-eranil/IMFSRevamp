import { TabViewModule } from 'primeng/tabview';
import { CommonModule, DatePipe } from '@angular/common';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { OAuthModule } from 'angular-oauth2-oidc';
import { ConfirmationService, MessageService } from 'primeng/api';
import { AutoCompleteModule } from 'primeng/autocomplete';
import { BreadcrumbModule } from 'primeng/breadcrumb';
import { ButtonModule } from 'primeng/button';
import { CalendarModule } from 'primeng/calendar';
import { DropdownModule } from 'primeng/dropdown';
import { FileUploadModule } from 'primeng/fileupload';
import { TabMenuModule } from 'primeng/tabmenu';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { HomeComponent } from './home/home.component';
import { AttachmentControllerService } from './services/controller-services/attachment-controller.service';
import { ContentControllerService } from './services/controller-services/content-controller.service';
import { EmailControllerService } from './services/controller-services/email-controller.service';
import { FinanceProductTypeControllerService } from './services/controller-services/finance-product-type-controller.servcie';
import { FinanceTypeControllerService } from './services/controller-services/finance-type-controller.service';
import { FunderControllerService } from './services/controller-services/funder-controller.service';
import { FunderPlanControllerService } from './services/controller-services/funder-plan-controller.service';
import { OptionsControllerService } from './services/controller-services/options-controller.service';
import { ORPCustomerControllerService } from './services/controller-services/orp-customer-controller.service';
import { ProductControllerService } from './services/controller-services/product-controller.service';
import { QuoteAcceptanceControllerService } from './services/controller-services/quote-acceptance-controller.service';
import { QuoteControllerService } from './services/controller-services/quote-controller.service';
import { QuotePercentRateControllerService } from './services/controller-services/quote-percent-rate-controller.service';
import { QuoteRateControllerService } from './services/controller-services/quote-rate-controller.service';
import { QuoteTotalRateControllerService } from './services/controller-services/quote-total-rate-controller.service';
import { UserControllerService } from './services/controller-services/user-controller.service';
import { VendorControllerService } from './services/controller-services/vendor-controller.service';
import { HttpsAuthRequestInterceptor } from './services/utility-services/auth.interceptor';
import { BroadcasterService } from './services/utility-services/broadcaster.service';
import { ComponentVisibilityService } from './services/utility-services/component-visibility.service';
import { ConfigDataLoadedEvent } from './services/utility-services/config-data-loaded.event';
import { IMFSFormService } from './services/utility-services/imfs-form-service';
import { IMFSUtilityService } from './services/utility-services/imfs-utility-services';
import { JsUtilityService } from './services/utility-services/js-utility.service';
import { OktaService } from './services/utility-services/okta.service';
import { IMFSSharedModule } from './shared/imfs-shared.module';
import { SSOComponent } from './sso/sso.component';
import { TestComponent } from './test.component';
import { AuthenticationService } from './services/utility-services/authenication.service';
import { ApplicationControllerService } from './services/controller-services/application-controller.service';
import { TableModule } from 'primeng/table';
import { PaginatorModule } from 'primeng/paginator';
import { LoginNewComponent } from './login-new/login-new.component';
import { addressvalidateservice } from './services/controller-services/address-validate-service';
import { DialogModule } from 'primeng/dialog';
@NgModule({
  declarations: [
    AppComponent,
    TestComponent,
    SSOComponent,
    HomeComponent,
    LoginNewComponent
  ],
  imports: [
    BrowserModule,
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    BrowserAnimationsModule,
    AppRoutingModule,
    ButtonModule,
    FileUploadModule,
    HttpClientModule,
    DropdownModule,
    IMFSSharedModule,
    TabMenuModule,
    TabViewModule,
    BreadcrumbModule,
    CalendarModule,
    AutoCompleteModule,
    OAuthModule.forRoot(),
    TableModule,
    PaginatorModule,
    DialogModule
  ],
  providers: [
    IMFSUtilityService,
    JsUtilityService,
    ConfirmationService,
    QuoteRateControllerService,
    FunderControllerService,
    FunderPlanControllerService,
    VendorControllerService,
    FinanceTypeControllerService,
    FinanceProductTypeControllerService,
    QuoteControllerService,
    QuoteTotalRateControllerService,
    QuotePercentRateControllerService,
    ORPCustomerControllerService,
    ProductControllerService,
    OptionsControllerService,
    ContentControllerService,
    EmailControllerService,
    QuoteAcceptanceControllerService,
    ApplicationControllerService,
    AttachmentControllerService,
    ComponentVisibilityService,
    UserControllerService,
    OktaService,
    AuthenticationService,
    ConfigDataLoadedEvent,
    BroadcasterService,
    DatePipe,
    MessageService,
    IMFSFormService,
    addressvalidateservice,
    {
      provide: HTTP_INTERCEPTORS,
      useClass: HttpsAuthRequestInterceptor,
      multi: true,
    }],
  bootstrap: [AppComponent]
})
export class AppModule { }
