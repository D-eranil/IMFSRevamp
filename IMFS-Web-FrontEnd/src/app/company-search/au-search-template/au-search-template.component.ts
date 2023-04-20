import { Component, OnInit, Output,Input, EventEmitter } from '@angular/core';
import { SearchService } from "../../services/controller-services/searchservice";
import { FormBuilder, FormGroup, Validators, FormControl } from '@angular/forms';
import { IMFSFormService } from 'src/app/services/utility-services/imfs-form-service';
import {AbnDetailsResponseModel, CompanySearchResponseModel} from '../../models/companySearchModal/companySearchModal';
import { IMFSUtilityService } from 'src/app/services/utility-services/imfs-utility-services';
import { JsUtilityService } from 'src/app/services/utility-services/js-utility.service';
@Component({
  selector: 'app-au-search-template',
  templateUrl: './au-search-template.component.html',
  styleUrls: ['./au-search-template.component.scss'],
  providers: [SearchService]
})
export class AuSearchTemplateComponent implements OnInit {
  submitted = false;
  searchPopupform: FormGroup;

  saveTempEndUserName: { name : string };
  saveTempEndUserABN: { abn : string };
  @Input() type:any = true;

  cnlastCall = 0;
  abnlastCall = 0;
  abndelay = 500;
  cndelay = 500;

  @Input() set displayEndUserDialog(value: boolean) {
    if(value) {
      if (this.cnlastCall >= (Date.now() - this.cndelay))
      return;
      this.cnlastCall = Date.now();
      this.searchPopupform.get('searchcompanyname')?.patchValue(this.saveTempEndUserName, {emitEvent: true});
      this.filterCompany({ query : this.saveTempEndUserName?.name});
    }
  }

  @Input() set displayAbnNumberDialog(value: boolean) {
    if(value) {
      if (this.abnlastCall >= (Date.now() - this.abndelay))
      return;
      this.abnlastCall = Date.now();
      this.searchPopupform.get('searchabn')?.patchValue(this.saveTempEndUserABN, {emitEvent: true});
      this.filterCompany({ query : this.saveTempEndUserABN?.abn });
    }
  }



  @Input() set endUserName(value: string) {
    if(this.searchPopupform) {
      this.searchPopupform.get('searchcompanyname')?.patchValue({name: value}, {emitEvent: true});
      this.filterCompany({ query : value});
      this.saveTempEndUserName = {name: value};
    }
  }

  // get endUserName(): string {
  //     return this.searchPopupform.get('searchcompanyname')?.value;
  // }


  @Input() set endUserABN(value: string) {
    if(this.searchPopupform) {
    this.searchPopupform.get('searchabn')?.patchValue({abn: value}, {emitEvent: true});
    this.filterabn({ query : value});
    this.saveTempEndUserABN = { abn : value };
  }
  }

  // get endUserABN(): string {
  //     return this.selectedCompanyName;
  // }

  companyName: CompanySearchResponseModel[];

  filteredCompanyName: any[];

  selectedCompany: any[];

  selectedAbn: any ='';

  abnNumber:AbnDetailsResponseModel;

  filteredAbn: any[];

  selectedAbnSearch: any[];
  number="";
  selectedCompanyName: string = '';
  constructor(private searchService: SearchService, private searchPopForm: FormBuilder, private formUtility: IMFSFormService, private imfsUtilityService: IMFSUtilityService,) {}

  ngOnInit(): void {
    this.searchPopupform = this.searchPopForm.group({
      searchcompanyname: new FormControl('', Validators.required),
      searchabn: new FormControl('',[Validators.required])
  });
  }
  numberOnly(event:any): boolean {
    const charCode = (event.which) ? event.which : event.keyCode;
    if (charCode > 31 && (charCode < 48 || charCode > 57)) {
      return false;
    }
    return true;
  }
  filterCompany(event:any) {
    this.searchService.getBusinessName(event.query).subscribe(companyName => {
      this.companyName = companyName.names;
      console.log(this.companyName)
    this.filteredCompanyName = companyName.names;
    console.log(this.filteredCompanyName);
    },
    (err: any) => {
      console.log(err);
      this.imfsUtilityService.hideLoading();
      this.imfsUtilityService.showToastr('error', 'Failed', 'ABR Search currently unavailable');
      if(err.status == 500){
      this.imfsUtilityService.hideLoading();
      this.imfsUtilityService.showToastr('error', 'Failed', 'ABR Search currently unavailable');
      }
    }
    );

  }
  filterabn(event:any) {
    this.searchService.getAbn(event.query).subscribe(abnNum => {
      this.abnNumber = abnNum;
      let filtered: any[] = [];
    let query = event.query;
    let abnSearch = this.abnNumber;
    if (abnSearch.abn.toLowerCase().indexOf(query.toLowerCase()) == 0) {
        filtered.push(abnSearch);
    }
    this.filteredAbn = filtered;
    },
    (err: any) => {
      console.log(err);
      this.imfsUtilityService.hideLoading();
      this.imfsUtilityService.showToastr('error', 'Failed', 'ABR Search currently unavailable');
      if(err.status == 500){
      this.imfsUtilityService.hideLoading();
      this.imfsUtilityService.showToastr('error', 'Failed', 'ABR Search currently unavailable');
      }
    }
    );
  }
  nameSelectedItem(event:any) {
    return event.name;
}
  abnSelectedItem(event:any) {
    return event.Abn;
}
  @Output() onDialogClose: EventEmitter<any> = new EventEmitter();
  closeDialog() {
    this.searchPopupform.get('searchcompanyname')?.patchValue(this.endUserName, {emitEvent: true});
    this.searchPopupform.get('searchabn')?.patchValue(this.endUserABN, {emitEvent: true});
    this.onDialogClose.emit();
    this.searchPopupform.reset();
  }
  @Output() selectSearchCompany: EventEmitter<string> = new EventEmitter();
  @Output() selectSearchAbn: EventEmitter<string> = new EventEmitter();
  onAddSelectCompany() {
    this.selectSearchCompany.emit(this.selectedCompanyName);
    this.selectSearchAbn.emit(this.selectedCompanyName);
    this.searchPopupform.reset();
    this.onDialogClose.emit();
  }
  onAddSelectAbn() {
    this.selectSearchCompany.emit(this.selectedAbn);
    this.selectSearchAbn.emit(this.selectedAbn);
    this.searchPopupform.reset();
    this.onDialogClose.emit();
  }
  get f() { return this.searchPopupform.controls; }

    onSubmit() {
        this.submitted = true;
        // stop here if form is invalid
        if (this.searchPopupform.invalid) {
          return;
      }
    }

}
