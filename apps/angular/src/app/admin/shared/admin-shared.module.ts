import { NgModule } from "@angular/core";
import { SharedModule } from "@shared/shared.module";
import { TreeModule } from 'primeng/tree';
import { PermissionTreeComponent } from "./permission-tree.component";

const imports = [
    SharedModule,
    TreeModule
];

const declarations = [
    PermissionTreeComponent
];

@NgModule({
    imports: [...imports],
    declarations: [...declarations],
    exports: [...declarations],
})
export class AdminSharedModule {}