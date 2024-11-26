import { CommonModule } from "@angular/common";
import { NgModule } from "@angular/core";
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ModalModule } from 'ngx-bootstrap/modal';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { CollapseModule } from 'ngx-bootstrap/collapse';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { provideHttpClient, withInterceptorsFromDi } from "@angular/common/http";
import { AppRoutingModule } from "./app-routing.module";
import { AppComponent } from "./app.component";
import { AboutComponent } from "./about/about.component";
import { HeaderComponent } from "./layout/header/header.component";
import { SidebarComponent } from "./layout/sidebar/sidebar.component";
import { SidebarMenuComponent } from "./layout/sidebar/sidebar-menu.component";
import { ServiceProxyModule } from "@shared/service-proxies/service-proxy.module";
import { SharedModule } from "@shared/shared.module";
import { HeaderNotificationsComponent } from "./layout/header/header-notifications.component";
import { HeaderUserMenuComponent } from "./layout/header/header-user-menu.component";
import { AppSharedModule } from "./shared/app-shared.module";
import { FooterComponent } from "./layout/footer/footer.component";

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        ModalModule.forChild(),
        BsDropdownModule,
        CollapseModule,
        TabsModule,
        AppRoutingModule,
        ServiceProxyModule,
        AppSharedModule,
        SharedModule
    ],
    declarations: [
        AppComponent,
        AboutComponent,
        HeaderComponent,
        SidebarComponent,
        SidebarMenuComponent,
        HeaderNotificationsComponent,
        HeaderUserMenuComponent,
        FooterComponent
    ],
    providers: [
        provideHttpClient(withInterceptorsFromDi())
        //ImpersonationService,
        //UserNotificationHelper,
        //CommonSignalrService
    ]
})
export class AppModule {}