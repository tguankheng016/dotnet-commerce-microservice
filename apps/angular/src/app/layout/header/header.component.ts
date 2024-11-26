import { Component, HostListener, Injector, OnInit, Renderer2 } from "@angular/core";
import { AppComponentBase } from "@shared/app-component-base";
import { LayoutStoreService } from "@shared/layout/layout-store.service";
import { Subscription } from "rxjs";

@Component({
    selector: 'app-header',
    templateUrl: './header.component.html'
})
export class HeaderComponent extends AppComponentBase implements OnInit {
    sidebarExpanded = true;
    subscription = new Subscription();

    constructor(
        injector: Injector,
        private renderer: Renderer2,
        private _layoutStore: LayoutStoreService
    ) {
        super(injector);
    }

    @HostListener('window:resize', ['$event'])
    onResize(event: Event) {
        this.checkView();
    }

    ngOnInit(): void {
        this.checkView();

        let sub = this._layoutStore.sidebarExpanded.subscribe((value) => {
            this.sidebarExpanded = value;
        });

        this.subscription.add(sub);
    }

    ngOnDestroy() {
        this.subscription.unsubscribe();
    }

    expandSidebar(): void {
        if (this.sidebarExpanded) {
            this.renderer.setAttribute(document.body, 'data-kt-app-sidebar-minimize', 'on');
        } else {
            this.renderer.removeAttribute(document.body, 'data-kt-app-sidebar-minimize');
        }

        this.sidebarExpanded = !this.sidebarExpanded;
    }

    expandSidebarMobile(): void {
        this.sidebarExpanded = !this.sidebarExpanded;

        this._layoutStore.setSidebarExpanded(this.sidebarExpanded);
    }

    private checkView(): void {
        // Assuming mobile view is defined as less than or equal to 768px
        let mobileView = window.innerWidth <= 768;

        if (mobileView) {
            this.sidebarExpanded = false;
            this._layoutStore.setSidebarExpanded(this.sidebarExpanded);
            this.renderer.removeAttribute(document.body, 'data-kt-app-sidebar-minimize');
        } else {
            this.sidebarExpanded = true;
            this.renderer.removeAttribute(document.body, 'data-kt-drawer');
            this.renderer.removeAttribute(document.body, 'data-kt-drawer-app-sidebar');
        }
    }
}