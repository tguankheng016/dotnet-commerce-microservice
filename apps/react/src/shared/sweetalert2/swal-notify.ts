import Swal, { SweetAlertOptions } from 'sweetalert2';

const Toast = Swal.mixin({
    toast: true,
    position: "bottom-end",
    showConfirmButton: false,
    timer: 3000,
    customClass: {
        popup: 'swal-toast',
    }
});

const showNotification = (message: string, options: SweetAlertOptions, title?: string, iconClass?: string) => {
    const icon = iconClass ? "<i class=\"mr-2 text-dark ".concat(iconClass, "\"></i>") : "";

    if (title) {
        options.title = "".concat(icon, "<span class=\"text-dark\">").concat(title, "</span>");
    }

    options.html = "".concat(title ? "" : icon, "\n    <span class=\"text-dark\">").concat(message, "</span>");
    Toast.fire(options);
};

class SwalNotifyService {
    static success(text: string, title?: string) {
        showNotification(text, {
            background: '#34bfa3'
        }, title, "fas fa-check-circle");
    }
    
    static error(text: string, title?: string) {
        showNotification(text, {
            background: '#f4516c'
        }, title, "fas fa-exclamation-circle");
    }
    
    static info(text: string, title?: string) {
        showNotification(text, {
            background: '#36a3f7'
        }, title, "fas fa-info-circle");
    }
    
    static warning(text: string, title?: string) {
        showNotification(text, {
            background: '#ffb822'
        }, title, "fas fa-exclamation-triangle");
    }
}

export default SwalNotifyService