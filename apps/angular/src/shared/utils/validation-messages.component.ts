import { Component, Input } from "@angular/core";
import { filter as _filter, find as _find, concat as _concat } from 'lodash-es';

class ErrorDef {
    error: string;
    errorMessage: string;
    errorProperty: string;
}

@Component({
    selector: '<validation-messages>',
    template: `
        <div class="has-danger" *ngIf="formCtrl.invalid && (formCtrl.dirty || formCtrl.touched)">
            <div *ngFor="let errorDef of errorDefsInternal">
                <div *ngIf="getErrorDefinitionIsInValid(errorDef)" class="form-control-feedback">
                    {{ errorDef.errorMessage }}
                </div>
            </div>
        </div>
    `,
})
export class ValidationMessagesComponent {
    @Input() formCtrl;

    _errorDefs: ErrorDef[] = [];

    readonly standartErrorDefs: ErrorDef[] = [
        { error: 'required', errorMessage: 'This field is required' } as ErrorDef,
        { error: 'email', errorMessage: 'Invalid email address' } as ErrorDef,
    ];

    constructor() {}

    get errorDefsInternal(): ErrorDef[] {
        let standarts = _filter(
            this.standartErrorDefs,
            (ed) => !_find(this._errorDefs, (edC) => edC.error === ed.error)
        );
        let all = <ErrorDef[]>_concat(standarts, this._errorDefs);

        return all;
    }

    @Input() set errorDefs(value: ErrorDef[]) {
        this._errorDefs = value;
    }

    getErrorDefinitionIsInValid(errorDef: ErrorDef): boolean {
        return !!this.formCtrl.errors[errorDef.error];
    }
}