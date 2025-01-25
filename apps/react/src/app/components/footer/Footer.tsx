
const Footer = () => {
    return (
        <div className="footer py-4 d-flex flex-lg-column" id="kt_footer">
            <div className="container-xl d-flex flex-column flex-md-row align-items-center justify-content-between">
                <div className="text-dark order-2 order-md-1">
                    <span className="text-muted fw-bold me-1">2021©</span>
                    <a href="https://keenthemes.com" target="_blank" className="text-gray-800 text-hover-primary">Keenthemes</a>
                </div>
                <ul className="menu menu-gray-600 menu-hover-primary fw-bold order-1">
                    <li className="menu-item">
                        <a href="https://keenthemes.com" target="_blank" className="menu-link px-2">About</a>
                    </li>
                    <li className="menu-item">
                        <a href="https://keenthemes.com/support" target="_blank" className="menu-link px-2">Support</a>
                    </li>
                    <li className="menu-item">
                        <a href="https://keenthemes.com/products/ceres-html-pro" target="_blank" className="menu-link px-2">Purchase</a>
                    </li>
                </ul>
            </div>
        </div>
    )
}

export default Footer