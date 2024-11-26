import { Component, Injector, OnInit, ViewEncapsulation } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { UserServiceProxy } from '@shared/service-proxies/identity-service-proxies';

@Component({
    templateUrl: './app.component.html'
})
export class AppComponent extends AppComponentBase implements OnInit {
    title = 'angular';

    constructor(
        injector: Injector
    ) {
        super(injector);
    }

    ngOnInit(): void {
    }
}
