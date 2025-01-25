import { Cookies } from 'react-cookie';

const cookies = new Cookies();

export const CookieService = {
    setCookie: (name: string, value: string, seconds?: number) => {
        if (seconds) {
            const options = { path: '/', maxAge: seconds };
            cookies.set(name, value, options);
        } else {
            const options = { path: '/' };
            cookies.set(name, value, options);
        }
    },

    getCookie: (name: string) => {
        return cookies.get(name);
    },

    removeCookie: (name: string) => {
        cookies.remove(name, { path: '/' });
    },
};