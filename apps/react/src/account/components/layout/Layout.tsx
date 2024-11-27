import { Outlet } from "react-router-dom"
import logo from '@assets/media/logo-default.svg';


const Layout = () => {
    return (
        <div className="d-flex flex-column flex-column-fluid bgi-position-y-bottom position-x-center bgi-no-repeat bgi-size-contain bgi-attachment-fixed auth-page-bg">
            <div className="d-flex flex-center flex-column flex-column-fluid p-10 pb-lg-20">
                <a href="/ceres-html-free/index.html" className="mb-12">
                    <img src={logo} className="h-30px theme-light-show" alt="Logo"  />
                    <img src={logo} className="h-30px theme-dark-show" alt="Logo"  />
                </a>
                <div className="w-lg-500px bg-body rounded shadow-sm p-10 p-lg-15 mx-auto">
                    <Outlet />
                </div>
            </div>
            <div className="d-flex flex-center flex-column-auto p-10">
                <div className="d-flex align-items-center fw-semibold fs-6">
                    <a href="https://keenthemes.com" className="text-muted text-hover-primary px-2">About</a>
                    <a href="mailto:support@keenthemes.com" className="text-muted text-hover-primary px-2">Contact</a>
                    <a href="https://keenthemes.com/products/ceres-html-pro" className="text-muted text-hover-primary px-2">Contact Us</a>
                </div>
            </div>
        </div>
    )
}

export default Layout