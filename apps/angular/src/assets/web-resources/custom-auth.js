"use strict";

var appHelper = appHelper || {};

appHelper.auth = appHelper.auth || {};
appHelper.auth.allPermissions = appHelper.auth.allPermissions || {};
appHelper.auth.grantedPermissions = appHelper.auth.grantedPermissions || {};

(function () {
    appHelper.auth.isGranted = function (permissionName) {
        return appHelper.auth.grantedPermissions[permissionName] != undefined;
    };
})();