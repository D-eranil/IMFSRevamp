import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import * as _ from 'lodash-es';
import { HttpResponseData } from 'src/app/models/utility-models/response.model';
import { SortEvent, ConfirmationService, ConfirmEventType, MessageService } from 'primeng/api';
import { IMFSUtilityService } from 'src/app/services/utility-services/imfs-utility-services';
import { QuotePercentRateResponseModel } from 'src/app/models/quote-percent-rate/quote-percent-rate-response.model';
import { QuotePercentRateControllerService } from 'src/app/services/controller-services/quote-percent-rate-controller.service';


@Component({
    selector: 'app-quote-percent-rate-details-modal',
    templateUrl: './quote-percent-rate-details-modal.component.html',
})

export class QuotePercentRateDetailsModalComponent implements OnInit {

    currentItem: QuotePercentRateResponseModel = new QuotePercentRateResponseModel();
    displayDialog = false;

    @Output() refreshEmit = new EventEmitter();

    constructor(
        private imfsUtilityService: IMFSUtilityService,
        private quotePercentRateControllerService: QuotePercentRateControllerService,
        private confirmationService: ConfirmationService,
    ) { }

    ngOnInit() {

    }

    open(editItem: QuotePercentRateResponseModel) {
        this.displayDialog = true;
        this.currentItem = _.cloneDeep(editItem);
    }

    saveRate() {

        this.confirmationService.confirm({
            message: 'Are you sure that you want to save this rate?',
            accept: () => {

                this.imfsUtilityService.showLoading('Saving...');
                // tslint:disable-next-line: deprecation
                this.quotePercentRateControllerService.saveQuotePercentRates(this.currentItem).subscribe(
                    (response: HttpResponseData) => {
                        this.imfsUtilityService.hideLoading();
                        if (response.status === 'Success') {
                            this.imfsUtilityService.showToastr('success', 'Save Successful', 'Quote Percent Rate information saved.');
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
