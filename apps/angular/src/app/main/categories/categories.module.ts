import { NgModule } from "@angular/core";
import { AppSharedModule } from "@app/shared/app-shared.module";
import { CategoriesRoutingModule } from "./categories-routing.module";
import { CategoriesComponent } from "./categories.component";
import { CreateOrEditCategoryModalComponent } from "./create-or-edit-category-modal.component";
import { SharedModule } from "@shared/shared.module";

@NgModule({
    declarations: [CategoriesComponent, CreateOrEditCategoryModalComponent],
    imports: [CategoriesRoutingModule, SharedModule, AppSharedModule]
})
export class CategoriesModule { }