import { useSessionStore } from "@shared/session";
import { Link } from "react-router-dom";
import HeaderUserMenu from "./HeaderUserMenu";
import HeaderCart from "./HeaderCart";

const HeaderTopbar = () => {
    const { user } = useSessionStore();

    return (
        <div className="d-flex align-items-stretch flex-shrink-0">
            <div className="topbar d-flex align-items-stretch flex-shrink-0">
                <div className="d-flex align-items-center ms-1" title="Admin Portal">
                    <Link target="_blank" to="https://eportal.gktan.com" className="btn btn-icon btn-custom btn-color-gray-600 btn-active-color-primary w-35px h-35px w-md-40px h-md-40px">
                        <i className="fas fa-shield-alt fs-3"></i>
                    </Link>
                </div>
                <div className="d-flex align-items-center ms-1" title="Github Link">
                    <Link target="_blank" to="https://github.com/tguankheng016/dotnet-commerce-microservice" className="btn btn-icon btn-custom btn-color-gray-600 btn-active-color-primary w-35px h-35px w-md-40px h-md-40px">
                        <i className="fab fa-github fs-2"></i>
                    </Link>
                </div>
                {
                    !user &&
                    <Link to='/account/login' className="menu-item menu-lg-down-accordion me-lg-1">
                        <span className="menu-link py-3">
                            <span className="menu-title fs-5 fw-bold">Login</span>
                        </span>
                    </Link>
                }
                {
                    user &&
                    <>
                        <HeaderCart />
                        <HeaderUserMenu />
                    </>
                }
            </div>
        </div>
    )
}

export default HeaderTopbar