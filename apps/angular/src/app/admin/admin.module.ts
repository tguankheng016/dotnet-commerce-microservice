import { NgModule } from "@angular/core";
import { SharedModule } from "@shared/shared.module";
import { BsDropdownModule } from "ngx-bootstrap/dropdown";
import { ModalModule } from "ngx-bootstrap/modal";
import { TabsModule } from "ngx-bootstrap/tabs";
import { TooltipModule } from 'ngx-bootstrap/tooltip';
import { AdminRoutingModule } from "./admin-routing.module";

@NgModule({
    imports: [
        AdminRoutingModule,
        SharedModule.forRoot(),
        ModalModule.forRoot(),
        TabsModule.forRoot(),
        TooltipModule.forRoot(),
        BsDropdownModule.forRoot()
    ]
})
export class AdminModule {}