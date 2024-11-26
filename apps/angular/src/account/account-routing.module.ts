import { NgModule } from "@angular/core";
import { AccountComponent } from "./account.component";
import { RouterModule } from "@angular/router";
import { LoginComponent } from "./login/login.component";
import { AccountRouteGuard } from "@shared/auth/account-route-guard";
import { Error404Component } from "@shared/errors/error-404.component";
import { CallbackLoginComponent } from "./callback-login/callback-login.component";

@NgModule({
    imports: [
        RouterModule.forChild([
            {
                path: '',
                component: AccountComponent,
                canActivate: [AccountRouteGuard],
                children: [
                    { path: '', redirectTo: 'login', pathMatch: 'full' },
                    { path: 'login', component: LoginComponent, canActivate: [AccountRouteGuard] },
                    { path: 'callback/login', component: CallbackLoginComponent, canActivate: [AccountRouteGuard] },
                    { path: '**', component: Error404Component },
                    // { 
                    //     path: 'oauth', 
                    //     loadChildren: () => 
                    //         import('./oauth/oauth.module').then((m) => m.OAuthModule), 
                    //     canActivate: [AccountRouteGuard] 
                    // },
                ]
            }
        ])
    ],
    exports: [
        RouterModule
    ]
})
export class AccountRoutingModule { }