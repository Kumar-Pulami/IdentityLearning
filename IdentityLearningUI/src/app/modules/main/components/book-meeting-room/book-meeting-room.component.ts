import { Component } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { MeetingRoom } from 'src/app/domain model/MeetingRoom';
import { MeetingRoomApiService } from '../../services/apiServices/meetingRoomApiService.service';
import { NavbarServices } from '../../services/navBarServices/navBarService.service';
import { GetValidationErrorMessage } from 'src/app/modules/auth/services/customValidators/getValidationErrorMessage';
import { EmptyFormValidator } from 'src/app/modules/auth/services/customValidators/emptyFormValidator';
import { BookingDateValidator, DateAndTimeValidator, EndTimeValidator, SelecteMeetingRoomValidator, StartTimeValidator } from '../../services/customValidators/bookMeetingRoomCustomValidators';
import { localHostUrl } from 'src/app/constant';
import { BookMeetingRoomDTO } from '../../models/bookMeetingRoomDTO';
import { StorageService } from 'src/app/modules/auth/services/storageService/storage.service';
import { HttpClient } from '@angular/common/http';
import { Response } from 'src/app/domain model/response';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-book-meeting-room',
  templateUrl: './book-meeting-room.component.html',
  styleUrls: ['./book-meeting-room.component.scss']
})
export class BookMeetingRoomComponent {
  bookingForm: FormGroup;
  meetingRoomList: MeetingRoom[];

  currentDateTime: Date = new Date;

  minStartTime: Date = new Date();
  maxStartTime: Date = new Date();

  minEndTime: Date = new Date();
  maxEndTime: Date = new Date();

  minDate:Date = new Date();
  maxDate:Date = new Date();
  
  defaultStartTime: Date = new Date();
  defaultEndTime: Date = new Date();


  constructor(
    private navBarService: NavbarServices,
    private meetingRoomApiService: MeetingRoomApiService,
    private storageService: StorageService,
    private httpClient: HttpClient,
    private toastrService: ToastrService
  ){}

  ngOnInit(){

    this.navBarService.selectNavBarServiceEmitter.emit('Booking');
    
    this.meetingRoomApiService.GetAllMeetingRoom().subscribe({
      next: (data: MeetingRoom[]) =>{
        this.meetingRoomList = data;
      }
    })
    this.maxDate.setDate(this.maxDate.getDate() + 30);
    
    this.minStartTime.setHours(10);
    this.minStartTime.setMinutes(0);
    this.maxStartTime.setHours(15);
    this.maxStartTime.setMinutes(30);
    
    
    this.minEndTime.setHours(10);
    this.minEndTime.setMinutes(30);
    this.maxEndTime.setHours(16);
    this.maxEndTime.setMinutes(0);

    this.SetDefaultTime();

    this.bookingForm = new FormGroup({
      meetingRoomId: new FormControl(null, SelecteMeetingRoomValidator()),
      bookingDate: new FormControl(this.minDate, BookingDateValidator()),
      startTime: new FormControl(this.defaultStartTime, StartTimeValidator()),
      endTime: new FormControl(this.defaultEndTime, EndTimeValidator()),
    }, {validators: DateAndTimeValidator});
  }


  Book(){
    if(this.bookingForm.invalid){
      EmptyFormValidator(this.bookingForm);
    }else{
      let bookMeetingApiUrl: string = localHostUrl + "MeetingRoom/bookMeetingRoom";
      let bookingInformation: BookMeetingRoomDTO = new BookMeetingRoomDTO();
      bookingInformation.userId = this.storageService.GetUserId();
      bookingInformation.meetingRoomId = this.bookingForm.controls['meetingRoomId'].value;
      bookingInformation.bookedDate = this.bookingForm.controls['bookingDate'].value;
      bookingInformation.startTime = this.bookingForm.controls['startTime'].value;
      bookingInformation.endTime = this.bookingForm.controls['endTime'].value;

      bookingInformation.bookedDate.setHours(0);
      bookingInformation.bookedDate.setMinutes(0);
      bookingInformation.bookedDate.setSeconds(0);

      this.httpClient.post<Response>(bookMeetingApiUrl, bookingInformation).subscribe({
        next: (res: Response) =>{
          if(res.success){
            this.toastrService.success("The proposed booking has been booked.", "Success");
          }else{
            this.toastrService.error("The proposed booking has been already booked.", "Failed");
          }
        }, 
        error: () => {
          this.toastrService.error("Oops! Something went wrong. Try Again!", "Error");
        }
      });
    }
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

  private SetDefaultTime(){
    if(this.currentDateTime.getHours() < 10){
      this.defaultStartTime.setHours(10);
      this.defaultStartTime.setMinutes(0);
      this.defaultEndTime.setHours(10);
      this.defaultEndTime.setMinutes(30);
    }

    if(this.currentDateTime.getMinutes() < 30){
      this.defaultStartTime.setMinutes(30);
      this.defaultEndTime.setHours(this.currentDateTime.getHours() + 1);
      this.defaultEndTime.setMinutes(0);
    }

    if(this.currentDateTime.getHours() <= 10 && this.currentDateTime.getMinutes() < 30){
      this.defaultEndTime.setHours(11);
    }

    if(this.currentDateTime.getMinutes() > 30){
      this.defaultStartTime.setHours(this.currentDateTime.getHours() + 1);
      this.defaultStartTime.setMinutes(0);
      this.defaultEndTime.setHours(this.currentDateTime.getHours() + 1);
      this.defaultEndTime.setMinutes(30);
    }

    if(this.currentDateTime.getHours() >= 16){
      this.defaultStartTime.setHours(15);
      this.defaultStartTime.setMinutes(30);
      this.defaultEndTime.setHours(16);
      this.defaultEndTime.setMinutes(0);
    }
  }
}
