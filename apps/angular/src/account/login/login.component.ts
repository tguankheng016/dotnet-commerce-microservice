import { Component, Injector, OnInit, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { accountModuleAnimation } from '@shared/animations/router-transition';
import { AppAuthService } from '@shared/auth/app-auth.service';
import { NgForm } from '@angular/forms';
import { CookieService } from 'ngx-cookie-service';
import { AppConsts } from '@shared/app-consts';

@Component({
    templateUrl: './login.component.html',
    animations: [accountModuleAnimation()]
})
export class LoginComponent extends AppComponentBase implements OnInit {
    @ViewChild('loginForm', { static: false }) loginForm: NgForm;

    submitting = false;
    
    constructor(
        injector: Injector,
        public authService: AppAuthService,
        private _cookieService: CookieService
    ) {
        super(injector);
    }

    ngOnInit(): void {
    }

    login(): void {
        this.submitting = true;

        if (this.validateFormGroup(this.loginForm.form)) {
            this.authService.authenticate(
                () => {
                    this.submitting = false;
                },
                null,
            );
        }
        else {
            this.submitting = false;
        }
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
