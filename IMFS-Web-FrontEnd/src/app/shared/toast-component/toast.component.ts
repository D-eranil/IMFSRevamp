import { Component, OnInit } from '@angular/core';
import { MessageService } from 'primeng/api';
import { IMFSUtilityService } from '../../services/utility-services/imfs-utility-services';

@Component({
  selector: 'app-toast',
  templateUrl: './toast.component.html',
})
export class ToastComponent implements OnInit {
  dialogTitle = '';
  dialogContentHTML = '';
  dialogDisplay = false;

  confirmTitle = '';
  confirmContentHTML = '';

  constructor(
    private messageService: MessageService,
    private imfsUtilityService: IMFSUtilityService
  ) {
    this.imfsUtilityService.displayDialogService.subscribe((input: any) => {
      this.displayDialog(input.title, input.contentHTML);
    });
  }

  ngOnInit() { }

  displayDialog(title: string, contentHTML: string) {
    this.dialogTitle = title;
    this.dialogContentHTML = contentHTML;
    this.dialogDisplay = true;
  }

  close() {
    this.messageService.clear();
  }
}
