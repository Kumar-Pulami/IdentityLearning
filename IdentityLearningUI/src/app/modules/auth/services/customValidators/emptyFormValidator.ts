import { FormControl, FormGroup } from "@angular/forms";

export function EmptyFormValidator(formGroup: FormGroup): void{
    Object.keys(formGroup.controls).forEach(field => {
        const control = formGroup.get(field);
        if (control instanceof FormControl) {
          control.markAsTouched({ onlySelf: true });
          control.markAsDirty({onlySelf: true});
          control.markAsPristine({onlySelf: false});
        }
    });
}