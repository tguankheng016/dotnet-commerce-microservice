import { Pipe, PipeTransform } from "@angular/core";
import moment from "moment";

@Pipe({ name: 'dateFormat' })
export class MomentFormatPipe implements PipeTransform {
    transform(value: any, dateFormat: string): any {
        if (!value) {
            return '-';
        }

        return moment(value).format(dateFormat);
    }
}