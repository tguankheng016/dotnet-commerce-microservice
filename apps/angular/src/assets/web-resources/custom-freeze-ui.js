"use strict";

var appHelper = appHelper || {};

appHelper.ui = appHelper.ui || {};

(function () {
    if (!FreezeUI || !UnFreezeUI) {
        return;
    }

    appHelper.ui.setBusy = function (elm, text, delay) {
        FreezeUI({
            element: elm,
            text: text ? text : " ",
            delay: delay
        });
    };

    appHelper.ui.clearBusy = function (elm, delay) {
        UnFreezeUI({
            element: elm,
            delay: delay
        });
    };
})();