import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { Dropdown } from 'primeng/dropdown';
import { MessageService, SortEvent } from 'primeng/api';
import { FileUpload } from 'primeng/fileupload';
import { HttpClientModule, HttpResponse } from '@angular/common/http';
import { IMFSUtilityService } from 'src/app/services/utility-services/imfs-utility-services';
import { JsUtilityService } from 'src/app/services/utility-services/js-utility.service';
import { EmailControllerService } from 'src/app/services/controller-services/email-controller.service';
import { AttachmentControllerService } from 'src/app/services/controller-services/attachment-controller.service';
import { EmailAttachment } from 'src/app/models/email/email.model';
import { HttpResponseData } from 'src/app/models/utility-models/response.model';
import { QuoteControllerService } from 'src/app/services/controller-services/quote-controller.service';
import { QuoteDocuments } from 'src/app/models/quote/quote.model';
import { ApplicationControllerService } from 'src/app/services/controller-services/application-controller.service';

@Component({
    selector: 'app-email-upload-modal',
    templateUrl: './email-upload-modal.component.html',
})
export class EmailUploadModalComponent implements OnInit {
    displayDialog = false;
    tempEmailId: string;
    @Output() refreshAttachmentFilesEmit = new EventEmitter();
    @Input() quoteID: string;
    @Input() applicationID: string;
    @Input() applicationNumber: string;

    filesToUpload: File[] = [];
    fileNamesToUpload: string;

    @Output() addAttachmentToEmailEmitter = new EventEmitter();

    filesUploaded: EmailAttachment[] = [];

    @ViewChild('fileUpload') fileUpload: FileUpload;
  filesLists: QuoteDocuments [] = [];

    constructor(
        private imfsUtilityService: IMFSUtilityService,
        private jsUtilityService: JsUtilityService,
        private quoteControllerService: QuoteControllerService,
        private applicationControllerService: ApplicationControllerService,
        private emailControllerService: EmailControllerService,
        private attachmentControllerService: AttachmentControllerService,
        private messageService: MessageService,
    ) {
    }

    ngOnInit() {
      if(this.quoteID && !this.applicationID)
      this.getQuoteFiles();

      if(!this.quoteID && this.applicationID)
      this.getApplicationFiles();

    }

    getQuoteFiles() {
      let getQuoteId = Number(this.quoteID);
      if (getQuoteId && getQuoteId > 0) {
          this.quoteControllerService.getQuoteFileList(getQuoteId).subscribe((response: QuoteDocuments[]) => {
              this.filesLists = response;
              console.log(this.filesLists)
          });
      }
  }

    getApplicationFiles() {
      let getApplicationID = Number(this.applicationID);
      let getApplicationNumber = Number(this.applicationNumber);

      // if (getApplicationID && getApplicationID > 0) {
      if (getApplicationNumber && getApplicationNumber > 0) {
          this.applicationControllerService.getApplicationFileList(getApplicationNumber).subscribe((response: QuoteDocuments[]) => {
              this.filesLists = response;
              console.log(this.filesLists)
          });
      }
    }

    open(tempEmailId: string) {
        this.tempEmailId = tempEmailId;
        this.displayDialog = true;
        this.filesToUpload = [];
        this.fileNamesToUpload = '';
        this.fileUpload.clear();   // clear all existing files
    }

    addAttachmentToEmail(event: any) {
      this.addAttachmentToEmailEmitter.emit({id: event.id, fileName: event.fileName});
    }

    onSelect(event: any) {

        const fileNames: string[] = [];
        this.filesToUpload = [];
        for (const file of event.files) {
            this.filesToUpload.push(new File([file], file.name));
            fileNames.push(file.name);
        }

        this.fileNamesToUpload = fileNames.join('\n').toString();


        if (!this.filesToUpload) {
            return;
        }

        const formData: FormData = new FormData();
        formData.append('tempEmailId', this.tempEmailId);

        for (const file of this.filesToUpload) {
            formData.append('Files', file, file.name);
        }

        this.imfsUtilityService.showLoading('Saving temp file...');
        this.attachmentControllerService.saveEmailAttachmentTempFile(formData).subscribe(
            (response: EmailAttachment[]) => {

                this.imfsUtilityService.hideLoading();

                for (const emailAttachment of response) {
                    this.filesUploaded.push(emailAttachment);
                }

                response.forEach(
                  ele => {
                    this.addAttachmentToEmail({id: ele.id, fileName: ele.fileName});
                  }
                )


                this.refreshAttachmentFilesEmit.emit();
            },
            (err: any) => {
                this.imfsUtilityService.hideLoading();
                console.log(err);
                this.imfsUtilityService.showToastr('error', 'Error', 'Unable to upload file.');
            }
        );

    }

    onRemove(event: any) {
        const formData: FormData = new FormData();
        formData.append('tempEmailId', this.tempEmailId);

        const file = event.file;
        const fileToDelete = this.filesUploaded.find(x => x.fileName === file.name);

        this.imfsUtilityService.showLoading('Deleting temp file...');
        this.attachmentControllerService.removeEmailAttachmentTempFile(fileToDelete?.id).subscribe(
            (response: HttpResponseData) => {
                this.addAttachmentToEmail({id: fileToDelete?.id, fileName: fileToDelete?.fileName});

                // remove the file from local array too, when it's removed from server too
                const indexOfDeletedFile = this.filesUploaded.findIndex(x => x.id === file.id);
                this.filesUploaded.splice(indexOfDeletedFile,1);

                this.imfsUtilityService.hideLoading();
                this.refreshAttachmentFilesEmit.emit();
            },
            (err: any) => {
                this.imfsUtilityService.hideLoading();
                console.log(err);
                this.imfsUtilityService.showToastr('error', 'Error', 'Unable to delete file.');
            }
        );
    }

    uploadFiles() {

    }

    closeDialog() {

    }
}
