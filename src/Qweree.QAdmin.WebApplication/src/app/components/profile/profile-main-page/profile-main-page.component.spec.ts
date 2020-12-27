import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ProfileMainPageComponent } from './profile-main-page.component';

describe('ProfileMainPageComponent', () => {
  let component: ProfileMainPageComponent;
  let fixture: ComponentFixture<ProfileMainPageComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ProfileMainPageComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ProfileMainPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
