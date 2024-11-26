import { NgModule } from "@angular/core";
import { ModalModule } from "ngx-bootstrap/modal";
import { TabsModule } from "ngx-bootstrap/tabs";
import { TooltipModule } from 'ngx-bootstrap/tooltip';
import { MainRoutingModule } from "./main-routing.module";
import { SharedModule } from "@shared/shared.module";
import { BsDropdownModule } from "ngx-bootstrap/dropdown";

@NgModule({
    imports: [
        MainRoutingModule,
        SharedModule,
        ModalModule.forRoot(),
        TabsModule.forRoot(),
        TooltipModule.forRoot(),
        BsDropdownModule.forRoot()
    ]
})
export class MainModule {}