import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import * as _ from 'lodash-es';
import { HttpResponseData } from 'src/app/models/utility-models/response.model';
import { SortEvent, ConfirmationService, ConfirmEventType, MessageService } from 'primeng/api';
import { QuoteTotalRateModel } from 'src/app/models/quote-Total-rate/quote-total-rate.model';
import { QuoteTotalRateResponseModel } from 'src/app/models/quote-Total-rate/quote-total-rate-response.model';
import { QuoteTotalRateControllerService } from 'src/app/services/controller-services/quote-total-rate-controller.service';
import { IMFSUtilityService } from 'src/app/services/utility-services/imfs-utility-services';


@Component({
    selector: 'app-quote-total-rate-details-modal',
    templateUrl: './quote-total-rate-details-modal.component.html',
})

export class QuoteTotalRateDetailsModalComponent implements OnInit {

    currentItem: QuoteTotalRateResponseModel = new QuoteTotalRateResponseModel();
    displayDialog = false;

    @Output() refreshEmit = new EventEmitter();

    constructor(
        private imfsUtilityService: IMFSUtilityService,
        private quoteTotalRateControllerService: QuoteTotalRateControllerService,
        private confirmationService: ConfirmationService,
    ) { }

    ngOnInit() {

    }

    open(editItem: QuoteTotalRateResponseModel) {
        this.displayDialog = true;
        this.currentItem = _.cloneDeep(editItem);
    }

    saveRate() {

        this.confirmationService.confirm({
            message: 'Are you sure that you want to save this rate?',
            accept: () => {

                this.imfsUtilityService.showLoading('Saving...');
                // tslint:disable-next-line: deprecation
                this.quoteTotalRateControllerService.saveQuoteTotalRates(this.currentItem).subscribe(
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
