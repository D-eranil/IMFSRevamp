import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DocumentUploadPopupComponent } from './document-upload-popup.component';

describe('DocumentUploadPopupComponent', () => {
  let component: DocumentUploadPopupComponent;
  let fixture: ComponentFixture<DocumentUploadPopupComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DocumentUploadPopupComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DocumentUploadPopupComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
