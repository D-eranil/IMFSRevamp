import { AbstractControl, FormGroup, ValidationErrors, FormArray, FormControl } from '@angular/forms';
import { Injectable } from '@angular/core';

export interface AllValidationErrors {
    control_name: string;
    error_name: string;
    error_value: any;
}

export interface FormGroupControls {
    [key: string]: AbstractControl;
}


@Injectable()
export class IMFSFormService {

    showErrorMessage(formGroup: FormGroup, controlName: string): boolean {
        if (formGroup.controls[controlName] && !formGroup.controls[controlName].valid && formGroup.controls[controlName].dirty) {
            return true;
        }
        return false;
    }

    markFormAsPristine(controls: FormGroupControls) {
        Object.keys(controls).forEach(key => {
            const control = controls[key];
            if (control instanceof FormGroup || control instanceof FormArray) {
                this.markFormAsPristine((control as FormGroup).controls);
            }
            controls[key].markAsPristine();
            controls[key].markAsUntouched();
        });
    }

    markFormAsDirty(controls: FormGroupControls) {
        Object.keys(controls).forEach(key => {
            const control = controls[key];
            if (control instanceof FormGroup || control instanceof FormArray) {
                this.markFormAsDirty((control as FormGroup).controls);
            }
            controls[key].markAsDirty();
            controls[key].markAsTouched();
        });
    }

    updateValueAndValidity(controls: FormGroupControls) {
        Object.keys(controls).forEach(key => {
            const control = controls[key];
            if (control instanceof FormGroup || control instanceof FormArray) {
                this.updateValueAndValidity((control as FormGroup).controls);
            }
            controls[key].updateValueAndValidity();
        });
    }
    // usage
    // const error: AllValidationErrors[] = orpFormService.getFormValidationErrors(this.regForm.controls);
    //
    getFormValidationErrors(controls: FormGroupControls): AllValidationErrors[] {
        let errors: AllValidationErrors[] = [];
        Object.keys(controls).forEach(key => {
            const control = controls[key];
            if (control instanceof FormGroup || control instanceof FormArray) {
                errors = errors.concat(this.getFormValidationErrors((control as FormGroup).controls));
            }

            const controlErrors = controls[key].errors;
            if (controlErrors !== null) {

                Object.keys(controlErrors).forEach(keyError => {
                    if (control instanceof FormGroup) {
                        errors.push({
                            control_name: keyError,
                            error_name: key,
                            error_value: controlErrors[keyError]
                        });
                    } else {
                        errors.push({
                            control_name: key,
                            error_name: keyError,
                            error_value: controlErrors[keyError]
                        });
                    }
                });
            }
        });
        return errors;
    }
}
