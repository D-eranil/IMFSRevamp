import { Component, EventEmitter, OnInit, Output, ViewChild } from '@angular/core';
import { Dropdown } from 'primeng/dropdown';
import { MessageService, SortEvent } from 'primeng/api';
import { FileUpload } from 'primeng/fileupload';
import { HttpClientModule } from '@angular/common/http';
import { QuotePercentRateControllerService } from 'src/app/services/controller-services/quote-percent-rate-controller.service';
import { FunderControllerService } from 'src/app/services/controller-services/funder-controller.service';

import { FinanceProductTypeControllerService } from 'src/app/services/controller-services/finance-product-type-controller.servcie';
import { FinanceTypeControllerService } from 'src/app/services/controller-services/finance-type-controller.service';
import { IMFSUtilityService } from 'src/app/services/utility-services/imfs-utility-services';
import { FunderModel } from 'src/app/models/funder/funder.model';

import { FinanceProductTypeModel } from 'src/app/models/finance-product-type/finance-product-type.model';
import { FinanceTypeModel } from 'src/app/models/finance-type/finance-type.model';
import { JsUtilityService } from 'src/app/services/utility-services/js-utility.service';
import { FunderPlanModel } from 'src/app/models/funder-plan/funder-plan.model';
import { FunderPlanControllerService } from 'src/app/services/controller-services/funder-plan-controller.service';

@Component({
    selector: 'app-quote-percent-rate-upload-modal',
    templateUrl: './quote-percent-rate-upload-modal.component.html',
})
export class QuotePercentRateUploadModalComponent implements OnInit {
    displayDialog = false;
    funderPlans: FunderPlanModel[];
    selectedFunderPlan: number;

    @Output() refreshEmit = new EventEmitter();

    productTypes: FinanceProductTypeModel[]; // Device, Software, Service etc
    selectedProductType: number;

    financeTypes: FinanceTypeModel[]; // Lease, rental, instalment etc
    selectedFinanceType: number;

    funders: FunderModel[];
    selectedFunder: number;

    filesToUpload: File[] = [];
    fileNamesToUpload: string;

    @ViewChild('fileUpload') fileUpload: FileUpload;

    constructor(
        private imfsUtilityService: IMFSUtilityService,
        private jsUtilityService: JsUtilityService,
        private quotePercentRateControllerService: QuotePercentRateControllerService,
        private funderControllerService: FunderControllerService,
        private financeProductTypeControllerService: FinanceProductTypeControllerService,
        private financeTypeControllerService: FinanceTypeControllerService,
        private funderPlanControllerService: FunderPlanControllerService,
        private messageService: MessageService,
    ) {

        this.loadFunders();
        this.loadFinanceTypes();
        this.loadFinanceProductTypes();
        this.loadFunderPlans();
    }

    ngOnInit() {


    }

    open() {
        this.displayDialog = true;
        this.filesToUpload = [];
        this.fileNamesToUpload = '';
        this.fileUpload.clear();   // clear all existing files
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

    onSelect(event: any) {

        const fileNames: string[] = [];

        for (const file of event.files) {
            this.filesToUpload.push(new File([file], file.name));
            fileNames.push(file.name);
        }

        this.fileNamesToUpload = fileNames.join('\n').toString();
    }

    uploadRates() {
        if (!this.filesToUpload) {
            return;
        }
        const that = this;
        const formData: FormData = new FormData();

        if (that.selectedFunder === undefined || that.selectedFunder === null) {
            that.imfsUtilityService.showToastr('error', 'Funder Type', 'Please select Funder Type');
            return;
        }
        else {
            formData.append('Funder', that.selectedFunder.toString());
        }

        if (that.selectedProductType === undefined || that.selectedProductType === null) {
            that.imfsUtilityService.showToastr('error', 'Finance Product Type', 'Please select Finance Product Type');
            return;
        }
        else {
            formData.append('ProductType', that.selectedProductType.toString());
        }

        if (that.selectedFinanceType === undefined || that.selectedFinanceType === null) {
            that.imfsUtilityService.showToastr('error', 'Finance Type', 'Please select Finance Type');
            return;
        }
        else {
            formData.append('FinanceType', that.selectedFinanceType.toString());
        }

        if (that.selectedFunderPlan === undefined || that.selectedFunderPlan === null) {
            that.imfsUtilityService.showToastr('error', 'Funder Plan', 'Please select Funder Plan');
            return;
        }
        else {
            formData.append('FunderPlan', that.selectedFunderPlan.toString());
        }

        for (const file of this.filesToUpload) {
            formData.append('Files', file, file.name);
        }

        this.imfsUtilityService.showLoading('Uploading Quote Percent Rates, please wait it might take a while...');
        this.quotePercentRateControllerService.uploadQuotePercentRates(formData).subscribe(
            (response: any) => {
                this.filesToUpload = [];
                this.fileNamesToUpload = '';
                this.fileUpload.clear();   // clear all existing files
                this.imfsUtilityService.hideLoading();
                if (response.status === 'Success') {
                    this.imfsUtilityService.showToastr('success', 'Upload Successful', 'Successfully uploaded file.');
                    this.displayDialog = false;
                } else {
                    this.imfsUtilityService.showDialog('Error in Upload Quote Percent Rate', response.message);
                    // this.imfsUtilityService.showToastr('error', 'Error', response.message);
                    console.log('Error in Upload Quote Percent Rate: ' + response.message);
                }
            },
            (err: any) => {
                this.imfsUtilityService.hideLoading();
                console.log(err);
                this.imfsUtilityService.showToastr('error', 'Error', 'Unable to upload file.');
            }
        );
    }
}
