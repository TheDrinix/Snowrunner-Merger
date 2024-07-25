import {inject} from "vue";
import type {AxiosInstance} from "axios";
import axios from "axios";
import {useUserStore} from "@/stores/userStore";

export function useHttp() {
    const http = inject<AxiosInstance>('axios', axios.create());

    const userStore = useUserStore();

    http.interceptors.request.use(config => {
        if (userStore.isAuthenticated) {
            userStore.getAccessToken().then(token => {
                config.headers.Authorization = `Bearer ${token}`;
            });
        }

        return config;
    });

    return http;
}