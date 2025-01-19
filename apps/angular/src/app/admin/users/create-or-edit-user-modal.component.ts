import { ChangeDetectionStrategy, ChangeDetectorRef, Component, EventEmitter, Injector, Output, ViewChild, ViewEncapsulation } from "@angular/core";
import { NgForm } from "@angular/forms";
import { AppComponentBase } from "@shared/app-component-base";
import { CreateOrEditUserDto, CreateUserDto, EditUserDto, RoleDto, RoleServiceProxy, UserServiceProxy } from "@shared/service-proxies/identity-service-proxies";
import { plainToInstance } from "class-transformer";
import { ModalDirective } from "ngx-bootstrap/modal";
import { concatMap, finalize } from "rxjs";
import { forEach as _forEach } from 'lodash-es';

@Component({
    selector: 'createOrEditUserModal',
    templateUrl: './create-or-edit-user-modal.component.html',
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class CreateOrEditUserModalComponent extends AppComponentBase {
    @ViewChild('userForm', { static: false }) userForm: NgForm;
    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;
    isEdit = false;

    user: CreateOrEditUserDto = new CreateOrEditUserDto();
    roles: RoleDto[] = [];

    constructor(
        injector: Injector,
        private _cdRef: ChangeDetectorRef,
        private _userService: UserServiceProxy,
        private _roleService: RoleServiceProxy
    ) {
        super(injector);
    }

    show(userId?: number): void {
        this.active = true;
        this.isEdit = this.stringService.notNullOrEmpty(userId);

        let roles$ = this._roleService.getRoles(0, 0, '', '');
        let users$ = this._userService.getUserById(userId ?? 0);

        roles$.pipe(
            concatMap((res) => {
                this.roles = res.items;
                return users$;
            }),
            finalize(() => {
                this._cdRef.markForCheck();
            })
        ).subscribe((res) => {
            this.user = res.user;

            this.roles.forEach(r => {
                if (!userId) {
                    r.isAssigned = r.isDefault;
                } else {
                    r.isAssigned = this.user.roles.indexOf(r.name) !== -1;
                }
            });
        });

        this.modal.show();
    }

    onShown(): void {
        document.getElementById('FirstName')?.focus();
    }

    save(): void {
        this.saving = true;

        if (this.validateFormGroup(this.userForm.form)) {
            this.user.roles = this.roles.filter(x => x.isAssigned).map(x => x.name);

            if (this.isEdit) {
                let input = EditUserDto.fromJS(this.user);

                this._userService.updateUser(input)
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
                let input = CreateUserDto.fromJS(this.user);

                this._userService.createUser(input)
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
        this.user = new CreateOrEditUserDto();
        this.roles = [];
        this.active = false;
        this.modal.hide();
    }
}