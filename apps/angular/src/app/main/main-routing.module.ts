import { NgModule } from "@angular/core";
import { RouterModule } from "@angular/router";

@NgModule({
    imports: [
        RouterModule.forChild([
            {
                path: '',
                children: [
                    {
                        path: 'categories',
                        loadChildren: () => import('./categories/categories.module').then((m) => m.CategoriesModule),
                    },
                    {
                        path: 'products',
                        loadChildren: () => import('./products/products.module').then((m) => m.ProductsModule),
                    },
                ]
            },
        ]),
    ],
    exports: [RouterModule],
})
export class MainRoutingModule { }