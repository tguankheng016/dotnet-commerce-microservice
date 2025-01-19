import { NgModule } from "@angular/core";
import { DefaultPageComponent } from "@app/layout/default-page.component";
import { SharedModule } from "@shared/shared.module";
import { PTableSkeletonTemplateComponent } from "./ptable-skeleton-template.component";
import { AdvancedFilterComponent } from "./advanced-filtered.component";
import { AdvancedSearchBoxComponent } from "./advanced-search-box.component";


@NgModule({
    imports: [
        SharedModule
    ],
    declarations: [
        DefaultPageComponent,
        PTableSkeletonTemplateComponent,
        AdvancedFilterComponent,
        AdvancedSearchBoxComponent
    ],
    exports: [
        DefaultPageComponent,
        PTableSkeletonTemplateComponent,
        AdvancedFilterComponent,
        AdvancedSearchBoxComponent
    ]
})
export class AppSharedModule { }