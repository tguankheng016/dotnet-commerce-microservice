import { ModuleWithProviders, NgModule } from "@angular/core";
import { StringService } from "./utils/string.service";
import { CommonModule } from "@angular/common";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { RouterModule } from "@angular/router";

//PrimeNg
import { PaginatorModule } from 'primeng/paginator';
import { TableModule } from 'primeng/table';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { InputTextModule } from 'primeng/inputtext';
import { CalendarModule } from 'primeng/calendar';
import { DropdownModule } from 'primeng/dropdown';
import { InputNumberModule } from 'primeng/inputnumber';
import { TieredMenuModule } from 'primeng/tieredmenu';
import { MultiSelectModule } from 'primeng/multiselect';
import { InputMaskModule } from 'primeng/inputmask';
import { PasswordModule } from 'primeng/password';
import { KeyFilterModule } from 'primeng/keyfilter';
import { FileUploadModule as PrimeNgFileUploadModule } from 'primeng/fileupload';
import { CheckboxModule } from 'primeng/checkbox';
import { AutoCompleteModule } from 'primeng/autocomplete';
import { OverlayPanelModule } from 'primeng/overlaypanel';
import { SkeletonModule } from 'primeng/skeleton';

import { ModalModule } from "ngx-bootstrap/modal";
import { TabsModule } from "ngx-bootstrap/tabs";
import { AppNavigationService } from "./layout/app-navigation.service";
import { LocalStorageService } from "./utils/local-storage.service";
import { AppAuthService } from "./auth/app-auth.service";
import { AppSessionService } from "./session/app-session.service";
import { AppRouteGuard } from "./auth/app-route-guard";
import { CookieService } from "ngx-cookie-service";
import { BusyDirective } from "./directives/busy.directive";

//Scrollbar
import { NgScrollbarModule, NG_SCROLLBAR_OPTIONS, NgScrollbarOptions } from 'ngx-scrollbar';
import { ButtonBusyDirective } from "./directives/button-busy.directive";
import { ValidationMessagesComponent } from "./utils/validation-messages.component";
import { MomentFormatPipe } from "./pipes/moment-format.pipe";
import { PermissionPipe } from "./pipes/permission.pipe";
import { LayoutStoreService } from "./layout/layout-store.service";
import { ClickOutsideDirective } from "./directives/click-outside.directive";
const DEFAULT_SCROLLBAR_CONFIG: NgScrollbarOptions = {
    visibility: 'hover',
    appearance: 'compact',
    autoHeightDisabled: false,
};

const imports = [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    RouterModule,
    ModalModule,
    TabsModule,
    OverlayPanelModule,
    TableModule,
    PaginatorModule,
    InputTextareaModule,
    InputTextModule,
    CalendarModule,
    DropdownModule,
    InputNumberModule,
    TieredMenuModule,
    MultiSelectModule,
    InputMaskModule,
    PasswordModule,
    KeyFilterModule,
    PrimeNgFileUploadModule,
    AutoCompleteModule,
    CheckboxModule,
    SkeletonModule,
    NgScrollbarModule
];

const declarations = [
    BusyDirective,
    ButtonBusyDirective,
    ClickOutsideDirective,
    ValidationMessagesComponent,
    PermissionPipe,
    MomentFormatPipe,
];


@NgModule({
    imports: [...imports],
    declarations: [...declarations],
    exports: [...imports, ...declarations],
    providers: [
        StringService,
        AppNavigationService,
        LocalStorageService,
        AppAuthService,
        CookieService,
        // DateTimeService,
        {
            provide: NG_SCROLLBAR_OPTIONS,
            useValue: DEFAULT_SCROLLBAR_CONFIG,
        }
    ]
})
export class SharedModule {
    static forRoot(): ModuleWithProviders<SharedModule> {
        return {
            ngModule: SharedModule,
            providers: [
                StringService,
                AppNavigationService,
                AppAuthService,
                CookieService,
                AppSessionService,
                AppRouteGuard,
                LayoutStoreService
                // AppUrlService,
            ]
        };
    }
}