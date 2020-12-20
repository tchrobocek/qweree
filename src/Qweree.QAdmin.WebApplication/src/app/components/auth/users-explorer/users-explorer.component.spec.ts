import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { UsersExplorerComponent } from './users-explorer.component';

describe('UsersExplorerComponent', () => {
  let component: UsersExplorerComponent;
  let fixture: ComponentFixture<UsersExplorerComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ UsersExplorerComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(UsersExplorerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
