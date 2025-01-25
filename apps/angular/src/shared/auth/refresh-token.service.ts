import { Injectable } from "@angular/core";
import { AppConsts } from "@shared/app-consts";
import { IdentityServiceProxy, RefreshTokenRequest, RefreshTokenResult } from "@shared/service-proxies/identity-service-proxies";
import { CookieService } from "ngx-cookie-service";
import { Observable, Subject, of } from "rxjs";

@Injectable({
    providedIn: 'root',
})
export class RefreshTokenService {
    constructor(
        private _identityService: IdentityServiceProxy,
        private _cookieService: CookieService
    ) { }

    tryAuthWithRefreshToken(): Observable<boolean> {
        let refreshTokenObservable = new Subject<boolean>();

        let token = this._cookieService.get(AppConsts.cookieName.refreshToken);
        if (!token || token.trim() === '') {
            return of(false);
        }

        let request = new RefreshTokenRequest();
        request.token = token;

        this._identityService.refreshToken(request).subscribe(
            (response: RefreshTokenResult) => {
                let tokenResult = response;

                if (tokenResult && tokenResult.accessToken) {
                    let tokenExpireDate = new Date(new Date().getTime() + 1000 * tokenResult.expireInSeconds);
                    this._cookieService.set(AppConsts.cookieName.accessToken, tokenResult.accessToken, tokenExpireDate, AppConsts.cookiePath);

                    refreshTokenObservable.next(true)

                } else {
                    refreshTokenObservable.next(false);
                }
            },
            (error: any) => {
                refreshTokenObservable.next(false);
            }
        );
        return refreshTokenObservable;
    }
}