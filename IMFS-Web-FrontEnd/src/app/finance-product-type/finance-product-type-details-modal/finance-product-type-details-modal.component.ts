import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import * as _ from 'lodash-es';
import { FinanceProductTypeModel } from 'src/app/models/finance-product-type/finance-product-type.model';
import { HttpResponseData } from 'src/app/models/utility-models/response.model';
import { FinanceProductTypeControllerService } from 'src/app/services/controller-services/finance-product-type-controller.servcie';
import { IMFSUtilityService } from 'src/app/services/utility-services/imfs-utility-services';


@Component({
  selector: 'app-finance-product-type-details-modal',
  templateUrl: './finance-product-type-details-modal.component.html',
})

export class FinanceProductTypeDetailsModalComponent implements OnInit {

  currentItem: FinanceProductTypeModel = new FinanceProductTypeModel();
  displayDialog = false;
  header = 'Finance Product Type';

  @Output() refreshEmit = new EventEmitter();

  constructor(
    private imfsUtilityService: IMFSUtilityService,
    private financeProductTypeControllerService: FinanceProductTypeControllerService
  ) { }

  ngOnInit() {

  }

  open(editItem: FinanceProductTypeModel) {
    this.displayDialog = true;
    this.currentItem = _.cloneDeep(editItem);
  }

  saveFinanceProductType() {

    if (!this.currentItem.code) {
      this.imfsUtilityService.showToastr('warning', 'Warning', 'Please enter Code');
      return;
    }

    if (!this.currentItem.description) {
      this.imfsUtilityService.showToastr('warning', 'Warning', 'Please enter Description for Finance Product Type');
      return;
    }

    this.imfsUtilityService.showLoading('Saving...');
    this.financeProductTypeControllerService.saveFinanceProductTypes(this.currentItem).subscribe(
      (response: HttpResponseData) => {
        this.imfsUtilityService.hideLoading();
        if (response.status === 'Success') {
          this.imfsUtilityService.showToastr('success', 'Save Successful', 'Finance Product Type information saved.');
          this.displayDialog = false;
          this.refreshEmit.emit();
        } else {
          this.imfsUtilityService.showToastr('error', 'Error', response.message);
        }
      },
      (err: any) => {
        console.log(err);
        this.imfsUtilityService.hideLoading();
        this.imfsUtilityService.showToastr('error', 'Failed', 'Unable to save finance Product type');
      }

    );
  }

}
