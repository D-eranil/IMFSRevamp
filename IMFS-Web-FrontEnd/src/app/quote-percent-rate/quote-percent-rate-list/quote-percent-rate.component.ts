import { Component, OnInit, ViewChild } from '@angular/core';
import { Dropdown } from 'primeng/dropdown';
import { MessageService, SortEvent } from 'primeng/api';
import { FileUpload } from 'primeng/fileupload';

import { HttpClientModule } from '@angular/common/http';
import { QuotePercentRateModel } from 'src/app/models/quote-percent-rate/quote-percent-rate.model';
import { QuotePercentRateResponseModel } from 'src/app/models/quote-percent-rate/quote-percent-rate-response.model';
import { QuotePercentRateInputModel } from 'src/app/models/quote-percent-rate/quote-percent-rate-input.model';
import { QuotePercentRateControllerService } from 'src/app/services/controller-services/quote-percent-rate-controller.service';
import { FunderControllerService } from 'src/app/services/controller-services/funder-controller.service';
import { FinanceTypeControllerService } from 'src/app/services/controller-services/finance-type-controller.service';
import { IMFSUtilityService } from 'src/app/services/utility-services/imfs-utility-services';
import { FunderModel } from 'src/app/models/funder/funder.model';
import { FinanceTypeModel } from 'src/app/models/finance-type/finance-type.model';
import { FinanceProductTypeModel } from 'src/app/models/finance-product-type/finance-product-type.model';
import { QuotePercentRateDetailsModalComponent } from '../quote-percent-rate-details-modal/quote-percent-rate-details-modal.component';
import { JsUtilityService } from 'src/app/services/utility-services/js-utility.service';
import { QuotePercentRateUploadModalComponent } from '../quote-percent-rate-upload-modal/quote-percent-rate-upload-modal.component';
import { ComponentVisibilityService } from 'src/app/services/utility-services/component-visibility.service';
import { FinanceProductTypeControllerService } from 'src/app/services/controller-services/finance-product-type-controller.servcie';
import { FunderPlanModel } from 'src/app/models/funder-plan/funder-plan.model';
import { FunderPlanControllerService } from 'src/app/services/controller-services/funder-plan-controller.service';

@Component({
    selector: 'app-quote-percent-rate',
    templateUrl: './quote-percent-rate.component.html',
    styleUrls: ['./quote-percent-rate.component.scss']
})

export class QuotePercentRateComponent implements OnInit {

    percentRates: QuotePercentRateModel[];

    productTypes: FinanceProductTypeModel[]; // Device, Software, Service etc
    selectedProductType: number;

    financeTypes: FinanceTypeModel[]; // Lease, rental, instalment etc
    selectedFinanceType: number;

    funders: FunderModel[];
    selectedFunder: number;

    funderPlans: FunderPlanModel[];
    selectedFunderPlan: number;

    quotePercentRateInputModel: QuotePercentRateInputModel;


    @ViewChild('fileUpload') fileUpload: FileUpload;
    @ViewChild('uploadModal') uploadModal: QuotePercentRateUploadModalComponent;
    @ViewChild('detailsModal') detailsModal: QuotePercentRateDetailsModalComponent;

    constructor(
        private imfsUtilityService: IMFSUtilityService,
        private jsUtilityService: JsUtilityService,
        private quotePercentRateControllerService: QuotePercentRateControllerService,
        private funderControllerService: FunderControllerService,
        private funderPlanControllerService: FunderPlanControllerService,
        private financeTypeControllerService: FinanceTypeControllerService,
        private financeProductTypeControllerService: FinanceProductTypeControllerService,
        private messageService: MessageService,
        private componentVisibilityService: ComponentVisibilityService
    ) {

        this.loadFunders();
        this.loadFinanceTypes();
        this.loadFinanceProductTypes();
        this.loadFunderPlans();
    }

    ngOnInit() {
        // make full screen
        this.componentVisibilityService.fullScreenFire(true);
    }

    loadQuotePercentRates() {
        const that = this;

        if (that.selectedProductType === undefined) {
            that.imfsUtilityService.showToastr('error', 'Product Type', 'Please select Product Type');
            return;
        }

        if (that.selectedFinanceType === undefined) {
            that.imfsUtilityService.showToastr('error', 'Finance Type', 'Please select Finance Type');
            return;
        }

        if (that.selectedFunder === undefined) {
            that.imfsUtilityService.showToastr('error', 'Funder Type', 'Please select Funder Type');
            return;
        }

        if (that.selectedFunderPlan === undefined) {
            that.imfsUtilityService.showToastr('error', 'Funder Plan', 'Please select Funder Plan');
            return;
        }

        // console.log('Finance Type Selected:' + that.selectedFinanceType);
        // console.log('Funder Selected:' + that.selectedFunder);
        // console.log('Funder Plan Selected:' + that.selectedFunderPlan);

        that.quotePercentRateInputModel = new QuotePercentRateInputModel();

        that.quotePercentRateInputModel.FunderId = that.selectedFunder;
        that.quotePercentRateInputModel.FinanceType = that.selectedFinanceType;
        that.quotePercentRateInputModel.ProductType = that.selectedProductType;
        that.quotePercentRateInputModel.FunderPlan = that.selectedFunderPlan;

        that.imfsUtilityService.showLoading('Loading Quote Break Percent Rates');

        that.quotePercentRateControllerService.getQuotePercentRates(that.quotePercentRateInputModel).subscribe(
            (response: QuotePercentRateModel[]) => {
                that.imfsUtilityService.hideLoading();
                that.percentRates = response;
            },
            (err: any) => {
                console.log(err);
                that.imfsUtilityService.hideLoading();
                that.imfsUtilityService.showToastr('error', 'Failed', 'Error loading quote percent rates');
            }

        );
    }

    loadFunderPlans() {
        const that = this;

        // tslint:disable-next-line: deprecation
        that.funderPlanControllerService.getFunderPlans().subscribe(
            (response: FunderPlanModel[]) => {
                that.funderPlans = response;
            },
            (err: any) => {
                console.log(err);
                that.imfsUtilityService.showToastr('error', 'Failed', 'Error loading funder plans');
            }

        );

    }

    loadFunders() {
        const that = this;

        // tslint:disable-next-line: deprecation
        that.funderControllerService.getFunders().subscribe(
            (response: FunderModel[]) => {
                that.funders = response;
            },
            (err: any) => {
                console.log(err);
                that.imfsUtilityService.showToastr('error', 'Failed', 'Error loading funders');
            }

        );
    }


    loadFinanceProductTypes() {
        const that = this;

        // tslint:disable-next-line: deprecation
        that.financeProductTypeControllerService.getFinanceProductTypes().subscribe(
            (response: FinanceProductTypeModel[]) => {
                that.productTypes = response;
            },
            (err: any) => {
                console.log(err);
                that.imfsUtilityService.showToastr('error', 'Failed', 'Error loading Product Types');
            }

        );
    }

    loadFinanceTypes() {
        const that = this;

        // tslint:disable-next-line: deprecation
        that.financeTypeControllerService.getFinanceTypes().subscribe(
            (response: FinanceTypeModel[]) => {
                that.financeTypes = response;
            },
            (err: any) => {
                console.log(err);
                that.imfsUtilityService.showToastr('error', 'Failed', 'Error loading Finance Types');
            }

        );
    }

    editQuoteBreakPercentRate(editItem: QuotePercentRateResponseModel) {
        this.detailsModal.open(editItem);
    }

    exportQuotePercentRates() {

        const that = this;
        if (that.selectedFunder === undefined) {
            that.imfsUtilityService.showToastr('error', 'Funder Type', 'Please select Funder Type');
            return;
        }

        that.quotePercentRateInputModel = new QuotePercentRateInputModel();
        that.quotePercentRateInputModel.FunderId = that.selectedFunder;
        that.quotePercentRateInputModel.FinanceType = that.selectedFinanceType;
        that.quotePercentRateInputModel.ProductType = that.selectedProductType;
        that.quotePercentRateInputModel.FunderPlan = that.selectedFunderPlan;

        this.imfsUtilityService.showLoading('Downloading');

        this.quotePercentRateControllerService.exportQuotePercentRates(that.quotePercentRateInputModel).subscribe(
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

    uploadQuotePercentRates() {
        this.uploadModal.open();
    }

}
