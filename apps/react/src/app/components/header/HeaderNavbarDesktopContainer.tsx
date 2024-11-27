import { useLayoutStore } from "@shared/theme";
import HeaderNavbar from "./HeaderNavbar";

const HeaderNavbarDesktopContainer = () => {
    const { isMobileView } = useLayoutStore();

    if (isMobileView) {
        return undefined;
    }

    return (
        <div className="d-flex align-items-stretch" id="kt_header_nav">
            <HeaderNavbar isMobile={false} />
        </div>
    )
}

export default HeaderNavbarDesktopContainer