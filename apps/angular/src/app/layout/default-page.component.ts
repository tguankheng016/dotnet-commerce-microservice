import { ChangeDetectionStrategy, Component, Injector, Input } from "@angular/core";
import { NavigationExtras, Router } from "@angular/router";
import { appModuleAnimation } from "@shared/animations/router-transition";
import { AppComponentBase } from "@shared/app-component-base";

export class BreadcrumbItem {
    text: string;
    routerLink?: string;
    navigationExtras?: NavigationExtras;

    constructor(text: string, routerLink?: string, navigationExtras?: NavigationExtras) {
        this.text = text;
        this.routerLink = routerLink;
        this.navigationExtras = navigationExtras;
    }

    isLink(): boolean {
        return !!this.routerLink;
    }
}

@Component({
    selector: 'default-page',
    templateUrl: './default-page.component.html',
    animations: [appModuleAnimation()],
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class DefaultPageComponent extends AppComponentBase {
    @Input() title: string;
    @Input() breadcrumbs: BreadcrumbItem[];

    constructor(private _router: Router, injector: Injector) {
        super(injector);
    }

    goToBreadcrumb(breadcrumb: BreadcrumbItem): void {
        if (!breadcrumb.routerLink) {
            return;
        }

        if (breadcrumb.navigationExtras) {
            this._router.navigate([breadcrumb.routerLink], breadcrumb.navigationExtras);
        } else {
            this._router.navigate([breadcrumb.routerLink]);
        }
    }
}