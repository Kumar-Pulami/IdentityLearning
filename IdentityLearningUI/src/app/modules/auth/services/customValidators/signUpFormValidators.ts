import { AbstractControl, ValidationErrors, ValidatorFn } from "@angular/forms";

export function NameValidator(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
        const controlValue:string = control.value;
        if(controlValue === null || controlValue === undefined){
            return {msg: "Name is required."}
        }else if(controlValue.length < 5 || controlValue.length > 50){
            return {msg: "Name length should be > 5 and < 50;"}
        }
        return null;
    };
}


export function EmailValidator(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
        const controlValue:string = control.value;
        let emailRegex = new RegExp(/[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?/g);
        if(controlValue === null || controlValue === undefined){
            return {msg: "Email is required."}
        }else if(!emailRegex.test(controlValue)){
            return {msg: "Invalid email address."}
        }
        return null;
    };
}

export function ContactNumberValidator(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
        const controlValue:string = control.value;
        if(controlValue === null || controlValue === undefined){
            return {msg: "Contact number is required."}
        }else if(isNaN(Number(controlValue))){
            return {msg: "Contact number must be number."}
        }else if(Number(controlValue) < 9700000000 || Number(controlValue) > 9899999999){
            return {msg: "Invalid contact number."}
        }
        return null;
    };
}

export function PasswordValidator(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
        const controlValue:string = control.value;
        const passwordRegexPattern = new RegExp('(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[$@$!%*?&])[A-Za-zd$@$!%*?&].{7,15}');
        if(controlValue === null || controlValue === undefined){
            return {msg: "Password is required."}
        }else if(!passwordRegexPattern.test(controlValue)){
            return { invalid: "Doesnot meet password requriement"}
        }
        return null;
    };
}


export function MatchPasswordValidator(passwordControlName: string, confirmPasswordControlName: string){
    return (form: AbstractControl): ValidationErrors | null => {
        const passwordValue: string = form.get(passwordControlName)?.value;
        const confirmPasswordValue: string = form.get(confirmPasswordControlName)?.value;
        if (passwordValue !== confirmPasswordValue){
            return {pwNotMatch: "Password doesnot match."}
        }
        return null;
    };
}