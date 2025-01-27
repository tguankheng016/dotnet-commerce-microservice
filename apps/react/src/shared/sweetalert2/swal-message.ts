import Swal, { SweetAlertOptions } from 'sweetalert2';

class SwalMessageService {
    static showAlert(options: SweetAlertOptions) {
        return Swal.fire(options);
    }

    static showSuccess(title: string, text?: string) {
        return this.showAlert({
            icon: 'success',
            title,
            text,
            confirmButtonText: 'OK',
        });
    }

    static showError(title: string, text?: string) {
        return this.showAlert({
            icon: 'error',
            title,
            text,
            confirmButtonText: 'OK',
        });
    }

    static showWarning(title: string, text?: string) {
        return this.showAlert({
            icon: 'warning',
            title,
            text,
            confirmButtonText: 'OK',
        });
    }

    static showInfo(title: string, text?: string) {
        return this.showAlert({
            icon: 'info',
            title,
            text,
            confirmButtonText: 'OK',
        });
    }

    static showConfirmation(
        title: string,
        text: string,
        onConfirm: () => void,
        onCancel?: () => void,
        confirmButtonText?: string,
        cancelButtonText?: string,
    ) {
        return Swal.fire({
            title,
            text,
            icon: 'warning',
            showCancelButton: true,
            confirmButtonText: confirmButtonText ?? "Yes",
            cancelButtonText: cancelButtonText ?? "No",
        }).then((result) => {
            if (result.isConfirmed) {
                onConfirm();
            } else if (onCancel) {
                onCancel();
            }
        });
    }
}

export default SwalMessageService