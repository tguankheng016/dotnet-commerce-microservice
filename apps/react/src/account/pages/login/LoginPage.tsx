import { useState } from "react";
import { z } from "zod";
import { CustomMessage, ValidationMessage } from "@shared/components/form-validation";
import { zodResolver } from "@hookform/resolvers/zod";
import { useForm } from "react-hook-form";
import { BusyButton } from "@shared/components/buttons";
import { AppAuthService } from "@shared/auth/app-auth-service";
import { CookieService } from "@shared/cookies/cookie-service";
import { AppConsts } from "@shared/app-consts";
import { StringHelper } from "@shared/utils";
import { SwalMessageService } from "@shared/sweetalert2";

const schema = z.object({
    username: z.string()
        .min(1, { message: CustomMessage.required }),
    password: z.string()
        .min(1, { message: CustomMessage.required })
});

type FormData = z.infer<typeof schema>;

const LoginPage = () => {
    const { register, handleSubmit, formState: { errors } } = useForm<FormData>({
        resolver: zodResolver(schema),
        defaultValues: {
            username: "admin",
            password: "123qwe",
        },
    });
    const [saving, setSaving] = useState(false);

    const submitHandler = (data: FormData) => {
        SwalMessageService.showConfirmation("Site Availability Hours", `This site is available Monday to Friday, from 8 AM to 8 PM.`, () => {
            setSaving(true);

            const authService = new AppAuthService();
            authService.authenticateRequest.usernameOrEmailAddress = data.username;
            authService.authenticateRequest.password = data.password;

            authService.authenticate(() => {
                setSaving(false);

            });
        }, () => { }, "OK", "Cancel");
    };

    const redirectOAuthHandler = () => {
        const stateKey = StringHelper.randomString(15);
        CookieService.setCookie(AppConsts.cookieName.openIddictStateKey, stateKey);

        const redirectUrl = import.meta.env.VITE_OPENIDDICT_URL
            + '/connect/authorize?client_id='
            + import.meta.env.VITE_OPENIDDICT_CLIENTID
            + '&redirect_uri=' + import.meta.env.VITE_APP_BASE_URL + '/account/callback/login'
            + '&scope=email profile'
            + '&response_type=code&response_mode=query'
            + '&state=' + stateKey;

        location.href = redirectUrl;
    }

    return (
        <form
            onSubmit={handleSubmit(data => submitHandler(data))}
            className="form w-100 fv-plugins-bootstrap5 fv-plugins-framework"
            id="kt_sign_in_form"
        >
            <div className="text-center mb-10">
                <h1 className="text-dark mb-3">Sign In to Ceres HTML Free</h1>
                <div className="text-gray-400 fw-semibold fs-4">
                    New Here?{' '}
                    <a href="#" className="link-primary fw-bold">
                        Create an Account
                    </a>
                </div>
            </div>
            <div className="fv-row mb-10 fv-plugins-icon-container">
                <label className="form-label fs-6 fw-bold text-dark">Username or email address</label>
                <input {...register('username')} className="form-control form-control-lg form-control-solid" type="text" name="username" autoComplete="off" />
                <ValidationMessage errorMessage={errors?.username?.message} />
            </div>
            <div className="fv-row mb-10 fv-plugins-icon-container">
                <div className="d-flex flex-stack mb-2">
                    <label className="form-label fw-bold text-dark fs-6 mb-0">Password</label>
                    <a href="#" className="link-primary fs-6 fw-bold">
                        Forgot Password ?
                    </a>
                </div>
                <input {...register('password')} className="form-control form-control-lg form-control-solid" type="password" name="password" autoComplete="off" />
                <ValidationMessage errorMessage={errors?.password?.message} />
            </div>
            <div className="text-center">
                <BusyButton
                    id="kt_sign_in_submit"
                    className="btn btn-lg btn-primary w-100 mb-5"
                    busyIf={saving}
                >
                    Continue
                </BusyButton>
            </div>
            <div className="text-center text-muted text-uppercase fw-bold mb-5">or</div>
            <button type="button" onClick={redirectOAuthHandler} className="btn btn-flex flex-center btn-light btn-lg w-100 mb-5" disabled={saving}>
                Continue with SSO
            </button>
        </form>
    )
}

export default LoginPage