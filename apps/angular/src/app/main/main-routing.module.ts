import { NgModule } from "@angular/core";
import { RouterModule } from "@angular/router";

@NgModule({
    imports: [
        RouterModule.forChild([
            {
                path: '',
                children: [
                ]
            },
        ]),
    ],
    exports: [RouterModule],
})
export class MainRoutingModule { }