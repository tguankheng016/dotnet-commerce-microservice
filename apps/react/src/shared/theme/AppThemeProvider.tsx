import { ReactNode, useEffect } from "react";
import { useThemeStore, Theme } from '@shared/theme';
import { AppConsts } from "@shared/app-consts";
import useLayoutStore from "./layout-store";

interface Props {
    children: ReactNode;
}

const AppThemeProvider = ({ children }: Props) => {
    const baseUrl = import.meta.env.VITE_APP_BASE_URL

    const { theme, setTheme } = useThemeStore();
    const { setIsMobile } = useLayoutStore();

    useEffect(() => {
        // List of styles to be loaded
        const styleUrls = [
            `${baseUrl}/assets/primeng/themes/mdc-${theme}-indigo/theme.css`,
            `${baseUrl}/assets/primeng/primeng-customize.min.css`,
            `${baseUrl}/assets/primeng/primeng-customize-${theme}.min.css`
        ];

        // Append each style URL dynamically
        const head = document.head;
        const linkElements: HTMLLinkElement[] = [];

        styleUrls.forEach((url) => {
            const link = document.createElement("link");
            link.type = "text/css";
            link.rel = "stylesheet";
            link.href = url;
            head.appendChild(link);
            linkElements.push(link);
        });

        // Cleanup: remove the links when the component is unmounted or theme changes
        return () => {
            linkElements.forEach((link) => {
                head.removeChild(link);
            });
        };
    }, [theme]);

    useEffect(() => {
        let savedTheme = localStorage.getItem(AppConsts.localStorage.theme) as Theme;

        if (!savedTheme) {
            savedTheme = 'dark';
        }

        if (savedTheme) {
            setTheme(savedTheme);
        }
    }, []);

    useEffect(() => {
        const handleResize = () => {
            setIsMobile(window.innerWidth < 768);
        };

        window.addEventListener('resize', handleResize);

        return () => {
            window.removeEventListener('resize', handleResize);
        };
    }, []);

    return (
        <>{children}</>
    )
}

export default AppThemeProvider