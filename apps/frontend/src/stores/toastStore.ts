import {defineStore} from "pinia";
import type {Toast, ToastType} from "@/types/toast";

export const useToaster = defineStore('toaster', {
    state: () => ({
        toasts: [] as Toast[]
    }),
    actions: {
        removeToast(id: number) {
            this.toasts = this.toasts.filter((toast) => toast.id !== id);
        },
        createToast(message: string, type: ToastType = 'info', duration = 5000, icon = true) {
            const id = Date.now()
            const toast: Toast = {
                id,
                message,
                icon,
                type,
                duration
            };

            console.log('Creating toast:', toast.id);

            this.toasts = [...this.toasts, toast];

            setTimeout(() => {
                this.removeToast(id);
            }, duration);
        }
    }
})