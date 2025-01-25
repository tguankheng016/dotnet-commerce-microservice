import { ReactNode, useEffect } from 'react'
import { useSessionStore } from "@shared/session";
import { LoadingScreen } from '@shared/components/loading-screen';

interface Props {
    children: ReactNode;
}

const AppSessionProvider = ({ children }: Props) => {
    const { fetchCurrentUser, loading } = useSessionStore();

    useEffect(() => {
        const abortController = new AbortController();
        const signal = abortController.signal;

        fetchCurrentUser(signal);

        return () => {
            abortController.abort();
        };
    }, []);

    if (loading) {
        return <LoadingScreen />;
    }

    return (
        <>{children}</>
    )
}

export default AppSessionProvider