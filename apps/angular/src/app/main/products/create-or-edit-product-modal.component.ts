import { Component, EventEmitter, Injector, Output, ViewChild, ViewEncapsulation } from "@angular/core";
import { NgForm } from "@angular/forms";
import { AppComponentBase } from "@shared/app-component-base";
import { CategoryDto, CategoryServiceProxy, CreateOrEditProductDto, CreateProductDto, EditProductDto, ProductServiceProxy } from "@shared/service-proxies/product-service-proxies";
import { plainToInstance } from "class-transformer";
import { ModalDirective } from "ngx-bootstrap/modal";
import { concatMap, finalize, forkJoin } from "rxjs";
import { forEach as _forEach } from 'lodash-es';

@Component({
    selector: 'createOrEditProductModal',
    templateUrl: './create-or-edit-product-modal.component.html',
    encapsulation: ViewEncapsulation.None
})
export class CreateOrEditProductModalComponent extends AppComponentBase {
    @ViewChild('productForm', { static: false }) productForm: NgForm;
    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;
    isEdit = false;

    product: CreateOrEditProductDto = new CreateOrEditProductDto();

    categories: CategoryDto[] = [];

    constructor(
        injector: Injector,
        private _productService: ProductServiceProxy,
        private _categoryService: CategoryServiceProxy
    ) {
        super(injector);
    }

    show(productId?: number): void {
        this.active = true;
        this.isEdit = this.stringService.notNullOrEmpty(productId);

        const categories$ = this._categoryService.getCategories(0, 0, undefined, undefined);
        const product$ = this._productService.getProductById(productId ?? 0);

        forkJoin([categories$]).pipe(
            concatMap(results => {
                this.categories = results[0].items;

                return product$;
            })
        ).subscribe((res) => {
            this.product = res.product;
        });

        this.modal.show();
    }

    onShown(): void {
        document.getElementById('Name')?.focus();
    }

    save(): void {
        this.saving = true;

        if (this.validateFormGroup(this.productForm.form)) {
            if (this.isEdit) {
                let input = plainToInstance(EditProductDto, this.product);

                this._productService.updateProduct(input)
                    .pipe(
                        finalize(() => {
                            this.saving = false;
                        })
                    )
                    .subscribe(() => {
                        appHelper.notify.info('Saved successfully');
                        this.close();
                        this.modalSave.emit(null);
                    });
            } else {
                let input = plainToInstance(CreateProductDto, this.product);

                this._productService.createProduct(input)
                    .pipe(
                        finalize(() => {
                            this.saving = false;
                        })
                    )
                    .subscribe(() => {
                        appHelper.notify.info('Saved successfully');
                        this.close();
                        this.modalSave.emit(null);
                    });
            }
        }
        else {
            this.saving = false;
        }
    }

    close(): void {
        this.product = new CreateOrEditProductDto();
        this.active = false;
        this.modal.hide();
    }
}