import { AppAuthService } from "@shared/auth/app-auth-service";
import { useSessionStore } from "@shared/session";
import { useThemeStore, Theme } from '@shared/theme';
import { StringHelper } from "@shared/utils";
import { useCallback, useEffect, useRef, useState } from "react";
import { Overlay } from 'react-bootstrap';

const HeaderUserMenu = () => {
    const { isDarkMode, setTheme } = useThemeStore();
    const [show, setShow] = useState(false);
    const target = useRef<HTMLDivElement | null>(null);
    const overlayRef = useRef<HTMLDivElement | null>(null);

    const handleToggle = () => setShow((prev) => !prev);
    const handleClose = () => setShow(false);

    const { user } = useSessionStore();
    const pictureUrl = StringHelper.formatString(import.meta.env.VITE_UI_AVATAR_URL, user?.firstName ?? '', user?.lastName ?? '');

    const handleClickOutside = useCallback((event: MouseEvent) => {
        if (
            overlayRef.current &&
            !overlayRef.current.contains(event.target as Node) &&
            target.current &&
            !target.current.contains(event.target as Node)
        ) {
            handleClose();
        }
    }, []);

    useEffect(() => {
        if (show) {
            document.addEventListener('click', handleClickOutside);
        } else {
            document.removeEventListener('click', handleClickOutside);
        }

        return () => {
            document.removeEventListener('click', handleClickOutside);
        };
    }, [show, handleClickOutside]);

    const handleSignout = () => {
        handleClose();

        const authService = new AppAuthService();
        authService.signOut();
    };

    const handleToggleDarkMode = () => {
        handleClose();
        const themeToSet = (!isDarkMode ? 'dark' : 'light') as Theme;
        setTheme(themeToSet);
    };

    return (
        <div className="d-flex align-items-center ms-1 ms-lg-3" id="kt_header_user_menu_toggle">
            <div className="cursor-pointer symbol symbol-30px symbol-lg-40px" ref={target} onClick={handleToggle}>
                <img src={pictureUrl} alt="user" />
            </div>
            <Overlay
                ref={overlayRef}
                target={target.current}
                show={show}
                placement="bottom-end"
                onHide={handleClose}
            >
                {({
                    placement: _placement,
                    arrowProps: _arrowProps,
                    show: _show,
                    popper: _popper,
                    hasDoneInitialMeasure: _hasDoneInitialMeasure,
                    ...props
                }) => (
                    <div {...props} className="menu menu-sub menu-sub-dropdown dropdown-menu-right menu-column menu-rounded menu-gray-800 menu-state-bg menu-state-color fw-semibold py-4 fs-6 w-275px position-absolute">
                        <div className="menu-item px-3">
                            <div className="menu-content d-flex align-items-center px-3">
                                <div className="symbol symbol-50px me-5">
                                    <img alt="Logo" src={pictureUrl} />
                                </div>
                                <div className="d-flex flex-column">
                                    <div className="fw-bold d-flex align-items-center fs-5">{user?.userName}
                                        <span className="badge badge-light-success fw-bold fs-8 px-2 py-1 ms-2">Pro</span>
                                    </div>
                                    <a href="#" className="fw-semibold text-muted text-hover-primary fs-7">{user?.email}</a>
                                </div>
                            </div>
                        </div>
                        <div className="separator my-2"></div>
                        <div className="menu-item px-5">
                            <a href="/" className="menu-link px-5">My Profile</a>
                        </div>
                        <div className="menu-item px-5">
                            <a onClick={handleSignout} className="menu-link px-5">Sign Out</a>
                        </div>
                        <div className="separator my-2"></div>
                        <div className="menu-item px-5">
                            <div className="menu-content px-5">
                                <label className="form-check form-switch form-check-custom form-check-solid pulse pulse-success cursor-pointer" htmlFor="kt_dark_mode_toggle">
                                    <input className="form-check-input w-30px h-20px" type="checkbox" value="0" name="mode" id="kt_dark_mode_toggle" checked={isDarkMode} onChange={handleToggleDarkMode} />
                                    <span className="pulse-ring ms-n1"></span>
                                    <span className="form-check-label text-gray-600 fs-7">Dark Mode</span>
                                </label>
                            </div>
                        </div>
                    </div>
                )}
            </Overlay>
        </div>
    )
}

export default HeaderUserMenu