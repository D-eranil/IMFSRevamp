import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import * as _ from 'lodash-es';
import { HttpResponseData } from 'src/app/models/utility-models/response.model';
import { Dropdown } from 'primeng/dropdown';
import { SortEvent, ConfirmationService, ConfirmEventType, MessageService } from 'primeng/api';
import { QuoteRateModel } from 'src/app/models/quote-rate/quote-rate.model';
import { QuoteRateResponseModel } from 'src/app/models/quote-rate/quote-rate-response.model';
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
import { ConfirmDialogModule } from 'primeng/confirmdialog';


@Component({
  selector: 'app-quote-rate-details-modal',
  templateUrl: './quote-rate-details-modal.component.html',
})

export class QuoteRateDetailsModalComponent implements OnInit {

  currentItem: QuoteRateResponseModel = new QuoteRateResponseModel();
  displayDialog = false;

  @Output() refreshEmit = new EventEmitter();

  constructor(
    private imfsUtilityService: IMFSUtilityService,
    private financeTypeControllerService: FinanceTypeControllerService,
    private quoteRateControllerService: QuoteRateControllerService,
    private confirmationService: ConfirmationService,
  ) { }

  ngOnInit() {

  }

  open(editItem: QuoteRateResponseModel) {
    this.displayDialog = true;
    this.currentItem = _.cloneDeep(editItem);
  }

  saveRate() {

    this.confirmationService.confirm({
      message: 'Are you sure that you want to save this rate?',
      accept: () => {
         // Actual logic to perform a confirmation
        // if (!this.currentItem.months12) {
        //   this.imfsUtilityService.showToastr('warning', 'Warning', 'Please enter 12 months rate');
        //   return;
        // }

        // if (!this.currentItem.months24) {
        //   this.imfsUtilityService.showToastr('warning', 'Warning', 'Please enter 24 months rate');
        //   return;
        // }

        // if (!this.currentItem.months36) {
        //   this.imfsUtilityService.showToastr('warning', 'Warning', 'Please enter 36 months rate');
        //   return;
        // }

        // if (!this.currentItem.months48) {
        //   this.imfsUtilityService.showToastr('warning', 'Warning', 'Please enter 48 months rate');
        //   return;
        // }

        // if (!this.currentItem.months60) {
        //   this.imfsUtilityService.showToastr('warning', 'Warning', 'Please enter 60 months rate');
        //   return;
        // }

        this.imfsUtilityService.showLoading('Saving...');
        // tslint:disable-next-line: deprecation
        this.quoteRateControllerService.saveRates(this.currentItem).subscribe(
          (response: HttpResponseData) => {
            this.imfsUtilityService.hideLoading();
            if (response.status === 'Success') {
              this.imfsUtilityService.showToastr('success', 'Save Successful', 'Quote Rate information saved.');
              this.displayDialog = false;
              this.refreshEmit.emit();
            } else {
              this.imfsUtilityService.showToastr('error', 'Error', response.message);
            }
          },
          (err: any) => {
            console.log(err);
            this.imfsUtilityService.hideLoading();
            this.imfsUtilityService.showToastr('error', 'Failed', 'Unable to save quote rate');
          }

        );

      }
    });

  }

}
