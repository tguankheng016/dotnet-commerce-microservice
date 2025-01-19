import { ChangeDetectionStrategy, Component, EventEmitter, Input, Output } from "@angular/core";
import { debounceTime, Subject } from "rxjs";

@Component({
    selector: 'advanced-searchbox',
    template: `
        <div class="row align-items-center mb-2">
            <div class="col-xl-12">
                <div class="mb-2 m-form__group align-items-center">
                    <div class="input-group">
                        <div class="input-group-prepend">
                            <button
                                (click)="search()"
                                class="btn btn-primary"
                                type="button"
                            >
                                <i
                                    class="fas fa-search"
                                    aria-label="Search"
                                ></i>
                            </button>
                        </div>
                        <input
                            pInputText
                            [(ngModel)]="filterText"
                            name="filterText"
                            placeholder="Search ..."
                            type="text"
                            (keyup)="search()"
                        />
                    </div>
                </div>
            </div>
        </div>
    `,
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class AdvancedSearchBoxComponent {
    @Input() filterText = "";
    @Output() onSearch: EventEmitter<string> = new EventEmitter<string>();
    searchTerm: Subject<any> = new Subject<any>();

    constructor() {
        this.searchTerm.pipe(
            debounceTime(300)  // Only emit the value after 300ms of no new input
        ).subscribe(() => {
            this.onSearch.emit(this.filterText);
        });
    }

    search(): void {
        this.searchTerm.next(undefined);
    }
}