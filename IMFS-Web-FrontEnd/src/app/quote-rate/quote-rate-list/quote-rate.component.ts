import { Component, OnInit, ViewChild } from '@angular/core';
import { Dropdown } from 'primeng/dropdown';
import { MessageService, SortEvent } from 'primeng/api';
import { FileUpload } from 'primeng/fileupload';

import { HttpClientModule } from '@angular/common/http';
import { QuoteRateModel } from 'src/app/models/quote-rate/quote-rate.model';
import { QuoteRateResponseModel } from 'src/app/models/quote-rate/quote-rate-response.model';
import { QuoteRateInputModel } from 'src/app/models/quote-rate/quote-rate-input.model';
import { QuoteRateControllerService } from 'src/app/services/controller-services/quote-rate-controller.service';
import { FunderControllerService } from 'src/app/services/controller-services/funder-controller.service';
import { VendorControllerService } from 'src/app/services/controller-services/vendor-controller.service';
import { FinanceProductTypeControllerService } from 'src/app/services/controller-services/finance-product-type-controller.servcie';
import { FinanceTypeControllerService } from 'src/app/services/controller-services/finance-type-controller.service';
import { IMFSUtilityService } from 'src/app/services/utility-services/imfs-utility-services';
import { FunderModel } from 'src/app/models/funder/funder.model';
import { VendorModel } from 'src/app/models/vendor/vendor.model';
import { FinanceProductTypeModel } from 'src/app/models/finance-product-type/finance-product-type.model';
import { FinanceTypeModel } from 'src/app/models/finance-type/finance-type.model';
import { QuoteRateDetailsModalComponent } from '../quote-rate-details-modal/quote-rate-details-modal.component';
import { JsUtilityService } from 'src/app/services/utility-services/js-utility.service';
import { QuoteRateUploadModalComponent } from '../quote-rate-upload-modal/quote-rate-upload-modal.component';
import { ComponentVisibilityService } from 'src/app/services/utility-services/component-visibility.service';

@Component({
  selector: 'app-quote-rate',
  templateUrl: './quote-rate.component.html',
  styleUrls: ['./quote-rate.component.scss']
})

export class QuoteRateComponent implements OnInit {

  rates: QuoteRateModel[];

  productTypes: FinanceProductTypeModel[]; // Device, Software, Service etc
  selectedProductType: number;

  financeTypes: FinanceTypeModel[]; // Lease, rental, instalment etc
  selectedFinanceType: number;

  funders: FunderModel[];
  selectedFunder: number;

  vendors: VendorModel[];
  selectedVendor: number;

  quoteRateInputModel: QuoteRateInputModel;


  @ViewChild('fileUpload') fileUpload: FileUpload;
  @ViewChild('uploadModal') uploadModal: QuoteRateUploadModalComponent;
  @ViewChild('detailsModal') detailsModal: QuoteRateDetailsModalComponent;

  constructor(
    private imfsUtilityService: IMFSUtilityService,
    private jsUtilityService: JsUtilityService,
    private quoteRateControllerService: QuoteRateControllerService,
    private funderControllerService: FunderControllerService,
    private vendorControllerService: VendorControllerService,
    private financeProductTypeControllerService: FinanceProductTypeControllerService,
    private financeTypeControllerService: FinanceTypeControllerService,
    private messageService: MessageService,
    private componentVisibilityService: ComponentVisibilityService
  ) {

    this.loadFunders();
    this.loadFinanceProductTypes();
    this.loadFinanceTypes();
    this.loadVendors();

  }

  ngOnInit() {
    // make full screen
    this.componentVisibilityService.fullScreenFire(true);
  }

  loadRates() {
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

    console.log('Product Selected:' + that.selectedProductType);
    console.log('Finance Type Selected:' + that.selectedFinanceType);
    console.log('Funder Selected:' + that.selectedFunder);
    console.log('Vendor Selected:' + that.selectedVendor);

    that.quoteRateInputModel = new QuoteRateInputModel();

    that.quoteRateInputModel.FunderId = that.selectedFunder;
    that.quoteRateInputModel.ProductType = that.selectedProductType;
    that.quoteRateInputModel.FinanceType = that.selectedFinanceType;
    that.quoteRateInputModel.VendorId = that.selectedVendor;


    that.imfsUtilityService.showLoading('Loading Rates');

    that.quoteRateControllerService.getQuoteRates(that.quoteRateInputModel).subscribe(
      (response: QuoteRateModel[]) => {
        that.imfsUtilityService.hideLoading();

        that.rates = response;

      },
      (err: any) => {
        console.log(err);
        that.imfsUtilityService.hideLoading();
        that.imfsUtilityService.showToastr('error', 'Failed', 'Error loading rates');
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


  loadVendors() {
    const that = this;

    // tslint:disable-next-line: deprecation
    that.vendorControllerService.getVendors().subscribe(
      (vendorsResponse: VendorModel[]) => {
        that.vendors = vendorsResponse;
      },
      (err: any) => {
        console.log(err);
        that.imfsUtilityService.showToastr('error', 'Failed', 'Error loading vendors');
      }

    );

  }

  editQuoteRate(editItem: QuoteRateResponseModel) {
    this.detailsModal.open(editItem);
  }

  exportRates() {

    const that = this;
    if (that.selectedFunder === undefined) {
      that.imfsUtilityService.showToastr('error', 'Funder Type', 'Please select Funder Type');
      return;
    }

    that.quoteRateInputModel = new QuoteRateInputModel();

    that.quoteRateInputModel.FunderId = that.selectedFunder;
    that.quoteRateInputModel.ProductType = that.selectedProductType;
    that.quoteRateInputModel.FinanceType = that.selectedFinanceType;
    that.quoteRateInputModel.VendorId = that.selectedVendor;

    this.imfsUtilityService.showLoading('Downloading');
    // tslint:disable-next-line: deprecation
    this.quoteRateControllerService.exportQuoteRates(that.quoteRateInputModel).subscribe(
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

  uploadRates()
  {
    this.uploadModal.open();
  }

}
