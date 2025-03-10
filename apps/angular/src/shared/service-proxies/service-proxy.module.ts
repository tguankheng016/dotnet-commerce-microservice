import { NgModule } from "@angular/core";
import * as IdentityApiServiceProxies from './identity-service-proxies';
import * as ProductApiServiceProxies from './product-service-proxies';
import { HTTP_INTERCEPTORS } from "@angular/common/http";
import { CustomHttpInterceptor } from "./http-interceptor.service";

@NgModule({
    providers: [
        ProductApiServiceProxies.ProductServiceProxy,
        ProductApiServiceProxies.CategoryServiceProxy,
        IdentityApiServiceProxies.UserServiceProxy,
        IdentityApiServiceProxies.IdentityServiceProxy,
        IdentityApiServiceProxies.RoleServiceProxy,
        { provide: HTTP_INTERCEPTORS, useClass: CustomHttpInterceptor, multi: true }
    ]
})
export class ServiceProxyModule { }