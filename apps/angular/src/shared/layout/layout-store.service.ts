import { Injectable } from "@angular/core";
import { Observable, BehaviorSubject, pluck, distinctUntilChanged, map } from "rxjs";

@Injectable()
export class LayoutStoreService {
    public readonly config$: Observable<boolean>;
    private readonly initialLayoutConfig = true;
    private configSource: BehaviorSubject<boolean>;

    constructor() {
        this.configSource = new BehaviorSubject(this.initialLayoutConfig);
        this.config$ = this.configSource.asObservable();
    }

    get sidebarExpanded(): Observable<boolean> {
        return this.config$.pipe(
            map(config => config),
            distinctUntilChanged()
        ) as Observable<boolean>;
    }

    public setSidebarExpanded(value: boolean): void {
        this.configSource.next(value);
    }
}