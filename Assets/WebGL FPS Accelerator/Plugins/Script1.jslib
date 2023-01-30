mergeInto(LibraryManager.library, {

    isAndroid2: function () {
        return (/Android/i.test(navigator.userAgent));
    },

    isMobileBrowser2: function () {
        return (/iPhone|iPad|iPod|Android/i.test(navigator.userAgent));
    },

    isiOS2: function () {
        return (/iPhone|iPad/i.test(navigator.userAgent));
    },

    _setDPR: function (float1) {
        window.devicePixelRatio = float1;
    },

    getDefaultDPR: function () {
        var defaultDPR = window.devicePixelRatio;

        return defaultDPR;
    }

});