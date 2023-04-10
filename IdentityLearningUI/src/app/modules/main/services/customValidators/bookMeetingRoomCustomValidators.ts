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
      if(formControlValue === null || formControlValue === undefined){
        return {msg: "Please, select the booking Date."};
      }
      return null;
    };
}

export function StartTimeValidator(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      const formControlValue = control.value;      
      if(formControlValue === null  || formControlValue === undefined){
        return {msg: "Invalid date format."}
      }
      
      if(formControlValue.getHours() < 10 || (formControlValue.getHours() === 15 && formControlValue.getMinutes() > 30 ) || formControlValue.getHours() > 15){
        return {msg: "Booking start time start from 10 AM - 3:30 PM."}
      }
      return null;
    };
}


export function EndTimeValidator(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      const formControlValue = control.value;      
      if(formControlValue === null  || formControlValue === undefined){
        return {msg: "Invalid date format."}
      }

      if((formControlValue.getHours() === 10 && formControlValue.getMinutes() < 30) || formControlValue.getHours() < 10 ||formControlValue.getHours() > 16 || (formControlValue.getHours() === 16 && formControlValue.getMinutes() > 0)){
        return {msg: "Booking end time start from 10:30 AM - 4 PM."}
      }
      return null;
    };
}


export const DateAndTimeValidator: ValidatorFn = (control: AbstractControl): ValidationErrors | null => {
    const bookedDate: Date = control.get('bookingDate')?.value;
    const startTime = control.get('startTime')?.value;
    const endTime = control.get('endTime')?.value;
  
    let flag:boolean = false;
  
    if( bookedDate === null || bookedDate ===undefined || startTime === null || startTime ===undefined || endTime === null || endTime ===undefined ){
      flag = true;
    }

    if(!control.get('bookingDate')?.valid || !control.get('startTime')?.valid || !control.get('endTime')?.valid ){
      flag = true;
    }
    
    if(!flag){
      const currentDateTime = new Date();
      startTime.setSeconds(0);
      startTime.setMilliseconds(0);
      endTime.setSeconds(0);
      endTime.setMilliseconds(0);
        
      if( bookedDate.getDate() === currentDateTime.getDate() && startTime.getTime() < currentDateTime.getTime() ){
        return {startTime: "Please, select start time greater than current time."}
      }
      
      if(endTime.getHours() < startTime.getHours()){
        return {endTime: "End time can't be less than start time."}
      }
      
      if (startTime.getHours() === endTime.getHours() && endTime.getMinutes() < startTime.getMinutes()){
        return {endTime: "End time can't be less than start time."}
      }
      
      if((endTime.getTime() - startTime.getTime())/60000 < 30){
        return {endTime: "The booking duration should be atleast 30 min."}
      }
    }
    return null;
};
