import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Component, ElementRef, EventEmitter, OnInit, Output, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { IMFSUtilityService } from 'src/app/services/utility-services/imfs-utility-services';
import { JsUtilityService } from 'src/app/services/utility-services/js-utility.service';
import { environment } from 'src/environments/environment';
@Component({
  selector: 'app-application-doc-upload-popup',
  templateUrl: './application-doc-upload-popup.component.html',
  styleUrls: ['./application-doc-upload-popup.component.scss']
})
export class ApplicationDocUploadPopupComponent implements OnInit {
  fileAdded: any[] = []
  name: any
  fileNames: any = []
  applicationIdGlobal: number | any;
  applicationNumber : any;
  active: boolean = true;
  fileDescription: any = 'Files uploaded'
  fileList: any = []
  Source: any
  constructor(
    private http: HttpClient,
    private route: ActivatedRoute, private imfsUtilityService: IMFSUtilityService,
    private jsUtilityService: JsUtilityService
  ) { }

  ngOnInit(): void {
    this.route.queryParamMap.subscribe((queryParams) => {
      this.applicationIdGlobal = queryParams.get("id");
      this.applicationNumber = queryParams.get("appNo");
    })
  }
  @Output() refreshAttachmentFilesEmit = new EventEmitter();
  @Output() onDialogClose: EventEmitter<any> = new EventEmitter();
  closeDocumentPopupDialog() {
    this.onDialogClose.emit();
    this.fileAdded = [];
  }
  @ViewChild("fileDropRef", { static: false }) fileDropEl: ElementRef;
  getFile(event: any) {
    if (event.target.files.length > 12) {
      alert('Max file 12')
      return
    }
    for (var i = 0; i < event.target.files.length; i++) {
      if (['application/vnd.openxmlformats-officedocument.wordprocessingml.document', 'application/msword', 'application/pdf', 'image/jpg', 'image/jpeg', 'image/png', 'application/vnd.ms-excel', 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet', 'application/vnd.ms-excel', 'text/csv'].includes(event.target.files[i].type)) {
        this.fileAdded.push(<File>event.target.files[i]);
        this.fileNames.push(event.target.files[i].name)
      } else {
        this.imfsUtilityService.showToastr('error', 'Failed', 'File format not supported')
        return;
      }
    }
  }
  onDelete(index: number) {
    this.fileAdded.splice(index, 1);
  }
  onFileAttach() {
    this.Source = 2
    let formData = new FormData()
    formData.append('Source', this.Source)
    formData.append("Id", this.applicationIdGlobal)
    formData.append("ApplicationNumber", this.applicationNumber)
    formData.append("Description", this.fileDescription)
    for (let i = 0; i < this.fileAdded.length; i++) {
      formData.append("files", this.fileAdded[i]);
    }
    if (this.fileAdded.length > 0) {
      const headers = new HttpHeaders();
      headers.set('Content-Type', 'multipart/form-data')
      console.log(formData);
      this.http.post(environment.API_BASE + '/Quote/UploadQuoteAttachments', formData, { headers: headers }).subscribe((res) => {
        console.log(res);
        this.refreshAttachmentFilesEmit.emit();
        this.fileAdded = [];
      },
        (err: any) => {
          console.log(err);
          this.imfsUtilityService.hideLoading();
          this.imfsUtilityService.showToastr('error', 'Failed', 'Error in uploading file');
          this.fileAdded = [];
        })
    } else {
      this.imfsUtilityService.showToastr('error', 'Failed', 'Please attach a file');
    }

  }


  /**
  * format bytes
  * @param bytes (File size in bytes)
  * @param decimals (Decimals point)
  */
  formatBytes(bytes: number, decimals = 2) {
    if (bytes === 0) {
      return "0 Bytes";
    }
    const k = 1024;
    const dm = decimals <= 0 ? 0 : decimals;
    const sizes = ["Bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB"];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return parseFloat((bytes / Math.pow(k, i)).toFixed(dm)) + " " + sizes[i];
  }

}
