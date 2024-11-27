import { Component, EventEmitter, Injector, Output, ViewChild, ViewEncapsulation } from "@angular/core";
import { NgForm } from "@angular/forms";
import { AppComponentBase } from "@shared/app-component-base";
import { CreateOrEditCategoryDto, CreateCategoryDto, EditCategoryDto, CategoryServiceProxy } from "@shared/service-proxies/product-service-proxies";
import { plainToInstance } from "class-transformer";
import { ModalDirective } from "ngx-bootstrap/modal";
import { concatMap, finalize } from "rxjs";
import { forEach as _forEach } from 'lodash-es';

@Component({
    selector: 'createOrEditCategoryModal',
    templateUrl: './create-or-edit-category-modal.component.html',
    encapsulation: ViewEncapsulation.None
})
export class CreateOrEditCategoryModalComponent extends AppComponentBase {
    @ViewChild('categoryForm', { static: false }) categoryForm: NgForm;
    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;
    isEdit = false;

    category: CreateOrEditCategoryDto = new CreateOrEditCategoryDto();

    constructor(
        injector: Injector,
        private _categoryService: CategoryServiceProxy
    ) {
        super(injector);
    }

    show(categoryId?: number): void {
        this.active = true;
        this.isEdit = this.stringService.notNullOrEmpty(categoryId);

        this._categoryService.getCategoryById(categoryId ?? 0)
            .subscribe((res) => {
                this.category = res.category;
            });

        this.modal.show();
    }

    onShown(): void {
        document.getElementById('CategoryName')?.focus();
    }

    save(): void {
        this.saving = true;

        if (this.validateFormGroup(this.categoryForm.form)) {
            if (this.isEdit) {
                let input = plainToInstance(EditCategoryDto, this.category);

                this._categoryService.updateCategory(input)
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
                let input = plainToInstance(CreateCategoryDto, this.category);

                this._categoryService.createCategory(input)
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
        this.category = new CreateOrEditCategoryDto();
        this.active = false;
        this.modal.hide();
    }
}