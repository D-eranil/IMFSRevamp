import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AuSearchTemplateComponent } from './au-search-template.component';

describe('AuSearchTemplateComponent', () => {
  let component: AuSearchTemplateComponent;
  let fixture: ComponentFixture<AuSearchTemplateComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AuSearchTemplateComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AuSearchTemplateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
