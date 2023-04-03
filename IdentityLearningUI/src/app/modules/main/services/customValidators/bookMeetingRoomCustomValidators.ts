import { AbstractControl, ValidationErrors, ValidatorFn } from "@angular/forms";

export function SelecteMeetingRoomValidator(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
        const controlValue:string = control.value;        
        if(controlValue === null || controlValue === undefined){
            return { msg: "Please, select the meeting room."}
        }
        return null;
    };
}


export function BookingDateValidator(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      const formControlValue = control.value;
      const date_regex = /([12]\d{3}-(0[1-9]|1[0-2])-(0[1-9]|[12]\d|3[01]))/;
      if(formControlValue === null || formControlValue === undefined){
        return {msg: "Please, select the booking Date."};
      }
      if(!date_regex.test(formControlValue)){        
        return {msg: "Invalid date format. Use MM/DD/YYYY"}
      }
      return null;
    };
}

export function StartTimeValidator(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      const formControlValue = control.value;      
      if(formControlValue === null  || formControlValue === undefined){
        return {msg: "Please, enter the start time."}
      }

      let startTime = Convert24hrTimeStringToDateTime(formControlValue);
      
      if(startTime.getHours() < 10 || (startTime.getHours() > 15 && startTime.getMinutes() > 30 )){
        return {msg: "Booking start time start from 10 AM - 3:30 PM."}
      }
      return null;
    };
}


export function EndTimeValidator(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      const formControlValue = control.value;      
      if(formControlValue === null  || formControlValue === undefined){
        return {msg: "Please, enter the time."}
      }

      let endTime = Convert24hrTimeStringToDateTime(formControlValue);
      
      if((endTime.getHours() < 9 && endTime.getMinutes() < 30) || endTime.getHours() > 16){
        return {msg: "Booking end time start from 10:30 AM - 4 PM."}
      }
      return null;
    };
}


export const DateAndTimeValidator: ValidatorFn = (control: AbstractControl): ValidationErrors | null => {
    const bookedDateValue: Date = control.get('bookingDate')?.value;
    const startTimeValue = control.get('startTime')?.value;
    const endTimeValue = control.get('endTime')?.value;
  
    let flag:boolean = false;
  
    if( bookedDateValue === null || bookedDateValue ===undefined || startTimeValue === null || startTimeValue ===undefined || endTimeValue === null || endTimeValue ===undefined ){
      flag = true;
    }

    
    if(!flag){


        let bookedDate: Date = new Date(bookedDateValue);
        let startTime: Date = Convert24hrTimeStringToDateTime(startTimeValue);
        let endTime: Date = Convert24hrTimeStringToDateTime(endTimeValue);


        console.log("Start Time" + startTime);
        console.log("End Time" + endTime);
        console.log("Booing Date" + bookedDate);
        
        const currentDateTime = new Date();
        startTime.setSeconds(0);
        startTime.setMilliseconds(0);
        endTime.setSeconds(0);
        endTime.setMilliseconds(0);
        
      if( bookedDate.getDate() === currentDateTime.getDate() && startTime.getTime() < currentDateTime.getTime() ){
        return {startTime: "Please, select start time greater than current time."}
      }
      
      if(endTime.getHours() < 10 || endTime.getHours() > 16){
        return {endTime: "Booking opens from 10 AM - 4 PM."}
      }
      
      if(endTime.getHours() < startTime.getHours()){
        return {endTime: "End time can't be less than start time."}
      }
      
      if (startTime.getHours() === endTime.getHours() && endTime.getMinutes() < startTime.getMinutes()){
        return {endTime: "End time can't be less than start time."}
      }
  
      if((endTime.getTime() - startTime.getTime())/60000 < 30){
        return {time: "The booking duration should be atleast 30 min."}
      }
    }
    return null;
};


function Convert24hrTimeStringToDateTime(timeString: string): Date{
    let [hour, minute] = timeString.split(':');
    let time: Date = new Date();
    time.setHours(parseInt(hour));
    time.setMinutes(parseInt(minute));
    return time;
}

