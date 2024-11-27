/// <reference types="vite/client" />

interface ImportMetaEnv {
    readonly VITE_APP_BASE_URL: string
    readonly VITE_REMOTE_SERVICE_BASE_URL: string
    readonly VITE_UI_AVATAR_URL: string
    readonly VITE_OPENIDDICT_URL: string
    readonly VITE_OPENIDDICT_CLIENTID: string
}

interface ImportMeta {
    readonly env: ImportMetaEnv
}