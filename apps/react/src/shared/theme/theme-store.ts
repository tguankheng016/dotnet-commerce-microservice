import { AppConsts } from "@shared/app-consts";
import { create } from "zustand";

export type Theme = 'light' | 'dark';

interface ThemeState {
    theme: Theme;
    isDarkMode: boolean;
    setTheme: (theme: Theme, callback?: () => void) => void;
}

const useThemeStore = create<ThemeState>((set) => ({
    theme: 'dark',
    isDarkMode: true,
    setTheme: (theme: Theme, callback?: () => void) => {
        set({ theme: theme });
        set({ isDarkMode: theme === 'dark' });
        
        localStorage.setItem(AppConsts.localStorage.theme, theme);
        document.documentElement.setAttribute(AppConsts.localStorage.theme, theme);

        if (callback) {
            callback();
        }
    }
}));

export default useThemeStore;