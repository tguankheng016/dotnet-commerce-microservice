import { HttpErrorResponse, HttpEvent, HttpHandler, HttpHeaders, HttpInterceptor, HttpRequest } from "@angular/common/http";
import { Injectable, Injector } from "@angular/core";
import { AppConsts } from "@shared/app-consts";
import { RefreshTokenService } from "@shared/auth/refresh-token.service";
import { CookieService } from "ngx-cookie-service";
import { BehaviorSubject, catchError, filter, map, Observable, of, switchMap, take, throwError } from "rxjs";

export class ApiException {
    status: number;
    detail: string;
}

@Injectable({
    providedIn: 'root',
})
export class CustomHttpInterceptor implements HttpInterceptor {
    private isRefreshing = false;
    private refreshTokenSubject: BehaviorSubject<any> = new BehaviorSubject<any>(null);

    constructor(
        private _injector: Injector,
        private _cookieService: CookieService
    ) {
    }

    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        var modifiedRequest = this.normalizeRequestHeaders(request);

        return next.handle(modifiedRequest)
            .pipe(
                catchError(error => {
                    if (error instanceof HttpErrorResponse && error.status === 401) {
                        return this.tryAuthWithRefreshToken(request, next, error);
                    } else {
                        return this.handleErrorResponse(error);
                    }
                }),
                switchMap((event) => {
                    return of(event);
                })
            );
    }

    protected tryGetRefreshTokenService(): Observable<boolean> {
        let _refreshTokenService = this._injector.get(RefreshTokenService, null);

        if (_refreshTokenService) {
            return _refreshTokenService.tryAuthWithRefreshToken();
        }

        return of(false);
    }

    private tryAuthWithRefreshToken(request: HttpRequest<any>, next: HttpHandler, error: any) {
        if (!this.isRefreshing) {
            this.isRefreshing = true;
            this.refreshTokenSubject.next(null);

            return this.tryGetRefreshTokenService().pipe(
                switchMap((authResult: boolean) => {
                    this.isRefreshing = false;
                    if (authResult) {
                        this.refreshTokenSubject.next(authResult);
                        let modifiedRequest = this.normalizeRequestHeaders(request);
                        return next.handle(modifiedRequest);
                    } else {
                        return this.handleErrorResponse(error);
                    }
                }));
        } else {
            return this.refreshTokenSubject.pipe(
                filter(authResult => authResult != null),
                take(1),
                switchMap(authResult => {
                    let modifiedRequest = this.normalizeRequestHeaders(request);
                    return next.handle(modifiedRequest);
                }));
        }
    }

    protected normalizeRequestHeaders(request: HttpRequest<any>): HttpRequest<any> {
        var modifiedHeaders = new HttpHeaders();
        modifiedHeaders = request.headers.set("Pragma", "no-cache")
            .set("Cache-Control", "no-cache")
            .set("Expires", "Sat, 01 Jan 2000 00:00:00 GMT");

        modifiedHeaders = this.addXRequestedWithHeader(modifiedHeaders);
        modifiedHeaders = this.addAuthorizationHeaders(modifiedHeaders);

        return request.clone({
            headers: modifiedHeaders
        });
    }

    protected addXRequestedWithHeader(headers: HttpHeaders): HttpHeaders {
        if (headers) {
            headers = headers.set('X-Requested-With', 'XMLHttpRequest');
        }

        return headers;
    }

    protected addAuthorizationHeaders(headers: HttpHeaders): HttpHeaders {
        let authorizationHeaders = headers ? headers.getAll('Authorization') : null;
        if (!authorizationHeaders) {
            authorizationHeaders = [];
        }

        if (!this.itemExists(authorizationHeaders, (item: string) => item.indexOf('Bearer ') == 0)) {
            let token = this._cookieService.get(AppConsts.cookieName.accessToken);

            if (headers && token) {
                headers = headers.set('Authorization', 'Bearer ' + token);
            }
        }
        return headers;
    }

    protected handleErrorResponse(error: any): Observable<never> {
        if (!(error.error instanceof Blob)) {
            return throwError(() => error);
        }

        const blob = error.error;
    
        // Convert blob to a readable format
        const reader = new FileReader();
        
        reader.onload = () => {
            const errorText = reader.result as string;
            const apiError = JSON.parse(errorText) as ApiException;

            appHelper.message.error(apiError.detail);
        };
        
        reader.readAsText(blob);

        return throwError(() => error);
    }

    private itemExists<T>(items: T[], predicate: (item: T) => boolean): boolean {
        for (let i = 0; i < items.length; i++) {
            if (predicate(items[i])) {
                return true;
            }
        }

        return false;
    }
}