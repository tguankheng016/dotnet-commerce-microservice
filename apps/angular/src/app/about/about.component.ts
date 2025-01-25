import { Component, Injector, ChangeDetectionStrategy } from '@angular/core';
import { BreadcrumbItem } from '@app/layout/default-page.component';
import { appModuleAnimation } from '@shared/animations/router-transition';
import { AppComponentBase } from '@shared/app-component-base';

@Component({
    templateUrl: './about.component.html',
    animations: [appModuleAnimation()],
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class AboutComponent extends AppComponentBase {
    breadcrumbs: BreadcrumbItem[] = [
        new BreadcrumbItem('About')
    ];
    
    constructor(injector: Injector) {
        super(injector);
    }
}