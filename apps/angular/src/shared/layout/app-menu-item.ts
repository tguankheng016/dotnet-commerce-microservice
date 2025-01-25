export class AppMenuItem {
    id: number;
    parentId: number;
    label: string;
    route: string;
    icon: string;
    permissionName: string;
    isActive?: boolean;
    isCollapsed?: boolean;
    children: AppMenuItem[];

    constructor(
        label: string,
        route: string,
        icon: string,
        permissionName: string = null,
        children: AppMenuItem[] = null
    ) {
        this.label = label;
        this.route = route;
        this.icon = icon;
        this.permissionName = permissionName;
        this.children = children;
    }
}