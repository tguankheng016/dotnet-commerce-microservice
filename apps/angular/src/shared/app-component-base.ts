import { Component, Injector } from "@angular/core";
import { AppSessionService } from "./session/app-session.service";
import { StringService } from "./utils/string.service";
import { AppConsts } from "./app-consts";
import { PrimengTableHelper } from "./helpers/primeng-table-helper";
import { FormGroup, ValidationErrors } from "@angular/forms";

export class CustomValidateFormGroupErrorMessage {
    errorKey: string;
    customErrorMsg: string;

    constructor(errorKey: string, customErrorMsg: string) {
        this.errorKey = errorKey;
        this.customErrorMsg = customErrorMsg;
    }
}

@Component({
    template: '',
})
export abstract class AppComponentBase {

    appSession: AppSessionService;
    stringService: StringService;
    primengTableHelper: PrimengTableHelper;

    scrollable = false;

    constructor(injector: Injector) {
        this.appSession = injector.get(AppSessionService);
        this.stringService = injector.get(StringService);
        this.primengTableHelper = new PrimengTableHelper();
    }

    ngAfterViewInit(): void {
        this.scrollable = false;
        setTimeout(() => {
            this.scrollable = window.innerWidth > 960;
        }, 0);
    }

    getUiAvatarProfilePicture(): string {
        return this.stringService.formatString(
            AppConsts.uiAvatarsBaseUrl,
            this.appSession.user.firstName + ' ' + this.appSession.user.lastName);
    }

    validateFormGroup(
        form: FormGroup,
        errorCount: number = 0,
        errorKey: string = '',
        scrollToErrorElement: boolean = true
    ): boolean {
        Object.keys(form.controls).forEach((key) => {
            form.get(key).markAsDirty();

            const controlErrors: ValidationErrors = form.get(key).errors;

            if (controlErrors != null) {
                errorKey = key;
                errorCount++;

                if (scrollToErrorElement) {
                    let targetElement = document.getElementById(key);
                    window.scroll({ top: targetElement.offsetTop, behavior: 'smooth' });
                    scrollToErrorElement = false;
                }
            }
        });

        if (!form.valid || errorCount > 0) {
            appHelper.notify.error('Kindly complete all the required fields.');

            return false;
        }

        return true;
    }

    isGranted(permissionName: string): boolean {
        return appHelper.auth.isGranted(permissionName);
    }
}