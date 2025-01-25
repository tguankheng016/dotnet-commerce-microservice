import { Directive, ElementRef, HostListener, EventEmitter, Output, Input, Renderer2 } from "@angular/core";
import { StringService } from "@shared/utils/string.service";

@Directive({
    selector: '[clickOutside]'
})
export class ClickOutsideDirective {
    @Output() clickOutside = new EventEmitter<void>();
    @Input() excludeDocumentId = '';

    constructor(
        private elementRef: ElementRef,
        private renderer: Renderer2,
        private _stringService: StringService
    ) { }

    @HostListener('document:click', ['$event.target'])
    public onClick(target) {
        let clickedInside = this.elementRef.nativeElement.contains(target);

        if (!clickedInside && this._stringService.notNullOrEmpty(this.excludeDocumentId)) {
            const excludedElement = this.renderer.selectRootElement('#' + this.excludeDocumentId, true) as HTMLInputElement;
            clickedInside = excludedElement.contains(target);
        }

        if (!clickedInside) {
            this.clickOutside.emit();
        }
    }
}