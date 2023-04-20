import { Component, OnInit, ViewChild } from '@angular/core';
import { AbstractControl, FormArray, FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import * as _ from 'lodash-es';
import * as moment from 'moment';
import { ConfirmationService, ConfirmEventType, MenuItem, MessageService } from 'primeng/api';
import { combineLatest } from 'rxjs';
import { takeWhile } from 'rxjs/operators';
import {
    ApplicationContact, ApplicationDetailsModel, ApplicationDetailsResponseModel,
    ApplicationDownloadInput, EndCustomerDetails, EntityDetails, ResellerDetails
} from 'src/app/models/application/application.model';

import { DurationOptions, PaymentFrequencyOptions, QuoteStatusOptions, ApplicationStatusOptions, EntityTypeOptions, TrustTypeOptions, IsGuarantorProperty } from 'src/app/models/drop-down-options/drop-down-options.model';
import { StatusModel } from 'src/app/models/options/options.model';
import { IMFSRoutes } from 'src/app/models/routes/imfs-routes';
import { HttpResponseData } from 'src/app/models/utility-models/response.model';
import { ApplicationControllerService } from 'src/app/services/controller-services/application-controller.service';
import { EmailControllerService } from 'src/app/services/controller-services/email-controller.service';
import { OptionsControllerService } from 'src/app/services/controller-services/options-controller.service';
import { AuthenticationService } from 'src/app/services/utility-services/authenication.service';
import { IMFSFormService } from 'src/app/services/utility-services/imfs-form-service';
import { IMFSUtilityService } from 'src/app/services/utility-services/imfs-utility-services';
import { JsUtilityService } from 'src/app/services/utility-services/js-utility.service';
import { ApplicationDetailsContactModalComponent } from '../application-details-contact-modal/application-details-contact-modal.component';
import { ApplicationEmailHistoryModalComponent } from '../application-email-history-modal/application-email-history-modal.component';
// import { QuoteEmailHistoryModalComponent } from '../quote-email-history-modal/quote-email-history-modal.component';
import { QuoteDocuments } from 'src/app/models/quote/quote.model';
import { QuoteControllerService } from 'src/app/services/controller-services/quote-controller.service';
import { environment } from 'src/environments/environment';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { ApplicationDocUploadPopupComponent } from '../application-doc-upload-popup/application-doc-upload-popup.component';
import { ApplicationContactData, ApplicationContactResponseModel } from 'src/app/models/applicationContact/applicationContact.model';
import { Contact } from 'src/app/models/contact/contact.model';
@Component({
    selector: 'app-application-details',
    templateUrl: './application-details.component.html',
    styleUrls: ['./application-details.component.scss']
})


export class ApplicationDetailsComponent implements OnInit {
    header = 'Application Details';
    applicationCustomerDetailsForm: FormGroup;
    financeInformationForm: FormGroup;
    signatoriesForm: FormGroup;
    documentForm: FormGroup;
    durationOptions = DurationOptions;
    entityTypeOptions = EntityTypeOptions;
    paymentFrequencyOptions = PaymentFrequencyOptions;
    applicationStatusOptions: StatusModel[];
    trustTypeOptions = TrustTypeOptions;
    active = true;
    applicationIdGlobal = 0;
    resellerIdGlobal = '';
    activeTabIndex = 0;
    applicationApproved = 12;
    guarantorTableData: ApplicationContact[] = [];
    trusteeTableData: ApplicationContact[] = [];
    accountantTableData: ApplicationContact[] = [];
    beneficialOwnersTableData: ApplicationContact[] = [];
    applicationDetails = new ApplicationDetailsModel();
    otherEntityType = 5;
    otherTrustType = 'Other';
    IMStaffRole = false;
    IMStaffAdminRole = false;
    ResellerStandardRole = false;
    controlReadOnly: boolean;
    activeIndex: any;
    custDetails: boolean = true;
    financeInfo: boolean;
    signatories: boolean;
    documents: boolean;
    itemsForSteps: any[]
    entityTrustTypeRadio: any;
    entityTypeRadio: any;
    isGuarantorProperty = IsGuarantorProperty;
    getfileApplicationid: number | any
    applicationNumber: number | any

    @ViewChild('contactModal') contactModal: ApplicationDetailsContactModalComponent;
    @ViewChild('emailHistoryModal') emailHistoryModal: ApplicationEmailHistoryModalComponent;
    @ViewChild('ApplicationDocUploadPopupComponent') ApplicationDocUploadPopupComponent: ApplicationDocUploadPopupComponent;

    constructor(
        private formBuilder: FormBuilder,
        private imfsUtilityService: IMFSUtilityService,
        private jsUtilityService: JsUtilityService,
        private formUtility: IMFSFormService,
        private optionsControllerService: OptionsControllerService,
        private emailControllerService: EmailControllerService,
        private applicationControllerService: ApplicationControllerService,
        private authenticationService: AuthenticationService,
        private quoteControllerService: QuoteControllerService,
        private router: Router,
        private route: ActivatedRoute,
        private confirmationService: ConfirmationService,
        private http: HttpClient,
        private messageService: MessageService) {
    }

    ngOnInit() {
        this.initForm();
        this.activeTabIndex = 0;
        this.applicationIdGlobal = 0;
        this.getApplicationStatus();
        this.route.queryParamMap.subscribe((queryParams) => {
            this.getfileApplicationid = queryParams.get("id");
        })

        // that.todayDate = new Date();
        const obsComb = combineLatest([this.route.paramMap, this.route.queryParams]);

        obsComb.pipe(takeWhile(() => this.active)).subscribe((params: any) => {
            //const applicationId = params[0].get('id');
            //const mode = params['mode'];

            this.route.queryParamMap.subscribe(queryParams => {
                const applicationId = queryParams.get("id");
                const mode = queryParams.get("mode");

                if (mode === "view") {
                    this.controlReadOnly = true;
                }

                if (applicationId) {
                    this.header = 'Application Details';
                    if (this.authenticationService.userHasRole('IMStaffAdmin')) {
                        this.IMStaffAdminRole = true;
                    }
                    this.getApplicationDetails(parseInt(applicationId));
                    this.applicationIdGlobal = parseInt(applicationId);
                    this.setFormControlsByPermission();
                }
            });
        });

        this.itemsForSteps = [{ label: 'Customer Details' }, { label: 'Finance Information' }, { label: 'Signatories' }]


        this.navigateApplication(0, 'next');


        // this.downloadMenuItems = [
        //     {label: 'Generate Proposal (xlsx)', icon: 'pi pi-cloud-download', command: () => {
        //         this.downloadQuote('Proposal');
        //     },
        // },
        // ];
    }

    initForm() {
        this.applicationCustomerDetailsForm = this.formBuilder.group({
            Id: new FormControl({ value: '', disabled: true }),
            ApplicationNumber: new FormControl({ value: '', disabled: true }),
            Status: new FormControl(''),
            CreatedDate: new FormControl({ value: '', disabled: true }),
            EntityType: new FormControl(''),
            EntityTrustType: new FormControl(''),
            EntityTypeOther: new FormControl(''),
            EntityTrustOther: new FormControl(''),
            EntityTrustName: new FormControl(''),
            EntityTrustABN: new FormControl(''),
            FinanceFunder: new FormControl(''),
            FinanceFunderName: new FormControl(''),
            FinanceFunderEmail: new FormControl(''),
            EndCustomerName: new FormControl(''),
            EndCustomerABN: new FormControl(''),
            EndCustomerTradingAs: new FormControl(''),
            EndCustomerPhone: new FormControl(''),
            BusinessActivity: new FormControl(''),
            EndCustomerFax: new FormControl(''),
            EndCustomerYearsTrading: new FormControl(''),
            AveAnnualSales: new FormControl(''),
            EndCustomerContactName: new FormControl(''),
            EndCustomerContactPhone: new FormControl(''),
            EndCustomerContactEmail: new FormControl(''),
            EndCustomerPrimaryAddressLine1: new FormControl(''),
            EndCustomerPrimaryAddressLine2: new FormControl(''),
            EndCustomerPrimaryCity: new FormControl(''),
            EndCustomerPrimaryState: new FormControl(''),
            EndCustomerPrimaryCountry: new FormControl(''),
            EndCustomerPrimaryPostcode: new FormControl(''),

            EndCustomerPostalAddressLine1: new FormControl(''),
            EndCustomerPostalAddressLine2: new FormControl(''),
            EndCustomerPostalCity: new FormControl(''),
            EndCustomerPostalState: new FormControl(''),
            EndCustomerPostalCountry: new FormControl(''),
            EndCustomerPostalPostcode: new FormControl(''),

            EndCustomerDeliveryAddressLine1: new FormControl(''),
            EndCustomerDeliveryAddressLine2: new FormControl(''),
            EndCustomerDeliveryCity: new FormControl(''),
            EndCustomerDeliveryState: new FormControl(''),
            EndCustomerDeliveryCountry: new FormControl(''),
            EndCustomerDeliveryPostcode: new FormControl(''),
        });

        this.financeInformationForm = this.formBuilder.group({
            QuoteID: new FormControl({ value: '', disabled: true }),
            GoodsDescription: new FormControl(''),
            FinanceTotal: new FormControl(''),
            QuoteTotal: new FormControl(''),
            FinanceFunderName: new FormControl(''),
            FinanceType: new FormControl(''),
            FinanceFrequency: new FormControl(''),
            FinanceDuration: new FormControl(''),
            FinanceValue: new FormControl(''),
            ResellerName: new FormControl(''),
            ResellerContactName: new FormControl(''),
            ResellerId: new FormControl(''),
            FunderQuote: new FormControl(''),
            RevisedApprovedAmount: new FormControl(''),
            DownPaymentAmount: new FormControl(''),
            PaymentWithoutTax: new FormControl(''),
            EstimatedTaxAmount: new FormControl(''),
            PaymentAmount: new FormControl(''),
            UpFrontPayment: new FormControl(''),
            BankGuaranteeLetterOfCreditPcr: new FormControl(''),
            TermUpperLimit: new FormControl(''),
            LeaseStructureNotPermitted: new FormControl(''),
            ArrearsMonthlyNotPermitted: new FormControl(''),
            ArrearsQuarterlyNotPermitted: new FormControl('')
        });

        this.signatoriesForm = this.formBuilder.group({
            IsGuarantorPropertyOwner: new FormControl(''),
            GuarantorSecurityValue: new FormControl(''),
            GuarantorSecurityOwing: new FormControl(''),

            items: this.formBuilder.array([this.createTableRow(new ApplicationContactData())]),

            guarantorItems: this.formBuilder.array([this.createTableRow(new ApplicationContactData())]),
            trusteeItems: this.formBuilder.array([this.createTableRow(new ApplicationContactData())]),
            accountantItems: this.formBuilder.array([this.createTableRow(new ApplicationContactData())]),
            beneficialOwners: this.formBuilder.array([this.createTableRow(new ApplicationContactData())]),

        });

        this.documentForm = this.formBuilder.group({
          LeaseAgreementSignorEmail: new FormControl(''),
          CertificateOfAcceptanceSignorEmail: new FormControl(''),
          AntiMoneyLaunderingSignorEmail: new FormControl(''),
          DirectDebitACHSignorEmail: new FormControl(''),
        });


        const that = this;

    }


    getApplicationStatus() {
        this.optionsControllerService.getStatus(false, false, true).subscribe(
            (response: StatusModel[]) => {
                this.imfsUtilityService.hideLoading();
                if (response.length === 0) {
                    this.imfsUtilityService.showToastr('error', 'Failed', 'No Status found');
                }
                else {
                    this.applicationStatusOptions = response;
                }
            },
            (err: any) => {
                console.log(err);
                this.imfsUtilityService.hideLoading();
                this.imfsUtilityService.showToastr('error', 'Failed', 'Error in Get Status');
            }
        );
    }

    getApplicationDetails(applicationId: number) {
        this.imfsUtilityService.showLoading('Loading...');
        this.applicationControllerService.getApplicationDetails(applicationId).subscribe(
            (response: ApplicationDetailsResponseModel) => {

              this.imfsUtilityService.hideLoading();
              this.resellerIdGlobal = response.applicationDetails.resellerDetails.resellerID;
              this.applicationNumber = response.applicationDetails.applicationNumber;
              this.getFiles();
              this.getSignatoriesDetails(parseInt(this.applicationNumber), this.resellerIdGlobal);
              console.log(response.applicationDetails);
                this.setFormValue(response.applicationDetails);

                // Disable form if it has been End Customer Accepted
                // if (Number(response.applicationDetails.status) === this.applicationApproved) {
                //     this.applicationCustomerDetailsForm.disable();
                //     this.financeInformationForm.disable();
                //     this.signatoriesForm.disable();
                // }

            },
            (err: any) => {
                console.log(err);
                this.imfsUtilityService.hideLoading();
                this.imfsUtilityService.showToastr('error', 'Failed', 'Error in getting Application Details');
            }
        );

    }


    getSignatoriesDetails(applicationNumber: any, resellerID: any) {
        this.imfsUtilityService.showLoading('Loading...');
        this.applicationControllerService.getSignatoriesDetails(applicationNumber, resellerID).subscribe(
            (response: ApplicationContactResponseModel) => {
                this.imfsUtilityService.hideLoading();

                this.guarantorItems.clear();
                this.trusteeItems.clear();
                this.accountantItems.clear();
                this.beneficialOwners.clear();

                if(response.guarantors.length > 0) {
                  response.guarantors.forEach(
                  (guarantor) => {
                    guarantor.dob = guarantor.dob != "0001-01-01T00:00:00" ? guarantor.dob : null
                    this.guarantorItems.push(this.createTableRow(guarantor));
                  }
                  )
                }



                if(response.trustees.length > 0) {
                  response.trustees.forEach(
                    (trustees) => {
                      trustees.dob = trustees.dob != "0001-01-01T00:00:00" ? trustees.dob : null
                      this.trusteeItems.push(this.createTableRow(trustees));
                    }
                  )
                }

                if(response.accountants.length > 0) {
                  response.accountants.forEach(
                    (accountant) => {
                      accountant.dob = accountant.dob != "0001-01-01T00:00:00" ? accountant.dob : null
                      this.accountantItems.push(this.createTableRow(accountant));
                    }
                    )
                  }


                  if(response.owners.length > 0) {
                    response.owners.forEach(
                      (owner) => {
                      owner.dob = owner.dob != "0001-01-01T00:00:00" ? owner.dob : null
                      this.beneficialOwners.push(this.createTableRow(owner));
                   }
                  )
                }
            },
            (err: any) => {
                console.log(err);
                this.imfsUtilityService.hideLoading();
                this.imfsUtilityService.showToastr('error', 'Failed', 'Error in getting Application Signatories');
            }
        );

    }


    createTableRow(item: ApplicationContactData): FormGroup {
        return this.formBuilder.group({
          fullName: item.fullName,
          email: item.email,
          residentialAddress: item.residentialAddress,
          dob: item.dob,
          driversLicNo: item.driversLicNo,
          phone: item.phone,
          abn: item.abn,
          type: item.type,
          contactType: item.contactType,
          contactID: item.contactID,
          signatory: item.signatory
        });
    }

    get guarantorItems(): FormArray {
        if (this.signatoriesForm) {
            return this.signatoriesForm.get('guarantorItems') as FormArray;
        }
        return this.formBuilder.array([this.createTableRow(new ApplicationContactData())]);
    }

    get trusteeItems(): FormArray {
        if (this.signatoriesForm) {
            return this.signatoriesForm.get('trusteeItems') as FormArray;
        }
        return this.formBuilder.array([this.createTableRow(new ApplicationContactData())]);
    }

    get accountantItems(): FormArray {
        if (this.signatoriesForm) {
            return this.signatoriesForm.get('accountantItems') as FormArray;
        }
        return this.formBuilder.array([this.createTableRow(new ApplicationContactData())]);
    }

    get beneficialOwners(): FormArray {
        if (this.signatoriesForm) {
            return this.signatoriesForm.get('beneficialOwners') as FormArray;
        }
        return this.formBuilder.array([this.createTableRow(new ApplicationContactData())]);
    }


    setFormValue(applicationDetails: ApplicationDetailsModel): void {

        this.applicationCustomerDetailsForm.patchValue({
            Id: applicationDetails.id,
            ApplicationNumber: applicationDetails.applicationNumber,
            Status: applicationDetails.status,
            CreatedDate: moment(applicationDetails.createdDate).toDate(),
            FinanceFunder: applicationDetails.financeFunder,
            FinanceFunderEmail: applicationDetails.financeFunderEmail,
            EntityType: applicationDetails.entityDetails.entityType,
            EntityTrustType: applicationDetails.entityDetails.entityTrustType,
            EntityTypeOther: applicationDetails.entityDetails.entityTypeOther,
            EntityTrustOther: applicationDetails.entityDetails.entityTrustOther,
            EntityTrustName: applicationDetails.entityDetails.entityTrustName,
            EntityTrustABN: applicationDetails.entityDetails.entityTrustABN,
            EndCustomerName: applicationDetails.endCustomerDetails.endCustomerName,
            EndCustomerABN: applicationDetails.endCustomerDetails.endCustomerABN,
            EndCustomerTradingAs: applicationDetails.endCustomerDetails.endCustomerTradingAs,
            EndCustomerPhone: applicationDetails.endCustomerDetails.endCustomerPhone,
            BusinessActivity: applicationDetails.businessActivity,
            EndCustomerFax: applicationDetails.endCustomerDetails.endCustomerFax,
            EndCustomerYearsTrading: applicationDetails.endCustomerDetails.endCustomerYearsTrading,
            AveAnnualSales: applicationDetails.aveAnnualSales,
            EndCustomerContactName: applicationDetails.endCustomerDetails.endCustomerContactName,
            EndCustomerContactPhone: applicationDetails.endCustomerDetails.endCustomerContactPhone,
            EndCustomerContactEmail: applicationDetails.endCustomerDetails.endCustomerContactEmail,
            EndCustomerPrimaryAddressLine1: applicationDetails.endCustomerDetails.endCustomerPrimaryAddressLine1,
            EndCustomerPrimaryAddressLine2: applicationDetails.endCustomerDetails.endCustomerPrimaryAddressLine2,
            EndCustomerPrimaryCity: applicationDetails.endCustomerDetails.endCustomerPrimaryCity,
            EndCustomerPrimaryState: applicationDetails.endCustomerDetails.endCustomerPrimaryState,
            EndCustomerPrimaryCountry: applicationDetails.endCustomerDetails.endCustomerPrimaryCountry,
            EndCustomerPrimaryPostcode: applicationDetails.endCustomerDetails.endCustomerPrimaryPostcode,

            EndCustomerPostalAddressLine1: applicationDetails.endCustomerDetails.endCustomerPostalAddressLine1,
            EndCustomerPostalAddressLine2: applicationDetails.endCustomerDetails.endCustomerPostalAddressLine2,
            EndCustomerPostalCity: applicationDetails.endCustomerDetails.endCustomerPostalCity,
            EndCustomerPostalState: applicationDetails.endCustomerDetails.endCustomerPostalState,
            EndCustomerPostalCountry: applicationDetails.endCustomerDetails.endCustomerPostalCountry,
            EndCustomerPostalPostcode: applicationDetails.endCustomerDetails.endCustomerPostalPostcode,

            EndCustomerDeliveryAddressLine1: applicationDetails.endCustomerDetails.endCustomerDeliveryAddressLine1,
            EndCustomerDeliveryAddressLine2: applicationDetails.endCustomerDetails.endCustomerDeliveryAddressLine2,
            EndCustomerDeliveryCity: applicationDetails.endCustomerDetails.endCustomerDeliveryCity,
            EndCustomerDeliveryState: applicationDetails.endCustomerDetails.endCustomerDeliveryState,
            EndCustomerDeliveryCountry: applicationDetails.endCustomerDetails.endCustomerDeliveryCountry,
            EndCustomerDeliveryPostcode: applicationDetails.endCustomerDetails.endCustomerDeliveryPostcode,

        });

        this.guarantorItems.clear();
        this.trusteeItems.clear();
        this.accountantItems.clear();
        this.beneficialOwners.clear();

        this.financeInformationForm.patchValue({
            QuoteID: applicationDetails.quoteID,
            GoodsDescription: applicationDetails.goodsDescription,
            FinanceTotal: applicationDetails.financeTotal,
            QuoteTotal: applicationDetails.quoteTotal,
            FinanceFunderName: applicationDetails.financeFunderName,
            FinanceType: applicationDetails.financeType,
            FinanceFrequency: applicationDetails.financeFrequency,
            FinanceDuration: applicationDetails.financeDuration,
            FinanceValue: applicationDetails.financeValue,
            ResellerName: applicationDetails.resellerDetails.resellerName,
            ResellerContactName: applicationDetails.resellerDetails.resellerContactName,
            ResellerId: applicationDetails.resellerDetails.resellerID,
            FunderQuote: applicationDetails.funderQuote,
            RevisedApprovedAmount: applicationDetails.revisedApprovedAmount,
            DownPaymentAmount: applicationDetails.downPaymentAmount,
            PaymentWithoutTax: applicationDetails.paymentWithoutTax,
            EstimatedTaxAmount: applicationDetails.estimatedTaxAmount,
            PaymentAmount: applicationDetails.paymentAmount,
            UpFrontPayment: applicationDetails.upFrontPayment,
            BankGuaranteeLetterOfCreditPcr: applicationDetails.bankGuaranteeLetterOfCreditPcr,
            TermUpperLimit: applicationDetails.termUpperLimit,
            LeaseStructureNotPermitted: applicationDetails.leaseStructureNotPermitted,
            ArrearsMonthlyNotPermitted: applicationDetails.arrearsMonthlyNotPermitted,
            ArrearsQuarterlyNotPermitted: applicationDetails.arrearsQuarterlyNotPermitted
        });

        this.items.clear();
        // applicationDetails.applicationContacts.forEach((appContact: ApplicationContact) => {

        //     this.items.push(this.createTableRow(appContact));
        // });

        this.signatoriesForm.patchValue({
            IsGuarantorPropertyOwner: applicationDetails.isGuarantorPropertyOwner,
            GuarantorSecurityValue: applicationDetails.guarantorSecurityValue,
            GuarantorSecurityOwing: applicationDetails.guarantorSecurityOwing,
        });

        this.documentForm.patchValue({
            LeaseAgreementSignorEmail: applicationDetails.leaseAgreementSignorEmail,
            CertificateOfAcceptanceSignorEmail: applicationDetails.certificateOfAcceptanceSignorEmail,
            AntiMoneyLaunderingSignorEmail: applicationDetails.antiMoneyLaunderingSignorEmail,
            DirectDebitACHSignorEmail: applicationDetails.directDebitACHSignorEmail,
        });


    }

    setFormControlsByPermission() {
        if (this.IMStaffAdminRole) {
            this.applicationCustomerDetailsForm.get('Status')?.enable();
        }
        else {
            this.applicationCustomerDetailsForm.get('Status')?.disable();
        }
    }

    tabChange(event: any) {
        if (event.index === 1) {
            // quote header text
            // if (!this.applicationCustomerDetailsForm.valid) {
            //     this.imfsUtilityService.showToastr('error', 'Invalid Input', 'Please enter all required fields.');
            //     setTimeout(() => {
            //         this.activeTabIndex = 0;
            //     }, 0);
            // }
        } else if (event.index === 2) {
            // going to summary tab
            // if (this.quoteLineForm.valid) {
            //     this.imfsUtilities.showToastr('error', 'Invalid Input', 'Please enter all required fields.');
            //     setTimeout(() => {
            //         this.activeTabIndex = 0;
            //     }, 0);
            // }
        }
    }


    getApplicationGuarantorItems() {
        return this.guarantorItems;
    }

    getApplicationTrusteeItems() {

    }

    getApplicationAccountantItems() {

    }

    getApplicationBeneficialOwnersItems() {

    }

    saveApplication() {
        const applicationDetails = this.convertFormToModel();
        console.log(applicationDetails)
        this.imfsUtilityService.showLoading('Saving...');
        // save quote details to server
        this.applicationControllerService.saveApplication(applicationDetails).subscribe(
            (response: HttpResponseData) => {

              this.imfsUtilityService.hideLoading();
              this.imfsUtilityService.showToastr('success', 'Success', 'Application Updated Successfully:' + response.data);
              // void this.router.navigate([IMFSRoutes.Application], { queryParams: { id: response.data, appNo: this.applicationNumber, mode: 'edit'}});


              // save quote details to server
              // this.applicationControllerService.saveContact(applicationDetails).subscribe(
              //     (response: HttpResponseData) => {
              //       this.imfsUtilityService.hideLoading();
              //       this.imfsUtilityService.showToastr('success', 'Success', 'Application Updated Successfully:' + response.data);
              //       void this.router.navigate([IMFSRoutes.Application], { queryParams: { id: response.data, mode: 'edit'}});
              //     },
              //     (err: any) => {
              //         console.log(err);
              //         this.imfsUtilityService.hideLoading();
              //         this.imfsUtilityService.showToastr('error', 'Failed', 'Error Saving Application');
              //     }
              // );
            },
            (err: any) => {
                console.log(err);
                this.imfsUtilityService.hideLoading();
                this.imfsUtilityService.showToastr('error', 'Failed', 'Error Saving Application');
            }
        );

    }








    convertFormToModel() {
        // const quoteDetails = new QuoteDetailsModel();
        this.applicationDetails = new ApplicationDetailsModel();
        this.applicationDetails.entityDetails = new EntityDetails();
        this.applicationDetails.endCustomerDetails = new EndCustomerDetails();
        this.applicationDetails.resellerDetails = new ResellerDetails();
        this.applicationDetails.applicationContacts = [];

        this.applicationDetails.isGuarantorPropertyOwner = this.signatoriesForm.get('IsGuarantorPropertyOwner')?.value;
        this.applicationDetails.guarantorSecurityValue = this.signatoriesForm.get('GuarantorSecurityValue')?.value;
        this.applicationDetails.guarantorSecurityOwing = this.signatoriesForm.get('GuarantorSecurityOwing')?.value;

        this.applicationDetails.id = this.applicationCustomerDetailsForm.get('Id')?.value;
        this.applicationDetails.applicationNumber = this.applicationCustomerDetailsForm.get('ApplicationNumber')?.value;
        this.applicationDetails.status = this.applicationCustomerDetailsForm.get('Status')?.value;
        this.applicationDetails.entityDetails.entityType = this.entityTypeRadio ? this.entityTypeRadio : ""
        this.applicationDetails.entityDetails.entityTrustName = this.applicationCustomerDetailsForm.get('EntityTrustName')?.value;
        this.applicationDetails.entityDetails.entityTrustABN = this.applicationCustomerDetailsForm.get('EntityTrustABN')?.value;

        if (this.applicationCustomerDetailsForm.get('EntityType')?.value === this.otherEntityType) {
          this.applicationDetails.entityDetails.entityTypeOther = this.applicationCustomerDetailsForm.get('EntityTypeOther')?.value;
        }

        this.applicationDetails.entityDetails.entityTrustType = this.applicationCustomerDetailsForm.get('EntityTrustType')?.value;

        if (this.applicationCustomerDetailsForm.get('EntityTrustType')?.value === this.otherTrustType) {
            this.applicationDetails.entityDetails.entityTrustOther = this.applicationCustomerDetailsForm.get('EntityTrustOther')?.value;
        }

        this.applicationDetails.endCustomerDetails.endCustomerName = this.applicationCustomerDetailsForm.get('EndCustomerName')?.value;
        this.applicationDetails.endCustomerDetails.endCustomerABN = this.applicationCustomerDetailsForm.get('EndCustomerABN')?.value;
        // tslint:disable-next-line: max-line-length
        this.applicationDetails.endCustomerDetails.endCustomerTradingAs = this.applicationCustomerDetailsForm.get('EndCustomerTradingAs')?.value;
        // tslint:disable-next-line: max-line-length
        this.applicationDetails.endCustomerDetails.endCustomerPhone = this.applicationCustomerDetailsForm.get('EndCustomerPhone')?.value;
        this.applicationDetails.businessActivity = this.applicationCustomerDetailsForm.get('BusinessActivity')?.value;
        this.applicationDetails.endCustomerDetails.endCustomerFax = this.applicationCustomerDetailsForm.get('EndCustomerFax')?.value;
        this.applicationDetails.endCustomerDetails.endCustomerYearsTrading = this.applicationCustomerDetailsForm.get('EndCustomerYearsTrading')?.value;
        this.applicationDetails.aveAnnualSales = this.applicationCustomerDetailsForm.get('AveAnnualSales')?.value;
        // tslint:disable-next-line: max-line-length
        this.applicationDetails.endCustomerDetails.endCustomerContactName = this.applicationCustomerDetailsForm.get('EndCustomerContactName')?.value;
        this.applicationDetails.endCustomerDetails.endCustomerContactEmail = this.applicationCustomerDetailsForm.get('EndCustomerContactEmail')?.value;
        this.applicationDetails.endCustomerDetails.endCustomerContactPhone = this.applicationCustomerDetailsForm.get('EndCustomerContactPhone')?.value;


        // Business Address
        this.applicationDetails.endCustomerDetails.endCustomerPrimaryAddressLine1 = this.applicationCustomerDetailsForm.get('EndCustomerPrimaryAddressLine1')?.value;
        this.applicationDetails.endCustomerDetails.endCustomerPrimaryAddressLine2 = this.applicationCustomerDetailsForm.get('EndCustomerPrimaryAddressLine2')?.value;
        // tslint:disable-next-line: max-line-length
        this.applicationDetails.endCustomerDetails.endCustomerPrimaryCity = this.applicationCustomerDetailsForm.get('EndCustomerPrimaryCity')?.value;
        this.applicationDetails.endCustomerDetails.endCustomerPrimaryState = this.applicationCustomerDetailsForm.get('EndCustomerPrimaryState')?.value;
        this.applicationDetails.endCustomerDetails.endCustomerPrimaryPostcode = this.applicationCustomerDetailsForm.get('EndCustomerPrimaryPostcode')?.value;
        this.applicationDetails.endCustomerDetails.endCustomerPrimaryCountry = this.applicationCustomerDetailsForm.get('EndCustomerPrimaryCountry')?.value;

        // Postal Address
        this.applicationDetails.endCustomerDetails.endCustomerPostalAddressLine1 = this.applicationCustomerDetailsForm.get('EndCustomerPostalAddressLine1')?.value;
        this.applicationDetails.endCustomerDetails.endCustomerPostalAddressLine2 = this.applicationCustomerDetailsForm.get('EndCustomerPostalAddressLine2')?.value;
        // tslint:disable-next-line: max-line-length
        this.applicationDetails.endCustomerDetails.endCustomerPostalCity = this.applicationCustomerDetailsForm.get('EndCustomerPostalCity')?.value;
        // tslint:disable-next-line: max-line-length
        this.applicationDetails.endCustomerDetails.endCustomerPostalState = this.applicationCustomerDetailsForm.get('EndCustomerPostalState')?.value;
        this.applicationDetails.endCustomerDetails.endCustomerPostalPostcode = this.applicationCustomerDetailsForm.get('EndCustomerPostalPostcode')?.value;
        this.applicationDetails.endCustomerDetails.endCustomerPostalCountry = this.applicationCustomerDetailsForm.get('EndCustomerPostalCountry')?.value;

        // Delivery Address
        this.applicationDetails.endCustomerDetails.endCustomerDeliveryAddressLine1 = this.applicationCustomerDetailsForm.get('EndCustomerDeliveryAddressLine1')?.value;
        this.applicationDetails.endCustomerDetails.endCustomerDeliveryAddressLine2 = this.applicationCustomerDetailsForm.get('EndCustomerDeliveryAddressLine2')?.value;
        // tslint:disable-next-line: max-line-length
        this.applicationDetails.endCustomerDetails.endCustomerDeliveryCity = this.applicationCustomerDetailsForm.get('EndCustomerDeliveryCity')?.value;
        // tslint:disable-next-line: max-line-length
        this.applicationDetails.endCustomerDetails.endCustomerDeliveryState = this.applicationCustomerDetailsForm.get('EndCustomerDeliveryState')?.value;
        this.applicationDetails.endCustomerDetails.endCustomerDeliveryPostcode = this.applicationCustomerDetailsForm.get('EndCustomerDeliveryPostcode')?.value;
        this.applicationDetails.endCustomerDetails.endCustomerDeliveryCountry = this.applicationCustomerDetailsForm.get('EndCustomerDeliveryCountry')?.value;

        this.applicationDetails.leaseAgreementSignorEmail = this.documentForm.get('LeaseAgreementSignorEmail')?.value;
        this.applicationDetails.certificateOfAcceptanceSignorEmail = this.documentForm.get('CertificateOfAcceptanceSignorEmail')?.value;
        this.applicationDetails.antiMoneyLaunderingSignorEmail = this.documentForm.get('AntiMoneyLaunderingSignorEmail')?.value;
        this.applicationDetails.directDebitACHSignorEmail = this.documentForm.get('DirectDebitACHSignorEmail')?.value;

        this.guarantorItems.controls.forEach((item: AbstractControl) => {
            const appContact = new ApplicationContact();

            appContact.contactID = 'string';
            appContact.contactName = 'string';
            appContact.contactEmail = 'string';
            appContact.contactAddress = 'string';
            appContact.contactDOB = new Date("2022-09-23T06:55:33.571Z");
            appContact.contactDriversLicNo = 'string';
            appContact.contactPhone = 'string';
            appContact.contactType = 4;
            appContact.contactABNACN = 'string';

            // appContact.contactID = item.get('ContactID')?.value;
            // appContact.contactName = item.get('ContactName')?.value;
            // appContact.contactEmail = item.get('ContactEmail')?.value;
            // appContact.contactAddress = item.get('ContactAddress')?.value;
            // appContact.contactDOB = item.get('ContactDOB')?.value;
            // appContact.contactDriversLicNo = item.get('ContactDriversLicNo')?.value;
            // appContact.contactPhone = item.get('ContactPhone')?.value;
            // appContact.contactType = item.get('ContactType')?.value;
            // appContact.contactABNACN = item.get('ContactABNACN')?.value;

            // "applicationNumber": 0,
            // "resellerID": "string",
            // "contactID": "string",
            // "contactPosition": "string",
            // "isContactSignatory": true,


            // Add to the list
            this.applicationDetails.applicationContacts.push(appContact);
        });

        this.trusteeItems.controls.forEach((item: AbstractControl) => {
            const appContact = new ApplicationContact();

            appContact.contactID = item.get('ContactID')?.value;
            appContact.contactName = item.get('ContactName')?.value;
            appContact.contactEmail = item.get('ContactEmail')?.value;
            appContact.contactAddress = item.get('ContactAddress')?.value;
            appContact.contactDOB = item.get('ContactDOB')?.value;
            appContact.contactDriversLicNo = item.get('ContactDriversLicNo')?.value;
            appContact.contactPhone = item.get('ContactPhone')?.value;
            appContact.contactType = item.get('ContactType')?.value;
            appContact.contactABNACN = item.get('ContactABNACN')?.value;

            // Add to the list
            this.applicationDetails.applicationContacts.push(appContact);
        });

        this.beneficialOwners.controls.forEach((item: AbstractControl) => {
            const appContact = new ApplicationContact();

            appContact.contactID = item.get('ContactID')?.value;
            appContact.contactName = item.get('ContactName')?.value;
            appContact.contactEmail = item.get('ContactEmail')?.value;
            appContact.contactAddress = item.get('ContactAddress')?.value;
            appContact.contactDOB = item.get('ContactDOB')?.value;
            appContact.contactDriversLicNo = item.get('ContactDriversLicNo')?.value;
            appContact.contactPhone = item.get('ContactPhone')?.value;
            appContact.contactType = item.get('ContactType')?.value;
            appContact.contactABNACN = item.get('ContactABNACN')?.value;

            // Add to the list
            this.applicationDetails.applicationContacts.push(appContact);
        });


        return this.applicationDetails;
    }

    getFormData(rowData: any, key: string) {

        if (key === 'ContactDOB') {
            return moment(rowData.get(key).value).format('DD/MM/YYYY');
        }
        if (rowData && rowData.get(key)) {
            return rowData.get(key).value;
        }
        return '';
    }


    get items(): FormArray {
        if (this.signatoriesForm) {
            return this.signatoriesForm.get('items') as FormArray;
        }
        return this.formBuilder.array([this.createTableRow(new ApplicationContactData())]);
    }


    insertNewGuarantorItem() {
        const applicationContact = new ApplicationContactData();
        this.guarantorItems.push(this.createTableRow(applicationContact));
    }

    downloadApplication() {

        this.imfsUtilityService.showLoading('Downloading...');
        const applicationDownloadInput = new ApplicationDownloadInput();
        applicationDownloadInput.ApplicationNumber = this.getFormData(this.applicationCustomerDetailsForm, 'ApplicationNumber');

        this.applicationControllerService.downloadApplication(applicationDownloadInput).subscribe(
            (res: any) => {
                this.jsUtilityService.fileSaveAs(res);
                this.imfsUtilityService.hideLoading();
            },
            (err: any) => {
                this.imfsUtilityService.hideLoading();
                console.log(err);
                this.imfsUtilityService.showToastr('error', 'Error', 'Unable to download file.');
            }
        );

    }

    emailApplication(applicationId: number, applicationNumber: string | number) {
        // tslint:disable-next-line: max-line-length
        void this.router.navigate([IMFSRoutes.Email], {
            queryParams: {
                applicationId: applicationId,
                applicationNumber: applicationNumber,
                toEmail: this.applicationCustomerDetailsForm.controls.EndCustomerContactEmail.value
            }
        });
    }


    getApplicationId() {
        return this.applicationCustomerDetailsForm.controls.Id.value;
    }

    emailFunder(applicationNumber: number) {
        void this.router.navigate([IMFSRoutes.Email], {
            queryParams: {
                applicationId: applicationNumber,
                toEmail: this.applicationCustomerDetailsForm.controls.FinanceFunderEmail.value
            }
        });
    }

    openEmailHistoryModal() {
        this.emailHistoryModal.open(this.applicationCustomerDetailsForm.controls.ApplicationNumber.value);
    }

    removeContactLocally(type: number, index: number) {
      console.log(type, index)
      if(type === 4) {
        console.log(this.guarantorItems)
        this.guarantorItems.removeAt(index);

        console.log(this.guarantorItems)
      } else if(type === 5) {
        this.trusteeItems.removeAt(index);
      } else if(type === 7) {
        this.beneficialOwners.removeAt(index);
      }
    }

    openContactModal(type: string) {
        this.contactModal.selectedContact = null;
        this.contactModal.currentItem = new ApplicationContactData;
        this.contactModal.open(type, this.resellerIdGlobal);
      }

      onDeleteSignatory(contactID: string, type: number, rowIndex: number) {
        console.log(contactID, type, rowIndex)
        this.confirmationService.confirm({
          message: 'Are you sure that you want to delete?',
          header: 'Confirmation',
          icon: 'pi pi-exclamation-triangle',
          accept: () => {

            this.imfsUtilityService.showLoading('Loading...');
            this.applicationControllerService.deleteContact(contactID).subscribe(
            (response: any) => {

            this.imfsUtilityService.hideLoading();
            this.removeContactLocally(type, rowIndex);

            },
            (err: any) => {
            console.log(err);
            this.imfsUtilityService.hideLoading();
            this.imfsUtilityService.showToastr('error', 'Failed', 'Error in removing contact');
            }
            );
          },
          reject: () => {
          }

      });
      }

      onContactSelected(event: any, edit = 0) {

        let contactData: any;
        if(edit) {
        contactData = {
        contactEmail: event.controls.email.value,
        contactABNACN: event.controls.abn.value,
        contactAddress: event.controls.residentialAddress.value,
        contactName: event.controls.fullName.value,
        contactDriversLicNo: String(event.controls.driversLicNo.value),
        contactPhone: String(event.controls.phone.value),
        contactType: event.controls.contactType.value,
        contactPosition: event.controls.type.value,
        resellerID: this.resellerIdGlobal,
        isContactSignatory: event.controls.signatory.value,
        contactID: event.controls.contactID.value,
        applicationNumber: this.applicationNumber
      };

      contactData.contactDOB = moment(event.controls.dob.value).toDate();

    }
        else {
          if (this.contactModal?.currentItem?.fullName) {
            contactData = {
                contactEmail: this.contactModal.currentItem.email,
                contactABNACN: this.contactModal.currentItem.abn,
                contactAddress: this.contactModal.currentItem.residentialAddress,
                contactName: this.contactModal.currentItem.fullName,
                contactDriversLicNo: String(this.contactModal.currentItem.driversLicNo),
                contactPhone: String(this.contactModal.currentItem.phone),
                contactType: (this.contactModal.currentItem.contactType) ? this.contactModal.currentItem.contactType : this.contactModal.selectedContact.contactType,
                contactPosition: this.contactModal.currentItem.type,
                resellerID: this.resellerIdGlobal,
                isContactSignatory: this.contactModal.currentItem.signatory,
                contactID: null,
                applicationNumber: this.applicationNumber
              };
              contactData.contactDOB = moment(this.contactModal.currentItem.dob).toDate();
            }
          else if (this.contactModal?.selectedContact?.contactName) {
            contactData = this.contactModal.selectedContact;
            contactData.contactABNACN = this.contactModal.selectedContact.contactABNACN;
            contactData.applicationNumber = this.applicationNumber;
            contactData.resellerID = this.resellerIdGlobal;
            contactData.contactPosition = this.contactModal.selectedContact.contactTypeDescription;
            contactData.contactType = this.contactModal.selectedContact.contactType;
            contactData.contactID = this.contactModal.selectedContact.contactID;
            // contactData.contactID = this.contactModal.selectedContact.contactID;
            contactData.contactDOB = this.contactModal.selectedContact.contactDOB;
          }
        }



            // delete contactData.contactDOB;

            this.imfsUtilityService.showLoading('Loading...');
            this.applicationControllerService.saveContacts(contactData).subscribe(
            (response: any) => {

            this.imfsUtilityService.hideLoading();
            this.getSignatoriesDetails(parseInt(this.applicationNumber), this.resellerIdGlobal);

            },
            (err: any) => {
            console.log(err);
            this.imfsUtilityService.hideLoading();
            this.imfsUtilityService.showToastr('error', 'Failed', 'Error in saving Application Details');
            }
            );



    }


    navigateApplication(value: any, back: any) {

        if (back == 'back') {
            value = this.activeIndex - 1
        }
        if (value == 0) {
            this.custDetails = true
            this.financeInfo = false
            this.signatories = false
            this.documents = false
            this.activeIndex = 0

        } else if (value == 1) {
            this.custDetails = false
            this.financeInfo = true
            this.signatories = false
            this.activeIndex = 1
            this.documents = false

        } else if (value == 2) {
            this.custDetails = false
            this.financeInfo = false
            this.signatories = true
            this.documents = false
            this.activeIndex = 2

        } else if (value == 3) {
            this.custDetails = false
            this.financeInfo = false
            this.signatories = false
            this.documents = true
            this.activeIndex = 3
        }
    }

    displayDocumentUploadModal = false;
    //uploadFilebutton:boolean=true;
    showDocumentUploadDialog() {
        if (this.getfileApplicationid) {
            this.displayDocumentUploadModal = true;
        }
        else {
            this.imfsUtilityService.showToastr('error', 'Save Data', 'Please save the quote before uploading the file');
            this.displayDocumentUploadModal = false;
        }
    }
    closeDocumentPopupDialog(): void {
        this.displayDocumentUploadModal = false;
    }
    filesLists: any;
    filelist = new QuoteDocuments();

    getFiles() {
        // if ( this.getfileApplicationid &&  this.getfileApplicationid > 0) {
        if ( this.applicationNumber &&  this.applicationNumber > 0) {
            this.applicationControllerService.getApplicationFileList( this.applicationNumber).subscribe((response: QuoteDocuments) => {
                this.filesLists = response;
            });
        }
    }
    getRemoveFile(event: any) {
        this.getFiles();
    }

    onDeleteFile(fileId: number) {

        // const options = {
        //     headers: new HttpHeaders({
        //         'Content-Type': 'application/json'
        //     }),

        //     body: {
        //         FileId: fileId
        //     }

        // }
        const options = { FileId: fileId };

        this.confirmationService.confirm({
            message: 'Are you sure that you want to proceed?',
            header: 'Confirmation',
            icon: 'pi pi-exclamation-triangle',
            accept: () => {
                this.http.post(environment.API_BASE + '/Quote/DeleteQuoteAttachments', options).subscribe((response) => {
                    this.imfsUtilityService.showToastr('success', 'Success', 'File deleted successfully');
                    this.getRemoveFile('');

                },
                    (err: any) => {
                        console.log(err);
                        this.imfsUtilityService.hideLoading();
                        this.imfsUtilityService.showToastr('error', 'Failed', 'Error in deleting file');
                    })
            },
            reject: () => {
            }

        });
    }
    downloadDocumentFile(fileId: number) {
        if (!fileId) {
            return;
        }
        this.applicationControllerService.downloadQuoteAttachment(fileId).subscribe((response: any) => {
            const fileObj = this.filesLists?.filter((file: QuoteDocuments) => file?.fileId === fileId)[0];
            const fileName = fileObj?.fileName;
            const fileContent = response.body;
            const blob = new Blob([fileContent], { type: 'application/octet-stream; charset=utf-8' });
            saveAs(blob, fileName);
        })
    }
    lastStepTab(){
        this.saveApplication();
        this.documents=true
        this.activeIndex=3
        this.signatories=false
    }
    documentStepTab(){
        this.documents=true
        this.activeIndex=3
        this.signatories=false
    }


}
