import logo from '@assets/media/logo-default.svg';
import logoLight from '@assets/media/logo-light.svg';

const HeaderLogo = () => {
    return (
        <div className="header-logo me-5 me-md-10 flex-grow-1 flex-md-grow-0">
            <a href="/">
                <img alt="Logo" src={logoLight} className="h-15px h-lg-20px logo-default" />
                <img alt="Logo" src={logo} className="h-15px h-lg-20px logo-sticky" />
            </a>
        </div>
    )
}

export default HeaderLogo