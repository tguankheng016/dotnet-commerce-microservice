import { Component, Injector, ViewChild } from "@angular/core";
import { AppComponentBase } from "@shared/app-component-base";
import { UserDto, UserServiceProxy } from "@shared/service-proxies/identity-service-proxies";
import { ModalDirective } from "ngx-bootstrap/modal";
import { PermissionTreeComponent } from "../shared/permission-tree.component";
import { finalize } from "rxjs";

@Component({
    selector: 'editUserPermissionsModal',
    templateUrl: './edit-user-permissions-modal.component.html',
})
export class EditUserPermissionsModalComponent extends AppComponentBase {
    @ViewChild('editModal', { static: true }) modal: ModalDirective;
    @ViewChild('permissionTree', { static: false }) permissionTree: PermissionTreeComponent;

    saving = false;
    active = false;
    resetting = false;

    user: UserDto = new UserDto();
    grantedUserPermissions: string[] = [];

    constructor(
        injector: Injector,
        private _userService: UserServiceProxy
    ) {
        super(injector);
    }

    show(user: UserDto): void {
        this.active = true;
        this.user = user;

        this._userService.getUserPermissions(user.id)
            .subscribe((res) => {
                this.grantedUserPermissions = res.items;
                this.permissionTree.editData = this.grantedUserPermissions;
            });

        this.modal.show();
    }

    save(): void {
        this.saving = true;

        this._userService
            .updateUserPermissions(this.user.id, this.permissionTree.getGrantedPermissions())
            .pipe(
                finalize(() => {
                    this.saving = false;
                })
            )
            .subscribe((res => {
                appHelper.notify.info('Saved successfully');
                this.close();
            }));
    }

    resetSpecialPermissions(): void {
        this.resetting = true;

        this._userService
            .resetUserPermissions(this.user.id)
            .pipe(
                finalize(() => {
                    this.resetting = false;
                })
            )
            .subscribe((res => {
                appHelper.notify.info('Saved successfully');
                this.close();
            }));
    }

    close(): void {
        this.active = false;
        this.user = new UserDto();
        this.modal.hide();
    }
}