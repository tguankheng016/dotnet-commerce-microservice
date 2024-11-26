import { Pipe, PipeTransform } from "@angular/core";

@Pipe({
    name: 'permission',
})
export class PermissionPipe implements PipeTransform {
    transform(permission: string): boolean {
        return appHelper.auth.isGranted(permission);
    }
}
