import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import * as _ from 'lodash-es';
import { DurationOptions, FinanceTypes, PaymentFrequencyOptions, QuoteStatusOptions } from 'src/app/models/drop-down-options/drop-down-options.model';
import { UserResponseModel, UserUpdateModel } from 'src/app/models/user/user.model';
import { HttpResponseData } from 'src/app/models/utility-models/response.model';
import { UserControllerService } from 'src/app/services/controller-services/user-controller.service';
import { IMFSUtilityService } from 'src/app/services/utility-services/imfs-utility-services';


@Component({
  selector: 'app-user-details-modal',
  templateUrl: './user-details-modal.component.html',
})

export class UserDetailsModalComponent implements OnInit {

  currentItem: UserResponseModel = new UserResponseModel();
  displayDialog = false;
  header = 'Edit User Info';
  durationOptions = DurationOptions;
  paymentFrequencyOptions = PaymentFrequencyOptions;
  financeTypeOptions = FinanceTypes;

  // selectedDuration: number;
  // selectedPaymentFrequency: string;
  // selectedFinanceType: number;

  @Output() refreshEmit = new EventEmitter();

  constructor(
    private imfsUtilityService: IMFSUtilityService,
    private userControllerService: UserControllerService
  ) { }

  ngOnInit() {

  }

  open(editItem: UserResponseModel) {
    this.displayDialog = true;
    this.currentItem = _.cloneDeep(editItem);
    // this.selectedDuration = Number(this.currentItem.defaultFinanceDurationName);
    // this.selectedPaymentFrequency = this.currentItem.defaultFinanceFrequency;
    // this.selectedFinanceType = Number(this.currentItem.defaultFinanceType);
  }

  saveUser() {

    // error check here
    // make sure all fields has values
    // if not show error and return;

    if (!this.currentItem.firstName) {
      this.imfsUtilityService.showToastr('warning', 'Warning', 'Please enter First Name');
      return;
    }

    if (!this.currentItem.lastName) {
      this.imfsUtilityService.showToastr('warning', 'Warning', 'Please enter last name');
      return;
    }

    const updateModel = new UserUpdateModel();


    this.imfsUtilityService.showLoading('Saving...');
    // tslint:disable-next-line: deprecation
    this.userControllerService.saveUser(this.currentItem).subscribe(
      (response: HttpResponseData) => {
        this.imfsUtilityService.hideLoading();
        if (response.status === 'Success') {
          this.imfsUtilityService.showToastr('success', 'Save Successful', 'User information saved.');
          this.displayDialog = false;
          this.refreshEmit.emit();
        } else {
          this.imfsUtilityService.showToastr('error', 'Error', response.message);
        }
      },
      (err: any) => {
        console.log(err);
        this.imfsUtilityService.hideLoading();
        this.imfsUtilityService.showToastr('error', 'Failed', 'Unable to save user');
      }

    );
  }

}
