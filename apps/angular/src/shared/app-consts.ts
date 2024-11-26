export class AppConsts {
    static remoteServiceBaseUrl: string;
    static appBaseUrl: string;
    static appBaseHref: string;
    static openIddictUrl: string;
    static openIddictClientId: string;
    static uiAvatarsBaseUrl: string;

    static readonly cookieName = {
        accessToken: "flightbooking.access.token",
        refreshToken: "flightbooking.refresh.token",
        statekey: "flightbooking.portal.openiddict.statekey"
    }

    static readonly cookiePath = "/";

    static readonly themeStorageKey = "data-bs-theme";
}