import { ChangeDetectionStrategy, ChangeDetectorRef, Component, Injector, OnInit, TemplateRef, ViewChild } from "@angular/core";
import { BreadcrumbItem } from "@app/layout/default-page.component";
import { appModuleAnimation } from "@shared/animations/router-transition";
import { AppComponentBase } from "@shared/app-component-base";
import { RoleDto, RoleServiceProxy } from "@shared/service-proxies/identity-service-proxies";
import { LazyLoadEvent, MenuItem } from "primeng/api";
import { Paginator } from "primeng/paginator";
import { Table } from "primeng/table";
import { finalize } from "rxjs";
import { CreateOrEditRoleModalComponent } from "./create-or-edit-role-modal.component";
import { PTableSkeletonTemplateComponent } from "@app/shared/ptable-skeleton-template.component";

@Component({
    templateUrl: './roles.component.html',
    animations: [appModuleAnimation()],
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class RolesComponent extends AppComponentBase implements OnInit {
    @ViewChild('createOrEditRoleModal', { static: true }) createOrEditRoleModal: CreateOrEditRoleModalComponent;
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;
    @ViewChild('skeletonTableTemplate', { static: true }) skeletonTableTemplate: PTableSkeletonTemplateComponent;

    //Filters
    advancedFiltersAreShown = false;
    filterText = '';

    breadcrumbs: BreadcrumbItem[] = [
        new BreadcrumbItem('Administration'),
        new BreadcrumbItem('Roles')
    ];

    menuItems: MenuItem[];

    skeletonCols: number[] = [];
    currentTemplate!: TemplateRef<any>;

    constructor(
        injector: Injector,
        private _cdRef: ChangeDetectorRef,
        private _roleService: RoleServiceProxy
    ) {
        super(injector);

        this.primengTableHelper.setInitialRows();
        this.skeletonCols = this.primengTableHelper.getSkeletonCols(3);
    }

    ngOnInit(): void {
        this.currentTemplate = this.skeletonTableTemplate.getTemplate();
    }

    getMenuItemsForItem(item: RoleDto): MenuItem[] {
        return [
            {
                label: 'Edit',
                command: (event) => {
                    this.editRole(item);
                },
                visible: this.isGranted('Pages.Administration.Roles.Edit')
            },
            {
                label: 'Delete',
                command: (event) => {
                    this.delete(item);
                },
                visible: this.isGranted('Pages.Administration.Roles.Delete')
            }
        ];
    }

    getRoles(event?: LazyLoadEvent): void {
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);

            if (this.primengTableHelper.records && this.primengTableHelper.records.length > 0) {
                return;
            }
        }

        const loadingTimer = setTimeout(() => {
            this.primengTableHelper.showLoadingIndicator();
            this._cdRef.markForCheck();
        }, 200);

        this._roleService
            .getRoles(
                this.primengTableHelper.getSkipCount(this.paginator, event),
                this.primengTableHelper.getMaxResultCount(this.paginator, event),
                this.filterText,
                this.primengTableHelper.getSorting(this.dataTable)
            )
            .pipe(finalize(() => {
                clearTimeout(loadingTimer);
                this.primengTableHelper.hideLoadingIndicator();
                this._cdRef.markForCheck();
            }))
            .subscribe((res) => {
                this.primengTableHelper.totalRecordsCount = res.totalCount;
                this.primengTableHelper.records = res.items;
            });
    }

    reloadPage(): void {
        if (this.primengTableHelper.records && this.primengTableHelper.records.length > 0) {
            this.paginator.changePage(this.paginator.getPage());
        } else {
            this.getRoles();
        }
    }

    createRole(): void {
        this.createOrEditRoleModal.show();
    }

    editRole(role: RoleDto): void {
        this.createOrEditRoleModal.show(role.id);
    }

    delete(role: RoleDto): void {
        appHelper.message.confirm(this.stringService.formatString('Role "{0}" will be deleted.', role.name), 'Are you sure?', (isConfirmed) => {
            if (isConfirmed) {
                this._roleService.deleteRole(role.id).subscribe(() => {
                    this.reloadPage();
                    appHelper.notify.success('Successfully deleted');
                });
            }
        });
    }

    resetFilters(): void {
        this.filterText = '';
        this.getRoles();
    }
}