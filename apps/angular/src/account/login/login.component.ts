import { ChangeDetectionStrategy, ChangeDetectorRef, Component, Injector, OnInit, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { accountModuleAnimation } from '@shared/animations/router-transition';
import { AppAuthService } from '@shared/auth/app-auth.service';
import { NgForm } from '@angular/forms';
import { CookieService } from 'ngx-cookie-service';
import { AppConsts } from '@shared/app-consts';

@Component({
    templateUrl: './login.component.html',
    animations: [accountModuleAnimation()],
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class LoginComponent extends AppComponentBase implements OnInit {
    @ViewChild('loginForm', { static: false }) loginForm: NgForm;

    submitting = false;

    constructor(
        injector: Injector,
        private _cdRef: ChangeDetectorRef,
        public authService: AppAuthService,
        private _cookieService: CookieService
    ) {
        super(injector);
    }

    ngOnInit(): void {
        this.authService.authenticateRequest.usernameOrEmailAddress = 'admin';
        this.authService.authenticateRequest.password = '123qwe';
    }

    login(): void {
        appHelper.message.confirm('This site is available Monday to Friday, from 8 AM to 8 PM.', 'Site Availability Hours', (isConfirmed) => {
            if (isConfirmed) {
                this.submitting = true;

                if (this.validateFormGroup(this.loginForm.form)) {
                    this.authService.authenticate(
                        () => {
                            this.submitting = false;
                            this._cdRef.markForCheck();
                        },
                        null,
                    );
                }
                else {
                    this.submitting = false;
                }
            }
        }, false, { confirmButtonText: "Ok" });
    }

    ssoLogin(): void {
        const state = this.stringService.randomString(15);
        this._cookieService.set(AppConsts.cookieName.statekey, state, undefined, AppConsts.cookiePath);

        const redirectUrl = AppConsts.openIddictUrl
            + '/connect/authorize?client_id='
            + AppConsts.openIddictClientId
            + '&redirect_uri=' + AppConsts.appBaseUrl + '/account/callback/login'
            + '&scope=email profile'
            + '&response_type=code&response_mode=query'
            + '&state=' + state;

        location.href = redirectUrl;
    }
}
