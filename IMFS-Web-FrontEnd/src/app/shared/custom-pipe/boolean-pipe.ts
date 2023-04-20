import { Pipe, PipeTransform } from '@angular/core';
/*
 * Change boolean value to Yes or No
  * Usage:
 *   value | yesNo
 * Example:
 *   {{ isActive | yesNo }}
 *   formats to: Yes
*/
@Pipe({ name: 'yesNo' })

export class YesNoPipe implements PipeTransform {
    transform(value: boolean) {
        if (value === true) {
            return 'Yes';
        }
        if (value === false) {
            return 'No';
        }
        return '';

    }
}
