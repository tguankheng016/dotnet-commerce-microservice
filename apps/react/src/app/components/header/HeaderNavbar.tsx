import { useLayoutStore } from "@shared/theme";
import { RefObject } from "react";
import { Link, useLocation } from "react-router-dom";

class AppMenuItem {
    route: string;
    label: string;

    constructor(route: string, label: string) {
        this.route = route;
        this.label = label;
    }
}

interface Props {
    isMobile: boolean;
    navbarRef?: RefObject<HTMLDivElement>;
}

const HeaderNavbar = ({ isMobile, navbarRef }: Props) => {
    const location = useLocation();
    const { isExpanded, setExpanded } = useLayoutStore();

    const menuItemClassName = 'menu-item menu-lg-down-accordion me-lg-1';
    const activeMenuItemClassName = menuItemClassName + ' here';

    const menuItems: AppMenuItem[] = [
        new AppMenuItem('/app/home', 'About'),
        new AppMenuItem('/app/flights', 'Shops'),
        new AppMenuItem('/app/my-booking', 'My Bookings'),
    ];

    const checkIsActive = (item: AppMenuItem) => {
        return location.pathname === item.route;
    }

    const className = "header-menu align-items-stretch";
    const mobileClassName = className + " w-225px drawer drawer-start" + (isExpanded ? " drawer-on" : "");

    return (
        <div ref={navbarRef} className={isMobile ? mobileClassName : className}>
            <div
                className="menu menu-rounded menu-column menu-md-row menu-state-bg menu-title-white menu-state-icon-primary menu-state-bullet-primary menu-arrow-gray-400 fw-bold px-4 px-lg-0 my-5 my-lg-0 align-items-stretch"
                id="#kt_header_menu"
                data-kt-menu="true"
            >
                {
                    menuItems.map((item) =>
                        <Link key={item.label + 'MenuItem'} to={item.route} className={checkIsActive(item) ? activeMenuItemClassName : menuItemClassName}>
                            <span className="menu-link py-3">
                                <span className="menu-title">{item.label}</span>
                            </span>
                        </Link>
                    )
                }
            </div>
        </div>
    )
}

export default HeaderNavbar