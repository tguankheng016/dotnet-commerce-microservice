import { ChangeDetectionStrategy, ChangeDetectorRef, Component, ElementRef, Injector, Input, ViewChild } from "@angular/core";
import { AppComponentBase } from "@shared/app-component-base";
import { IdentityServiceProxy, PermissionGroupDto } from "@shared/service-proxies/identity-service-proxies";
import { plainToInstance } from "class-transformer";
import { TreeNode } from "primeng/api";
import { finalize } from "rxjs";

export class ExtendedPermissionGroupDto extends PermissionGroupDto {
    isActive: boolean;
    selectedCount: number;
}

@Component({
    selector: 'permission-tree',
    templateUrl: './permission-tree.component.html',
    styleUrl: './permission-tree.component.css',
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class PermissionTreeComponent extends AppComponentBase {
    @ViewChild('isGrantAllCheckbox', { static: false }) isGrantAllCheckboxElem!: ElementRef;
    @ViewChild('selectAllCheckbox', { static: false }) selectAllCheckboxElem!: ElementRef;
    @Input() grantedPermissions: string[] = [];

    allPermissions: ExtendedPermissionGroupDto[] = [];
    activePermissionGroup: ExtendedPermissionGroupDto;
    permissionNodes: TreeNode[];
    selectedPermissions: TreeNode[];
    allPermissionsCount = 0;

    isGrantAllCheckbox: HTMLInputElement;
    selectAllCheckbox: HTMLInputElement;

    isGrantAll = false;
    selectAll = false;

    constructor(
        injector: Injector,
        private _cdRef: ChangeDetectorRef,
        private _identityService: IdentityServiceProxy
    ) {
        super(injector);
    }

    ngAfterViewInit(): void {
        this.setTreeData();
    }

    setTreeData() {
        this.isGrantAllCheckbox = this.isGrantAllCheckboxElem.nativeElement as HTMLInputElement;
        this.selectAllCheckbox = this.selectAllCheckboxElem.nativeElement as HTMLInputElement;

        this._identityService.getAllPermissions()
            .pipe(finalize(() => {
                this._cdRef.markForCheck();
            }))
            .subscribe((res) => {
                this.allPermissions = plainToInstance(ExtendedPermissionGroupDto, res.items);

                if (this.allPermissions?.length > 0) {
                    let firstGroup = this.allPermissions[0];
                    this.setActive(firstGroup);
                    this.allPermissionsCount = this.allPermissions.map(x => x.permissions).flat().length;

                    if (this.grantedPermissions?.length > 0) {
                        if (this.grantedPermissions.length === this.allPermissionsCount) {
                            this.isGrantAll = true;
                        } else {
                            this.setCheckboxIntermediate(this.isGrantAllCheckbox);
                        }

                        this.allPermissions.forEach((p) => {
                            if (p.groupName !== firstGroup.groupName) {
                                p.selectedCount = 0;

                                p.permissions.forEach((i) => {
                                    p.selectedCount += (this.grantedPermissions.indexOf(i.name) !== -1 ? 1 : 0);
                                });
                            }
                        })
                    }
                }
            });
    }

    getGrantedPermissions(): string[] {
        return this.grantedPermissions;
    }

    trackByPermission(index: number, item: ExtendedPermissionGroupDto) {
        return item.groupName;
    }

    setActive(group: ExtendedPermissionGroupDto): void {
        this.allPermissions.forEach(p => {
            p.isActive = group.groupName === p.groupName;
        });

        this.populateTreeNodes(group);
    }

    populateTreeNodes(group: ExtendedPermissionGroupDto): void {
        this.permissionNodes = [];
        this.selectedPermissions = [];
        this.activePermissionGroup = group;

        group.selectedCount = 0;

        group.permissions.forEach(p => {
            let newNode = {
                label: p.displayName,
                data: p.name
            };

            group.selectedCount += (this.grantedPermissions.indexOf(p.name) !== -1 ? 1 : 0);

            this.permissionNodes.push(newNode);
        });

        this.selectedPermissions = this.permissionNodes
            .filter(p => this.grantedPermissions.indexOf(p.data) !== -1);

        this.populateSelectAllCheckboxStatus();
    }

    nodeSelect(event) {
        if (event?.node?.data) {
            this.activePermissionGroup.selectedCount++;
            this.grantedPermissions.push(event.node.data);

            if (this.activePermissionGroup.selectedCount === this.activePermissionGroup.permissions.length) {
                this.selectAll = true;
                this.setCheckboxIntermediate(this.selectAllCheckbox, false);

                if (this.grantedPermissions.length == this.allPermissionsCount) {
                    this.isGrantAll = true;
                    this.setCheckboxIntermediate(this.isGrantAllCheckbox, false);
                }
            } else {
                this.setCheckboxIntermediate(this.selectAllCheckbox);
                this.setCheckboxIntermediate(this.isGrantAllCheckbox);
            }
            console.log(this.allPermissions);
        }
    }

    onNodeUnselect(event) {
        if (event?.node?.data) {
            this.activePermissionGroup.selectedCount--;
            this.grantedPermissions = this.grantedPermissions.filter(p => p !== event.node.data);

            if (this.activePermissionGroup.selectedCount === 0) {
                this.selectAll = false;
                this.setCheckboxIntermediate(this.selectAllCheckbox, false);

                if (this.grantedPermissions.length === 0) {
                    this.isGrantAll = false;
                    this.setCheckboxIntermediate(this.isGrantAllCheckbox, false);
                }
            } else {
                this.setCheckboxIntermediate(this.selectAllCheckbox, true);
                this.setCheckboxIntermediate(this.isGrantAllCheckbox, true);
            }
        }
    }

    populateSelectAllCheckboxStatus(): void {
        this.selectAll = false;
        this.setCheckboxIntermediate(this.selectAllCheckbox, false);

        if (this.activePermissionGroup.selectedCount === this.activePermissionGroup.permissions.length) {
            this.selectAll = true;
        } else if (this.activePermissionGroup.selectedCount > 0) {
            this.setCheckboxIntermediate(this.selectAllCheckbox, true);
        }
    }

    selectAllPermission(): void {
        if (this.selectAll) {
            this.selectedPermissions = this.permissionNodes.filter(p => 1 === 1);
            this.grantedPermissions.push(...this.selectedPermissions.map(x => x.data));
            this.activePermissionGroup.selectedCount = this.activePermissionGroup.permissions.length;

            if (this.grantedPermissions.length == this.allPermissionsCount) {
                this.isGrantAll = true;
                this.setCheckboxIntermediate(this.isGrantAllCheckbox, false);
            } else {
                this.setCheckboxIntermediate(this.isGrantAllCheckbox, true);
            }
        } else {
            this.selectedPermissions = [];
            this.grantedPermissions = this.grantedPermissions
                .filter(x => this.activePermissionGroup.permissions.map(p => p.name).indexOf(x) === -1);
            this.activePermissionGroup.selectedCount = 0;
            this.isGrantAll = false;

            if (this.grantedPermissions.length == 0) {
                this.setCheckboxIntermediate(this.isGrantAllCheckbox, false);
            } else if (this.grantedPermissions.length > 0) {
                this.setCheckboxIntermediate(this.isGrantAllCheckbox, true);
            }
        }
    }

    selectIsGrantAllPermission(): void {
        this.setCheckboxIntermediate(this.selectAllCheckbox, false);
        this.setCheckboxIntermediate(this.isGrantAllCheckbox, false);

        if (this.isGrantAll) {
            this.selectAll = true;
            this.selectedPermissions = this.permissionNodes.filter(p => 1 === 1);
            this.grantedPermissions = [...this.allPermissions.map(x => x.permissions).flat().map(x => x.name)];
            this.allPermissions.forEach(p => {
                p.selectedCount = p.permissions.length;
            });
        } else {
            this.selectedPermissions = [];
            this.grantedPermissions = [];
            this.selectAll = false;
            this.allPermissions.forEach(p => {
                p.selectedCount = 0;
            });
        }
    }

    setCheckboxIntermediate(checkbox: HTMLInputElement, setIndeterminate: boolean = true): void {
        checkbox.indeterminate = setIndeterminate;
    }
}