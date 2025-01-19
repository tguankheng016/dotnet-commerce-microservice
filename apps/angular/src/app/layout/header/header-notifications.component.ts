import { Component, ViewEncapsulation, OnInit, ChangeDetectionStrategy } from "@angular/core";
import { AppComponentBase } from "@shared/app-component-base";

@Component({
    selector: 'header-notifications',
    templateUrl: './header-notifications.component.html',
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class HeaderNotificationsComponent extends AppComponentBase implements OnInit {

    ngOnInit(): void {
    }
}