
class StringHelper {
    static formatString(template: string, ...args: string[]): string {
        if (!template) return '';
        
        return template.replace(/{(\d+)}/g, (match, number) => {
            const index = parseInt(number, 10);
            return index >= 0 && index < args.length ? args[index] : match;
        });
    }

    static randomString(length: number): string {
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

    static notNullOrEmpty(str: string | null | undefined): boolean {
        if (str != '' && str != undefined && str != null) {
            return true;
        } else {
            return false;
        }
    }
}

export default StringHelper