import { Component, Injector, OnInit, TemplateRef, ViewChild } from "@angular/core";
import { BreadcrumbItem } from "@app/layout/default-page.component";
import { appModuleAnimation } from "@shared/animations/router-transition";
import { AppComponentBase } from "@shared/app-component-base";
import { CategoryDto, CategoryServiceProxy } from "@shared/service-proxies/product-service-proxies";
import { LazyLoadEvent, MenuItem } from "primeng/api";
import { Paginator } from "primeng/paginator";
import { Table } from "primeng/table";
import { finalize } from "rxjs";
import { CreateOrEditCategoryModalComponent } from "./create-or-edit-category-modal.component";
import { PTableSkeletonTemplateComponent } from "@app/shared/ptable-skeleton-template.component";

@Component({
    templateUrl: './categories.component.html',
    animations: [appModuleAnimation()]
})
export class CategoriesComponent extends AppComponentBase implements OnInit {
    @ViewChild('createOrEditCategoryModal', { static: true }) createOrEditCategoryModal: CreateOrEditCategoryModalComponent;
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;
    @ViewChild('skeletonTableTemplate', { static: true }) skeletonTableTemplate: PTableSkeletonTemplateComponent;

    //Filters
    advancedFiltersAreShown = false;
    filterText = '';

    breadcrumbs: BreadcrumbItem[] = [
        new BreadcrumbItem('Categories')
    ];

    menuItems: MenuItem[];

    skeletonCols: number[] = [];
    currentTemplate!: TemplateRef<any>;

    constructor(
        injector: Injector,
        private _categoryService: CategoryServiceProxy
    ) {
        super(injector);
    }

    ngOnInit(): void {
        this.primengTableHelper.setInitialRows();
        this.skeletonCols = this.primengTableHelper.getSkeletonCols(2);
        this.currentTemplate = this.skeletonTableTemplate.getTemplate();
    }

    getMenuItemsForItem(item: CategoryDto): MenuItem[] {
        return [
            {
                label: 'Edit',
                command: (event) => {
                    this.editCategory(item);
                },
                visible: this.isGranted('Pages.Categories.Edit')
            },
            {
                label: 'Delete',
                command: (event) => {
                    this.delete(item);
                },
                visible: this.isGranted('Pages.Categories.Delete')
            }
        ];
    }

    getCategories(event?: LazyLoadEvent): void {
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);

            if (this.primengTableHelper.records && this.primengTableHelper.records.length > 0) {
                return;
            }
        }

        this.primengTableHelper.showLoadingIndicator();

        this._categoryService
            .getCategories(
                this.primengTableHelper.getSkipCount(this.paginator, event),
                this.primengTableHelper.getMaxResultCount(this.paginator, event),
                this.filterText,
                this.primengTableHelper.getSorting(this.dataTable)
            )
            .pipe(finalize(() => this.primengTableHelper.hideLoadingIndicator()))
            .subscribe((res) => {
                this.primengTableHelper.totalRecordsCount = res.totalCount;
                this.primengTableHelper.records = res.items;
            });
    }

    reloadPage(): void {
        if (this.primengTableHelper.records && this.primengTableHelper.records.length > 0) {
            this.paginator.changePage(this.paginator.getPage());
        } else {
            this.getCategories();
        }
    }

    createCategory(): void {
        this.createOrEditCategoryModal.show();
    }

    editCategory(category: CategoryDto): void {
        this.createOrEditCategoryModal.show(category.id);
    }

    delete(category: CategoryDto): void {
        appHelper.message.confirm(this.stringService.formatString('Category "{0}" will be deleted.', category.categoryName), 'Are you sure?', (isConfirmed) => {
            if (isConfirmed) {
                this._categoryService.deleteCategory(category.id).subscribe(() => {
                    this.reloadPage();
                    appHelper.notify.success('Successfully deleted');
                });
            }
        });
    }
}