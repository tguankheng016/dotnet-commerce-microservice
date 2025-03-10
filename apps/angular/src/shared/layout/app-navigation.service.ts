import { Injectable } from "@angular/core";
import { StringService } from "@shared/utils/string.service";
import { AppMenuItem } from "./app-menu-item";


@Injectable()
export class AppNavigationService {
    constructor(
        private _stringService: StringService
    ) { }

    getMenuItems(): AppMenuItem[] {
        return [
            new AppMenuItem('About', '/app/about', 'fas fa-info-circle'),
            new AppMenuItem('Categories', '/app/main/categories', 'fas fa-tags', 'Pages.Categories'),
            new AppMenuItem('Products', '/app/main/products', 'fas fa-store', 'Pages.Products'),
            new AppMenuItem('Administration', '', 'fas fa-tasks', '', [
                new AppMenuItem('Roles', '/app/admin/roles', 'fas fa-layer-group', 'Pages.Administration.Roles'),
                new AppMenuItem('Users', '/app/admin/users', 'fas fa-users', 'Pages.Administration.Users'),
                new AppMenuItem('Audit Logs', '/app/admin/audit-logs', 'far fa-file-alt', 'Pages.Administration.AuditLogs')
            ]),
        ];
    }

    isMenuItemVisible(item: AppMenuItem): boolean {
        if (!item.permissionName && !item.children) {
            return true;
        }

        if (!item.permissionName && item.children) {
            return item.children.some(n => appHelper.auth.isGranted(n.permissionName));
        }

        return appHelper.auth.isGranted(item.permissionName);
    }
}