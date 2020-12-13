import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { StorageImportComponent } from './storage-import.component';

describe('StorageImportComponent', () => {
  let component: StorageImportComponent;
  let fixture: ComponentFixture<StorageImportComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ StorageImportComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(StorageImportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
