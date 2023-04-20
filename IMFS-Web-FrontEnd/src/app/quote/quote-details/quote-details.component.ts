import { Component, OnInit, ViewChild, Output, Input, EventEmitter, ViewEncapsulation } from '@angular/core';
import { AbstractControl, FormArray, FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import * as _ from 'lodash-es';
import * as moment from 'moment';
import { ConfirmationService, ConfirmEventType, MenuItem, MessageService } from 'primeng/api';
import { combineLatest } from 'rxjs';
import { takeWhile } from 'rxjs/operators';
import { CustomerModel, CustomerResponseModel, CustomerContactResponseModel } from 'src/app/models/customer/customer.model';
import { DurationOptions, FinanceTypes, GstTypes, LineTypes, PaymentFrequencyOptions, PaymentPlanTypes, QuoteStatusOptions } from 'src/app/models/drop-down-options/drop-down-options.model';
import { CategoriesModel, StatusModel, TypesModel } from 'src/app/models/options/options.model';
import { ProductDetailsModel, ProductInputModel } from 'src/app/models/product/product.model';
import { CustomerDetails, EndUserDetails, QuoteDetailsModel, QuoteDetailsResponseModel, QuoteDownloadInput, QuoteHeader, QuoteLine, QuoteDocuments } from 'src/app/models/quote/quote.model';
import { RateCalculateInputModel, RateCalculateLineItem, RateCalculateResponseModel } from 'src/app/models/rateCalculate/rateCalculate-input.model';
import { IMFSRoutes } from 'src/app/models/routes/imfs-routes';
import { EmailControllerService } from 'src/app/services/controller-services/email-controller.service';
import { OptionsControllerService } from 'src/app/services/controller-services/options-controller.service';
import { ORPCustomerControllerService } from 'src/app/services/controller-services/orp-customer-controller.service';
import { ProductControllerService } from 'src/app/services/controller-services/product-controller.service';
import { QuoteControllerService } from 'src/app/services/controller-services/quote-controller.service';
import { AuthenticationService } from 'src/app/services/utility-services/authenication.service';
import { IMFSFormService } from 'src/app/services/utility-services/imfs-form-service';
import { IMFSUtilityService } from 'src/app/services/utility-services/imfs-utility-services';
import { JsUtilityService } from 'src/app/services/utility-services/js-utility.service';
import { environment } from 'src/environments/environment';
import { QuoteEmailHistoryModalComponent } from '../quote-email-history-modal/quote-email-history-modal.component';
import { SelectItem } from 'primeng/api';
import { SelectItemGroup } from 'primeng/api';
import { FilterService } from "primeng/api";
import { AuSearchTemplateComponent } from 'src/app/company-search/au-search-template/au-search-template.component';
import { addressvalidateservice } from 'src/app/services/controller-services/address-validate-service';
import { AddressAutoComplete, AddressValidateTokenResponse, AddressInfo } from 'src/app/models/addressModal/addressValidate';
import { HttpResponseData } from 'src/app/models/utility-models/response.model';
import { FunderControllerService } from 'src/app/services/controller-services/funder-controller.service';
import { DocumentUploadPopupComponent } from 'src/app/document-upload-popup/document-upload-popup.component';
import { HttpClient, HttpHeaders } from '@angular/common/http';
interface City {
    name: string,
    code: string
}
@Component({
    selector: 'app-quote-details',
    templateUrl: './quote-details.component.html',
    styleUrls: ['./quote-details.component.scss'],
    encapsulation: ViewEncapsulation.None,
    providers: [FilterService]
})
export class QuoteDetailsComponent implements OnInit {

    invalidEmailCharacters: RegExp = /^[^<>\&*!,$~%*=\+#`\^\\\{\}/\?\[\]:]+$/
    header = 'New Quote';
    quoteHeaderDetailsForm: FormGroup;
    quoteItemsForm: FormGroup;
    isEmptyQuoteLine: boolean = false;
    timeout: any = null;
    timeoutForGst: any = null;
    activeTabIndex = 0;
    sameAsAboveCheckbox: any;
    durationOptions = DurationOptions;
    paymentFrequencyOptions = PaymentFrequencyOptions;
    quoteStatusOptions: StatusModel[];
    todayDate: Date;

    // value for end Customer Accepted
    endCustomerAccepted = 7;
    selectedRateOption = '';
    selectedMonthlyFunderId = '';
    selectedQuarterlyFunderId = '';
    selectedYearlyFunderId = '';
    selectedFrequency = '';
    selectedMonthlyFunder = '';
    selectedQuarterlyFunder = '';
    selectedYearlyFunder = '';


    selectedMonthlyFunderPlanId = '';
    selectedMonthlyFunderPlanDescription = '';
    selectedQuarterlyFunderPlanId = '';
    selectedQuarterlyFunderPlanDescription = '';
    selectedYearlyFunderPlanId = '';
    selectedYearlyFunderPlanDescription = '';

    selectedCustomer: CustomerModel;
    filteredCustomers: CustomerModel[];

    productInputModel: ProductInputModel;

    productDetails: ProductDetailsModel[];

    types: TypesModel[];
    filteredTypes: TypesModel[];
    categories: CategoriesModel[];
    filteredCategories: CategoriesModel[];

    quoteDetails = new QuoteDetailsModel();

    active = true;
    IMStaffRole = false;
    ResellerStandardRole = false;
    ResellerAdmin = false;
    IMStaffAdmin = false;
    displayDialog = false;
    financeProductTypeSelected: string;

    rowDataProductType: any;

    displayPaymentPlan: boolean = environment.DisplayFunderPlan;

    quoteIdGlobal = 0;
    calculatingRate = false;

    downloadMenuItems: MenuItem[];

    controlReadOnly: boolean;

    modes: string;

    cities: City[];

    selectedCity1: City;

    selectedCity2: City;

    selectedCity3: string;

    selectedCountry: string;

    countries: any[];

    groupedCities: SelectItemGroup[];

    selectItems: SelectItem[];

    item: string;

    selectedCountry2: any;

    selectedCity: any;

    selectedItem: any;

    autocountries: any[];

    autoitems: any[];

    filteredCountries: any[];

    filteredItems: any[];

    selectedCountries: any[];

    selectedCountryAdvanced: any[];

    filteredBrands: any[];

    groupedCities2: SelectItemGroup[];
    addLine1: string;
    addLine2: string;
    city: string;
    state: string;
    postCode: string;
    country: string;
    contactName: string;
    contactEmail: string;
    contactNumber: string;
    signaturePosition: string;
    signatureName: string;

    filteredAdd: any
    selectedAdd: any
    post: any

    filteredGroups: any[];
    street: any[];
    addressautocomplete: AddressValidateTokenResponse;
    addressautocomplete2: any
    filteredGroup: any[];
    inputModel: AddressAutoComplete;
    inputAddress: AddressInfo;
    selectedFunderPays: RateCalculateResponseModel;
    financeTypeOptions : any = [];
    paymentPlanTypes = PaymentPlanTypes;
    lineTypes = LineTypes;
    funderTypes: any = [];
    search: string
    gstTypes = GstTypes;
    grandTotal: any = 0;
    filteredContacts: any[];
    selectedCustomerName: any;
    customerContactNumber?: any;
    @ViewChild('emailHistoryModal') emailHistoryModal: QuoteEmailHistoryModalComponent;
    newGetAddresses: any;
    display: boolean = false;
    alert: any = true;
    addressInvalid: boolean;
    itemsForSteps: any[];
    activeIndex: any = 0;
    custDetails: boolean = true;
    summary: boolean;
    quoteItems: boolean;
    documents: boolean;
    addressValid: boolean;
    fileListGet: any;
    totalFiles: any[];
    mode: string | null;
    isfinanceAvailable: boolean = false;
    quoteDetailResponse: QuoteDetailsModel;
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
        private authenticationService: AuthenticationService,
        private router: Router,
        private route: ActivatedRoute,
        private confirmationService: ConfirmationService,
        private messageService: MessageService,
        private filterService: FilterService,
        private _addressvalidateservice: addressvalidateservice,
        private funderControllerService: FunderControllerService,
        private http: HttpClient

    ) {
        this.router.routeReuseStrategy.shouldReuseRoute = () => false;
        this.route.queryParamMap.subscribe((queryParams) => {
            this.getfilequoteid = queryParams.get("id")

            let OldId = this.router.getCurrentNavigation()?.previousNavigation?.finalUrl?.queryParams?.id;

            if ((OldId != this.getfilequoteid) && OldId != null && this.getfilequoteid == null)
                this.ngOnInit();


        })
        this.skuRadioItems = [

        ];
    }
    funder: any;
    filesLists: any;
    filelist = new QuoteDocuments();
    getfilequoteid: number | any;
    ngOnInit() {
        this.financeTypeOptions = this.getFinanceTypes();
        this.initForm();
        this.route.queryParamMap.subscribe((queryParams) => {
            this.getfilequoteid = queryParams.get("id")
            this.mode = queryParams.get("mode")
        })
        this.getQuoteStatus();
        this.getTypes();
        this.setDefaults();
        if(this.authenticationService.userHasRole('IMStaffAdmin')) {
          this.getFundersOptions();
        }
        this.getFiles();

        //   this.activeTabIndex = 0;
        this.quoteIdGlobal = 0;
        const that = this;
        that.todayDate = new Date();
        const obsComb = combineLatest([this.route.paramMap, this.route.queryParams]);

        obsComb.pipe(takeWhile(() => this.active)).subscribe((params: any) => {
            this.route.queryParamMap.subscribe(queryParams => {
                const quoteId = queryParams.get("id");
                const mode = queryParams.get("mode") !== null ? queryParams.get("mode") : "";

                if (mode === "view") {
                    this.controlReadOnly = true;
                }

                if (quoteId) {
                    this.funderTypes.shift();
                    this.header = 'Quote Details';
                    this.getQuoteDetails(parseInt(quoteId));
                    this.quoteIdGlobal = parseInt(quoteId);

                    if (this.authenticationService.userHasRole('ResellerAdmin')) {
                        this.ResellerStandardRole = false;
                        this.ResellerAdmin = true;
                        this.IMStaffAdmin = false;
                    }

                    if (this.authenticationService.userHasRole('IMStaffAdmin')) {
                        this.IMStaffAdmin = true;
                        this.ResellerAdmin = false;
                        this.ResellerStandardRole = false;
                    }
                }
                else {
                    this.quoteHeaderDetailsForm.get('CustomerContact')?.disable();
                    this.quoteHeaderDetailsForm.get('CustomerContactEmail')?.disable();
                    this.quoteHeaderDetailsForm.get('CustomerContactNumber')?.disable();
                    if (this.authenticationService.userHasRole('ResellerStandard')) {
                        this.ResellerStandardRole = true;
                        if (this.ResellerStandardRole) {
                            this.setCustomer(this.authenticationService.getCurrentUserInfo()?.customerNumber);

                        }
                    }
                    if (this.authenticationService.userHasRole('ResellerAdmin')) {
                        this.ResellerAdmin = true;
                    }
                    if (this.authenticationService.userHasRole('IMStaffAdmin')) {
                        this.IMStaffAdmin = true;
                        this.ResellerAdmin = false;
                        this.ResellerStandardRole = false;
                    }
                }
            })

        });

        this.itemsForSteps = [{ label: 'Customer details' }, { label: 'Quote Items' }, { label: 'Summary' }];

        this.downloadMenuItems = [
            {
                label: 'Generate Proposal (xlsx)', icon: 'pi pi-cloud-download', command: () => {
                    this.downloadQuote('Proposal');
                },
            },
        ];

        this.autoitems = [];
        for (let i = 0; i < 10000; i++) {
            this.autoitems.push({ label: 'Item ' + i, value: 'Item ' + i });
        }

        this.calculateLines();

    }

    getFinanceTypes() {
      if(this.authenticationService.userHasRole('IMSoftwareQuotes')) {
        return [FinanceTypes.find(x => x.label === 'Instalments')];
      } else if(this.authenticationService.userHasRole('IMHardwareQuotes')) {
        return [FinanceTypes.find(x => x.label === 'Leasing')];
      } else {
        return FinanceTypes;
      }
    }

    navigate(value: any, back: any) {

        if (back == 'back') {
            value = this.activeIndex - 1
        }
        if (value == 0) {
            this.custDetails = true
            this.summary = false
            this.quoteItems = false
            this.activeIndex = 0
            this.documents = false

        } else if (value == 1) {
            if (back != "back") {
              if(Number(this.quoteHeaderDetailsForm.value.QuoteStatus) !== this.endCustomerAccepted){
                if (!this.quoteHeaderDetailsForm.valid) {
                    this.imfsUtilityService.showToastr('error', 'Invalid Input', 'Please enter all required fields.');
                    setTimeout(() => {
                        this.navigate(1, "back");
                    }, 0);
                    return
                }
              }
            }

            setTimeout(() => {
                let indexToFocus = 0;
                let currentIndex = 0;
                this.items.controls.forEach((item: AbstractControl) => {
                    const currentIMSKU = item.get('VPN')?.value;
                    const currentCategory = item.get('Category')?.value;
                    if (!currentIMSKU && !currentCategory) {
                        indexToFocus = currentIndex;
                        return;
                    }
                    currentIndex++;
                });


                const element = document.getElementById('sku-column-' + indexToFocus.toString());

                if (element) {
                    element.click();
                }
            }, 100);

            this.setGstSelectionValue();
            this.custDetails = false
            this.summary = false
            this.documents = false
            this.quoteItems = true
            this.activeIndex = 1

        } else if (value == 2) {
            this.custDetails = false
            this.summary = true
            this.quoteItems = false
            this.documents = false
            this.activeIndex = 2

        } else if (value == 3) {
            this.custDetails = false
            this.summary = false
            this.quoteItems = false
            this.documents = true
            this.activeIndex = 3
        }
    }

    focusQty(rowData: any, rowIndex: number) {
      if(rowData?.value?.IsNew) {
      if(!(rowData?.value?.IMSKU && rowData?.value?.Description)) {
        const element = document.getElementById('description-column-' + rowIndex.toString());

        if (element) {
            element.click();
        }
        this.imfsUtilityService.showToastr('warning', 'Warning', 'Please enter Description');
      }

    }

    }

    changeGstOption() {
        const delay = 100;
        if (this.timeoutForGst) {
            clearTimeout(this.timeoutForGst);
        }
        this.timeoutForGst = setTimeout(() => {

            if (this.getFormData(this.quoteItemsForm, 'FinanceType').toString()) {


                if (this.getFormData(this.quoteItemsForm, 'FinanceType').toString() === '4') {


                    if (this.quoteItemsForm.controls.GstInclude.value === this.gstTypes[0] || (this.quoteItemsForm.controls.GstInclude.value === null || this.quoteItemsForm.controls.GstInclude.value === '')) {

                        this.quoteItemsForm.controls['GstInclude'].setValue(this.gstTypes[0].value);
                    }
                    else {

                        this.imfsUtilityService.showToastr('error', 'Error', 'Instalment quotes include GST');

                    }
                }
                else if (this.getFormData(this.quoteItemsForm, 'FinanceType').toString() === '1') {

                    if (this.quoteItemsForm.controls.GstInclude.value === this.gstTypes[1] || (this.quoteItemsForm.controls.GstInclude.value === null || this.quoteItemsForm.controls.GstInclude.value === '')) {

                        this.quoteItemsForm.controls['GstInclude'].setValue(this.gstTypes[1].value);
                    }
                    else {

                        this.imfsUtilityService.showToastr('error', 'Error', 'Lease quotes exclude GST');

                    }
                }
                else if (this.getFormData(this.quoteItemsForm, 'FinanceType').toString() === '2') {
                    if (this.quoteItemsForm.controls.GstInclude.value === this.gstTypes[1] || (this.quoteItemsForm.controls.GstInclude.value === null || this.quoteItemsForm.controls.GstInclude.value === '')) {

                        this.quoteItemsForm.controls['GstInclude'].setValue(this.gstTypes[1].value);
                    }
                    else {

                        this.imfsUtilityService.showToastr('error', 'Error', 'Rental quotes exclude GST');

                    }
                }
            }

        }, delay);
    }

    getFiles() {
        let getQuoteId = this.getfilequoteid;
        if (getQuoteId && getQuoteId > 0) {
            this.quoteControllerService.getQuoteFileList(getQuoteId).subscribe((response: QuoteDocuments) => {
                this.filesLists = response;
            });
        }
    }
    getRemoveFile(event: any) {
        this.getFiles();
    }

    onDeleteFile(fileId: number) {
        const inputModel = { FileId: fileId };
        this.confirmationService.confirm({
            message: 'Are you sure that you want to proceed?',
            header: 'Confirmation',
            icon: 'pi pi-exclamation-triangle',
            accept: () => {
                this.http.post(environment.API_BASE + '/Quote/DeleteQuoteAttachments', inputModel).subscribe((response) => {
                    this.imfsUtilityService.showToastr('success', 'Success', 'File deleted successfully');
                    this.getRemoveFile('');
                },
                    (err: any) => {
                        this.imfsUtilityService.hideLoading();
                        this.imfsUtilityService.showToastr('error', 'Failed', 'Error in deleting file');
                    })
            },
            reject: () => {

            }

        });
    }
    showDialog() {
        this.display = true;
    }
    getQuoteDetails(quoteId: number) {
        this.imfsUtilityService.showLoading('Loading');
        this.quoteControllerService.getQuoteDetails(quoteId).subscribe(
            (response: QuoteDetailsResponseModel) => {
                this.imfsUtilityService.hideLoading();
                this.quoteDetailResponse = response.quoteDetails;
                this.setFormValue(response.quoteDetails);

                // Disable form if it has been End Customer Accepted
                if (Number(response.quoteDetails.quoteHeader.status) === this.endCustomerAccepted) {
                    this.quoteHeaderDetailsForm.disable();
                    this.quoteItemsForm.disable();
                }

            },
            (err: any) => {
                this.imfsUtilityService.hideLoading();
                this.imfsUtilityService.showToastr('error', 'Failed', 'Error in getting quote');
            }
        );


    }
    setDefaults() {

        this.quoteHeaderDetailsForm.controls.QuoteNumber.patchValue('New');
        this.quoteHeaderDetailsForm.controls.QuoteStatus.patchValue(1);
        this.quoteItemsForm.controls.QuoteType.patchValue('BySKU');
        this.quoteItemsForm.controls.FinanceType.patchValue('1');

        if (this.displayPaymentPlan) {
            this.quoteItemsForm.controls.FunderPlan.patchValue('1');
        }

        this.quoteItemsForm.controls.QuoteDuration.patchValue(36);

        const defaultExpiryDate = moment().add(30, 'd').toDate();
        this.quoteHeaderDetailsForm.controls.ExpiryDate.patchValue(defaultExpiryDate);

    }


    setGstSelectionValue() {
        if (this.getFormData(this.quoteItemsForm, 'FinanceType') == '4')
            this.quoteItemsForm.controls['GstInclude'].setValue(this.gstTypes[0].value);
        else if (this.getFormData(this.quoteItemsForm, 'FinanceType') == '2' || this.getFormData(this.quoteItemsForm, 'FinanceType') == '1')
            this.quoteItemsForm.controls['GstInclude'].setValue(this.gstTypes[1].value);
    }
    get items(): FormArray {
        if (this.quoteItemsForm) {
            return this.quoteItemsForm.get('items') as FormArray;
        }
        return this.formBuilder.array([this.createTableRow(new QuoteLine())]);
    }
    initForm() {
        this.quoteHeaderDetailsForm = this.formBuilder.group({
            QuoteNumber: new FormControl(''),
            QuoteName: new FormControl('', Validators.required),
            ExpiryDate: new FormControl('', Validators.required),
            QuoteStatus: new FormControl('', Validators.required),
            Customer: new FormControl('', Validators.required),
            ABN: new FormControl('', [Validators.required,Validators.pattern("^[0-9]*$")]),
            CustomerAddressLine1: new FormControl({ value: '', disabled: true }),
            CustomerAddressLine2: new FormControl({ value: '', disabled: true }),
            CustomerAddressCity: new FormControl({ value: '', disabled: true }),
            CustomerAddressState: new FormControl({ value: '', disabled: true }),
            CustomerCountry: new FormControl({ value: '', disabled: true }),
            CustomerPostCode: new FormControl({ value: '', disabled: true }),
            CustomerContact: new FormControl('', Validators.required),
            CustomerContactEmail: new FormControl('', [Validators.required, Validators.email]),
            CustomerContactNumber: new FormControl('', [Validators.required, Validators.pattern(/^[0-9 ()+]+$/)]),
            AuthorisedSignatoryName: new FormControl('', Validators.required),
            AuthorisedSignatoryPosition: new FormControl('', Validators.required),
            EndUserName: new FormControl('', Validators.required),
            EndUserABN: new FormControl('', [Validators.required, Validators.pattern("^[0-9]*$")]),
            EndUserAddressLine1: new FormControl('', Validators.required),
            EndUserAddressLine2: new FormControl(''),
            EndUserCity: new FormControl('', Validators.required),
            EndUserState: new FormControl('', Validators.required),
            EndUserPostcode: new FormControl('', Validators.required),
            EndUserContactName: new FormControl('', Validators.required),
            EndUserContactEmail: new FormControl('', [Validators.required, Validators.email]),
            EndUserCountry: new FormControl('', Validators.required),
            EndUserContactNumber: new FormControl('', [Validators.required, Validators.pattern(/^[0-9 ()+]+$/)]),
            EndUserYearsTrading: new FormControl('', Validators.required),
        });

        this.quoteItemsForm = this.formBuilder.group({
            DefinitionName: new FormControl('', Validators.required),
            QuoteType: new FormControl('', Validators.required),
            FinanceType: new FormControl('', Validators.required),
            QuoteDuration: new FormControl(''),
            QuoteTotal: new FormControl(''),
            ApplyAllMargin: new FormControl(''),
            Funder: new FormControl(''),
            GstInclude: new FormControl(''),
            includeTotalGST: new FormControl(''),
            items: this.formBuilder.array([this.createTableRow(new QuoteLine())]),
            QuarterlyTotal: new FormControl(''),
            MonthlyTotal: new FormControl(''),
            YearlyTotal: new FormControl(''),
            TotalInc: new FormControl(''),

            MonthlyId: new FormControl(''),
            MonthlyRate: new FormControl(''),
            MonthlyCalculationType: new FormControl(''),
            MonthlyFunderPlanId: new FormControl(''),
            MonthlyFunderPlanDescription: new FormControl(''),

            QuarterlyId: new FormControl(''),
            QuarterlyRate: new FormControl(''),
            QuarterlyCalculationType: new FormControl(''),
            QuarterlyFunderPlanId: new FormControl(''),
            QuarterlyFunderPlanDescription: new FormControl(''),

            YearlyId: new FormControl(''),
            YearlyRate: new FormControl(''),
            YearlyCalculationType: new FormControl(''),
            YearlyFunderPlanId: new FormControl(''),
            YearlyFunderPlanDescription: new FormControl(''),

            FunderId: new FormControl(''),
            Frequency: new FormControl(''),
            FinanceValue: new FormControl(''),
            FunderPlan: new FormControl(''),
            FunderPlanId: new FormControl(''),
            FunderPlanDescription: new FormControl(''),

            MonthlyFunder: new FormControl(''),
            QuarterlyFunder: new FormControl(''),
            YearlyFunder: new FormControl(''),

        });
        const that = this;
        this.formUtility.markFormAsDirty(this.quoteItemsForm.controls);


    }

    onDurationChange(event: any) {
        if (event.value) {
            const _quoteTotal = this.getFormData(this.quoteItemsForm, 'QuoteTotal');
            if (_quoteTotal !== null && _quoteTotal > 0) {
                this.calculateRate();
            }
        }

    }

    resetRateOptions() {
        this.selectedRateOption = '';
        this.quoteItemsForm.controls.FunderId.reset();
        this.quoteItemsForm.controls.FinanceValue.reset();
        this.quoteItemsForm.controls.Frequency.reset();
        this.quoteItemsForm.controls.FunderPlanId.reset();
        this.grandTotal = 0;

    }

    _resetRateOptions() {
        this.selectedRateOption = '';
    }

    calculateRate() {
        let gstVal = this.quoteItemsForm.controls.GstInclude.value;

        const durationArray: string[] = [];
        const rateCalculateInputModel = new RateCalculateInputModel();
        rateCalculateInputModel.Source = 'IMFS Portal';
        rateCalculateInputModel.QuoteTotal = Number(this.quoteItemsForm.controls.QuoteTotal.value);
        rateCalculateInputModel.Funder = this.funder ? this.funder : '';
        durationArray.push(this.quoteItemsForm.controls.QuoteDuration.value.toString());
        rateCalculateInputModel.Duration = durationArray;
        rateCalculateInputModel.Frequency = ['Monthly', 'Quarterly', 'Upfront'];
        rateCalculateInputModel.IncludeTax = true;
        rateCalculateInputModel.TaxRate = 10;
        rateCalculateInputModel.FinanceType = String(this.quoteItemsForm.controls.FinanceType.value);
        this.setGstSelectionValue();
        rateCalculateInputModel.FunderPlan = String(this.quoteItemsForm.controls.FunderPlan.value);
        rateCalculateInputModel.GstInclude = gstVal;

        // initialise array
        rateCalculateInputModel.QuoteLines = [];

        const quoteLines: QuoteLine[] = this.getQuoteItems();
        quoteLines.forEach((lineItem: QuoteLine, index) => {
            if (!lineItem.imsku && !lineItem.vpn && !lineItem.item && !lineItem.category) {
                return;
            }

            // this.imfsUtilityService.showToastr('info', 'info', 'calculating rate for selected line Items');

            const item = new RateCalculateLineItem();
            item.Imsku = lineItem.imsku;
            item.VendorSKU = lineItem.vpn;
            item.Qty = lineItem.qty;
            if (!item.Qty) {
                item.Qty = 1;
            }

            item.LineTotal = lineItem.lineTotal;
            if (lineItem.lineNumber) {
                item.LineNumber = lineItem.lineNumber;
            }
            else {
                item.LineNumber = 1;
            }

            if (lineItem.item != null) {
                item.Type = lineItem.item.toString();
            }
            else {
                item.Type = '';
            }

            if (lineItem.category != null) {
                item.Category = lineItem.category.toString();
            }
            else {
                item.Category = '';
            }


            rateCalculateInputModel.QuoteLines.push(item);

        });
        this.calculatingRate = true;
        // Reset selected value after calculation
        this.selectedRateOption = '';
        this.quoteControllerService.calculateRate(rateCalculateInputModel).subscribe(
            (response: RateCalculateResponseModel) => {
                this.calculatingRate = false;

                this.imfsUtilityService.hideLoading();
                if (response.status === 'Success') {

                    // // Reset Finance Totals
                    this.quoteItemsForm.controls.YearlyTotal.patchValue('');
                    this.quoteItemsForm.controls.QuarterlyTotal.patchValue('');
                    this.quoteItemsForm.controls.MonthlyTotal.patchValue('');
                    this.quoteItemsForm.controls.MonthlyId.patchValue('');
                    this.quoteItemsForm.controls.MonthlyRate.patchValue('');
                    this.quoteItemsForm.controls.MonthlyCalculationType.patchValue('');
                    this.quoteItemsForm.controls.MonthlyFunderPlanId.patchValue('');
                    this.quoteItemsForm.controls.MonthlyFunderPlanDescription.patchValue('');

                    this.quoteItemsForm.controls.QuarterlyId.patchValue('');
                    this.quoteItemsForm.controls.QuarterlyRate.patchValue('');
                    this.quoteItemsForm.controls.QuarterlyCalculationType.patchValue('');
                    this.quoteItemsForm.controls.QuarterlyFunderPlanId.patchValue('');
                    this.quoteItemsForm.controls.QuarterlyFunderPlanDescription.patchValue('');

                    this.quoteItemsForm.controls.YearlyId.patchValue('');
                    this.quoteItemsForm.controls.YearlyRate.patchValue('');
                    this.quoteItemsForm.controls.YearlyCalculationType.patchValue('');
                    this.quoteItemsForm.controls.YearlyFunderPlanId.patchValue('');
                    this.quoteItemsForm.controls.YearlyFunderPlanDescription.patchValue('');
                    let isOptionFound = false;
                    if(response.financeDetails[0] != null || response.financeDetails[1] != null || response.financeDetails[2] != null) {
                      this.isfinanceAvailable = true;
                    } else {
                      this.isfinanceAvailable = false;
                    }
                    this._resetRateOptions();
                    response.financeDetails.forEach((item: any) => {

                        if (item != null) {
                            if (item?.frequency === 'Monthly') {
                                this.quoteItemsForm.controls.MonthlyTotal.patchValue(item.financeTotal);
                                this.selectedMonthlyFunderId = item.funderID;
                                this.selectedMonthlyFunder = item.funder;
                                this.selectedMonthlyFunderPlanId = item.funderPlanID;
                                this.selectedMonthlyFunderPlanDescription = item.funderPlanDescription;
                                this.quoteItemsForm.controls.MonthlyId.patchValue(item.id);
                                this.quoteItemsForm.controls.MonthlyRate.patchValue(item.rate);
                                this.quoteItemsForm.controls.MonthlyCalculationType.patchValue(item.calculationType);
                                this.quoteItemsForm.controls.MonthlyFunderPlanId.patchValue(item.funderPlanID);
                                this.quoteItemsForm.controls.MonthlyFunder.patchValue(item.funder);
                                this.quoteItemsForm.controls.MonthlyFunderPlanId.patchValue(item.funderPlanID);
                                this.quoteItemsForm.controls.MonthlyFunderPlanDescription.patchValue(item.funderPlanDescription);
                            }

                            if (item?.frequency === 'Quarterly') {
                                this.quoteItemsForm.controls.QuarterlyTotal.patchValue(item?.financeTotal);
                                this.selectedQuarterlyFunderId = item?.funderID;
                                this.selectedQuarterlyFunder = item.funder;
                                this.selectedQuarterlyFunderPlanId = item.funderPlanID;
                                this.selectedQuarterlyFunderPlanDescription = item.funderPlanDescription;
                                this.quoteItemsForm.controls.QuarterlyId.patchValue(item.id);
                                this.quoteItemsForm.controls.QuarterlyRate.patchValue(item.rate);
                                this.quoteItemsForm.controls.QuarterlyCalculationType.patchValue(item.calculationType);
                                this.quoteItemsForm.controls.QuarterlyFunder.patchValue(item.funder);
                                this.quoteItemsForm.controls.QuarterlyFunderPlanId.patchValue(item.funderPlanID);
                                this.quoteItemsForm.controls.QuarterlyFunderPlanDescription.patchValue(item.funderPlanDescription);
                            }

                            if (item.frequency === 'Upfront') {
                                this.quoteItemsForm.controls.YearlyTotal.patchValue(item.financeTotal);
                                this.selectedYearlyFunderId = item.funderID;
                                this.selectedYearlyFunder = item.funder;
                                this.selectedYearlyFunderPlanId = item.funderPlanID;
                                this.selectedYearlyFunderPlanDescription = item.funderPlanDescription;
                                this.quoteItemsForm.controls.YearlyId.patchValue(item.id);
                                this.quoteItemsForm.controls.YearlyRate.patchValue(item.rate);
                                this.quoteItemsForm.controls.YearlyCalculationType.patchValue(item.calculationType);
                                this.quoteItemsForm.controls.YearlyFunder.patchValue(item.funder);
                                this.quoteItemsForm.controls.YearlyFunderPlanId.patchValue(item.funderPlanID);
                                this.quoteItemsForm.controls.YearlyFunderPlanDescription.patchValue(item.funderPlanDescription);
                            }
                            if (this.selectedFrequency) {
                                this.rateOptionSelected(this.selectedFrequency);
                            }
                        }
                        else {
                            // this._resetRateOptions();
                            this.quoteItemsForm.controls.MonthlyFunder.patchValue(this.quoteItemsForm.controls.Funder.value);
                            this.quoteItemsForm.controls.QuarterlyFunder.patchValue(this.quoteItemsForm.controls.Funder.value);
                            this.quoteItemsForm.controls.YearlyFunder.patchValue(this.quoteItemsForm.controls.Funder.value);
                        }
                    });
                    // if(isOptionFound){
                    //this.imfsUtilityService.showToastr('error', 'Error', 'No Option Found');
                    //}

                } else {
                    this.imfsUtilityService.showToastr('error', 'Error', response.message);
                }
            },
            (err: any) => {
                this.calculatingRate = false;
                this.imfsUtilityService.hideLoading();
                // this.imfsUtilities.showToastr('error', 'Failed', 'Unable to calculate rate');
            }

        );
    }

    createTableRow(item: QuoteLine): FormGroup {
        return this.formBuilder.group({
            IMSKU: item.imsku,
            VPN: item.vpn,
            Description: item.description,
            LineNumber: item.lineNumber,
            VSR: item.vsr,
            Item: item.item,
            ItemName: item.itemName,
            Category: item.category,
            CategoryName: item.categoryName,
            SalePrice: item.salePrice,
            CostPrice: item.costPrice,
            Qty: item.qty,
            Margin: item.margin,
            TotalGST: item.totalGST,
            LineTotal: item.lineTotal,
            TotalInc: item.totalinc,
            FinanceProductTypeID: item.financeProductTypeID,
            IsNew: 0
        });
    }

    setRateOptions(quoteDetails: QuoteDetailsModel): void {
        if (quoteDetails.quoteHeader.frequency) {
            this.selectedRateOption = quoteDetails.quoteHeader.frequency;
        }
    }

    setFormValue(quoteDetails: QuoteDetailsModel): void {

        this.quoteHeaderDetailsForm.patchValue({
            QuoteNumber: quoteDetails.quoteHeader.quoteNumber,
            QuoteName: quoteDetails.quoteHeader.quoteName,
            ExpiryDate: moment(quoteDetails.quoteHeader.expiryDate).toDate(),
            QuoteStatus: quoteDetails.quoteHeader.status,
            Frequency: quoteDetails.quoteHeader.frequency,
            // Customer Details
            Customer: quoteDetails.selectedCustomer,
            CustomerNumber: quoteDetails.customerDetails.customerNumber,
            ABN: quoteDetails.customerDetails.customerABN,
            CustomerName: quoteDetails.customerDetails.customerName,
            CustomerAddressLine1: quoteDetails.customerDetails.customerAddressLine1,
            CustomerAddressLine2: quoteDetails.customerDetails.customerAddressLine2,
            CustomerAddressCity: quoteDetails.customerDetails.customerAddressCity,
            CustomerAddressState: quoteDetails.customerDetails.customerAddressState,
            CustomerCountry: quoteDetails.customerDetails.customerCountry,
            CustomerPostCode: quoteDetails.customerDetails.customerPostCode,
            CustomerContact: quoteDetails.customerDetails.customerContact,
            CustomerContactEmail: quoteDetails.customerDetails.customerEmail,
            CustomerContactNumber: quoteDetails.customerDetails.customerPhone,

            // End User Details
            EndUserName: quoteDetails.endUserDetails.endCustomerName,
            EndUserABN: quoteDetails.endUserDetails.endCustomerABN,
            EndUserAddressLine1: quoteDetails.endUserDetails.endCustomerAddressLine1,
            EndUserAddressLine2: quoteDetails.endUserDetails.endCustomerAddressLine2,
            EndUserCity: quoteDetails.endUserDetails.endCustomerCity,
            EndUserState: quoteDetails.endUserDetails.endCustomerState,
            EndUserPostcode: quoteDetails.endUserDetails.endCustomerPostCode,
            EndUserCountry: quoteDetails.endUserDetails.endCustomerCountry,
            EndUserContactName: quoteDetails.endUserDetails.endCustomerContact,
            EndUserContactEmail: quoteDetails.endUserDetails.endCustomerEmail,
            AuthorisedSignatoryName: quoteDetails.endUserDetails.authorisedSignatoryName,
            AuthorisedSignatoryPosition: quoteDetails.endUserDetails.authorisedSignatoryPosition,
            EndUserContactNumber: quoteDetails.endUserDetails.endCustomerPhone,
            EndUserYearsTrading: quoteDetails.endUserDetails.endCustomerYearsTrading,

        });

        this.selectedRateOption = quoteDetails.quoteHeader.frequency;

        if (this.ResellerStandardRole) {
            this.quoteHeaderDetailsForm.get('Customer')?.disable();
        }

        if (!this.ResellerStandardRole) {
            this.quoteHeaderDetailsForm.get('CustomerContact')?.disable();
            this.quoteHeaderDetailsForm.get('CustomerContactEmail')?.disable();
            this.quoteHeaderDetailsForm.get('CustomerContactNumber')?.disable();
        }

        this.items.clear();
        if(this.categories) {
        quoteDetails.quoteLines.forEach((quoteLine: QuoteLine) => {

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
            this.items.push(this.createTableRow(quoteLine));
        });
      }

        this.activeTabIndex = quoteDetails.quoteLines.length - 1;

        this.quoteItemsForm.patchValue({
            FunderId: quoteDetails.quoteHeader.funderId,
            FinanceValue: quoteDetails.quoteHeader.financeValue,
            Frequency: quoteDetails.quoteHeader.frequency,
            FinanceType: quoteDetails.quoteHeader.financeType,
            FunderPlan: quoteDetails.quoteHeader.funderPlan,
            QuoteType: quoteDetails.quoteHeader.quoteType,
            QuoteTotal: quoteDetails.quoteHeader.quoteTotal,
            Funder: quoteDetails.quoteHeader.funderCode,
            GstInclude: quoteDetails.quoteHeader.gstInclude,

        });
        this.setIncludeTotalGST();
        this.setFunderType(quoteDetails.quoteHeader.funderCode, 'funder')
        this.setFunderType(quoteDetails.quoteHeader.gstInclude, 'gst')
        if (quoteDetails.quoteHeader.frequency) {
            this.selectedFrequency = quoteDetails.quoteHeader.frequency;
        }

        if (quoteDetails.quoteHeader.quoteDuration !== 'undefined' && quoteDetails.quoteHeader.quoteDuration) {
            this.quoteItemsForm.controls.QuoteDuration.patchValue(Number(quoteDetails.quoteHeader.quoteDuration));
        }

        if (quoteDetails.quoteHeader.quoteTotal !== null && quoteDetails.quoteHeader.quoteTotal > 0) {
            this.calculateRate();
        }
        this.calculateLines()
    }

    setFunderType(Code: any, type: any) {
        if (type == "funder") {
            this.funderTypes.forEach((ele: any) => {
                if (ele.value == Code) {
                    this.funder = ele.value;
                }
            })
        } else if (type == "gst") {
            this.gstTypes.forEach((ele: any) => {
                if (Code == ele.value) {
                    this.quoteItemsForm.controls['GstInclude'].setValue(ele.value);

                }
            })
        }
    }

    setIncludeTotalGST() {
        let includeTotalGST = 0;
        this.items.controls.forEach((item: AbstractControl) => {
            includeTotalGST += item.get('TotalGST')?.value;
        })
        this.quoteItemsForm.controls['includeTotalGST'].setValue(includeTotalGST)
    }

    filterType(event: any) {

        this.filteredTypes = [];
        this.types.filter(t => {

            let valFetched: any[] = []
            for (let i = 0; i < this.types.length; i++) {

                let tVal = {

                    name: this.types[i].name,
                    value: this.types[i].id

                }
                valFetched.push(tVal)
            }
            this.filteredTypes = valFetched.sort((a, b) => (a.name || "").toString().localeCompare((b.name || "").toString()))

        })

    }



    onTypeValueSelect(id: number) {
        if (id) {
            this.filteredCategories = this.categories.filter(cat => cat.typeID === id);
        }

    }



    onTypeSelect(event: any, rowData: any) {
        if (event.value) {

            this.filteredTypes.forEach((ele: any) => {

                if (ele.value == event.value) {
                    rowData.controls.ItemName.value = ele.name
                }
            })
            this.filteredCategories = this.categories.filter(cat => cat.typeID === event.value);
            rowData.controls.Item.value = event.value;

        }

        this.onTypeValueSelect(event.value)
        this.filteredCategories.sort((a, b) => (a.name || "").toString().localeCompare((b.name || "").toString()))



    }



    onCategoryChange(event: any, rowData: any) {

        if (event.value) {
            // Set categoryID
            rowData.controls.CategoryName.value = event.value.name;
            rowData.controls.Category.value = event.value.id;

        }

    }



    filterCategory(event: any) {

        this.filteredCategories = this.categories.filter(cat => cat.name);

    }



    onCategorySelect(event: any) {

        if (event) {
            this.filteredCategories = this.categories.filter(cat => cat.typeID === event.id)
        }
    }

    setCustomer(customerNumber: any) {
        this.orpCustomerControllerService.getCustomer(customerNumber).subscribe(
            (response: CustomerResponseModel) => {
                this.imfsUtilityService.hideLoading();
                this.filteredCustomers = response.customerDetails;
                if (this.filteredCustomers.length > 0) {
                    this.quoteHeaderDetailsForm.controls.Customer.setValue(response.customerDetails[0]);
                    this.quoteHeaderDetailsForm.controls.ABN.setValue(response.customerDetails[0].abn);
                    this.quoteHeaderDetailsForm.controls.CustomerAddressLine1.setValue(response.customerDetails[0].addressLine1);
                    this.quoteHeaderDetailsForm.controls.CustomerAddressLine2.setValue(response.customerDetails[0].addressLine2);
                    this.quoteHeaderDetailsForm.controls.CustomerAddressCity.setValue(response.customerDetails[0].city);
                    this.quoteHeaderDetailsForm.controls.CustomerAddressState.setValue(response.customerDetails[0].state);
                    this.quoteHeaderDetailsForm.controls.CustomerCountry.setValue(response.customerDetails[0].country);
                    this.quoteHeaderDetailsForm.controls.CustomerPostCode.setValue(response.customerDetails[0].postCode);

                    const resellerDetails = this.authenticationService.getCurrentUserInfo();
                    // tslint:disable-next-line: max-line-length
                    this.quoteHeaderDetailsForm.controls.CustomerContact.setValue(resellerDetails?.firstName + ' ' + resellerDetails?.lastName);
                    this.quoteHeaderDetailsForm.controls.CustomerContactEmail.setValue(resellerDetails?.email);
                    this.quoteHeaderDetailsForm.controls.CustomerContactNumber.setValue(resellerDetails?.phone);

                    if (this.ResellerStandardRole) {
                        this.quoteHeaderDetailsForm.get('Customer')?.disable();
                    }
                    if (!this.ResellerStandardRole) {
                        // disable controls
                        this.quoteHeaderDetailsForm.get('CustomerContact')?.disable();
                        this.quoteHeaderDetailsForm.get('CustomerContactEmail')?.disable();
                        this.quoteHeaderDetailsForm.get('CustomerContactNumber')?.disable();
                    }
                }
            },
            (err: any) => {
                this.imfsUtilityService.hideLoading();
                this.imfsUtilityService.showToastr('error', 'Failed', 'Error loading customers');
            }

        );
    }
    filterCustomer(event: any) {
        this.orpCustomerControllerService.getCustomer(event.query).subscribe(
            (response: CustomerResponseModel) => {
                this.imfsUtilityService.hideLoading();
                this.filteredCustomers = response.customerDetails;
            },
            (err: any) => {
                this.imfsUtilityService.hideLoading();
                this.imfsUtilityService.showToastr('error', 'Failed', 'Error loading customers');
            }

        );
    }
    _filteredCustomerName: string;
    filterCustomerContact(event: any) {
        this.orpCustomerControllerService.getCustomerContact(this.customerContactNumber).subscribe(
            (response: CustomerContactResponseModel) => {
                let filtered: any[] = []
                let any = response.customerDetails
                for (let i = 0; i < any.length; i++) {
                    if (any[i].contactName.toLowerCase().includes(event.query)) {
                        filtered.push(any[i]);
                    }
                }
                this.filteredContacts = filtered
                this.imfsUtilityService.hideLoading();
            },

            (err: any) => {
                this.imfsUtilityService.hideLoading();
                this.imfsUtilityService.showToastr('error', 'Failed', 'Error loading customers');
            }
        );
    }
    onContactNameSelect(event: any) {
        if (event) {
            this.quoteHeaderDetailsForm.controls.CustomerContact.setValue(event.contactName);
            this.quoteHeaderDetailsForm.controls.CustomerContactEmail.setValue(event.contactEmail);
            this.quoteHeaderDetailsForm.controls.CustomerContactNumber.setValue(event.contactNumber);
        } else {
            this.quoteHeaderDetailsForm.controls.CustomerContact.setValue('');
            this.quoteHeaderDetailsForm.controls.CustomerContactEmail.setValue('');
            this.quoteHeaderDetailsForm.controls.CustomerContactNumber.setValue('');
        }
    }

    onCustomerNumberSelect(event: any) {
        if (event) {
            this.quoteHeaderDetailsForm.controls.CustomerAddressLine1.setValue(event.addressLine1);
            this.quoteHeaderDetailsForm.controls.CustomerAddressLine2.setValue(event.addressLine2);
            this.quoteHeaderDetailsForm.controls.CustomerAddressCity.setValue(event.city);
            this.quoteHeaderDetailsForm.controls.CustomerAddressState.setValue(event.state);
            this.quoteHeaderDetailsForm.controls.CustomerCountry.setValue(event.country);
            this.quoteHeaderDetailsForm.controls.CustomerPostCode.setValue(event.postCode);
            this.quoteHeaderDetailsForm.get('CustomerContact')?.enable();
            this.quoteHeaderDetailsForm.get('CustomerContactEmail')?.enable();
            this.quoteHeaderDetailsForm.get('CustomerContactNumber')?.enable();
            this.customerContactNumber = event.customerNumber
            this.orpCustomerControllerService.getCustomerContact(this.customerContactNumber).subscribe(
                (response: CustomerContactResponseModel) => {
                    this.imfsUtilityService.hideLoading();
                    let filtered: any[] = []
                    let any = response.customerDetails
                    for (let i = 0; i < any.length; i++) {
                        let val = any[i]
                        filtered.push(val);
                    }
                    this.filteredContacts = filtered;
                },
                (err: any) => {
                    this.imfsUtilityService.hideLoading();
                    this.imfsUtilityService.showToastr('error', 'Failed', 'Error loading customers');
                }

            );
        } else {
            this.quoteHeaderDetailsForm.controls.ABN.setValue('');
            this.quoteHeaderDetailsForm.controls.CustomerAddressLine1.setValue('');
            this.quoteHeaderDetailsForm.controls.CustomerAddressLine2.setValue('');
            this.quoteHeaderDetailsForm.controls.CustomerAddressCity.setValue('');
            this.quoteHeaderDetailsForm.controls.CustomerAddressState.setValue('');
            this.quoteHeaderDetailsForm.controls.CustomerPostCode.setValue('');
            this.quoteHeaderDetailsForm.controls.CustomerCountry.setValue('');
            this.quoteHeaderDetailsForm.get('CustomerContact')?.disable();
            this.quoteHeaderDetailsForm.get('CustomerContactEmail')?.disable();
            this.quoteHeaderDetailsForm.get('CustomerContactNumber')?.disable();
        }
    }
    // tabChange(event: any) {
    //     if (event.index === 1) {
    //         // quote header text
    //         if (!this.quoteHeaderDetailsForm.valid) {
    //             this.imfsUtilityService.showToastr('error', 'Invalid Input', 'Please enter all required fields.');
    //             setTimeout(() => {
    //                 this.activeTabIndex = 0;
    //             }, 0);
    //         }
    //     } else if (event.index === 2) {
    //         // going to summary tab
    //         // if (this.quoteLineForm.valid) {
    //         //     this.imfsUtilities.showToastr('error', 'Invalid Input', 'Please enter all required fields.');
    //         //     setTimeout(() => {
    //         //         this.activeTabIndex = 0;
    //         //     }, 0);
    //         // }
    //     }
    // }

    insertNewItem() {
        let maxLineNumber = 0;
        let error = false;
        this.items.controls.forEach((item: AbstractControl) => {
            let currentLineNumber = item.get('LineNumber')?.value;
            if (!currentLineNumber) {
                currentLineNumber = 1;
            }
            const currentIMSKU = item.get('IMSKU')?.value;
            const currentCategory = item.get('Category')?.value;
            if (!currentIMSKU && !currentCategory) {
                error = true;
            }
            if (currentLineNumber > maxLineNumber) {
                maxLineNumber = currentLineNumber;
            }
        });
        if (!maxLineNumber) {
            maxLineNumber = 0;
        }
        if (error) {
            this.imfsUtilityService.showToastr('error', 'Failed', 'Complete the quote line before inserting the new one');
            return;
        }
        const quoteLine = new QuoteLine();
        quoteLine.lineNumber = maxLineNumber + 1;
        this.items.push(this.createTableRow(quoteLine));
        setTimeout(() => {
            const element = document.getElementById('sku-column-' + maxLineNumber.toString());
            if (element) {
                element.click();
            }
        }, 100);

        this.activeTabIndex = maxLineNumber + 1;
    }

    resetItem() {

        let confirmationMessage = '';
        let radioPatchValue = '';
        if (this.getQuoteType() === 'BySKU') {
            confirmationMessage = 'Type/Category Information will be cleared. Are you sure you want to proceed?';
            radioPatchValue = 'ByCategory';
        }
        else {
            confirmationMessage = 'SKU Information will be cleared. Are you sure you want to proceed?';
            radioPatchValue = 'BySKU';
        }

        if (this.quoteItemsForm.controls.items.value) {
            this.items.controls.forEach((item: AbstractControl) => {
                const qty = item.get('Qty')?.value as number;
                if (qty === null || qty < 0) {
                    return;
                }
                else {
                    this.confirmationService.confirm({
                        message: confirmationMessage,
                        header: 'Confirmation',
                        icon: 'pi pi-exclamation-triangle',
                        accept: () => {

                            this.items.clear();
                            this.insertNewItem();
                            this.resetRateOptions();
                            this.resetFinanceTotals();
                            this.setIncludeTotalGST();
                        },
                        reject: (type: any) => {

                            this.quoteItemsForm.controls.QuoteType.patchValue(radioPatchValue);
                            switch (type) {
                                case ConfirmEventType.REJECT:
                                    break;
                                case ConfirmEventType.CANCEL:
                                    break;
                            }
                        }
                    });
                }
            });
        }

    }

    onTabKey(event: any) {
        console.log(event)
    }

    resetFinanceTotals() {
        this.quoteItemsForm.controls.QuoteTotal.patchValue('');
        this.quoteItemsForm.controls.YearlyTotal.patchValue('');
        this.quoteItemsForm.controls.QuarterlyTotal.patchValue('');
        this.quoteItemsForm.controls.MonthlyTotal.patchValue('');
    }

    deleteItem(rowIndex: number) {
        this.items.removeAt(rowIndex);
        if (!this.items.length) {
            this.insertNewItem();
        }
        this.resetFinanceTotals();
        this.calculateLines();
        this.calculateRate();
    }

    getFormData(rowData: any, key: string) {
        if (key === 'QuarterlyTotal') {
            const a = rowData.get(key).value;
        }
        if (rowData && rowData.get(key)) {
            return rowData.get(key).value;
        }
        return '';
    }

    getQuoteType(): string {
        const quoteType = this.getFormData(this.quoteItemsForm, 'QuoteType');
        if (quoteType) {
            return quoteType;
        }
        return 'BySKU';
    }

    getFinanceType(): string {
        const financeType = this.getFormData(this.quoteItemsForm, 'FinanceType');
        if (financeType) {
            switch (financeType) {
                case '1': {
                    return 'Leasing';
                }
                case '2': {
                    return 'Rental';
                }
                case '4': {
                    return 'Instalments';
                }
            }
        }
        return 'Leasing';
    }
    defaultFunderVal() {
        this.quoteItemsForm.controls.MonthlyTotal.patchValue('0');
        this.quoteItemsForm.controls.selectedMonthlyFunderId.patchValue('None');
        this.quoteItemsForm.controls.selectedMonthlyFunderId.patchValue('None');
        this.quoteItemsForm.controls.MonthlyFunder.patchValue(this.quoteItemsForm.controls['Funder'].value);
        this.quoteItemsForm.controls.MonthlyFunderPlanId.patchValue('item.funderPlanID');
        this.quoteItemsForm.controls.MonthlyFunderPlanDescription.patchValue('item.funderPlanDescription');
    }
    getFunderPlan(): string {
        const funderPlan = this.getFormData(this.quoteItemsForm, 'FunderPlan');
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
        return 'Advance';
    }

    getQuoteTotal(): string {
        const quoteTotal = this.getFormData(this.quoteItemsForm, 'QuoteTotal');
        if (quoteTotal) {
            return quoteTotal;
        }
        return '0.0';
    }


    getCustomerControlValue() {
        return this.quoteHeaderDetailsForm.controls.Customer.value;
    }

    getControlValue(controlName: string) {
        return this.quoteHeaderDetailsForm.controls[controlName].value;
    }


    getQuoteStatus() {
        this.optionsControllerService.getStatus(false, true, false).subscribe(
            (response: StatusModel[]) => {
                this.imfsUtilityService.hideLoading();
                if (response.length === 0) {
                    this.imfsUtilityService.showToastr('error', 'Failed', 'No Status found');
                }
                else {
                    this.quoteStatusOptions = response;
                    this.quoteHeaderDetailsForm.controls.QuoteStatus.patchValue(1);
                }
            },
            (err: any) => {
                this.imfsUtilityService.hideLoading();
                this.imfsUtilityService.showToastr('error', 'Failed', 'Error in Get Status');
            }
        );
    }

    getTypes() {

        this.optionsControllerService.getTypes().subscribe(
            (response: TypesModel[]) => {
              this.getCategories();
                this.imfsUtilityService.hideLoading();
                if (response.length === 0) {
                    this.imfsUtilityService.showToastr('error', 'Failed', 'No Types found');
                }
                else {
                    this.types = response;
                }
            },
            (err: any) => {
                this.getCategories();
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
                    if(this.quoteDetailResponse){
                    this.quoteDetailResponse.quoteLines.forEach((quoteLine: QuoteLine) => {

                      if (quoteLine.item) {
                        console.log(this.types)
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
                      this.items.push(this.createTableRow(quoteLine));
                  });
                  setTimeout(() => {
                    this.calculateLines();
                  }, 1000);
                  }
                }
            },
            (err: any) => {
                this.imfsUtilityService.hideLoading();
                this.imfsUtilityService.showToastr('error', 'Failed', 'Error in Get Categories');
            }
        );
    }

    setFinanceProductId(rowIndex: number) {

        this.rowDataProductType = this.items.at(rowIndex);
        this.financeProductTypeSelected = this.rowDataProductType.controls.FinanceProductTypeID.value;
        this.displayDialog = true;
    }

    handleData(rowData: any, index: any) {

        this.activeTabIndex = index;
        const vpns = rowData.value.VPN;
        const imsku = rowData.value.IMSKU;
        const description = rowData.value.Description;

        let paramsUrl = '?param=';
        paramsUrl += vpns ?? vpns;
        if (vpns?.length && !imsku?.length && !description?.length) {

            this.confirmationService.close();
            this.quoteControllerService.getProductDetailsBySkuOrVpn(paramsUrl).subscribe(
                (response: { products: ProductDetailsModel[] }) => {
                    this.imfsUtilityService.hideLoading();

                    if (response.products.length === 0) {

                        rowData.controls.IMSKU.value = '';


                        // rowData.controls.VPN.setValue('');
                        // rowData.controls.VPN.value = '';
                        rowData.controls.Description.value = '';
                        // if (!rowData.controls.CostPrice.value) {
                        rowData.controls.CostPrice.value = '';
                        // }


                        this.imfsUtilityService.showToastr('error', 'Failed', 'No SKU or VPN found');

                        // Show confirm Dialog first time only
                        if (!rowData.controls.FinanceProductTypeID.value) {
                            this.confirmProductItem(rowData);
                        }

                    }
                    else {

                        this.skuRadioItems = response.products;
                        this.productDetails = response.products;

                        rowData.controls.IsNew.setValue(0);

                        if (response.products.length === 1) {


                            rowData.controls.IMSKU.value = this.productDetails[0].internalSKUID;


                            rowData.controls.VPN.setValue(this.productDetails[0].vendorSKUID);
                            // rowData.controls.VPN.value = this.productDetails[0].vendorSKUID;
                            rowData.controls.Description.value = this.productDetails[0].productDescription;
                            if (!rowData.controls.CostPrice.value) {
                                rowData.controls.CostPrice.value = this.productDetails[0].unitNetAmount;
                            }
                            setTimeout(() => {
                                const element = document.getElementById('qty-column-' + this.activeTabIndex.toString());
                                if (element) {
                                    element.click();
                                }
                            }, 100);

                        }

                        else {
                            // this.SkUModel.option = response.products[0].internalSKUID;
                            this.showSkuPopup();

                        }

                    }


                },
                (err: any) => {
                    this.imfsUtilityService.hideLoading();
                    this.imfsUtilityService.showToastr('error', 'Failed', 'Error getting product');
                }
            );

        }

    }

    sameAsAbove() {
      if(this.sameAsAboveCheckbox[0]) {

        this.orpCustomerControllerService.getCustomer('INGRAM MICRO AUSTRALIA PTY LTD').subscribe(
          (response: CustomerResponseModel) => {
        // replace end user details with customer details
        const getCustomer = this.quoteHeaderDetailsForm.get('Customer')?.value?.displayLabel;
        const arrayOfString = getCustomer.split("-");
        const customerName = arrayOfString[1] ? arrayOfString[1] : '';
        this.quoteHeaderDetailsForm.controls.EndUserName.setValue(customerName);
        this.quoteHeaderDetailsForm.controls.EndUserABN.setValue(this.quoteHeaderDetailsForm.get('ABN')?.value.toString());
        this.quoteHeaderDetailsForm.controls.EndUserAddressLine1.setValue(this.quoteHeaderDetailsForm.get('CustomerAddressLine1')?.value);
        this.quoteHeaderDetailsForm.controls.EndUserAddressLine2.setValue(this.quoteHeaderDetailsForm.get('CustomerAddressLine2')?.value);
        this.quoteHeaderDetailsForm.controls.EndUserCity.setValue(this.quoteHeaderDetailsForm.get('CustomerAddressCity')?.value);
        this.quoteHeaderDetailsForm.controls.EndUserState.setValue(this.quoteHeaderDetailsForm.get('CustomerAddressState')?.value);
        this.quoteHeaderDetailsForm.controls.EndUserPostcode.setValue(this.quoteHeaderDetailsForm.get('CustomerPostCode')?.value);
        this.quoteHeaderDetailsForm.controls.EndUserCountry.setValue(this.quoteHeaderDetailsForm.get('CustomerCountry')?.value);

        this.quoteHeaderDetailsForm.controls.EndUserYearsTrading.setValue('');
        this.quoteHeaderDetailsForm.controls.EndUserContactName.setValue('');
        this.quoteHeaderDetailsForm.controls.EndUserContactEmail.setValue('');
        this.quoteHeaderDetailsForm.controls.EndUserContactNumber.setValue('');
        this.quoteHeaderDetailsForm.controls.AuthorisedSignatoryName.setValue('');
        this.quoteHeaderDetailsForm.controls.AuthorisedSignatoryPosition.setValue('');



        // // disable end user replaced fields
        // this.quoteHeaderDetailsForm.get('EndUserName')?.disable();

        if(this.quoteHeaderDetailsForm.get('ABN')?.value)
        this.quoteHeaderDetailsForm.get('EndUserABN')?.disable();
        else
        this.quoteHeaderDetailsForm.get('EndUserABN')?.enable();

        // this.quoteHeaderDetailsForm.get('EndUserAddressLine1')?.disable();
        // this.quoteHeaderDetailsForm.get('EndUserAddressLine2')?.disable();
        // this.quoteHeaderDetailsForm.get('EndUserCity')?.disable();
        // this.quoteHeaderDetailsForm.get('EndUserState')?.disable();
        // this.quoteHeaderDetailsForm.get('EndUserPostcode')?.disable();
        // this.quoteHeaderDetailsForm.get('EndUserCountry')?.disable();



        /* //dummy data create
        const response: any = {
          customerDetails: []
        };
        response.customerDetails.push(
        {
        abn
        :
        "",
        addressLine1
        :
        "61 DUNNING AVE",
        addressLine2
        :
        "",
        city
        :
        "ROSEBERY",
        country
        :
        "AU",
        customerID
        :
        "55ef99b7-b65a-421b-9d14-7c79d8215f39",
        customerName
        :
        "INGRAM MICRO AUSTRALIA PTY LTD",
        customerNumber
        :
        "184584",
        displayLabel
        :
        "184584-INGRAM MICRO AUSTRALIA PTY LTD",
        postCode
        :
        "2018",
        state
        :
        "NSW"}); */

        // replace customer details with ingram micro details
        this.quoteHeaderDetailsForm.controls.Customer.setValue(response.customerDetails[0]);
        this.quoteHeaderDetailsForm.controls.ABN.setValue(response.customerDetails[0].abn.toString());
        this.quoteHeaderDetailsForm.controls.CustomerAddressLine1.setValue(response.customerDetails[0].addressLine1);
        this.quoteHeaderDetailsForm.controls.CustomerAddressLine2.setValue(response.customerDetails[0].addressLine2);
        this.quoteHeaderDetailsForm.controls.CustomerAddressCity.setValue(response.customerDetails[0].city);
        this.quoteHeaderDetailsForm.controls.CustomerAddressState.setValue(response.customerDetails[0].state);
        this.quoteHeaderDetailsForm.controls.CustomerPostCode.setValue(response.customerDetails[0].postCode);
        this.quoteHeaderDetailsForm.controls.CustomerCountry.setValue(response.customerDetails[0].country);

        this.quoteHeaderDetailsForm.controls.CustomerContact.setValue('');
        this.quoteHeaderDetailsForm.controls.CustomerContactEmail.setValue('');
        this.quoteHeaderDetailsForm.controls.CustomerContactNumber.setValue('');

         // disable customer details replaced fields
        if(response.customerDetails[0].abn)
        this.quoteHeaderDetailsForm.get('ABN')?.disable();
        else
        this.quoteHeaderDetailsForm.get('ABN')?.enable();


        this.quoteHeaderDetailsForm.get('CustomerAddressLine1')?.disable();
        this.quoteHeaderDetailsForm.get('CustomerAddressLine2')?.disable();
        this.quoteHeaderDetailsForm.get('CustomerAddressCity')?.disable();
        this.quoteHeaderDetailsForm.get('CustomerAddressState')?.disable();
        this.quoteHeaderDetailsForm.get('CustomerCountry')?.disable();
        this.quoteHeaderDetailsForm.get('CustomerPostCode')?.disable();



        this.quoteHeaderDetailsForm.get('CustomerContact')?.enable();
        this.quoteHeaderDetailsForm.get('CustomerContactEmail')?.enable();
        this.quoteHeaderDetailsForm.get('CustomerContactNumber')?.enable();
          },
          (err: any) => {
               this.imfsUtilityService.hideLoading();
              this.imfsUtilityService.showToastr('error', 'Failed', 'Error loading customers');
          }

      );


      }
    }

    getProductsBySkuOrVpn(event: any, rowData: any, index: any) {

        setTimeout(() => {
            const element = document.getElementById('delete-column-' + index.toString());
            if (element) {
                element.click();
            }
        }, 0);

        const delay = 1;
        if (this.timeout) {
            clearTimeout(this.timeout);
        }
        this.timeout = setTimeout((rowData: any, index: any) => {
            this.handleData(rowData, index);
        }, delay, rowData, index);
    }

    selectSku(rowData: any, index: number, option: string) {

        this.skuPopup = false;
        this.SkUModel.option = option;
        rowData.controls.IMSKU.value = this.productDetails[index].internalSKUID;


        rowData.controls.VPN.setValue(this.productDetails[index].vendorSKUID);
        // rowData.controls.VPN.value = this.productDetails[index].vendorSKUID;
        rowData.controls.Description.value = this.productDetails[index].productDescription;
        if (!rowData.controls.CostPrice.value) {
            rowData.controls.CostPrice.value = this.productDetails[index].unitNetAmount;
        }
        let maxLineNumber = 0
        setTimeout(() => {
            const element = document.getElementById('qty-column-' + this.activeTabIndex.toString());
            if (element) {
                element.click();
            }
        }, 100);

    }

    getProductDetails(rowData: any, index: any) {
        this.activeTabIndex = index;
        this.productInputModel = new ProductInputModel();

        const sku = rowData.value.IMSKU;
        const vpn = rowData.value.VPN;
        const skus = [rowData.value.IMSKU];
        const vpns = [rowData.value.VPN];


        if (!((sku && sku.length <= 3) || (vpn && vpn.length <= 3))) {

            this.productInputModel.imskus = skus;
            this.productInputModel.vpns = vpns;
            this.confirmationService.close();
            this.productControllerService.getProductDetails(this.productInputModel).subscribe(
                (response: any) => {
                    this.imfsUtilityService.hideLoading();
                    if (response.length === 0) {
                        this.imfsUtilityService.showToastr('error', 'Failed', 'No SKU found');

                        // Show confirm Dialog first time only
                        if (!rowData.controls.FinanceProductTypeID.value) {
                            this.confirmProductItem(rowData);
                        }
                    }
                    else {
                        this.productDetails = response;
                        rowData.controls.IMSKU.value = response[0].internalSKUID;
                        rowData.controls.VPN.value = response[0].vendorSKUID;
                        rowData.controls.Description.value = response[0].productDescription;
                        if (!rowData.controls.CostPrice.value) {
                            rowData.controls.CostPrice.value = response[0].unitNetAmount;
                        }
                    }
                },
                (err: any) => {
                    this.imfsUtilityService.hideLoading();
                    this.imfsUtilityService.showToastr('error', 'Failed', 'Error getting product');
                }
            );

        }

    }


    setProductType(financeProductTypeSelected: any) {

        if (!financeProductTypeSelected) {
            this.imfsUtilityService.showDialog('Warning', 'Please select Product Type');
            this.displayDialog = false;
        }
        else {
            this.rowDataProductType.controls.FinanceProductTypeID.setValue(financeProductTypeSelected);
            this.displayDialog = false;

            setTimeout(() => {
                let indexToFocus = 0;
                let currentIndex = 0;
                this.items.controls.forEach((item: AbstractControl) => {
                    const currentDescription = item.get('Description')?.value;

                    if (!currentDescription) {
                        indexToFocus = currentIndex;
                        return;
                    }
                    currentIndex++;
                });

                const element = document.getElementById('description-column-' + indexToFocus.toString());

                if (element) {
                    element.click();
                }
            }, 500);
        }
    }

    getFinanceProductTypeId(rowData: any) {
        if (rowData.controls.FinanceProductTypeID.value) {
            return true;
        }
        return false;
    }

    getQuoteId() {
        if (this.quoteIdGlobal > 0) {
            return true;
        }
        return false;
    }

    confirmProductItem(rowData: any) {

        const confirmationMessage = 'This is not an Ingram Product, would you like to create it for this quote?';
        this.closeSkuPopup();
        this.confirmationService.confirm({
            message: confirmationMessage,
            header: 'Product not found',
            icon: 'pi pi-exclamation-triangle',
            accept: () => {
                this.rowDataProductType = rowData;
                this.financeProductTypeSelected = '';
                this.displayDialog = true;
                rowData.controls.IMSKU.setValue(rowData.controls.VPN.value);

                rowData.controls.IsNew.setValue(1);
            },
            reject: (type: any) => {
                rowData.controls.IMSKU.setValue('');
                // rowData.controls.VPN.setValue('');
                rowData.controls.Description.setValue('');
                rowData.controls.FinanceProductTypeID.setValue('');

                // this.resetRateOptions();
                // this.resetFinanceTotals();
            }
        });
    }


    calculateLines() {
        let quoteTotal = 0;
        let includeTotalGST = 0;
        let totalinc = 0
        const GST = 10;
        this.items.controls.forEach((item: AbstractControl) => {
            if (item.get('FinanceProductTypeID')?.value) {
                if (!item.get('IMSKU')?.value) {
                    this.imfsUtilityService.showToastr('warning', 'Warning', 'Please enter IM SKU');
                    return;
                }

                if (!item.get('Description')?.value) {
                    this.imfsUtilityService.showToastr('warning', 'Warning', 'Please enter Description');
                    return;
                }

                if (!item.get('Qty')?.value) {
                    this.imfsUtilityService.showToastr('warning', 'Warning', 'Please enter Quantity');
                    return;
                }

                if (!item.get('CostPrice')?.value) {
                    this.imfsUtilityService.showToastr('warning', 'Warning', 'Please enter Cost Price');
                    return;
                }

            }
            const qty = item.get('Qty')?.value as number;
            let salesPrice = item.get('SalePrice')?.value as number;
            const costPrice = item.get('CostPrice')?.value as number;
            let margin = item.get('Margin')?.value as number;


            if (margin) {
                salesPrice = costPrice + (costPrice * margin / 100);
                item.get('SalePrice')?.patchValue(salesPrice);
            } else if (salesPrice) {
                const grossProfit = this.roundTo(
                    parseFloat((salesPrice - costPrice).toFixed(3)),
                    2
                );

                // if sales price is 0, use different calculation to avoid divided by 0 problem
                if (salesPrice === 0) {
                    if (costPrice > 0) {
                        // if cost price has value and sales price is 0, set gross profit percentage to -100%
                        margin = -100;
                    } else {
                        // if cost price and sales price is 0, set gross profit percentage to 0
                        margin = 0;
                    }
                } else {
                    margin = this.roundTo((grossProfit / salesPrice) * 100, 2);
                }
                item.get('Margin')?.patchValue(margin);
            }
            if (qty && salesPrice) {
                const lineTotal = qty * salesPrice;
                const totalGST = (lineTotal * GST) / 100;
                let gstVal = this.quoteItemsForm.get('GstInclude')?.value
                if (gstVal == 1) {
                    quoteTotal += lineTotal;
                    includeTotalGST += totalGST;
                    totalinc = lineTotal + totalGST
                    this.grandTotal = quoteTotal + includeTotalGST;
                }
                else {
                    quoteTotal += lineTotal
                    includeTotalGST += totalGST;
                    totalinc = lineTotal
                    this.grandTotal = quoteTotal
                }
                item.get('TotalGST')?.patchValue(totalGST);
                item.get('LineTotal')?.patchValue(lineTotal);
                item.get('includeTotalGST')?.patchValue(includeTotalGST);
                item.get('TotalInc')?.patchValue(totalinc)

            } else {
                item.get('TotalGST')?.patchValue('');
                item.get('LineTotal')?.patchValue('');
                item.get('includeTotalGST')?.patchValue('');
                item.get('TotalInc')?.patchValue('');
            }
        });


        this.quoteItemsForm.get('QuoteTotal')?.patchValue(quoteTotal);
        this.quoteItemsForm.get('includeTotalGST')?.patchValue(includeTotalGST);
        this.quoteItemsForm.get('TotalInc')?.patchValue(quoteTotal);

        if (quoteTotal !== null && quoteTotal > 0) {
            this.calculateRate();
        }
    }


    roundTo(num: number, precision: number) {
        return parseFloat((+(Math.round(+(num + 'e' + precision)) + 'e' + -precision)).toFixed(precision));
    }

    // TotalGst(num: number, precision: number) {
    //     return parseFloat((+(Math.round(+(num + 'e' + precision)) + 'e' + -precision)).toFixed(precision));
    // }


    downloadQuote(downloadMode: string) {
        this.imfsUtilityService.showLoading('Downloading...');
        const quoteDownloadInput = new QuoteDownloadInput();
        quoteDownloadInput.QuoteId = this.getFormData(this.quoteHeaderDetailsForm, 'QuoteNumber');
        quoteDownloadInput.DownloadMode = downloadMode;
        this.quoteControllerService.downloadQuote(quoteDownloadInput).subscribe(
            (res: any) => {
                this.jsUtilityService.fileSaveAs(res);
                this.imfsUtilityService.hideLoading();
            },
            (err: any) => {
                this.imfsUtilityService.hideLoading();
                this.imfsUtilityService.showToastr('error', 'Error', 'Unable to download file.');
            }
        );
    }
    downloadDocumentFile(fileId: number) {
        if (!fileId) {
            return;
        }
        this.quoteControllerService.downloadQuoteAttachment(fileId).subscribe((response: any) => {
            const fileObj = this.filesLists?.filter((file: QuoteDocuments) => file?.fileId === fileId)[0];
            const fileName = fileObj?.fileName;
            const fileContent = response.body;
            const blob = new Blob([fileContent], { type: 'application/octet-stream; charset=utf-8' });
            saveAs(blob, fileName);
        })
    }


    saveQuote() {
        const quoteDetails = this.convertFormToModel();
        if (!quoteDetails.quoteHeader.quoteTotal) {
            quoteDetails.quoteHeader.quoteTotal = 0;
        }
        if (!quoteDetails.quoteLines[0].lineTotal) {
            quoteDetails.quoteLines[0].costPrice = 0;
            quoteDetails.quoteLines[0].lineNumber = 0;
            quoteDetails.quoteLines[0].lineTotal = 0;
            quoteDetails.quoteLines[0].margin = 0;
            quoteDetails.quoteLines[0].qty = 0;
            quoteDetails.quoteLines[0].salePrice = 0;
            quoteDetails.quoteLines[0].totalGST = 0;
        }

        this.isEmptyQuoteLine = false;
        for (let index = 0; index < quoteDetails.quoteLines.length; index++) {
          const quoteLine = quoteDetails.quoteLines[index];
          if((!quoteLine.imsku && !quoteLine.vpn && !quoteLine.category) || quoteLine.lineTotal == 0) {
            this.isEmptyQuoteLine = true;
            break;
          }
        }

        if(!this.mode && !this.activeIndex && this.isEmptyQuoteLine) {
          this.imfsUtilityService.showToastr('error', 'Error',  'Please enter quote lines before saving');
        }
        else if(this.isEmptyQuoteLine) {
        this.imfsUtilityService.showToastr('error', 'Error', 'Please complete the line entry before proceeding');
        } else {

        this.imfsUtilityService.showLoading('Saving...');
        // save quote details to server
        this.quoteControllerService.saveQuote(quoteDetails).subscribe(
            (response: HttpResponseData) => {
                this.imfsUtilityService.hideLoading();
                this.imfsUtilityService.showToastr('success', 'Success', 'Quote Updated Successfully:' + response.quoteId);
                //void this.router.navigate([IMFSRoutes.Quote, response.quoteId]);
                void this.router.navigate([IMFSRoutes.Quote], { queryParams: { id: response.quoteId, mode: 'edit' } });
               /*  if(this.mode === 'view') {
                  void this.router.navigate([IMFSRoutes.Quote], { queryParams: { id: response.quoteId, mode: 'edit' } });
                }
                else if(this.mode === 'edit') {
                  this.ngOnInit();
                } */
            },
            (err: any) => {
                this.imfsUtilityService.hideLoading();
                this.imfsUtilityService.showToastr('error', 'Failed', 'Error Saving quote');
            }
        );
      }
    }
    convertFormToModel() {
        // const quoteDetails = new QuoteDetailsModel();
        let funderVal = this.quoteItemsForm.get('Funder')?.value
        let gstVal = this.quoteItemsForm.get('GstInclude')?.value
        this.quoteDetails.quoteHeader = new QuoteHeader();
        this.quoteDetails.customerDetails = new CustomerDetails();
        this.quoteDetails.endUserDetails = new EndUserDetails();
        this.quoteDetails.quoteLines = [];
        this.quoteDetails.quoteHeader.quoteNumber = this.quoteHeaderDetailsForm.get('QuoteNumber')?.value;
        this.quoteDetails.quoteHeader.quoteName = this.quoteHeaderDetailsForm.get('QuoteName')?.value;
        this.quoteDetails.quoteHeader.expiryDate = this.quoteHeaderDetailsForm.get('ExpiryDate')?.value;
        this.quoteDetails.quoteHeader.status = this.quoteHeaderDetailsForm.get('QuoteStatus')?.value;

        this.quoteDetails.customerDetails.customerNumber = this.quoteHeaderDetailsForm.get('Customer')?.value.customerNumber;
        this.quoteDetails.customerDetails.customerName = this.quoteHeaderDetailsForm.get('Customer')?.value.customerName;
        this.quoteDetails.customerDetails.customerABN = this.quoteHeaderDetailsForm.get('ABN')?.value.toString();
        this.quoteDetails.customerDetails.customerAddressLine1 = this.quoteHeaderDetailsForm.get('CustomerAddressLine1')?.value;
        this.quoteDetails.customerDetails.customerAddressLine2 = this.quoteHeaderDetailsForm.get('CustomerAddressLine2')?.value;
        this.quoteDetails.customerDetails.customerAddressCity = this.quoteHeaderDetailsForm.get('CustomerAddressCity')?.value;
        this.quoteDetails.customerDetails.customerAddressState = this.quoteHeaderDetailsForm.get('CustomerAddressCity')?.value;
        this.quoteDetails.customerDetails.customerPostCode = this.quoteHeaderDetailsForm.get('CustomerPostCode')?.value;
        this.quoteDetails.customerDetails.customerCountry = this.quoteHeaderDetailsForm.get('CustomerCountry')?.value;
        this.quoteDetails.customerDetails.customerContact = this.quoteHeaderDetailsForm.get('CustomerContact')?.value;
        this.quoteDetails.customerDetails.customerEmail = this.quoteHeaderDetailsForm.get('CustomerContactEmail')?.value;
        this.quoteDetails.customerDetails.customerPhone = this.quoteHeaderDetailsForm.get('CustomerContactNumber')?.value;

        this.quoteDetails.endUserDetails.authorisedSignatoryName = this.quoteHeaderDetailsForm.get('AuthorisedSignatoryName')?.value;
        // tslint:disable-next-line: max-line-length
        this.quoteDetails.endUserDetails.authorisedSignatoryPosition = this.quoteHeaderDetailsForm.get('AuthorisedSignatoryPosition')?.value;
        this.quoteDetails.endUserDetails.endCustomerName = this.quoteHeaderDetailsForm.get('EndUserName')?.value;
        this.quoteDetails.endUserDetails.endCustomerABN = this.quoteHeaderDetailsForm.get('EndUserABN')?.value.toString();
        this.quoteDetails.endUserDetails.endCustomerAddressLine1 = this.quoteHeaderDetailsForm.get('EndUserAddressLine1')?.value.street || this.selectedAdd
        this.quoteDetails.endUserDetails.endCustomerAddressLine2 = this.addLine2;
        this.quoteDetails.endUserDetails.endCustomerCity = this.quoteHeaderDetailsForm.get('EndUserCity')?.value;
        this.quoteDetails.endUserDetails.endCustomerState = this.quoteHeaderDetailsForm.get('EndUserState')?.value;
        this.quoteDetails.endUserDetails.endCustomerPostCode = this.quoteHeaderDetailsForm.get('EndUserPostcode')?.value;
        this.quoteDetails.endUserDetails.endCustomerCountry = this.quoteHeaderDetailsForm.get('EndUserCountry')?.value;
        this.quoteDetails.endUserDetails.endCustomerContact = this.quoteHeaderDetailsForm.get('EndUserContactName')?.value;
        this.quoteDetails.endUserDetails.endCustomerEmail = this.quoteHeaderDetailsForm.get('EndUserContactEmail')?.value;
        this.quoteDetails.endUserDetails.endCustomerPhone = this.quoteHeaderDetailsForm.get('EndUserContactNumber')?.value;
        this.quoteDetails.endUserDetails.endCustomerYearsTrading = this.quoteHeaderDetailsForm.get('EndUserYearsTrading')?.value;

        this.quoteDetails.quoteHeader.quoteType = this.quoteItemsForm.get('QuoteType')?.value;
        this.quoteDetails.quoteHeader.financeType = String(this.quoteItemsForm.get('FinanceType')?.value);
        this.quoteDetails.quoteHeader.funderPlan = String(this.quoteItemsForm.get('FunderPlan')?.value);
        this.quoteDetails.quoteHeader.quoteTotal = this.quoteItemsForm.get('QuoteTotal')?.value;
        this.quoteDetails.quoteHeader.funderCode = funderVal ? funderVal : "";
        this.quoteDetails.quoteHeader.gstInclude = gstVal
        this.quoteDetails.quoteHeader.funderId = this.quoteItemsForm.get('FunderId')?.value;
        this.quoteDetails.quoteHeader.financeValue = Number(this.quoteItemsForm.get('FinanceValue')?.value);
        this.quoteDetails.quoteHeader.frequency = this.quoteItemsForm.get('Frequency')?.value;
        if (this.quoteItemsForm.get('QuoteDuration')?.value !== null) {
            this.quoteDetails.quoteHeader.quoteDuration = this.quoteItemsForm.get('QuoteDuration')?.value.toString();
        }

        this.items.controls.forEach((item: AbstractControl) => {
            const quoteLine = new QuoteLine();

            quoteLine.imsku = item.get('IMSKU')?.value;
            quoteLine.vpn = item.get('VPN')?.value;
            quoteLine.financeProductTypeID = item.get('FinanceProductTypeID')?.value;
            quoteLine.description = item.get('Description')?.value;
            quoteLine.lineNumber = item.get('LineNumber')?.value;
            quoteLine.vsr = item.get('VSR')?.value;
            if (item.get('Item')?.value !== null) {
                quoteLine.item = item.get('Item')?.value.toString();
            }

            if (item.get('Category')?.value !== null) {
                quoteLine.category = item.get('Category')?.value.toString();
            }

            quoteLine.salePrice = item.get('SalePrice')?.value;
            quoteLine.costPrice = item.get('CostPrice')?.value;
            quoteLine.qty = item.get('Qty')?.value;
            quoteLine.margin = item.get('Margin')?.value;
            quoteLine.totalGST = Number(item.get('TotalGST')?.value);
            quoteLine.lineTotal = Number(item.get('LineTotal')?.value);
            quoteLine.totalinc = Number(item.get('TotalInc')?.value);
            this.quoteDetails.quoteLines.push(quoteLine);
        });
        return this.quoteDetails;
    }

    getQuoteItems() {
        const quoteLines: QuoteLine[] = [];
        this.items.controls.forEach((item: AbstractControl) => {
            const quoteLine = new QuoteLine();

            quoteLine.imsku = item.get('IMSKU')?.value;
            quoteLine.vpn = item.get('VPN')?.value;
            quoteLine.financeProductTypeID = item.get('FinanceProductTypeID')?.value;
            quoteLine.description = item.get('Description')?.value;
            quoteLine.lineNumber = item.get('LineNumber')?.value;
            quoteLine.vsr = item.get('VSR')?.value;
            quoteLine.item = item.get('Item')?.value;
            quoteLine.itemName = item.get('ItemName')?.value;
            quoteLine.category = item.get('Category')?.value;
            quoteLine.categoryName = item.get('CategoryName')?.value;
            quoteLine.salePrice = item.get('SalePrice')?.value;
            quoteLine.costPrice = item.get('CostPrice')?.value;
            quoteLine.qty = item.get('Qty')?.value;
            quoteLine.margin = item.get('Margin')?.value;
            quoteLine.totalGST = Number(item.get('TotalGST')?.value);
            quoteLine.lineTotal = Number(item.get('LineTotal')?.value);
            quoteLine.totalinc = Number(item.get('TotalInc')?.value);
            quoteLines.push(quoteLine);
        });
        return quoteLines;
    }

    marginApplyAll() {
        const marginAll = this.quoteItemsForm.get('ApplyAllMargin')?.value;
        if (marginAll) {
            this.items.controls.forEach((item: AbstractControl) => {
                item.get('Margin')?.patchValue(marginAll);
            });
            this.calculateLines();
        }
    }

    goToSummaryTab() {
      const quoteDetails = this.convertFormToModel();
      this.isEmptyQuoteLine = false;
      for (let index = 0; index < quoteDetails.quoteLines.length; index++) {
        const quoteLine = quoteDetails.quoteLines[index];
        if((!quoteLine.imsku && !quoteLine.vpn && !quoteLine.category) || quoteLine.lineTotal == 0) {
          this.isEmptyQuoteLine = true;
        }
      }

        if(!this.mode && !this.activeIndex && this.isEmptyQuoteLine) {
          this.imfsUtilityService.showToastr('error', 'Error',  'Please enter quote lines before saving');
          return;
        }
        else if(this.isEmptyQuoteLine) {
        this.imfsUtilityService.showToastr('error', 'Error', 'Please complete the line entry before proceeding');
        return;
        }
        else if (!this.selectedRateOption) {
          this.imfsUtilityService.showDialog('Select Finance Option', 'Please select a Finance option before proceeding.');
          //  this.activeTabIndex = 1;
          this.activeIndex = 1;
          this.quoteItems = true;
          return;
        } else  {
            this.activeIndex = 2;
            this.summary = true;
            this.quoteItems = false;
        }

        // this.activeTabIndex = 2;

    }

    rateOptionSelected(selectedFrequency: string) {
        this.selectedRateOption = selectedFrequency;
        switch (selectedFrequency) {
            case 'Quarterly': {
                const selectedFinanceQuarterly = this.getFormData(this.quoteItemsForm, 'QuarterlyTotal');
                this.quoteItemsForm.controls.FunderId.patchValue(this.selectedQuarterlyFunderId);
                this.quoteItemsForm.controls.FinanceValue.patchValue(selectedFinanceQuarterly);
                this.quoteItemsForm.controls.Frequency.patchValue(selectedFrequency);

                this.quoteItemsForm.controls.FunderPlanId.patchValue(this.selectedMonthlyFunderPlanId);
                this.quoteItemsForm.controls.FunderPlanDescription.patchValue(this.selectedMonthlyFunderPlanDescription);
                break;
            }
            case 'Monthly': {
                const selectedFinanceMonthly = this.getFormData(this.quoteItemsForm, 'MonthlyTotal');
                this.quoteItemsForm.controls.FunderId.patchValue(this.selectedMonthlyFunderId);
                this.quoteItemsForm.controls.FinanceValue.patchValue(selectedFinanceMonthly);
                this.quoteItemsForm.controls.Frequency.patchValue(selectedFrequency);

                this.quoteItemsForm.controls.FunderPlanId.patchValue(this.selectedQuarterlyFunderPlanId);
                this.quoteItemsForm.controls.FunderPlanDescription.patchValue(this.selectedQuarterlyFunderPlanDescription);
                break;
            }
            case 'Yearly': {
                const selectedFinanceYearly = this.getFormData(this.quoteItemsForm, 'YearlyTotal');
                this.quoteItemsForm.controls.FunderId.patchValue(this.selectedYearlyFunderId);
                this.quoteItemsForm.controls.FinanceValue.patchValue(selectedFinanceYearly);
                this.quoteItemsForm.controls.Frequency.patchValue(selectedFrequency);

                this.quoteItemsForm.controls.FunderPlanId.patchValue(this.selectedYearlyFunderPlanId);
                this.quoteItemsForm.controls.FunderPlanDescription.patchValue(this.selectedYearlyFunderPlanDescription);
                break;
            }
        }
    }

    getFinanceTotal(fieldName: string) {
        const duration = this.getFormData(this.quoteItemsForm, 'QuoteDuration');
        let financeTotal = 0;
        switch (fieldName) {
            case 'MonthlyTotal': {
                const monthlyRate = this.getFormData(this.quoteItemsForm, fieldName);
                financeTotal = monthlyRate * duration;
                break;
            }
            case 'QuarterlyTotal': {
                const quarterlyRate = this.getFormData(this.quoteItemsForm, fieldName);
                const totalQuarterly = duration / 3;
                financeTotal = quarterlyRate * totalQuarterly;
                break;
            }
            case 'YearlyTotal': {
                const yearlyRate = this.getFormData(this.quoteItemsForm, fieldName);
                const totalYear = duration / 12;
                financeTotal = yearlyRate * totalYear;
                break;
            }
        }
        return financeTotal;
    }

    emailQuote(quoteNumber: number | string) {

        if(quoteNumber === 'New')
        this.imfsUtilityService.showToastr('error', 'Warning', 'Please Save this quote before requesting approval');
        else
        void this.router.navigate([IMFSRoutes.Email], { queryParams: { quoteId: quoteNumber } });
    }

    getQuoteNumber() {
        return this.quoteHeaderDetailsForm.controls.QuoteNumber.value;
    }

    openEmailHistoryModal() {
        this.emailHistoryModal.open(this.quoteHeaderDetailsForm.controls.QuoteNumber.value);
    }
    /*----Coded-by-DS----*/
    @ViewChild('SearchPopupModal') SearchPopupModal: AuSearchTemplateComponent;
    displayEndUserDialog = false;
    showEndUserDialog() {
        this.displayEndUserDialog = true;
    }
    displayAbnNumberDialog = false;
    showAbnNumberDialog() {
        this.displayAbnNumberDialog = true;
    }
    closeDialog(): void {
        this.displayEndUserDialog = false;
        this.displayAbnNumberDialog = false;
    }
    searchCompanyValue: string;
    receivedSearchText(event: any) {
        this.searchAbnValue = event.abn;
        this.quoteHeaderDetailsForm.controls.EndUserABN.setValue(event.abn);
        if (event.name) {
            this.searchCompanyValue = event.name;
            this.quoteHeaderDetailsForm.controls.EndUserName.setValue(event.name);
        }
        else {
            this.searchCompanyValue = event.entityName;
            this.quoteHeaderDetailsForm.controls.EndUserName.setValue(event.entityName);
        }
    }
    searchAbnValue: string;
    receivedSearchNumber(event: any) {
        this.searchAbnValue = event.abn;
        this.quoteHeaderDetailsForm.controls.EndUserABN.setValue(event.abn);
        if (event.entityName) {
            this.searchCompanyValue = event.entityName;
            this.quoteHeaderDetailsForm.controls.EndUserName.setValue(event.entityName);
        }
        else {
            this.searchCompanyValue = event.name;
            this.quoteHeaderDetailsForm.controls.EndUserName.setValue(event.name);
        }

    }

    @ViewChild('DocumentUploadModal') DocumentUploadModal: DocumentUploadPopupComponent;
    displayDocumentUploadModal = false;
    //uploadFilebutton:boolean=true;
    showDocumentUploadDialog() {
        if (this.getfilequoteid) {
            this.displayDocumentUploadModal = true;
        }
        else {
            this.imfsUtilityService.showToastr('error', 'Save Data', 'Please save the quote before uploading the file.');
            this.displayDocumentUploadModal = false;
        }
    }
    closeDocumentPopupDialog(): void {
        this.displayDocumentUploadModal = false;
    }
    quoteFormInvalid = false;
    /** For Verification of Adresses*/
    quoteName: string;
    yearsTrading: string;
    customer: string
    customerContactEmail: string;
    customerContact: string;
    endUserABN: string;
    endUserName: string;
    onNextFirstStep() {

        if(!this.quoteHeaderDetailsForm.controls.Customer.value.customerNumber) {
          this.imfsUtilityService.showToastr('error', 'Invalid Input', 'Please enter a valid Customer');
          return;
        }
        if(this.quoteHeaderDetailsForm.controls.CustomerContactNumber?.errors?.pattern) {
          this.imfsUtilityService.showToastr('error', 'Invalid Input', 'Please enter a valid customer phone number +612 9381 6000, (02) 9381 6000');
          return;
        }

        if(this.quoteHeaderDetailsForm.controls.EndUserContactNumber?.errors?.pattern) {
          this.imfsUtilityService.showToastr('error', 'Invalid Input', 'Please enter a valid end user phone number +612 9381 6000, (02) 9381 6000');
          return;
        }

        if(Number(this.quoteHeaderDetailsForm.value.QuoteStatus) !== this.endCustomerAccepted){
        if (!this.quoteHeaderDetailsForm.valid) {
            this.imfsUtilityService.showToastr('error', 'Invalid Input', 'Please enter all required fields.');
            this.quoteFormInvalid = true;
            this.addressInvalid = true;
            setTimeout(() => {
                //this.activeTabIndex = 0;
                this.custDetails = true;
                this.activeIndex = 0;
            }, 0);
            return;
        }
        }
        if (!(this.selectedAdd && this.postCode && this.state && this.city && this.country)) {
            this.addressInvalid = true;
            this.imfsUtilityService.showToastr('error', 'Failed', 'Please Add All Address Fields')
            return
        }

        else {
            this.quoteItems = true
            this.custDetails = false
            this.summary = false
            this.activeIndex = 1
            // this.activeTabIndex = 1;
            this.navigate(1, '');
        }

    }
    /** For getting Adresses*/

    newAdd(event: any) {
        let addFiltered: any[] = []
        let query = event.query
        let country = "AUS"
        let suggestedAdd: any = [];
        this._addressvalidateservice.getaddressauto(query, country).subscribe((res) => {

            suggestedAdd = res.candidates;
            if (suggestedAdd.length) {
                for (let i = 0; i < suggestedAdd.length; i++) {
                    let add = suggestedAdd[i]
                    addFiltered.push(add)
                    this.filteredAdd = addFiltered;
                }
            } else {
                this.filteredAdd = [];
            }
        }, (err) => {
        })

    }

    fillAddress(value: any) {
        this.city = this.selectedAdd.locality ? this.selectedAdd.locality : this.city;
        this.state = this.selectedAdd.administrative_area ? this.selectedAdd.administrative_area : this.state;
        this.postCode = this.selectedAdd.postal_code ? this.selectedAdd.postal_code : this.postCode;
        this.country = this.selectedAdd.country_iso3 ? this.selectedAdd.country_iso3 : this.country;
        setTimeout(() => {
          this.selectedAdd = value.street ? value.street : this.selectedAdd;
        }, 1);
    }

    getFundersOptions() {
        this.funderTypes.push({ label: "select", value: "" })
        this.funderControllerService.getFunders(false).subscribe((res) => {
            let len = res.length
            for (let i = 0; i < len; i++) {
                let obj = {
                    label: res[i].funderName + ", " + res[i].funderCode,
                    value: res[i].funderCode
                }
                this.funderTypes.push(obj)
            }
        })
    }
    lastStepTab() {
        this.saveQuote();
        this.documents = true
        this.activeIndex = 3
        this.summary = false
    }
    documentStepTab() {
        this.documents = true
        this.activeIndex = 3
        this.summary = false
    }
    skuPopup: boolean = false;
    showSkuPopup() {
        this.skuPopup = true;
    }
    closeSkuPopup() {
        this.skuPopup = false;
    }
    skuRadioItems: ProductDetailsModel[];
    SkUModel: { option: any } = {
        option: ''
    };


}


