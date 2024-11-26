import { Component, TemplateRef, ViewChild } from "@angular/core";

@Component({
    selector: 'p-datatable-skeleton-template',
    template: `
        <ng-template #dataTableSkeletonTemplate let-loading="loading" let-skeletonCols="skeletonCols">
            <tr *ngIf="loading">
                <td *ngFor="let col of skeletonCols">
                    <p-skeleton />
                </td>
            </tr>
        </ng-template>
    `,
})
export class PTableSkeletonTemplateComponent {
    @ViewChild('dataTableSkeletonTemplate', { static: true }) template!: TemplateRef<any>;

    getTemplate() {
        return this.template;
    }
}