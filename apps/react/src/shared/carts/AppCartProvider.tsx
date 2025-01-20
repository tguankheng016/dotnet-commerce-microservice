import { ReactNode, useEffect } from "react";
import useCartStore from "./cart-store";
import { APIClient } from "@shared/service-proxies";
import { useSessionStore } from "@shared/session";

interface Props {
    children: ReactNode;
}

const AppCartProvider = ({ children }: Props) => {
    const { user } = useSessionStore();
    const { isRefreshingCart, setRefreshingCart, setCartItemCount } = useCartStore();

    useEffect(() => {
        const abortController = new AbortController();
        const signal = abortController.signal;

        if (isRefreshingCart && user && user.id > 0) {
            const cartService = APIClient.getCartService();

            cartService.getCarts(undefined, undefined, undefined, undefined, signal)
                .then((res) => {
                    setCartItemCount(res.items?.length ?? 0);
                }).finally(() => {
                    setRefreshingCart();
                });
        }

        return () => {
            abortController.abort();
        };
    }, [isRefreshingCart]);

    return (
        <>{children}</>
    )
}

export default AppCartProvider