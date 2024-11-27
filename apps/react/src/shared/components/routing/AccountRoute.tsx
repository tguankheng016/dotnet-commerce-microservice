import { useSessionStore } from "@shared/session";
import { Navigate } from "react-router-dom";

interface Props {
    children: JSX.Element;
}

const AccountRoute = ({ children }: Props) => {
    const { user } = useSessionStore();

    return (
        !user ? children : <Navigate to="/" replace={true} />
    )
}

export default AccountRoute