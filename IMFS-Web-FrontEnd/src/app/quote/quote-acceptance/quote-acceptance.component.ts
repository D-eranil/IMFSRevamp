import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { AbstractControl, FormArray, FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import * as _ from 'lodash-es';
import * as moment from 'moment';
import { ConfirmationService, ConfirmEventType, MessageService } from 'primeng/api';
import { combineLatest } from 'rxjs';
import { takeWhile } from 'rxjs/operators';
import { CustomerModel, CustomerResponseModel } from 'src/app/models/customer/customer.model';
import { DurationOptions, PaymentFrequencyOptions, QuoteStatusOptions } from 'src/app/models/drop-down-options/drop-down-options.model';
import { Email } from 'src/app/models/email/email.model';
import { CategoriesModel, TypesModel } from 'src/app/models/options/options.model';
import { OTCEncryptionModel, OTCModel } from 'src/app/models/otc/otc.model';
import { ProductDetailsModel, ProductInputModel } from 'src/app/models/product/product.model';
import { CustomerDetails, EndUserDetails, QuoteDetailsModel, QuoteDetailsResponseModel, QuoteDownloadInput, QuoteHeader, QuoteLine, RejectQuoteModel} from 'src/app/models/quote/quote.model';
import { RateCalculateInputModel, RateCalculateLineItem, RateCalculateResponseModel } from 'src/app/models/rateCalculate/rateCalculate-input.model';
import { IMFSRoutes } from 'src/app/models/routes/imfs-routes';
import { HttpResponseData } from 'src/app/models/utility-models/response.model';
import { EmailControllerService } from 'src/app/services/controller-services/email-controller.service';
import { OptionsControllerService } from 'src/app/services/controller-services/options-controller.service';
import { ORPCustomerControllerService } from 'src/app/services/controller-services/orp-customer-controller.service';
import { ProductControllerService } from 'src/app/services/controller-services/product-controller.service';
import { QuoteAcceptanceControllerService } from 'src/app/services/controller-services/quote-acceptance-controller.service';
import { QuoteControllerService } from 'src/app/services/controller-services/quote-controller.service';
import { ComponentVisibilityService } from 'src/app/services/utility-services/component-visibility.service';
import { IMFSFormService } from 'src/app/services/utility-services/imfs-form-service';
import { IMFSUtilityService } from 'src/app/services/utility-services/imfs-utility-services';
import { JsUtilityService } from 'src/app/services/utility-services/js-utility.service';
import { environment } from 'src/environments/environment';
import { QuoteEmailHistoryModalComponent } from '../quote-email-history-modal/quote-email-history-modal.component';
import {RejectReasons} from 'src/app/models/drop-down-options/drop-down-options.model';
import { Message } from '@angular/compiler/src/i18n/i18n_ast';

@Component({
    selector: 'app-quote-acceptance',
    templateUrl: './quote-acceptance.component.html',
    styleUrls: ['./quote-acceptance.component.scss']
})


export class QuoteAcceptanceComponent implements OnInit {

    header = 'Quote Acceptance: ';

    emailSent = false;
    codeVerified = false;
    emailCode = '';
    daysLeft = '';
    haserror = false;
    displayModal: boolean;

    durationOptions = DurationOptions;
    paymentFrequencyOptions = PaymentFrequencyOptions;
    quoteStatusOptions = QuoteStatusOptions;

    todayDate: Date;

    types: TypesModel[];

    categories: CategoriesModel[];

    quoteDetails = new QuoteDetailsModel();

    quoteLines: QuoteLine[] = [];

    displayQuoteLines: QuoteLine[] = [];

    active = true;

    displayPaymentPlan: boolean = environment.DisplayFunderPlan;

    quoteIdGlobal:number;
    rejectQuoteDialog:boolean;
    quoteRejectForm:FormGroup;
    submitted = false;
    rejectreasons = RejectReasons;
    rejectQuoteSend = new RejectQuoteModel();
    constructor(
        private formBuilder: FormBuilder,
        private imfsUtilityService: IMFSUtilityService,
        private jsUtilityService: JsUtilityService,
        private formUtility: IMFSFormService,
        private orpCustomerControllerService: ORPCustomerControllerService,
        private quoteControllerService: QuoteControllerService,
        private productControllerService: ProductControllerService,
        private optionsControllerService: OptionsControllerService,
        private emailControllerService: EmailControllerService,
        private quoteAcceptanceControllerService: QuoteAcceptanceControllerService,
        private router: Router,
        private route: ActivatedRoute,
        private confirmationService: ConfirmationService,
        private messageService: MessageService,
        private componentVisibilityService: ComponentVisibilityService) {
    }



    ngOnInit() {
        // make full screen
        this.componentVisibilityService.fullScreenFire(true);
        // Public Page
        this.componentVisibilityService.publicPageFire(true);

        this.getTypes();
        this.getCategories();

        this.quoteIdGlobal = 0;
        const that = this;
        this.todayDate = new Date();

        const obsComb = combineLatest([this.route.paramMap, this.route.queryParams]);
        console.log(obsComb)
        obsComb.pipe(takeWhile(() => this.active)).subscribe((params: any) => {

            const quoteId = params[1].id;
            console.log(quoteId)
            if (quoteId) {
                // Decode quoteid here
                this.getDecodedQuoteId(quoteId);

            }
        });
        this.quoteRejectForm = this.formBuilder.group({
            RejectReason: new FormControl('', Validators.required),
            RejectMessage:new FormControl('', Validators.required),
        });
    }

    getDecodedQuoteId(encryptedQuoteId: string) {

        const otcEncryptionModel = new OTCEncryptionModel();
        otcEncryptionModel.encryptedQuoteId = encryptedQuoteId;

        this.quoteAcceptanceControllerService.getDecodedQuoteId(otcEncryptionModel).subscribe(
            (response: HttpResponseData) => {
                this.imfsUtilityService.hideLoading();
                this.imfsUtilityService.showToastr('success', 'Success', 'Decrypted Successfully:' + response.data);
                this.quoteIdGlobal = Number(response.data);
                this.getQuoteDetails(Number(response.data));
                this.haserror = false;
            },
            (err: any) => {
                console.log(err);
                this.haserror = true;
                this.imfsUtilityService.hideLoading();
                this.imfsUtilityService.showToastr('error', 'Failed', 'Error Decrypting Quote Id');
            }
        );
    }


    getQuoteDetails(quoteId: number) {
        this.imfsUtilityService.showLoading('Loading');
        this.quoteControllerService.getQuoteDetails(quoteId).subscribe(
            (response: QuoteDetailsResponseModel) => {
                this.imfsUtilityService.hideLoading();

                this.quoteDetails = response.quoteDetails;

                const given = moment(response.quoteDetails.quoteHeader.expiryDate);
                const current = moment().startOf('day');
                this.daysLeft = moment.duration(given.diff(current)).asDays().toString();

                this.quoteDetails.quoteLines.forEach((quoteLine: QuoteLine) => {

                    if (quoteLine.item) {
                        const currentItem = this.types.find(type => type.id.toString() === quoteLine.item)?.name;
                        if (currentItem) {
                            quoteLine.itemName = currentItem;
                        }
                    }

                    if (quoteLine.category) {
                        const currentCategory = this.categories.find(cat => cat.id.toString() === quoteLine.category)?.name;
                        if (currentCategory) {
                            quoteLine.categoryName = currentCategory;
                        }
                    }
                    this.displayQuoteLines.push(quoteLine);
                });


            },
            (err: any) => {
                console.log(err);
                this.imfsUtilityService.hideLoading();
                this.imfsUtilityService.showToastr('error', 'Failed', 'Error getting Quote details');
            }
        );

    }

    getTypes() {

        this.optionsControllerService.getTypes().subscribe(
            (response: TypesModel[]) => {
                this.imfsUtilityService.hideLoading();
                if (response.length === 0) {
                    this.imfsUtilityService.showToastr('error', 'Failed', 'No Types found');
                }
                else {
                    this.types = response;
                }
            },
            (err: any) => {
                console.log(err);
                this.imfsUtilityService.hideLoading();
                this.imfsUtilityService.showToastr('error', 'Failed', 'Error in Get Types');
            }
        );
    }

    getCategories() {

        this.optionsControllerService.getCategories().subscribe(
            (response: CategoriesModel[]) => {
                this.imfsUtilityService.hideLoading();
                if (response.length === 0) {
                    this.imfsUtilityService.showToastr('error', 'Failed', 'No Categories found');
                }
                else {
                    this.categories = response;
                }
            },
            (err: any) => {
                console.log(err);
                this.imfsUtilityService.hideLoading();
                this.imfsUtilityService.showToastr('error', 'Failed', 'Error in Get Categories');
            }
        );
    }


    getQuoteType(): string {
        if (this.quoteDetails && this.quoteDetails.quoteHeader) {
            return this.quoteDetails.quoteHeader.quoteType;
        }
        return '';
    }


    getFunderPlan(): string {
        if (this.quoteDetails && this.quoteDetails.quoteHeader) {
            const funderPlan = this.quoteDetails.quoteHeader.funderPlan;
            if (funderPlan) {
                switch (funderPlan) {
                    case '1': {
                        return 'Advance';
                    }
                    case '2': {
                        return 'Arrears';
                    }
                }
            }
        }
        return 'Advance';
    }

    roundTo(num: number, precision: number) {
        return parseFloat((+(Math.round(+(num + 'e' + precision)) + 'e' + -precision)).toFixed(precision));
    }

    emailQuote(quoteNumber: number) {
        void this.router.navigate([IMFSRoutes.Email], { queryParams: { quoteId: quoteNumber } });
    }

    proceedQuote() {
        // Send one time code
        const email = new Email();
        email.FromAddress = 'AU-IMFSPortal@Ingrammicro.com';
        // Remove this comment later after testing
        email.ToAddress = this.quoteDetails.endUserDetails.endCustomerEmail;
        email.CCEmail = 'avinash.udar@ingrammicro.com';
        email.Subject = 'IMFS Code Verification';
        email.Body = '';
        email.QuoteId = this.quoteIdGlobal.toString();

        this.imfsUtilityService.showLoading('Sending Email...');
        // save quote details to server
        this.quoteAcceptanceControllerService.sendCodeEmail(email).subscribe(
            (response: HttpResponseData) => {
                this.imfsUtilityService.hideLoading();
                this.imfsUtilityService.showToastr('success', 'Success', 'Email sent Successfully:' + response.data);
                this.emailSent = true;
            },
            (err: any) => {
                console.log(err);
                this.imfsUtilityService.hideLoading();
                this.imfsUtilityService.showToastr('error', 'Failed', 'Error Sending Email');
            }
        );
    }


    acceptQuote() {
        const quoteEntered = this.emailCode;
        console.log(quoteEntered);

        const otcModel = new OTCModel();
        otcModel.code = quoteEntered;
        otcModel.quoteId = this.quoteIdGlobal.toString();

        this.quoteAcceptanceControllerService.verifyCode(otcModel).subscribe(
            (response: HttpResponseData) => {
                this.imfsUtilityService.hideLoading();
                this.imfsUtilityService.showToastr('success', 'Success', 'Code verified Successfully: ' + response.message);
                this.codeVerified = true;
            },
            (err: any) => {
                console.log(err);
                this.displayModal = true;
                this.imfsUtilityService.hideLoading();
                this.imfsUtilityService.showToastr('error', 'Failed', 'Error Verifying code');
            }
        );

    }

    cancel() {

    }

    requestChanges() {
        this.imfsUtilityService.showLoading('Sending Email...');
        this.quoteAcceptanceControllerService.requestChangesQuote(this.quoteIdGlobal.toString()).subscribe(
            (response: HttpResponseData) => {
                this.imfsUtilityService.hideLoading();
                this.imfsUtilityService.showToastr('success', 'Success', response.message);
                this.rejectQuoteDisable=true;
            },
            (err: any) => {
                console.log(err);
                this.imfsUtilityService.hideLoading();
                this.imfsUtilityService.showToastr('error', 'Failed', 'Error sending email for request change');
            }
        );


    }

    resendCode() {
        this.proceedQuote();
    }
    reason:string;
    comment:string;
    rejectQuoteDisable:boolean=false;
    resetRejectQuote() {
        this.quoteRejectForm.controls.RejectMessage.reset();
        this.quoteRejectForm.controls.RejectReason.reset();
    }
    rejectQuote(reason:string, comment:string,) {
        if (this.quoteRejectForm.valid){
            let quoteId = this.quoteIdGlobal.toString();
            reason = this.quoteRejectForm.get('RejectReason')?.value;
            comment = this.quoteRejectForm.get('RejectMessage')?.value;
            this.rejectQuoteSend.quoteId=quoteId;
            this.rejectQuoteSend.reason=reason;
            this.rejectQuoteSend.comment=comment;
            this.imfsUtilityService.showLoading('Sending Email...');
            this.quoteAcceptanceControllerService.rejectQuoteSend(this.rejectQuoteSend).subscribe(
                (response: RejectQuoteModel) => {
                    this.imfsUtilityService.hideLoading();
                    this.imfsUtilityService.showToastr('success', 'Success', 'Quote Reject Successfully');
                    this.resetRejectQuote();
                    this.rejectQuoteDialog=false;
                    this.rejectQuoteDisable=true;
                },
                (err: any) => {
                    console.log(err);
                    this.imfsUtilityService.hideLoading();
                    this.imfsUtilityService.showToastr('error', 'Failed', 'Error sending email for reject quote');
                    this.resetRejectQuote();
                    this.rejectQuoteDialog=false;


                }
            );
        }
        else{
            this.imfsUtilityService.showToastr('error', 'Invalid Input', 'Please enter all required fields.');
            this.rejectQuoteDialog=true;
        }
    }
    rejectQuotes() {
        this.rejectQuoteDialog = true;
    }
    setFormValue(quoteRejectDetails: RejectQuoteModel): void {
        this.quoteRejectForm.patchValue({
            RejectReasons:quoteRejectDetails.reason,
            RejectMessage: quoteRejectDetails.comment,
        })
    }


}
