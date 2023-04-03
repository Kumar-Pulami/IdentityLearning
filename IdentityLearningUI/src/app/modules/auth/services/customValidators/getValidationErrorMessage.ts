import { AbstractControl, FormControl, FormGroup } from "@angular/forms";

export function GetValidationErrorMessage(formControl: AbstractControl | null ): string{
    if(formControl){
        const error = formControl.errors;
        if(error != null){
            return error['msg'];
        }
    }
    return "";
}