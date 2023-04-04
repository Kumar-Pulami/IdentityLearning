import { Component } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MeetingRoom } from 'src/app/domain model/MeetingRoom';
import { MeetingRoomApiService } from '../../services/apiServices/meetingRoomApiService.service';
import { NavbarServices } from '../../services/navBarServices/navBarService.service';
import { DatePipe } from '@angular/common';
import { GetValidationErrorMessage } from 'src/app/modules/auth/services/customValidators/getValidationErrorMessage';
import { EmptyFormValidator } from 'src/app/modules/auth/services/customValidators/emptyFormValidator';
import { BookingDateValidator, DateAndTimeValidator, EndTimeValidator, SelecteMeetingRoomValidator, StartTimeValidator } from '../../services/customValidators/bookMeetingRoomCustomValidators';
import { localHostUrl } from 'src/app/constant';

@Component({
  selector: 'app-book-meeting-room',
  templateUrl: './book-meeting-room.component.html',
  styleUrls: ['./book-meeting-room.component.scss']
})
export class BookMeetingRoomComponent {

  bookingForm: FormGroup;
  meetingRoomList: MeetingRoom[];
  currentDateTime: Date = new Date;
  minDate:string | null;
  maxDate:string | null;
  minTime: string = '10:00';
  maxTime: string = '16:00';

  constructor(
    private navBarService: NavbarServices,
    private meetingRoomApiService: MeetingRoomApiService,
    private datePipe: DatePipe
  ){}

  ngOnInit(){

    this.navBarService.selectNavBarServiceEmitter.emit('Booking');

    this.bookingForm = new FormGroup({
      meetingRoomId: new FormControl(null, SelecteMeetingRoomValidator()),
      bookingDate: new FormControl(null, BookingDateValidator()),
      startTime: new FormControl(null, StartTimeValidator()),
      endTime: new FormControl(null, EndTimeValidator()),
    }, {validators: DateAndTimeValidator});

    this.meetingRoomApiService.GetAllMeetingRoom().subscribe({
      next: (data: MeetingRoom[]) =>{
        this.meetingRoomList = data;
      }
    })

    let currentDateTime: Date = new Date();
    this.minDate = this.datePipe.transform(currentDateTime, 'yyyy-MM-dd');
    this.maxDate = this.datePipe.transform(this.GetMaxDate(currentDateTime), 'yyyy-MM-dd');    
  }


  Book(){

    if(this.bookingForm.invalid){
      EmptyFormValidator(this.bookingForm);
    }else{
      let bookMeetingApiUrl = localHostUrl + "Authentication/" 
    }
  }
  
  private GetMaxDate(currentDateTime: Date): Date{
    let newDate = currentDateTime;
    return new Date(newDate.setMonth(newDate.getMonth() + 1));
  }

  private GetUTCDateTime(dateTime: Date): Date{
    return new Date(dateTime.getUTCFullYear(),
                    dateTime.getUTCMonth(),
                    dateTime.getUTCDate(),
                    dateTime.getUTCHours(),
                    dateTime.getUTCMinutes(),
                    dateTime.getUTCSeconds()
                  );
  }

  GetErrorMessage(formControlName: string): string{
    return GetValidationErrorMessage(this.bookingForm.controls[formControlName]);
  }

  GetTimeDateErrorMessage(errorMsgKey: string): string{
    const error = this.bookingForm.errors;
    if(error != null){
      return error[errorMsgKey];
    }
    return "";
  }
}
