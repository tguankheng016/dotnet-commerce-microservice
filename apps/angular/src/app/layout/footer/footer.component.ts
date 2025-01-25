import { ChangeDetectionStrategy, Component } from "@angular/core";
import { AppComponentBase } from "@shared/app-component-base";

@Component({
    selector: 'app-footer',
    templateUrl: './footer.component.html',
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class FooterComponent extends AppComponentBase {

}