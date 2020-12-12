import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { StorageExplorerComponent } from './storage-explorer.component';

describe('StorageExplorerComponent', () => {
  let component: StorageExplorerComponent;
  let fixture: ComponentFixture<StorageExplorerComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ StorageExplorerComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(StorageExplorerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
