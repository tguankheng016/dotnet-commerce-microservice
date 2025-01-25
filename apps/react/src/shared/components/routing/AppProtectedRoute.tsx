import { AppAuthService } from "@shared/auth/app-auth-service";
import { useSessionStore } from "@shared/session";
import { useEffect } from "react";
import { Navigate, useNavigate } from "react-router-dom";


interface Props {
    children: JSX.Element;
}

const AppProtectedRoute = ({ children }: Props) => {
    const { user, loading } = useSessionStore();
    const navigate = useNavigate();

    useEffect(() => {
        if (!user) {
            // Try Refresh Token
            const authService = new AppAuthService();
            authService.refreshToken()
                .then((res) => {
                    if (res)
                        navigate('/', { replace: true })
                });
        }
    }, [user, navigate]);

    if (loading) {
        return <div>Loading...</div>;
    }

    return (
        user ? children : <Navigate to="/account/login" replace={true} />
    )
}

export default AppProtectedRoute