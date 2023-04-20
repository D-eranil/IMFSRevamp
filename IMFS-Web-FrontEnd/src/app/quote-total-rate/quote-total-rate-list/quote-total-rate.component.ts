import { Component, OnInit, ViewChild } from '@angular/core';
import { Dropdown } from 'primeng/dropdown';
import { MessageService, SortEvent } from 'primeng/api';
import { FileUpload } from 'primeng/fileupload';

import { HttpClientModule } from '@angular/common/http';

import { QuoteTotalRateModel } from 'src/app/models/quote-Total-rate/quote-total-rate.model';
import { QuoteTotalRateResponseModel } from 'src/app/models/quote-Total-rate/quote-total-rate-response.model';
import { QuoteTotalRateInputModel } from 'src/app/models/quote-Total-rate/quote-total-rate-input.model';
import { QuoteTotalRateControllerService } from 'src/app/services/controller-services/quote-total-rate-controller.service';
import { FunderControllerService } from 'src/app/services/controller-services/funder-controller.service';
import { FinanceTypeControllerService } from 'src/app/services/controller-services/finance-type-controller.service';
import { IMFSUtilityService } from 'src/app/services/utility-services/imfs-utility-services';
import { FunderModel } from 'src/app/models/funder/funder.model';
import { FinanceTypeModel } from 'src/app/models/finance-type/finance-type.model';
import { QuoteTotalRateDetailsModalComponent } from '../quote-total-rate-details-modal/quote-total-rate-details-modal.component';
import { JsUtilityService } from 'src/app/services/utility-services/js-utility.service';
import { QuoteTotalRateUploadModalComponent } from '../quote-total-rate-upload-modal/quote-total-rate-upload-modal.component';
import { ComponentVisibilityService } from 'src/app/services/utility-services/component-visibility.service';
import { FunderPlanControllerService } from 'src/app/services/controller-services/funder-plan-controller.service';
import { FunderPlanModel } from 'src/app/models/funder-plan/funder-plan.model';


@Component({
    selector: 'app-quote-total-rate',
    templateUrl: './quote-total-rate.component.html',
    styleUrls: ['./quote-total-rate.component.scss']
})

export class QuoteTotalRateComponent implements OnInit {

    totalRates: QuoteTotalRateModel[];

    funderPlans: FunderPlanModel[];
    financeTypes: FinanceTypeModel[]; // Lease, rental, instalment etc
    funders: FunderModel[];

    selectedFunder: number;
    selectedFunderPlan: number;
    selectedFinanceType: number;

    quoteTotalRateInputModel: QuoteTotalRateInputModel;

    @ViewChild('fileUpload') fileUpload: FileUpload;
    @ViewChild('uploadModal') uploadModal: QuoteTotalRateUploadModalComponent;
    @ViewChild('detailsModal') detailsModal: QuoteTotalRateDetailsModalComponent;

    constructor(
        private imfsUtilityService: IMFSUtilityService,
        private jsUtilityService: JsUtilityService,
        private quoteTotalRateControllerService: QuoteTotalRateControllerService,
        private funderControllerService: FunderControllerService,
        private financeTypeControllerService: FinanceTypeControllerService,
        private funderPlanControllerService: FunderPlanControllerService,
        private messageService: MessageService,
        private componentVisibilityService: ComponentVisibilityService
    ) {

        this.loadFunders();
        this.loadFinanceTypes();
        this.loadFunderPlans();

    }

    ngOnInit() {
        // make full screen
        this.componentVisibilityService.fullScreenFire(true);
    }

    loadQuoteTotalRates() {
        const that = this;

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

        console.log('Finance Type Selected:' + that.selectedFinanceType);
        console.log('Funder Selected:' + that.selectedFunder);
        console.log('Funder Plan Selected:' + that.selectedFunderPlan);

        that.quoteTotalRateInputModel = new QuoteTotalRateInputModel();

        that.quoteTotalRateInputModel.FunderId = that.selectedFunder;
        that.quoteTotalRateInputModel.FinanceType = that.selectedFinanceType;
        that.quoteTotalRateInputModel.FunderPlan = that.selectedFunderPlan;

        that.imfsUtilityService.showLoading('Loading Quote Break Total Rates');

        that.quoteTotalRateControllerService.getQuoteTotalRates(that.quoteTotalRateInputModel).subscribe(
            (response: QuoteTotalRateModel[]) => {
                that.imfsUtilityService.hideLoading();

                that.totalRates = response;

            },
            (err: any) => {
                console.log(err);
                that.imfsUtilityService.hideLoading();
                that.imfsUtilityService.showToastr('error', 'Failed', 'Error loading quote total rates');
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

    editQuoteBreakTotalRate(editItem: QuoteTotalRateResponseModel) {
        this.detailsModal.open(editItem);
    }

    exportQuoteTotalRates() {

        const that = this;
        if (that.selectedFunder === undefined) {
            that.imfsUtilityService.showToastr('error', 'Funder Type', 'Please select Funder Type');
            return;
        }

        if (that.selectedFunderPlan === undefined) {
            that.imfsUtilityService.showToastr('error', 'Funder Plan', 'Please select Funder Plan');
            return;
        }

        that.quoteTotalRateInputModel = new QuoteTotalRateInputModel();

        that.quoteTotalRateInputModel.FunderId = that.selectedFunder;
        that.quoteTotalRateInputModel.FinanceType = that.selectedFinanceType;
        that.quoteTotalRateInputModel.FunderPlan = that.selectedFunderPlan;

        this.imfsUtilityService.showLoading('Downloading');

        this.quoteTotalRateControllerService.exportQuoteTotalRates(that.quoteTotalRateInputModel).subscribe(
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

    uploadQuoteTotalRates() {
        this.uploadModal.open();
    }

}
