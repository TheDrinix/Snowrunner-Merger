export type ToastType = 'success' | 'error' | 'warning' | 'info';

export interface Toast {
    id: number;
    message: string;
    icon: string;
    type: ToastType;
    duration: number;
    sticky?: boolean;
}