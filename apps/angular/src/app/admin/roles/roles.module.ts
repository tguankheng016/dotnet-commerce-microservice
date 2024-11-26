import { NgModule } from "@angular/core";
import { AppSharedModule } from "@app/shared/app-shared.module";
import { AdminSharedModule } from "../shared/admin-shared.module";
import { RolesRoutingModule } from "./roles-routing.module";
import { RolesComponent } from "./roles.component";
import { SharedModule } from "@shared/shared.module";
import { CreateOrEditRoleModalComponent } from "./create-or-edit-role-modal.component";

@NgModule({
    declarations: [RolesComponent, CreateOrEditRoleModalComponent],
    imports: [RolesRoutingModule, SharedModule, AppSharedModule, AdminSharedModule]
})
export class RolesModule {}