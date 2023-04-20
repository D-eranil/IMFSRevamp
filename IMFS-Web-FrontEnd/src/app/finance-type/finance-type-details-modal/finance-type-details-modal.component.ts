import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import * as _ from 'lodash-es';
import { FinanceTypeModel } from 'src/app/models/finance-type/finance-type.model';
import { HttpResponseData } from 'src/app/models/utility-models/response.model';
import { FinanceTypeControllerService } from 'src/app/services/controller-services/finance-type-controller.service';
import { IMFSUtilityService } from 'src/app/services/utility-services/imfs-utility-services';


@Component({
  selector: 'app-finance-type-details-modal',
  templateUrl: './finance-type-details-modal.component.html',
})

export class FinanceTypeDetailsModalComponent implements OnInit {

  currentItem: FinanceTypeModel = new FinanceTypeModel();
  displayDialog = false;
  header = 'Finance Type';

  @Output() refreshEmit = new EventEmitter();

  constructor(
    private imfsUtilityService: IMFSUtilityService,
    private financeTypeControllerService: FinanceTypeControllerService
  ) { }

  ngOnInit() {

  }

  open(editItem: FinanceTypeModel) {
    this.displayDialog = true;
    this.currentItem = _.cloneDeep(editItem);
  }

  saveFinanceType() {

    // error check here
    // make sure all fields has values
    // if not show error and return;

    if (!this.currentItem.quoteDurationType) {
      this.imfsUtilityService.showToastr('warning', 'Warning', 'Please enter Quote Duration Type');
      return;
    }

    if (!this.currentItem.description) {
      this.imfsUtilityService.showToastr('warning', 'Warning', 'Please enter Description for Quote Duration Type');
      return;
    }

    this.imfsUtilityService.showLoading('Saving...');
    // tslint:disable-next-line: deprecation
    this.financeTypeControllerService.saveFinanceTypes(this.currentItem).subscribe(
      (response: HttpResponseData) => {
        this.imfsUtilityService.hideLoading();
        if (response.status === 'Success') {
          this.imfsUtilityService.showToastr('success', 'Save Successful', 'Finance Type information saved.');
          this.displayDialog = false;
          this.refreshEmit.emit();
        } else {
          this.imfsUtilityService.showToastr('error', 'Error', response.message);
        }
      },
      (err: any) => {
        console.log(err);
        this.imfsUtilityService.hideLoading();
        this.imfsUtilityService.showToastr('error', 'Failed', 'Unable to save finance type');
      }

    );
  }

}
