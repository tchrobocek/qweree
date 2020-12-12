import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PathExplorerComponent } from './path-explorer.component';

describe('PathExplorerComponent', () => {
  let component: PathExplorerComponent;
  let fixture: ComponentFixture<PathExplorerComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PathExplorerComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PathExplorerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
