import { Injectable } from "@angular/core";
import { GetCurrentSessionResult, IdentityServiceProxy, UserLoginInfoDto } from "@shared/service-proxies/identity-service-proxies";
import { lastValueFrom } from "rxjs";

@Injectable()
export class AppSessionService {
    private _user: UserLoginInfoDto;

    constructor(
        private _identityService: IdentityServiceProxy) {
    }

    get user(): UserLoginInfoDto {
        return this._user;
    }

    init(): Promise<boolean> {
        return new Promise<boolean>((resolve, reject) => {
            const currentSession$ = this._identityService.getCurrentSession();

            lastValueFrom(currentSession$).then((result: GetCurrentSessionResult) => {
                this._user = result.user;

                appHelper.auth.allPermissions = result.allPermissions;
                appHelper.auth.grantedPermissions = result.grantedPermissions;

                resolve(true);
            }, (err) => {
                reject(err);
            });
        });
    }
}