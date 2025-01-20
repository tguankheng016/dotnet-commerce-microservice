import { create } from "zustand";

interface CartState {
    cartItemCount: number;
    isRefreshingCart: boolean;
    setCartItemCount: (newCartItemCount: number) => void;
    setRefreshingCart: () => void;
}

const useCartStore = create<CartState>((set) => ({
    cartItemCount: 0,
    isRefreshingCart: true,
    setCartItemCount: (newCartItemCount: number) => set(() => ({ cartItemCount: newCartItemCount })),
    setRefreshingCart: () => set(store => ({ isRefreshingCart: !store.isRefreshingCart }))
}));

export default useCartStore;