declare namespace appHelper {
    namespace ui {
        function setBusy(elm?: any, optionsOrPromise?: any): void;
        function clearBusy(elm?: any): void;
    }

    namespace notify {
        function info(message: string, title?: string, options?: any): void;
        function success(message: string, title?: string, options?: any): void;
        function warn(message: string, title?: string, options?: any): void;
        function error(message: string, title?: string, options?: any): void;
    }

    namespace message {
        function info(message: string, title?: string, options?: any): any;
        function success(message: string, title?: string, options?: any): any;
        function warn(message: string, title?: string, options?: any): any;
        function error(message: string, title?: string, options?: any): any;
        function confirm(message: string, title?: string, callback?: (isConfirmed: boolean, isCancelled?: boolean) => void, options?: any): any;
    }

    namespace auth {
        let allPermissions: { [key: string]: boolean };
        let grantedPermissions: { [key: string]: boolean };

        function isGranted(permissionName: string): boolean;
    }
}