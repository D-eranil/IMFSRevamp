// this class is copied from https://github.com/text-mask/text-mask/issues/109 and modified to wrap text mask input

import { Directive, ElementRef, Input, OnChanges, OnInit, Renderer2, SimpleChanges } from '@angular/core';
import { ControlValueAccessor } from '@angular/forms';
import * as _ from 'lodash-es';
import createNumberMask from 'text-mask-addons/dist/createNumberMask';
import { createTextMaskInputElement } from 'text-mask-core/dist/textMaskCore';
import { ControlValueAccessorProviderFactory } from '../utils/control-value-accessor-provider.factory';

export interface CurrencyMaskDefinition {
    prefix: string;
    suffix: string;
    includeThousandsSeparator: boolean;
    thousandsSeparatorSymbol: string;
    allowDecimal: boolean;
    decimalSymbol: string;
    decimalLimit: number;
    integerLimit: number;
    allowNegative: boolean;
    allowLeadingZeroes: boolean;
}

const defaultCurrencyMaskDefinition: CurrencyMaskDefinition = {
    prefix: '$',
    suffix: '',
    includeThousandsSeparator: true,
    thousandsSeparatorSymbol: ',',
    allowDecimal: true,
    decimalSymbol: '.',
    decimalLimit: 2,
    integerLimit: 13,
    allowNegative: false,
    allowLeadingZeroes: false
};

@Directive({
    // tslint:disable-next-line: directive-selector
    selector: '[currencyInput]',
    // tslint:disable-next-line:use-host-property-decorator
    // tslint:disable-next-line: no-host-metadata-property
    host: {
        '(change)': 'onChange($event.target.value)',
        '(input)': 'onChange($event.target.value)',
        '(blur)': 'onTouched()'
    },
    providers: [ControlValueAccessorProviderFactory(CurrencyInputDirective)]
})
export class CurrencyInputDirective implements ControlValueAccessor, CurrencyMaskDefinition, OnInit, OnChanges {

    @Input() public prefix: string;
    @Input() public suffix: string;
    @Input() public includeThousandsSeparator: boolean;
    @Input() public allowDecimal: boolean;
    @Input() public decimalLimit: number;
    @Input() public integerLimit: number;
    @Input() public allowNegative: boolean;
    @Input() public allowLeadingZeroes: boolean;

    public thousandsSeparatorSymbol: string;
    public decimalSymbol: string;

    private textMaskInputElement: any;
    private inputElement: HTMLInputElement;

    // stores the last value for comparison
    private lastValue: any;

    // tslint:disable-next-line:no-empty
    // tslint:disable-next-line: no-shadowed-variable
    public onChange = (_: any) => { };
    // tslint:disable-next-line:no-empty
    public onTouched = () => { };

    private get currencyMaskDefinition(): CurrencyMaskDefinition {
        return {
            prefix: this.prefix,
            suffix: this.suffix,
            includeThousandsSeparator: this.includeThousandsSeparator,
            thousandsSeparatorSymbol: this.thousandsSeparatorSymbol,
            allowDecimal: this.allowDecimal,
            decimalSymbol: this.decimalSymbol,
            decimalLimit: this.decimalLimit,
            integerLimit: this.integerLimit,
            allowNegative: this.allowNegative,
            allowLeadingZeroes: this.allowLeadingZeroes
        };
    }

    constructor(private renderer: Renderer2, private elementRef: ElementRef) { }

    public ngOnInit() {
        this.prefix = this.prefix !== undefined ? this.prefix : defaultCurrencyMaskDefinition.prefix;
        this.suffix = this.suffix !== undefined ? this.suffix : defaultCurrencyMaskDefinition.suffix;
        this.includeThousandsSeparator = this.includeThousandsSeparator !== undefined ?
            this.includeThousandsSeparator : defaultCurrencyMaskDefinition.includeThousandsSeparator;
        this.thousandsSeparatorSymbol = this.thousandsSeparatorSymbol !== undefined ?
            this.thousandsSeparatorSymbol : defaultCurrencyMaskDefinition.thousandsSeparatorSymbol;
        this.allowDecimal = this.allowDecimal !== undefined ? this.allowDecimal : defaultCurrencyMaskDefinition.allowDecimal;
        this.decimalSymbol = this.decimalSymbol !== undefined ? this.decimalSymbol : defaultCurrencyMaskDefinition.decimalSymbol;
        this.decimalLimit = this.decimalLimit !== undefined ? this.decimalLimit : defaultCurrencyMaskDefinition.decimalLimit;
        this.integerLimit = this.integerLimit !== undefined ? this.integerLimit : defaultCurrencyMaskDefinition.integerLimit;
        this.allowNegative = this.allowNegative !== undefined ? this.allowNegative : defaultCurrencyMaskDefinition.allowNegative;
        this.allowLeadingZeroes = this.allowLeadingZeroes !== undefined ?
            this.allowLeadingZeroes : defaultCurrencyMaskDefinition.allowLeadingZeroes;

        this.setupMask(true);
    }

    public ngOnChanges(changes: SimpleChanges) {
        this.setupMask(true);
        if (this.textMaskInputElement !== undefined) {
            this.textMaskInputElement.update(this.inputElement.value);
        }
    }

    public writeValue(value: number): void {
        this.setupMask();

        const normalizedValue = value === null ? '' : this.mask(value);

        this.renderer.setProperty(this.elementRef.nativeElement, 'value', normalizedValue);

        if (this.textMaskInputElement !== undefined) {
            this.textMaskInputElement.update(normalizedValue);
            this.lastValue = normalizedValue;
        }
    }
    public registerOnChange(fn: (_: number | null) => void): void {
        this.onChange = value => {
            this.setupMask();

            if (this.textMaskInputElement !== undefined) {

                this.textMaskInputElement.update(value);

                // get the updated value
                value = this.inputElement.value;

                // check against the last value to prevent firing ngModelChange despite no changes
                if (this.lastValue !== value) {
                    this.lastValue = value;
                    const parsedValue = value === '' ? null : this.unmask(value);
                    fn(parsedValue);
                }
            }
        };
    }
    public registerOnTouched(fn: () => void): void {
        this.onTouched = () => {
            const parsedValue = this.lastValue === '' ? null : this.unmask(this.lastValue);
            if (!parsedValue || isNaN(parsedValue)) {
                this.onChange('');
            } else {
                this.onChange(this.mask(parsedValue as number));
            }

            fn();
        };
    }

    public setDisabledState(isDisabled: boolean): void {
        this.renderer.setProperty(this.elementRef.nativeElement, 'disabled', isDisabled);
    }

    private setupMask(create = false) {
        const currencyMask = createNumberMask({
            prefix: this.prefix,
            suffix: this.suffix,
            includeThousandsSeparator: this.includeThousandsSeparator,
            thousandsSeparatorSymbol: this.thousandsSeparatorSymbol,
            allowDecimal: this.allowDecimal,
            decimalSymbol: this.decimalSymbol,
            decimalLimit: this.decimalLimit,
            integerLimit: this.integerLimit,
            allowNegative: this.allowNegative,
            allowLeadingZeroes: this.allowLeadingZeroes
        });

        const textMaskConfig = {
            mask: currencyMask,
            guide: false,
            placeholderChar: '_',
            pipe: '',
            keepCharPositions: false
        };

        if (!this.inputElement) {
            if (this.elementRef.nativeElement.tagName === 'INPUT') {
                // `textMask` directive is used directly on an input element
                this.inputElement = this.elementRef.nativeElement;
            } else {
                // `textMask` directive is used on an abstracted input element, `md-input-container`, etc
                this.inputElement = this.elementRef.nativeElement.getElementsByTagName('INPUT')[0];
            }
        }

        if (this.inputElement && create) {
            this.textMaskInputElement = createTextMaskInputElement(Object.assign({ inputElement: this.inputElement }, textMaskConfig));
        }
    }

    private unmask(value: string): number | null {
        if (value === null || value === undefined || value === '') {
            return null;
        }

        let newValue = value;

        if (this.currencyMaskDefinition.thousandsSeparatorSymbol) {
            newValue = newValue.split(this.currencyMaskDefinition.thousandsSeparatorSymbol).join('');
        }

        if (this.currencyMaskDefinition.decimalSymbol) {
            newValue = newValue.replace(this.currencyMaskDefinition.decimalSymbol, '.');
        }

        if (this.currencyMaskDefinition.prefix) {
            newValue = newValue.replace(this.currencyMaskDefinition.prefix, '');
        }

        if (this.currencyMaskDefinition.suffix) {
            newValue = newValue.replace(this.currencyMaskDefinition.suffix, '');
        }

        newValue = newValue.replace('_', '');

        return newValue.length === 0 ? null : parseFloat(newValue);
    }

    private mask(value: number): string {
        let newValue = '';
        if (!_.isNull(value) && !_.isUndefined(value)) {
            newValue = value.toString();
            if (this.unmask(newValue) !== null && this.decimalLimit) {
                const decimalCount = this.decimalPlaces(newValue);
                // if user input 1 or 1.1, change it to 1.00 or 1.10
                if (decimalCount === 0 || decimalCount === 1) {
                    newValue = Number(this.unmask(newValue)).toFixed(2);
                }
            }
        }

        if (isFinite(value)) {
            newValue = newValue.toString().replace('.', this.decimalSymbol);
        }

        return newValue;
    }

    private decimalPlaces(num: string) {
        const match = ('' + num).match(/(?:\.(\d+))?(?:[eE]([+-]?\d+))?$/);
        if (!match) { return 0; }
        return Math.max(
            0,
            // Number of digits right of decimal point.
            (match[1] ? match[1].length : 0)
            // Adjust for scientific notation.
            - (match[2] ? +match[2] : 0));
    }
}
