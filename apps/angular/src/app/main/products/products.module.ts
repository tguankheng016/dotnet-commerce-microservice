import { NgModule } from "@angular/core";
import { AppSharedModule } from "@app/shared/app-shared.module";
import { ProductsRoutingModule } from "./products-routing.module";
import { ProductsComponent } from "./products.component";
import { CreateOrEditProductModalComponent } from "./create-or-edit-product-modal.component";
import { SharedModule } from "@shared/shared.module";

@NgModule({
    declarations: [ProductsComponent, CreateOrEditProductModalComponent],
    imports: [ProductsRoutingModule, SharedModule, AppSharedModule]
})
export class ProductsModule { }