import { NgModule } from "@angular/core";
import { UsersRoutingModule } from "./users-routing.module";
import { UsersComponent } from "./users.component";
import { SharedModule } from "@shared/shared.module";
import { AppSharedModule } from "@app/shared/app-shared.module";
import { CreateOrEditUserModalComponent } from "./create-or-edit-user-modal.component";
import { EditUserPermissionsModalComponent } from "./edit-user-permissions-modal.component";
import { AdminSharedModule } from "../shared/admin-shared.module";

@NgModule({
    declarations: [UsersComponent, CreateOrEditUserModalComponent, EditUserPermissionsModalComponent],
    imports: [UsersRoutingModule, SharedModule, AppSharedModule, AdminSharedModule]
})
export class UsersModule {}