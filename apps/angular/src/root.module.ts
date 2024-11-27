import { NgModule, APP_INITIALIZER } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { HTTP_INTERCEPTORS, provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';

import { ModalModule } from 'ngx-bootstrap/modal';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { CollapseModule } from 'ngx-bootstrap/collapse';
import { TabsModule } from 'ngx-bootstrap/tabs';

import { SharedModule } from '@shared/shared.module';
import { ServiceProxyModule } from '@shared/service-proxies/service-proxy.module';
import { RootRoutingModule } from './root-routing.module';
import { AppConsts } from '@shared/app-consts';

import { RootComponent } from './root.component';
import { AppInitializer } from './app-initializer';
import { API_BASE_URL as IdentityUrl } from '@shared/service-proxies/identity-service-proxies';
import { API_BASE_URL as ProductUrl } from '@shared/service-proxies/product-service-proxies';
import { CustomHttpInterceptor } from '@shared/service-proxies/http-interceptor.service';

@NgModule({
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    SharedModule.forRoot(),
    ModalModule.forRoot(),
    BsDropdownModule.forRoot(),
    CollapseModule.forRoot(),
    TabsModule.forRoot(),
    ServiceProxyModule,
    RootRoutingModule,
  ],
  declarations: [RootComponent],
  providers: [
    provideHttpClient(withInterceptorsFromDi()),
    { provide: HTTP_INTERCEPTORS, useClass: CustomHttpInterceptor, multi: true },
    {
      provide: APP_INITIALIZER,
      useFactory: (appInitializer: AppInitializer) => appInitializer.init(),
      deps: [AppInitializer],
      multi: true,
    },
    { provide: ProductUrl, useFactory: () => AppConsts.remoteServiceBaseUrl },
    { provide: IdentityUrl, useFactory: () => AppConsts.remoteServiceBaseUrl },
  ],
  bootstrap: [RootComponent],
})
export class RootModule { }
