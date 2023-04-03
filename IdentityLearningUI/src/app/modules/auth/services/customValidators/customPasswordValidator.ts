import { AbstractControl, ValidationErrors, ValidatorFn } from "@angular/forms";

export function CustomPasswordValidator(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
        const controlValue:string = control.value;
        const passwordRegexPattern = new RegExp('(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[$@$!%*?&])[A-Za-zd$@$!%*?&].{7,15}');
        if(controlValue === null || controlValue === undefined){
            return { msg: "Password is required."}
        }else if(!passwordRegexPattern.test(controlValue)){
            return { msg: "Password doesn't meet the requriements."}
        }
        return null;
    };
}
