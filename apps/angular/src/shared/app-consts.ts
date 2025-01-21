export class AppConsts {
    static remoteServiceBaseUrl: string;
    static appBaseUrl: string;
    static appBaseHref: string;
    static openIddictUrl: string;
    static openIddictClientId: string;
    static uiAvatarsBaseUrl: string;

    static readonly cookieName = {
        accessToken: "dotnet.commercemicro.portal.access.token",
        refreshToken: "dotnet.commercemicro.portal.refresh.token",
        statekey: "dotnet.commercemicro.portal.openiddict.statekey"
    }

    static readonly cookiePath = "/";

    static readonly themeStorageKey = "data-bs-theme";
}