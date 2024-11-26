import { NgModule } from "@angular/core";
import { DefaultPageComponent } from "@app/layout/default-page.component";
import { SharedModule } from "@shared/shared.module";
import { PTableSkeletonTemplateComponent } from "./ptable-skeleton-template.component";


@NgModule({
    imports: [
        SharedModule
    ],
    declarations: [
        DefaultPageComponent,
        PTableSkeletonTemplateComponent
    ],
    exports: [
        DefaultPageComponent,
        PTableSkeletonTemplateComponent
    ]
})
export class AppSharedModule {}