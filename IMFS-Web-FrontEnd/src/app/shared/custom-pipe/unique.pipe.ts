import { Pipe, PipeTransform } from '@angular/core';
import * as _ from 'lodash-es';

@Pipe({
    name: 'unique',
    pure: false
})

export class UniquePipe implements PipeTransform {
    transform(value: any, comparedValue: any): any {
        if (value !== undefined && value !== null) {
            return _.uniqBy(value, comparedValue);
        }
        return value;
    }
}
