import {defineStore} from "pinia";
import {ref} from "vue";
import {useHttp} from "@/composables/useHttp";
import {useUserStore} from "@/stores/userStore";

export const useOAuthStore = defineStore('oauth', () => {
    const oauthProviders = ref<string[]>([]);
    const linkedProviders = ref<string[]>([]);
    const isLoading = ref(false);
    const error = ref();

    const http = useHttp();
    const userStore = useUserStore();

    const fetchOAuthProviders = async () => {
        isLoading.value = true;
        error.value = undefined;

        try {
            const res = await http.get<string[]>('/auth/oauth/providers');
            oauthProviders.value = res.data;
        } catch (e) {
            error.value = e;
            console.error(e);
        } finally {
            isLoading.value = false;
        }
    }

    const fetchLinkedProviders = async () => {
        if (userStore.isAuthenticated) {
            isLoading.value = true;
            error.value = undefined;

            try {
                const res = await http.get<string[]>('/auth/oauth/linked', {
                    headers: {
                        'Authorization': `Bearer ${await userStore.getAccessToken()}`
                    }
                });
                linkedProviders.value = res.data;
            } catch (e: any) {
                error.value = e;
                if (e.response?.status === 401) {
                    userStore.logout();
                }
                console.error(e);
            } finally {
                isLoading.value = false;
            }
        }
    }

    return {
        oauthProviders,
        isLoading,
        error,
        fetchOAuthProviders,
        fetchLinkedProviders,
        get linkedProviders() {
            const providers: Record<string, boolean> = {};

            oauthProviders.value.forEach(provider => {
                providers[provider] = linkedProviders.value.includes(provider);
            })

            return providers;
        }
    }
})