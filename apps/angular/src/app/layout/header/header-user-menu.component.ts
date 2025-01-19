import { ChangeDetectionStrategy, Component, Injector, OnInit } from "@angular/core";
import { AppComponentBase } from "@shared/app-component-base";
import { AppConsts } from "@shared/app-consts";
import { AppAuthService } from "@shared/auth/app-auth.service";
import { LocalStorageService } from "@shared/utils/local-storage.service";

@Component({
    selector: 'header-user-menu',
    templateUrl: './header-user-menu.component.html',
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class HeaderUserMenuComponent extends AppComponentBase implements OnInit {
    profilePicture = '';
    userName = '';
    emailAddress = '';
    isDarkMode = false;

    constructor(
        injector: Injector,
        private _authService: AppAuthService,
        private _localStorageService: LocalStorageService
    ) {
        super(injector);
    }

    ngOnInit(): void {
        this.profilePicture = this.getUiAvatarProfilePicture();
        this.setCurrentLoginInformations();
        this.setToggle();
    }

    setToggle(): void {
        this._localStorageService.getItemObs(AppConsts.themeStorageKey)
            .subscribe((res: string) => {
                this.isDarkMode = res === 'dark';
            });
    }

    setCurrentLoginInformations(): void {
        this.userName = this.appSession.user.userName;
        this.emailAddress = this.appSession.user.email;
    }

    toggleDarkMode(): void {
        let switchTheme = this.isDarkMode ? 'light' : 'dark';

        this._localStorageService.setItemObs(AppConsts.themeStorageKey, switchTheme)
            .subscribe(() => {
                location.reload();
            });
    }

    signOut(forceLogout: boolean = false): void {
        this._authService.signOut();
    }
}