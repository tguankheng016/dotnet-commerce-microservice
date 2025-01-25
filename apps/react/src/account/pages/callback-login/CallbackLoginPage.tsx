import { AppConsts } from "@shared/app-consts";
import { AppAuthService } from "@shared/auth/app-auth-service";
import { CookieService } from "@shared/cookies/cookie-service";
import { SwalNotifyService } from "@shared/sweetalert2";
import { StringHelper } from "@shared/utils";
import { useEffect } from "react";
import { useLocation, useNavigate } from "react-router-dom";


const CallbackLoginPage = () => {
    const location = useLocation();
    const navigate = useNavigate();
    const authService = new AppAuthService();
    const queryParams = new URLSearchParams(location.search);
    const code = queryParams.get('code');
    const state = queryParams.get('state');
    const error_description = queryParams.get('error_description');
    const savedState = CookieService.getCookie(AppConsts.cookieName.openIddictStateKey);

    useEffect(() => {
        const abortController = new AbortController();
        const signal = abortController.signal;

        if (StringHelper.notNullOrEmpty(error_description)) {
            SwalNotifyService.error(error_description as string);
            navigate('/account/login');
        }

        if (!StringHelper.notNullOrEmpty(state) || savedState !== state) {
            SwalNotifyService.error('Invalid state');
            navigate('/account/login');
        }

        if (!StringHelper.notNullOrEmpty(code)) {
            SwalNotifyService.error('Invalid code');
            navigate('/account/login');
        }

        authService.openIddictAuthenticate(
            code as string,
            import.meta.env.VITE_APP_BASE_URL + '/account/callback/login',
            () => {
                CookieService.removeCookie(AppConsts.cookieName.openIddictStateKey);
            },
            signal);

        return () => {
            abortController.abort();
        };
    }, []);

    return (
        <div className="login-form">
            <div className="alert alert-success text-center" role="alert">
                <div className="alert-text">Please wait while we confirm your login{' '}<i className="fa fa-spin fa-spinner alert-success"></i></div>
            </div>
        </div>
    )
}

export default CallbackLoginPage