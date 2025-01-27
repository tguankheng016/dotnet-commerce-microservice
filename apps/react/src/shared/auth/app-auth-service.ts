import { AppConsts } from "@shared/app-consts";
import { CookieService } from "@shared/cookies/cookie-service";
import { APIClient } from "@shared/service-proxies";
import { AuthenticateRequest, AuthenticateResult, OAuthAuthenticateRequest, RefreshTokenRequest } from "@shared/service-proxies/identity-service-proxies";

export class AppAuthService {
    authenticateRequest = new AuthenticateRequest();
    identityService = APIClient.getIdentityService();

    refreshToken(): Promise<boolean> {
        return new Promise((resolve) => {
            const refreshToken = CookieService.getCookie(AppConsts.cookieName.refreshToken);

            if (!refreshToken) {
                return resolve(false);
            }

            const request = new RefreshTokenRequest();
            request.token = refreshToken;

            this.identityService.refreshToken(request)
                .then(res => {
                    const tokenResult = res;

                    if (tokenResult && tokenResult.accessToken) {
                        CookieService.setCookie(AppConsts.cookieName.accessToken, tokenResult.accessToken, tokenResult.expireInSeconds);
                        return resolve(true)
                    } else {
                        return resolve(false);
                    }
                })
                .catch(err => {
                    CookieService.removeCookie(AppConsts.cookieName.refreshToken);
                    return resolve(false);
                })
        });
    }

    signOut(): void {
        this.identityService.signOut()
            .then(() => {
                CookieService.removeCookie(AppConsts.cookieName.accessToken);
                CookieService.removeCookie(AppConsts.cookieName.refreshToken);
                location.reload();
            })
    }

    authenticate(finallyCallback?: () => void, redirectUrl?: string): void {
        this.identityService
            .authenticate(this.authenticateRequest)
            .then((res) => {
                this.processAuthenticateResult(res, redirectUrl);
            })
            .finally(() => {
                if (finallyCallback)
                    finallyCallback();
            });
    }

    openIddictAuthenticate(code: string,
        redirectUrl: string,
        finallyCallback?: () => void,
        signal?: AbortSignal
    ) {
        const model = new OAuthAuthenticateRequest();
        model.code = code;
        model.redirectUri = redirectUrl;

        this.identityService
            .oAuthAuthenticate(model, signal)
            .then((res) => {
                this.processAuthenticateResult(res);
            })
            .finally(() => {
                if (finallyCallback)
                    finallyCallback();
            });
    }

    private processAuthenticateResult(
        authenticateResult: AuthenticateResult,
        redirectUrl?: string
    ) {
        const authResult = authenticateResult;

        if (authenticateResult.accessToken) {
            // Successfully logged in
            if (authResult.accessToken)
                CookieService.setCookie(AppConsts.cookieName.accessToken, authResult.accessToken, authResult.expireInSeconds)

            if (authResult.refreshToken) {
                CookieService.setCookie(AppConsts.cookieName.refreshToken, authResult.refreshToken, authResult.refreshTokenExpireInSeconds)
            }

            this.redirectToLoginResult(redirectUrl);
        } else {
            // Unexpected result!
        }
    }

    private redirectToLoginResult(redirectUrl?: string): void {
        if (redirectUrl) {
            location.href = redirectUrl;
        } else {
            let initialUrl = location.href;

            if (initialUrl.indexOf('/login') > 0) {
                initialUrl = import.meta.env.VITE_APP_BASE_URL;
            }

            if (initialUrl.indexOf('/account/callback') > 0) {
                initialUrl = import.meta.env.VITE_APP_BASE_URL;
            }

            location.href = initialUrl;
        }
    }
}