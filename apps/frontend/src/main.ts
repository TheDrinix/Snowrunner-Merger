import './assets/main.css'

import { createApp } from 'vue'
import { createPinia } from 'pinia'
import createAxios from './plugins/axios'

import App from './App.vue'
import router from './router'
import type {AxiosInstance} from "axios";

declare module 'pinia' {
    export interface PiniaCustomProperties {
        axios: AxiosInstance;
    }
}

const app = createApp(App);

app.use(createAxios);

const pinia = createPinia().use(() => ({
    axios: app.config.globalProperties.$axios
}))

app.use(pinia);
app.use(router);

app.provide<AxiosInstance, string>('axios', app.config.globalProperties.$axios);

app.mount('#app');
