import { create } from "zustand";

interface LayoutState {
    isMobileView: boolean;
    isExpanded: boolean;
    setIsMobile: (isMobile: boolean) => void;
    setExpanded: () => void;
}

const useLayoutStore = create<LayoutState>((set) => ({
    isMobileView: (window.innerWidth < 768),
    isExpanded: false,
    setIsMobile: (isMobile: boolean) => set({ isMobileView: isMobile }),
    setExpanded: () => set(store => ({ isExpanded: !store.isExpanded }))
}));

export default useLayoutStore;