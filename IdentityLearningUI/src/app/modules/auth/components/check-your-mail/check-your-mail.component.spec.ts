import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CheckYourMailComponent } from './check-your-mail.component';

describe('CheckYourMailComponent', () => {
  let component: CheckYourMailComponent;
  let fixture: ComponentFixture<CheckYourMailComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CheckYourMailComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CheckYourMailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
