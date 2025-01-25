import { ChangeDetectionStrategy, ChangeDetectorRef, Component, Injector, OnInit, ViewEncapsulation } from "@angular/core";
import { NavigationEnd, PRIMARY_OUTLET, Router } from "@angular/router";
import { AppComponentBase } from "@shared/app-component-base";
import { AppMenuItem } from "@shared/layout/app-menu-item";
import { AppNavigationService } from "@shared/layout/app-navigation.service";

@Component({
    selector: 'sidebar-menu',
    templateUrl: './sidebar-menu.component.html',
    styleUrls: ['./sidebar-menu.component.css'],
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class SidebarMenuComponent extends AppComponentBase implements OnInit {
    menuItems: AppMenuItem[];
    menuItemsMap: { [key: number]: AppMenuItem } = {};
    activatedMenuItems: AppMenuItem[] = [];

    constructor(
        injector: Injector,
        private router: Router,
        private _appNavigationService: AppNavigationService,
        private _cdRef: ChangeDetectorRef
    ) {
        super(injector);
    }

    ngOnInit(): void {
        this.menuItems = this._appNavigationService.getMenuItems();
        this.patchMenuItems(this.menuItems);

        this.router.events.subscribe((event: NavigationEnd) => {
            const currentUrl = this.router.url.split(/[?#]/)[0];
            const primaryUrlSegmentGroup = this.router.parseUrl(currentUrl).root
                .children[PRIMARY_OUTLET];
            if (primaryUrlSegmentGroup) {
                this.activateMenuItems('/' + primaryUrlSegmentGroup.toString());
                this._cdRef.markForCheck();
            }
        });
    }

    patchMenuItems(items: AppMenuItem[], parentId?: number): void {
        items.forEach((item: AppMenuItem, index: number) => {
            item.id = parentId ? Number(parentId + '' + (index + 1)) : index + 1;
            if (parentId) {
                item.parentId = parentId;
            }
            if (parentId || item.children) {
                this.menuItemsMap[item.id] = item;
            }
            if (item.children) {
                this.patchMenuItems(item.children, item.id);
            }
        });
    }

    activateMenuItems(url: string): void {
        this.deactivateMenuItems(this.menuItems);
        this.activatedMenuItems = [];
        const foundedItems = this.findMenuItemsByUrl(url, this.menuItems);
        foundedItems.forEach((item) => {
            this.activateMenuItem(item);
        });
    }

    deactivateMenuItems(items: AppMenuItem[]): void {
        items.forEach((item: AppMenuItem) => {
            item.isActive = false;
            item.isCollapsed = true;
            if (item.children) {
                this.deactivateMenuItems(item.children);
            }
        });
    }

    findMenuItemsByUrl(
        url: string,
        items: AppMenuItem[],
        foundedItems: AppMenuItem[] = []
    ): AppMenuItem[] {
        items.forEach((item: AppMenuItem) => {
            if (item.route != '' && url.indexOf(item.route) !== -1) {
                foundedItems.push(item);
            } else if (item.children) {
                this.findMenuItemsByUrl(url, item.children, foundedItems);
            }
        });
        return foundedItems;
    }

    activateMenuItem(item: AppMenuItem): void {
        item.isActive = true;
        if (item.children) {
            item.isCollapsed = false;
        }
        this.activatedMenuItems.push(item);
        if (item.parentId) {
            this.activateMenuItem(this.menuItemsMap[item.parentId]);
        }
    }

    isMenuItemVisible(item: AppMenuItem): boolean {
        return this._appNavigationService.isMenuItemVisible(item);
    }

    trackByMenuItem(index: number, item: AppMenuItem) {
        return item.id;
    }
}