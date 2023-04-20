import { Component, EventEmitter, OnInit, Output, ViewChild } from '@angular/core';
import { Location } from '@angular/common';
import { Dropdown } from 'primeng/dropdown';
import { MessageService, SortEvent } from 'primeng/api';
import { FileUpload } from 'primeng/fileupload';
import { HttpClientModule, HttpResponse } from '@angular/common/http';
import { IMFSUtilityService } from 'src/app/services/utility-services/imfs-utility-services';
import { JsUtilityService } from 'src/app/services/utility-services/js-utility.service';
import { EmailControllerService } from 'src/app/services/controller-services/email-controller.service';
import { AttachmentControllerService } from 'src/app/services/controller-services/attachment-controller.service';
import { EmailAttachment, EmailHistory } from 'src/app/models/email/email.model';
import { HttpResponseData } from 'src/app/models/utility-models/response.model';
import { ActivatedRoute, Router } from '@angular/router';
import { IMFSRoutes } from 'src/app/models/routes/imfs-routes';
import { isNumber } from 'lodash';



@Component({
    selector: 'app-quote-email-history-modal',
    templateUrl: './quote-email-history-modal.component.html',
})


export class QuoteEmailHistoryModalComponent implements OnInit {
    displayDialog = false;
    @Output() refreshAttachmentFilesEmit = new EventEmitter();
    emailHistories: EmailHistory[] = [];
    quoteNumber: number;


    constructor(
        private imfsUtilityService: IMFSUtilityService,
        private jsUtilityService: JsUtilityService,
        private emailControllerService: EmailControllerService,
        private attachmentControllerService: AttachmentControllerService,
        private messageService: MessageService,
        private router: Router,
        private route: ActivatedRoute,
        private location: Location
    ) {

    }

    ngOnInit() {

    }

    open(quoteNumber: number) {
        this.quoteNumber = quoteNumber;
        this.displayDialog = true;
        this.loadEmailHistory(this.quoteNumber);
    }

    viewEmail(emailId: number) {
        void this.router.navigate([IMFSRoutes.Email], { queryParams: { emailId } });
    }

    closeDialog() {

    }

    loadEmailHistory(quoteNumber: number) {

        if(!isNaN(quoteNumber)) {
          this.imfsUtilityService.showLoading('loading email...');
        this.emailControllerService.getEmailHistory(Number(quoteNumber)).subscribe(
            (response: EmailHistory[]) => {
                this.emailHistories = response;
                this.imfsUtilityService.hideLoading();
            },
            (err: any) => {
                this.imfsUtilityService.showToastr('error', 'Error', 'Error loading email history');
                this.imfsUtilityService.hideLoading();
            }
        );
        }
    }
}
