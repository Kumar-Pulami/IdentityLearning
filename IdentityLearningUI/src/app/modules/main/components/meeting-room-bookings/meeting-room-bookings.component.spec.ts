import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MeetingRoomBookingsComponent } from './meeting-room-bookings.component';

describe('MeetingRoomBookingsComponent', () => {
  let component: MeetingRoomBookingsComponent;
  let fixture: ComponentFixture<MeetingRoomBookingsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ MeetingRoomBookingsComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MeetingRoomBookingsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
