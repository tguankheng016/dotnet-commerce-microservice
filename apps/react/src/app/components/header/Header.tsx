import HeaderLogo from "./HeaderLogo"
import HeaderNavbarDesktopContainer from "./HeaderNavbarDesktopContainer"
import HeaderToggle from "./HeaderToggle"
import HeaderTopbar from "./HeaderTopbar"

const Header = () => {
    return (
        <div 
            id="kt_header" 
            className="header align-items-stretch" 
            data-kt-sticky="true" 
            data-kt-sticky-name="header" 
            data-kt-sticky-offset="{default: '200px', lg: '300px'}"
        >
            <div className="container-xl d-flex align-items-center">
                <HeaderToggle />
                <HeaderLogo />
                <div className="d-flex align-items-stretch justify-content-between flex-md-grow-1">
                    <HeaderNavbarDesktopContainer />
                    <HeaderTopbar />
                </div>
            </div>
        </div>
    )
}

export default Header