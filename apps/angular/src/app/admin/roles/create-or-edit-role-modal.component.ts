import { ChangeDetectionStrategy, ChangeDetectorRef, Component, EventEmitter, Injector, Output, ViewChild, ViewEncapsulation } from "@angular/core";
import { NgForm } from "@angular/forms";
import { AppComponentBase } from "@shared/app-component-base";
import { CreateOrEditRoleDto, CreateRoleDto, EditRoleDto, RoleServiceProxy } from "@shared/service-proxies/identity-service-proxies";
import { ModalDirective } from "ngx-bootstrap/modal";
import { finalize } from "rxjs";
import { forEach as _forEach } from 'lodash-es';
import { PermissionTreeComponent } from "../shared/permission-tree.component";

@Component({
    selector: 'createOrEditRoleModal',
    templateUrl: './create-or-edit-role-modal.component.html',
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class CreateOrEditRoleModalComponent extends AppComponentBase {
    @ViewChild('roleForm', { static: false }) roleForm: NgForm;
    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @ViewChild('permissionTree', { static: false }) permissionTree: PermissionTreeComponent;

    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;
    isEdit = false;

    role: CreateOrEditRoleDto = new CreateOrEditRoleDto();

    constructor(
        injector: Injector,
        private _cdRef: ChangeDetectorRef,
        private _roleService: RoleServiceProxy
    ) {
        super(injector);
    }

    show(roleId?: number): void {
        this.active = true;
        this.isEdit = this.stringService.notNullOrEmpty(roleId);

        this._roleService.getRoleById(roleId ?? 0)
            .pipe(
                finalize(() => {
                    this._cdRef.markForCheck();
                })
            )
            .subscribe((res) => {
                this.role = res.role;
            });

        this.modal.show();
    }

    onShown(): void {
        document.getElementById('RoleName')?.focus();
    }

    save(): void {
        this.saving = true;

        if (this.validateFormGroup(this.roleForm.form)) {
            this.role.grantedPermissions = this.permissionTree.getGrantedPermissions();

            if (this.isEdit) {
                let input = EditRoleDto.fromJS(this.role);

                this._roleService.updateRole(input)
                    .pipe(
                        finalize(() => {
                            this.saving = false;
                            this._cdRef.markForCheck();
                        })
                    )
                    .subscribe(() => {
                        appHelper.notify.info('Saved successfully');
                        this.close();
                        this.modalSave.emit(null);
                    });
            } else {
                let input = CreateRoleDto.fromJS(this.role);

                this._roleService.createRole(input)
                    .pipe(
                        finalize(() => {
                            this.saving = false;
                            this._cdRef.markForCheck();
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
        this.role = new CreateOrEditRoleDto();
        this.active = false;
        this.modal.hide();
    }
}