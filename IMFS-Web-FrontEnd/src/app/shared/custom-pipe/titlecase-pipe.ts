import { Pipe, PipeTransform } from '@angular/core';
/*
 * Change text to title case
  * Usage:
 *   value | titleCase
 * Example:
 *   {{ "SUN AUNG" | titleCase }}
 *   formats to: Sun Aung
*/

@Pipe({ name: 'titleCase' })
export class TitleCasePipe implements PipeTransform {
    public transform(input: string): string {
        if (!input) {
            return '';
        } else {
            return input.replace(/\w\S*/g, (txt => txt[0].toUpperCase() + txt.substr(1).toLowerCase()));
        }
    }
}
