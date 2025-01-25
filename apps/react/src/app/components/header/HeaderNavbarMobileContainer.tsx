import { useLayoutStore } from "@shared/theme";
import HeaderNavbar from "./HeaderNavbar";
import { useEffect, useRef } from "react";
import { useLocation } from "react-router-dom";

const HeaderNavbarMobileContainer = () => {
    const location = useLocation();
    const { isMobileView, isExpanded, setExpanded } = useLayoutStore();
    const navbarRef = useRef<HTMLDivElement>(null);

    useEffect(() => {
        const handleClickOutside = (event: MouseEvent) => {
            if (isMobileView && isExpanded && navbarRef.current && !navbarRef.current.contains(event.target as Node)) {
                setExpanded();
            }
        };

        // Attach the event listener
        document.addEventListener('mousedown', handleClickOutside);

        // Cleanup the event listener on component unmount
        return () => {
            document.removeEventListener('mousedown', handleClickOutside);
        };
    }, [isExpanded, isMobileView]);

    useEffect(() => {
        if (isExpanded) {
            setExpanded();
        }
    }, [location]);

    if (!isMobileView) {
        return undefined;
    }

    return (
        <HeaderNavbar navbarRef={navbarRef} isMobile={true} />
    )
}

export default HeaderNavbarMobileContainer