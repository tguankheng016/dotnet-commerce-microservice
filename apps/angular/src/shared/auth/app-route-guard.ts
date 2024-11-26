import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, Router, RouterStateSnapshot } from "@angular/router";
import { AppSessionService } from "@shared/session/app-session.service";
import { Observable, of, Subject } from "rxjs";
import { RefreshTokenService } from "./refresh-token.service";

@Injectable()
export class AppRouteGuard {
    constructor(
        //private _permissionChecker: PermissionCheckerService,
        private _router: Router,
        private _sessionService: AppSessionService,
        private _refreshTokenService: RefreshTokenService
    ) { }

    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> {
        if (!this._sessionService.user) {
            let sessionObservable = new Subject<any>();

            this._refreshTokenService.tryAuthWithRefreshToken().subscribe(
                (autResult: boolean) => {
                    if (autResult) {
                        sessionObservable.next(true);
                        sessionObservable.complete();
                        location.reload();
                    } else {
                        sessionObservable.next(false);
                        sessionObservable.complete();
                        this._router.navigate(['/account/login']);
                    }
                },
                (error) => {
                    sessionObservable.next(false);
                    sessionObservable.complete();
                    this._router.navigate(['/account/login']);
                }
            );

            return sessionObservable;
        }
        if (!this._sessionService.user) {
            this._router.navigate(['/account/login']);
            return of(false);
        }

        if (!route.data || !route.data['permission']) {
            return of(true);
        }

        if (appHelper.auth.isGranted(route.data['permission'])) {
            return of(true);
        }

        this._router.navigate([this.selectBestRoute()]);
        return of(false);
    }

    canActivateChild(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> {
        return this.canActivate(route, state);
    }

    canLoad(route: any): Observable<boolean> | Promise<boolean> | boolean {
        return this.canActivate(route.data, null);
    }

    selectBestRoute(): string {
        if (!this._sessionService.user) {
            return '/account/login';
        }

        return '/app/about';
    }
}