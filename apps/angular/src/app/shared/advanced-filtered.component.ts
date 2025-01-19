import { ChangeDetectionStrategy, Component, EventEmitter, Output } from "@angular/core";

@Component({
    selector: 'advanced-filter',
    template: `
        <div *ngIf="advancedFiltersAreShown" class="row">
            <ng-content select="div[role=advanced-filter-content]"></ng-content>
            <div class="col-md-12 mt-5 mb-3">
                <button
                    (click)="resetFilters()"
                    class="btn btn-secondary btn-sm"
                    id="btn-reset-filters"
                >
                    Reset
                </button>
            </div>
        </div>
        <div class="row mb-2">
            <div class="col-sm-6">
                <span
                    class="cursor-pointer text-muted"
                    *ngIf="!advancedFiltersAreShown"
                    (click)="
                        advancedFiltersAreShown = !advancedFiltersAreShown
                    "
                >
                    <i class="fa fa-angle-down"></i>
                    Show advanced filters
                </span>
                <span
                    class="cursor-pointer text-muted"
                    *ngIf="advancedFiltersAreShown"
                    (click)="
                        advancedFiltersAreShown = !advancedFiltersAreShown
                    "
                >
                    <i class="fa fa-angle-up"></i>
                    Hide advanced filters
                </span>
            </div>
        </div>
    `,
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class AdvancedFilterComponent {
    @Output() onResetFilters: EventEmitter<any> = new EventEmitter<any>();

    advancedFiltersAreShown = false;

    resetFilters(): void {
        this.onResetFilters.emit();
    }
}