import { Injectable } from "@angular/core";
import { Router, ActivatedRouteSnapshot, RouterStateSnapshot } from "@angular/router";
import { AppConsts } from "@shared/app-consts";
import { AppSessionService } from "@shared/session/app-session.service";

@Injectable()
export class AccountRouteGuard {
    constructor(
        private _router: Router,
        private _sessionService: AppSessionService
    ) {}

    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
        if (route.queryParams['ss'] && route.queryParams['ss'] === 'true') {
            return true;
        }

        if (this._sessionService.user) {
            this._router.navigate([this.selectBestRoute()]);
            return false;
        }

        return true;
    }

    selectBestRoute(): string {
        return '/app/home';
    }

    urlContains(url, urlToSkip): boolean {
        return url && url.indexOf(urlToSkip) >= 0;
    }
}