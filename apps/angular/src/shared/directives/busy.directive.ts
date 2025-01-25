import { Directive, ElementRef, Input } from '@angular/core';

@Directive({
    selector: '[busy]',
})
export class BusyDirective {
    constructor(private _element: ElementRef) { }

    @Input() set busy(isBusy: boolean) {
        this.refreshState(isBusy);
    }

    refreshState(isBusy: boolean): void {
        if (isBusy === undefined) {
            return;
        }

        if (isBusy) {
            appHelper.ui.setBusy(this._element.nativeElement);
        } else {
            appHelper.ui.clearBusy(this._element.nativeElement);
        }
    }
}
