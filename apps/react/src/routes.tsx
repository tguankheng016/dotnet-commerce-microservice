import { LoadingScreen } from "@shared/components/loading-screen";
import { AccountRoute, AppProtectedRoute, Error404Page } from "@shared/components/routing";
import { lazy, Suspense } from "react";
import { createBrowserRouter, Navigate, RouteObject } from "react-router-dom";

// Lazy load the components
const AccountLayout = lazy(() => import('@account/components/layout').then(module => ({ default: module.AccountLayout })));
const LoginPage = lazy(() => import('@account/pages/login').then(module => ({ default: module.LoginPage })));
const CallbackLoginPage = lazy(() => import('@account/pages/callback-login').then(module => ({ default: module.CallbackLoginPage })));

const accountRoutes: RouteObject[] = [
    {
        path: '',
        element: (
            <Suspense fallback={<LoadingScreen />}>
                <AccountLayout />
            </Suspense>
        ),
        children: [
            { index: true, element: <Navigate to="login" /> },
            {
                path: 'login',
                element: (
                    <Suspense fallback={<LoadingScreen />}>
                        <AccountRoute><LoginPage /></AccountRoute>
                    </Suspense>
                )
            },
            {
                path: 'callback/login',
                element: (
                    <Suspense fallback={<LoadingScreen />}>
                        <CallbackLoginPage />
                    </Suspense>
                )
            },
        ]
    }
];

const AppLayout = lazy(() => import('@app/components/layout').then(module => ({ default: module.AppLayout })));
const CartPage = lazy(() => import('@app/pages/cart').then(module => ({ default: module.CartPage })));
const HomePage = lazy(() => import('@app/pages/home').then(module => ({ default: module.HomePage })));
const ShopPage = lazy(() => import('@app/pages/shop').then(module => ({ default: module.ShopPage })));

const appRoutes: RouteObject[] = [
    {
        path: '',
        element: (
            <Suspense fallback={<LoadingScreen />}>
                <AppLayout />
            </Suspense>
        ),
        children: [
            { index: true, element: <Navigate to="home" /> },
            {
                path: 'home',
                element: (
                    <Suspense fallback={<LoadingScreen />}>
                        <HomePage />
                    </Suspense>
                )
            },
            {
                path: 'shop',
                element: (
                    <Suspense fallback={<LoadingScreen />}>
                        <ShopPage />
                    </Suspense>
                )
            },
            {
                path: 'cart',
                element: (
                    <Suspense fallback={<LoadingScreen />}>
                        <AppProtectedRoute><CartPage /></AppProtectedRoute>
                    </Suspense>
                )
            },
        ]
    }
];

const routes: RouteObject[] = [
    {
        path: '/',
        errorElement: <Error404Page />,
        children: [
            { index: true, element: <Navigate to="app" /> },
            { path: 'app', children: appRoutes },
            { path: 'account', children: accountRoutes },
        ]
    }
];

const router = createBrowserRouter(routes);

export default router