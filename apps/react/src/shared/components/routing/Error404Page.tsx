import bg404 from '@assets/media/18.png';
import bg404Dark from '@assets/media/18-dark.png';
import { useThemeStore } from '@shared/theme';
import { Link } from 'react-router-dom';

const Error404Page = () => {
    const { isDarkMode } = useThemeStore();

    return (
        <div className="d-flex flex-column flex-root">
            <div className="d-flex flex-column flex-center flex-column-fluid p-10">
                <img src={isDarkMode ? bg404Dark : bg404} alt="" className="mw-100 mb-10 h-lg-450px" />
                <h1 className="fw-semibold mb-10">Seems there is nothing here</h1>
                <Link to="/" replace={true} className="btn btn-primary">Return Home</Link>
            </div>
        </div>
    )
}

export default Error404Page