import { Injectable } from "@angular/core";
import { Router } from "@angular/router";
import { AppConsts } from "@shared/app-consts";
import { UrlHelper } from "@shared/helpers/url-helper";
import { AuthenticateRequest, AuthenticateResult, IdentityServiceProxy, OAuthAuthenticateRequest, OAuthAuthenticateResult } from "@shared/service-proxies/identity-service-proxies";
import { CookieService } from "ngx-cookie-service";
import { finalize } from "rxjs";


@Injectable()
export class AppAuthService {
    authenticateRequest: AuthenticateRequest;
    authenticateResult: AuthenticateResult;

    constructor(
        private _identityService: IdentityServiceProxy,
        private _cookieService: CookieService,
        private _router: Router
    ) {
        this.authenticateRequest = new AuthenticateRequest();
    }

    signOut(reload?: boolean, returnUrl?: string): void {
        this._identityService.signOut()
            .subscribe(() => {
                this._cookieService.delete(AppConsts.cookieName.accessToken, AppConsts.cookiePath);
                this._cookieService.delete(AppConsts.cookieName.refreshToken, AppConsts.cookiePath);

                if (reload !== false) {
                    if (returnUrl) {
                        location.href = returnUrl;
                    } else {
                        location.href = '';
                    }
                }
            });
    }

    openIddictAuthenticate(code: string,
        redirectUrl: string,
        finallyCallback?: () => void
    ) {
        const model = new OAuthAuthenticateRequest();
        model.code = code;
        model.redirectUri = redirectUrl;

        this._identityService
            .oAuthAuthenticate(model)
            .pipe(
                finalize(() => {
                    finallyCallback();
                })
            )
            .subscribe((res: OAuthAuthenticateResult) => {
                this.processAuthenticateResult(res);
            });
    }

    authenticate(finallyCallback?: () => void, redirectUrl?: string): void {
        this._identityService
            .authenticate(this.authenticateRequest)
            .pipe(
                finalize(() => {
                    finallyCallback();
                })
            )
            .subscribe((result: AuthenticateResult) => {
                this.processAuthenticateResult(result, redirectUrl);
            });
    }

    private processAuthenticateResult(
        authenticateResult: AuthenticateResult,
        redirectUrl?: string
    ) {
        let authResult = authenticateResult;

        if (authenticateResult.accessToken) {
            // Successfully logged in
            const tokenExpireDate = new Date(new Date().getTime() + 1000 * authResult.expireInSeconds);

            this._cookieService.set(AppConsts.cookieName.accessToken, authResult.accessToken, tokenExpireDate, AppConsts.cookiePath);

            if (authResult.refreshToken) {
                let refreshTokenExpireDate = new Date(new Date().getTime() + 1000 * authResult.refreshTokenExpireInSeconds);
                this._cookieService.set(AppConsts.cookieName.refreshToken, authResult.refreshToken, refreshTokenExpireDate, AppConsts.cookiePath);
            }

            this.redirectToLoginResult(redirectUrl);
        } else {
            // Unexpected result!
            this._router.navigate(['account/login']);
        }
    }

    private redirectToLoginResult(redirectUrl?: string): void {
        if (redirectUrl) {
            location.href = redirectUrl;
        } else {
            let initialUrl = UrlHelper.initialUrl;

            if (initialUrl.indexOf('/login') > 0) {
                initialUrl = AppConsts.appBaseUrl;
            }

            if (initialUrl.indexOf('/account/oauth') > 0) {
                initialUrl = AppConsts.appBaseUrl;
            }

            location.href = initialUrl;
        }
    }
}