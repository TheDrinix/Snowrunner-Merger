import {inject} from "vue";
import type {AxiosInstance} from "axios";
import axios from "axios";
import {useUserStore} from "@/stores/userStore";

export function useHttp() {
    const http = inject<AxiosInstance>('axios', axios.create());

    http.interceptors.request.use(async (config) => {
        const userStore = useUserStore();

        if (userStore.isAuthenticated) {
            const token = await userStore.getAccessToken();

            if (token) {
                config.headers.Authorization = `Bearer ${token}`;
            }
        }

        return config;
    });

    return http;
}