import { Injectable } from "@angular/core";

@Injectable()
export class StringService {
    notNullOrEmpty(str): boolean {
        if (str != '' && str != undefined && str != null) {
            return true;
        } else {
            return false;
        }
    }

    randomString(length: number): string {
        let result = '';
        const characters = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';
        const charactersLength = characters.length;
        let counter = 0;
        while (counter < length) {
            result += characters.charAt(Math.floor(Math.random() * charactersLength));
            counter += 1;
        }
        return result;
    }

    isNumber(value: string | number): boolean {
        return value != null && value !== '' && !isNaN(Number(value.toString()));
    }

    formatString(template: string, ...args: any[]): string {
        return template.replace(/{(\d+)}/g, (match, number) => {
            const index = parseInt(number, 10);
            return index >= 0 && index < args.length ? args[index] : match;
        });
    }
}