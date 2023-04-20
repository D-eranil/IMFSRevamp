import { Directive } from '@angular/core';
import { NgControl } from '@angular/forms';

@Directive({
    selector: '[ignoreDirty]'
})

export class IgnoreDirtyDirective {
    constructor(private control: NgControl) {
        if (this.control && this.control.valueChanges) {
            this.control.valueChanges.subscribe(v => {
                if (this.control.control && this.control.dirty) {
                    this.control.control.markAsPristine();
                }
            });
        }
    }
}
