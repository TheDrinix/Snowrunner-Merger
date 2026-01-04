import {defineStore} from "pinia";
import type {Toast, ToastType} from "@/types/toast";
import {ref} from "vue";

export const useToaster = defineStore('toaster', () => {
    const toasts = ref<Toast[]>([]);

    const removeToast = (id: number) => {
        toasts.value = toasts.value.filter((toast) => toast.id !== id);
    }

    const createToast = (
        title: string,
        message: string = '',
        type: ToastType = 'info',
        duration = 5000,
        icon = true
    ) => {
        const id = Date.now();

        const toast: Toast = {
            id,
            title,
            message,
            icon,
            type,
            duration
        }

        toasts.value = [...toasts.value, toast];

        setTimeout(() => {
            removeToast(id);
        }, duration);
    }

    return {
        toasts,
        removeToast,
        createToast
    }
})