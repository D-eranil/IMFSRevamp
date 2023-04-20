import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ApplicationDocUploadPopupComponent } from './application-doc-upload-popup.component';

describe('ApplicationDocUploadPopupComponent', () => {
  let component: ApplicationDocUploadPopupComponent;
  let fixture: ComponentFixture<ApplicationDocUploadPopupComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ApplicationDocUploadPopupComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ApplicationDocUploadPopupComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
