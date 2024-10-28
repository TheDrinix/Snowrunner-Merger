import {useRouter} from "vue-router";
import {useUserStore} from "@/stores/userStore";
import {useHttp} from "@/composables/useHttp";

export function useLogout() {
    const router = useRouter();
    const userStore = useUserStore();

    const http = useHttp();

    async function logout() {
        try {
            await http.post('/auth/logout', {}, {
                withCredentials: true
            });
        } catch (e) {
            console.error(e);
        }

        userStore.logout();

        await router.push({name: 'home'});
    }

    return logout;
}