import type {App} from "vue";
import axios from "axios";


export default {
    install: (app: App) => {
        // Check if app is in production or development
        const url = import.meta.env.DEV ? 'https://localhost:44303/api' : '/api';

        app.config.globalProperties.$axios = axios.create({
            baseURL: url,
        })
    }
}