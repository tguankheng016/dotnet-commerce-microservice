import { ChangeDetectionStrategy, Component, ElementRef, HostListener, Injector, OnInit, Renderer2 } from "@angular/core";
import { NavigationEnd, NavigationStart, Router, RouterEvent } from "@angular/router";
import { AppComponentBase } from "@shared/app-component-base";
import { LayoutStoreService } from "@shared/layout/layout-store.service";
import { Subscription } from "rxjs";

@Component({
    selector: 'app-sidebar',
    templateUrl: './sidebar.component.html',
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class SidebarComponent extends AppComponentBase {
    mobileView = false;
    sidebarExpanded = false;
    subscriptions = new Subscription();

    constructor(
        injector: Injector,
        private renderer: Renderer2,
        private el: ElementRef,
        private layoutStore: LayoutStoreService,
        private router: Router
    ) {
        super(injector);
    }

    @HostListener('window:resize', ['$event'])
    onResize(event: Event) {
        this.checkView();
    }

    ngOnInit(): void {
        this.checkView();

        let layoutSub = this.layoutStore.sidebarExpanded.subscribe((value) => {
            this.sidebarExpanded = value;
            this.showSidebar();
        });

        let routerEventsSub = this.router.events.subscribe((event: NavigationEnd) => {
            this.hideSidebar();
        });

        this.subscriptions.add(layoutSub);
        this.subscriptions.add(routerEventsSub);
    }

    showSidebar(): void {
        if (this.mobileView && this.sidebarExpanded) {
            const element = this.el.nativeElement.querySelector('#kt_app_sidebar') as HTMLInputElement;
            this.renderer.addClass(element, 'drawer-on');
            const overlayDiv = this.renderer.createElement('div') as HTMLInputElement;
            overlayDiv.setAttribute('id', 'sidebarOverlayDiv');
            this.renderer.addClass(overlayDiv, 'drawer-overlay');
            this.renderer.setStyle(overlayDiv, 'z-index', '105');
            this.renderer.appendChild(document.body, overlayDiv);
            this.renderer.setAttribute(document.body, 'data-kt-drawer', 'on');
            this.renderer.setAttribute(document.body, 'data-kt-drawer-app-sidebar', 'on');
        }
    }

    hideSidebar(): void {
        if (this.mobileView && this.sidebarExpanded) {
            const element = this.el.nativeElement.querySelector('#kt_app_sidebar') as HTMLInputElement;
            this.renderer.removeClass(element, 'drawer-on');
            this.layoutStore.setSidebarExpanded(false);
            this.removeOverlayDiv();
            this.renderer.removeAttribute(document.body, 'data-kt-drawer');
            this.renderer.removeAttribute(document.body, 'data-kt-drawer-app-sidebar');
        }
    }

    removeOverlayDiv(): void {
        const overlayDiv = this.renderer.selectRootElement('#sidebarOverlayDiv') as HTMLInputElement;
        if (overlayDiv) {
            this.renderer.removeChild(document.body, overlayDiv);
        }
    }

    ngOnDestroy() {
        this.subscriptions.unsubscribe();
    }

    private checkView(): void {
        // Assuming mobile view is defined as less than or equal to 768px
        this.mobileView = window.innerWidth <= 768;

        const element = this.el.nativeElement.querySelector('#kt_app_sidebar') as HTMLInputElement;

        if (this.mobileView) {
            if (element) {
                // Setting the width with !important using JavaScript
                element.style.setProperty('width', '250px', 'important');
            }
        } else {
            element.style.removeProperty('width');
        }
    }
}