import { Injectable } from "@angular/core";
import localForage from 'localforage';
import { defer, from, Observable } from "rxjs";

@Injectable()
export class LocalStorageService {
    getItemObs(key: string): Observable<any> {
        let observable$ = defer(() => from(localForage.getItem(key)))
        return observable$;
    }

    setItemObs(key: string, value: any): Observable<any> {
        if (value === null) {
            value = undefined;
        }

        let observable$ = defer(() => from(localForage.setItem(key, value)))
        return observable$;
    }

    getItem(key: string, callback: any): void {
        if (!localForage) {
            return;
        }

        localForage.getItem(key, callback);
    }

    setItem(key, value, callback?: any): void {
        if (!localForage) {
            return;
        }

        if (value === null) {
            value = undefined;
        }

        localForage.setItem(key, value, callback);
    }

    removeItem(key, callback?: any): void {
        if (!localForage) {
            return;
        }

        localForage.removeItem(key, callback);
    }
}