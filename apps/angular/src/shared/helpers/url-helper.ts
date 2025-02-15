export class UrlHelper {
    /**
     * The URL requested, before initial routing.
     */
    static readonly initialUrl = location.href;

    static getQueryParameters(): any {
        return UrlHelper.getQueryParametersUsingParameters(document.location.search);
    }

    static getQueryParametersUsingParameters(search: string): any {
        return search
            .replace(/(^\?)/, '')
            .split('&')
            .map(
                function (n) {
                    return (n = n.split('=')), (this[n[0]] = n[1]), this;
                }.bind({})
            )[0];
    }

    static getInitialUrlParameters(): any {
        let questionMarkIndex = UrlHelper.initialUrl.indexOf('?');
        if (questionMarkIndex >= 0) {
            return UrlHelper.initialUrl.slice(questionMarkIndex, UrlHelper.initialUrl.length);
        }

        return '';
    }

    static getReturnUrl(): string {
        const queryStringObj = UrlHelper.getQueryParametersUsingParameters(UrlHelper.getInitialUrlParameters());
        if (queryStringObj.returnUrl) {
            return decodeURIComponent(queryStringObj.returnUrl);
        }

        return null;
    }
}