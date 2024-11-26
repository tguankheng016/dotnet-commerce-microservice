import { Component, EventEmitter, Injector, Output, ViewChild, ViewEncapsulation } from "@angular/core";
import { NgForm } from "@angular/forms";
import { AppComponentBase } from "@shared/app-component-base";
import { CreateOrEditRoleDto, CreateRoleDto, EditRoleDto, RoleServiceProxy } from "@shared/service-proxies/identity-service-proxies";
import { plainToInstance } from "class-transformer";
import { ModalDirective } from "ngx-bootstrap/modal";
import { finalize } from "rxjs";
import { forEach as _forEach } from 'lodash-es';
import { PermissionTreeComponent } from "../shared/permission-tree.component";

@Component({
    selector: 'createOrEditRoleModal',
    templateUrl: './create-or-edit-role-modal.component.html',
    encapsulation: ViewEncapsulation.None
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
        private _roleService: RoleServiceProxy
    ) {
        super(injector);
    }

    show(roleId?: number): void {
        this.active = true;
        this.isEdit = this.stringService.notNullOrEmpty(roleId);

        this._roleService.getRoleById(roleId ?? 0)
            .subscribe((res) => {
                this.role = res.role;
                this.permissionTree.editData = this.role?.grantedPermissions ?? [];
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
                let input = plainToInstance(EditRoleDto, this.role); 

                this._roleService.updateRole(input)
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
                let input = plainToInstance(CreateRoleDto, this.role); 

                this._roleService.createRole(input)
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
        this.role = new CreateOrEditRoleDto();
        this.active = false;
        this.modal.hide();
    }
}