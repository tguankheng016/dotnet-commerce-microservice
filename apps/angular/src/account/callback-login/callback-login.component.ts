import { Component, Injector, OnInit } from "@angular/core";
import { Router, ActivatedRoute } from "@angular/router";
import { AppComponentBase } from "@shared/app-component-base";
import { AppConsts } from "@shared/app-consts";
import { AppAuthService } from "@shared/auth/app-auth.service";
import { StringService } from "@shared/utils/string.service";
import { CookieService } from "ngx-cookie-service";

@Component({
    template: `
        <div class="login-form">
            <div class="alert {{ alertClass }} text-center" role="alert">
                <div class="alert-text">{{ waitMessage }} <i class="fa fa-spin fa-spinner alert-success"></i></div>
            </div>
        </div>
    `,
})
export class CallbackLoginComponent extends AppComponentBase implements OnInit {
    waitMessage: string;
    alertClass = 'alert-success';

    constructor(
        injector: Injector,
        public authService: AppAuthService,
        private _router: Router,
        private _activatedRoute: ActivatedRoute,
        private _stringService: StringService,
        private _cookieService: CookieService
    ) {
        super(injector);
    }

    ngOnInit(): void {
        this.waitMessage = 'Please wait while we confirm your login';

        let stateKey = AppConsts.cookieName.statekey;
        let code = this._activatedRoute.snapshot.queryParams['code'];
        let state = this._activatedRoute.snapshot.queryParams['state'];
        let error_description = this._activatedRoute.snapshot.queryParams['error_description'];

        if (this._stringService.notNullOrEmpty(error_description)) {
            appHelper.notify.error(error_description);
            this._router.navigate(['account/login']);
            return;
        }

        let savedState = this._cookieService.get(stateKey);

        if (!this._stringService.notNullOrEmpty(state) || savedState !== state) {
            appHelper.notify.error('Invalid state');
            this._router.navigate(['account/login']);
            return;
        }

        this.authService.openIddictAuthenticate(
            code,
            AppConsts.appBaseUrl + '/account/callback/login',
            () => {
                this._cookieService.delete(stateKey, AppConsts.cookiePath);
            }
        );
    }
}