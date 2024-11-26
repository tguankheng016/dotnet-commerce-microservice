import { CommonModule } from "@angular/common";
import { provideHttpClient, withInterceptorsFromDi } from "@angular/common/http";
import { NgModule } from "@angular/core";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { ServiceProxyModule } from "@shared/service-proxies/service-proxy.module";
import { SharedModule } from "@shared/shared.module";
import { ModalModule } from "ngx-bootstrap/modal";
import { AccountComponent } from "./account.component";
import { AccountRoutingModule } from "./account-routing.module";
import { LoginComponent } from "./login/login.component";
import { AccountRouteGuard } from "@shared/auth/account-route-guard";
import { CallbackLoginComponent } from "./callback-login/callback-login.component";

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        SharedModule,
        ServiceProxyModule,
        AccountRoutingModule,
        ModalModule.forChild()
    ],
    declarations: [
        AccountComponent,
        LoginComponent,
        CallbackLoginComponent
    ],
    providers: [
        provideHttpClient(withInterceptorsFromDi()),
        AccountRouteGuard
    ]
})
export class AccountModule {
}