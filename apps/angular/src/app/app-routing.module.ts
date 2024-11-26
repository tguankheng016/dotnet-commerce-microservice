import { NgModule } from "@angular/core";
import { AboutComponent } from "./about/about.component";
import { AppComponent } from "./app.component";
import { RouterModule } from "@angular/router";
import { AppRouteGuard } from "@shared/auth/app-route-guard";
import { Error404Component } from "@shared/errors/error-404.component";

@NgModule({
    imports: [
        RouterModule.forChild([
            {
                path: '',
                component: AppComponent,
                canActivate: [AppRouteGuard],
                canActivateChild: [AppRouteGuard],
                children: [
                    // {
                    //     path: '',
                    //     children: [
                    //         { path: 'notifications', component: NotificationsComponent },
                    //         { path: '', redirectTo: '/app/home', pathMatch: 'full' },
                    //     ],
                    // },
                    {
                        path: 'admin',
                        loadChildren: () => import('app/admin/admin.module').then((m) => m.AdminModule), //Lazy load admin module
                        data: { preload: true },
                        canLoad: [AppRouteGuard],
                    },
                    {
                        path: 'main',
                        loadChildren: () => import('app/main/main.module').then((m) => m.MainModule), //Lazy load main module
                        data: { preload: true },
                    },
                    //{ path: 'home', component: HomeComponent },
                    { path: 'about', component: AboutComponent },
                    { path: '**', component: Error404Component },
                ]
            }
        ])
    ],
    exports: [RouterModule]
})
export class AppRoutingModule { }
