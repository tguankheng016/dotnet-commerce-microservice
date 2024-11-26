import { Component, ViewEncapsulation, OnInit } from "@angular/core";
import { AppComponentBase } from "@shared/app-component-base";

@Component({
    selector: 'header-notifications',
    templateUrl: './header-notifications.component.html',
    encapsulation: ViewEncapsulation.None
})
export class HeaderNotificationsComponent extends AppComponentBase implements OnInit {
    
    ngOnInit(): void {
    }
}