import { Component, Injector, OnInit, TemplateRef, ViewChild } from "@angular/core";
import { BreadcrumbItem } from "@app/layout/default-page.component";
import { appModuleAnimation } from "@shared/animations/router-transition";
import { AppComponentBase } from "@shared/app-component-base";
import { AppConsts } from "@shared/app-consts";
import { UserDto, UserServiceProxy } from "@shared/service-proxies/identity-service-proxies";
import { LazyLoadEvent, MenuItem } from "primeng/api";
import { Paginator } from "primeng/paginator";
import { Table } from "primeng/table";
import { finalize } from "rxjs";
import { CreateOrEditUserModalComponent } from "./create-or-edit-user-modal.component";
import { EditUserPermissionsModalComponent } from "./edit-user-permissions-modal.component";
import { PTableSkeletonTemplateComponent } from "@app/shared/ptable-skeleton-template.component";

@Component({
    templateUrl: './users.component.html',
    animations: [appModuleAnimation()]
})
export class UsersComponent extends AppComponentBase implements OnInit {
    @ViewChild('createOrEditUserModal', { static: true }) createOrEditUserModal: CreateOrEditUserModalComponent;
    @ViewChild('editUserPermissionsModal', { static: true }) editUserPermissionsModal: EditUserPermissionsModalComponent;
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;
    @ViewChild('skeletonTableTemplate', { static: true }) skeletonTableTemplate: PTableSkeletonTemplateComponent;

    //Filters
    advancedFiltersAreShown = false;
    filterText = '';

    breadcrumbs: BreadcrumbItem[] = [
        new BreadcrumbItem('Administration'),
        new BreadcrumbItem('Users')
    ];

    menuItems: MenuItem[];

    skeletonCols: number[] = [];
    currentTemplate!: TemplateRef<any>;

    constructor(
        injector: Injector,
        private _usersService: UserServiceProxy
    ) {
        super(injector);
    }

    ngOnInit(): void {
        this.primengTableHelper.setInitialRows();
        this.skeletonCols = this.primengTableHelper.getSkeletonCols(6);
        this.currentTemplate = this.skeletonTableTemplate.getTemplate();
    }

    getMenuItemsForItem(item: UserDto): MenuItem[] {
        return [
            {
                label: 'Edit',
                command: (event) => {
                    this.editUser(item);
                },
                visible: this.isGranted('Pages.Administration.Users.Edit')
            },
            {
                label: 'Permissions',
                command: (event) => {
                    this.changePermission(item);
                },
                visible: this.isGranted('Pages.Administration.Users.ChangePermissions')
            },
            {
                label: 'Delete',
                command: (event) => {
                    this.delete(item);
                },
                visible: this.isGranted('Pages.Administration.Users.Delete')
            }
        ];
    }

    getUsers(event?: LazyLoadEvent): void {
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);

            if (this.primengTableHelper.records && this.primengTableHelper.records.length > 0) {
                return;
            }
        }

        this.primengTableHelper.showLoadingIndicator();

        this._usersService
            .getUsers(
                this.primengTableHelper.getSkipCount(this.paginator, event),
                this.primengTableHelper.getMaxResultCount(this.paginator, event),
                this.filterText,
                this.primengTableHelper.getSorting(this.dataTable)
            )
            .pipe(finalize(() => this.primengTableHelper.hideLoadingIndicator()))
            .subscribe((res) => {
                this.primengTableHelper.totalRecordsCount = res.totalCount;
                this.primengTableHelper.records = res.items;
                this.setUsersProfilePictureUrl(this.primengTableHelper.records);
            });
    }

    reloadPage(): void {
        if (this.primengTableHelper.records && this.primengTableHelper.records.length > 0) {
            this.paginator.changePage(this.paginator.getPage());
        } else {
            this.getUsers();
        }
    }

    createUser(): void {
        this.createOrEditUserModal.show();
    }

    editUser(user: UserDto): void {
        this.createOrEditUserModal.show(user.id);
    }

    changePermission(user: UserDto): void {
        this.editUserPermissionsModal.show(user);
    }

    delete(user: UserDto): void {
        appHelper.message.confirm(this.stringService.formatString('User "{0}" will be deleted.', user.userName), 'Are you sure?', (isConfirmed) => {
            if (isConfirmed) {
                this._usersService.deleteUser(user.id).subscribe(() => {
                    this.reloadPage();
                    appHelper.notify.success('Successfully deleted');
                });
            }
        });
    }

    setUsersProfilePictureUrl(users: UserDto[]): void {
        for (let i = 0; i < users.length; i++) {
            let user = users[i];
            (user as any).profilePictureUrl = this.stringService.formatString(
                AppConsts.uiAvatarsBaseUrl,
                user.firstName + ' ' + user.lastName);
        }
    }
}