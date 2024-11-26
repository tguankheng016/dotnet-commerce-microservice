import { PlatformLocation } from "@angular/common";
import { HttpClient } from "@angular/common/http";
import { Injectable, Injector } from "@angular/core";
import { AppConsts } from "@shared/app-consts";
import { AppSessionService } from "@shared/session/app-session.service";
import { LocalStorageService } from "@shared/utils/local-storage.service";
import { StringService } from "@shared/utils/string.service";
import { environment } from "environments/environment";
import localForage from 'localforage';

@Injectable({
    providedIn: 'root',
})
export class AppInitializer {
    constructor(
        private _injector: Injector,
        private _platformLocation: PlatformLocation,
        private _httpClient: HttpClient
    ) { }

    init(): () => Promise<boolean> {
        return () => {
            appHelper.ui.setBusy();
            return new Promise<boolean>((resolve, reject) => {
                AppConsts.appBaseHref = this.getBaseHref();
                const appBaseUrl = this.getDocumentOrigin() + AppConsts.appBaseHref;
                
                this.initializeLocalForage();

                this.getApplicationConfig(appBaseUrl, () => {
                    let callback = () => {
                        // do not use constructor injection for AppSessionService
                        const appSessionService = this._injector.get(AppSessionService);
                        appSessionService.init().then(
                            (result) => {
                                appHelper.ui.clearBusy();
                                resolve(result);
                            },
                            (err) => {
                                reject(err);
                            }
                        );
                    };
                    
                    this.setTheme(callback);
                });
            });
        };
    }

    private getApplicationConfig(appRootUrl: string, callback: () => void) {
        this._httpClient
            .get<any>(`${appRootUrl}assets/${environment.appConfig}`)
            .subscribe((response) => {
                AppConsts.appBaseUrl = response.appBaseUrl;
                AppConsts.remoteServiceBaseUrl = response.remoteServiceBaseUrl;
                AppConsts.openIddictUrl = response.openIddict.baseUrl;
                AppConsts.openIddictClientId = response.openIddict.clientId;
                AppConsts.uiAvatarsBaseUrl = response.uiAvatarsBaseUrl;

                callback();
            });
    }

    private setTheme(callback: () => void) {
        let localStorageService = new LocalStorageService();
        let stringService = new StringService();
        let self = this;

        localStorageService.getItem(AppConsts.themeStorageKey, function (err, value) {
            if (!stringService.notNullOrEmpty(value)) {
                value = 'dark'
            }

            document.documentElement.setAttribute(AppConsts.themeStorageKey, value);

            localStorageService.setItem(AppConsts.themeStorageKey, value, self.loadStyles(value, callback));
        });
    }

    private loadStyles(theme: string, callback: () => void) {
        let styleUrls = [
            AppConsts.appBaseUrl + '/assets/primeng/themes/mdc-' + theme + '-indigo/theme.css', // PrimeNG Dark mode styles
            AppConsts.appBaseUrl + '/assets/primeng/primeng-customize.min.css',
            AppConsts.appBaseUrl + '/assets/primeng/primeng-customize-' + theme + '.min.css'
        ];

        let promises: any[] = [];
        styleUrls.forEach((styleName) => promises.push(
            new Promise((resolve, reject) => {
                let style = document.createElement('link') as any;
                style.type = 'text/css';
                style.rel = 'stylesheet';
                style.href = styleName;
    
                if (style.readyState) {
                    //IE
                    style.onreadystatechange = () => {
                        if (style.readyState === 'loaded' || style.readyState === 'complete') {
                            style.onreadystatechange = null;
                            resolve({ style: styleName, loaded: true, status: 'Loaded' });
                        }
                    };
                } else {
                    //Others
                    style.onload = () => {
                        resolve({ style: styleName, loaded: true, status: 'Loaded' });
                    };
                }
    
                style.onerror = (error: any) => resolve({ style: styleName, loaded: false, status: 'Loaded' });
                document.getElementsByTagName('head')[0].appendChild(style);
            })
        ));

        Promise.all(promises).then(() => {
            callback();
        });
    }

    private getBaseHref(): string {
        const baseUrl = this._platformLocation.getBaseHrefFromDOM();
        if (baseUrl) {
            return baseUrl;
        }

        return '/';
    }

    private getDocumentOrigin(): string {
        if (!document.location.origin) {
            const port = document.location.port ? ':' + document.location.port : '';
            return (
                document.location.protocol + '//' + document.location.hostname + port
            );
        }

        return document.location.origin;
    }

    private initializeLocalForage() {
        localForage.config({
            driver: localForage.LOCALSTORAGE,
            name: 'FlightBookingPortal',
            version: 1.0,
            storeName: 'flight_booking_portal_local_storage',
            description: 'Cached data for Flight Booking Portal',
        });
    }
}