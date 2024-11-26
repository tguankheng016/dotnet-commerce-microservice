import { Component, Injector, OnInit } from "@angular/core";
import { AppComponentBase } from "@shared/app-component-base";
import { AppConsts } from "@shared/app-consts";
import { LocalStorageService } from "@shared/utils/local-storage.service";

@Component({
    templateUrl: './error-404.component.html'
})
export class Error404Component extends AppComponentBase implements OnInit {
    isDarkMode = false;

    constructor(
        injector: Injector,
        private _localStorageService: LocalStorageService
    ) {
        super(injector);
    }

    ngOnInit(): void {
        this._localStorageService.getItemObs(AppConsts.themeStorageKey)
            .subscribe((res: string) => {
                this.isDarkMode = res === 'dark';
            });
    }
}