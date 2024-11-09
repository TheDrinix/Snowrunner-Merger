export type ToastType = 'success' | 'error' | 'warning' | 'info';

export interface Toast {
    id: number;
    message: string;
    icon: boolean;
    type: ToastType;
    duration: number;
    sticky?: boolean;
}