import { Component, Injector, OnInit, TemplateRef, ViewChild } from "@angular/core";
import { BreadcrumbItem } from "@app/layout/default-page.component";
import { appModuleAnimation } from "@shared/animations/router-transition";
import { AppComponentBase } from "@shared/app-component-base";
import { CategoryDto, CategoryServiceProxy, ProductDto, ProductServiceProxy } from "@shared/service-proxies/product-service-proxies";
import { LazyLoadEvent, MenuItem } from "primeng/api";
import { Paginator } from "primeng/paginator";
import { Table } from "primeng/table";
import { concatMap, finalize, forkJoin } from "rxjs";
import { CreateOrEditProductModalComponent } from "./create-or-edit-product-modal.component";
import { PTableSkeletonTemplateComponent } from "@app/shared/ptable-skeleton-template.component";

@Component({
    templateUrl: './products.component.html',
    animations: [appModuleAnimation()]
})
export class ProductsComponent extends AppComponentBase implements OnInit {
    @ViewChild('createOrEditProductModal', { static: true }) createOrEditProductModal: CreateOrEditProductModalComponent;
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;
    @ViewChild('skeletonTableTemplate', { static: true }) skeletonTableTemplate: PTableSkeletonTemplateComponent;

    //Filters
    advancedFiltersAreShown = false;
    filterText = '';

    breadcrumbs: BreadcrumbItem[] = [
        new BreadcrumbItem('Products')
    ];

    menuItems: MenuItem[];

    skeletonCols: number[] = [];
    currentTemplate!: TemplateRef<any>;

    categoryIdFilter: number;
    categories: CategoryDto[] = [];

    constructor(
        injector: Injector,
        private _productService: ProductServiceProxy,
        private _categoryService: CategoryServiceProxy
    ) {
        super(injector);
    }

    ngOnInit(): void {
        this.primengTableHelper.setInitialRows();
        this.skeletonCols = this.primengTableHelper.getSkeletonCols(5);
        this.currentTemplate = this.skeletonTableTemplate.getTemplate();

        const categories$ = this._categoryService.getCategories(0, 0, undefined, undefined);

        forkJoin([categories$])
            .subscribe((results) => {
                this.categories = results[0].items;
            });
    }

    getMenuItemsForItem(item: ProductDto): MenuItem[] {
        return [
            {
                label: 'Edit',
                command: (event) => {
                    this.editProduct(item);
                },
                visible: this.isGranted('Pages.Products.Edit')
            },
            {
                label: 'Delete',
                command: (event) => {
                    this.delete(item);
                },
                visible: this.isGranted('Pages.Products.Delete')
            }
        ];
    }

    getProducts(event?: LazyLoadEvent): void {
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);

            if (this.primengTableHelper.records && this.primengTableHelper.records.length > 0) {
                return;
            }
        }

        this.primengTableHelper.showLoadingIndicator();

        this._productService
            .getProducts(
                this.categoryIdFilter ?? undefined,
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
            this.getProducts();
        }
    }

    createProduct(): void {
        this.createOrEditProductModal.show();
    }

    editProduct(product: ProductDto): void {
        this.createOrEditProductModal.show(product.id);
    }

    delete(product: ProductDto): void {
        appHelper.message.confirm(this.stringService.formatString('Product "{0}" will be deleted.', product.name), 'Are you sure?', (isConfirmed) => {
            if (isConfirmed) {
                this._productService.deleteProduct(product.id).subscribe(() => {
                    this.reloadPage();
                    appHelper.notify.success('Successfully deleted');
                });
            }
        });
    }
}