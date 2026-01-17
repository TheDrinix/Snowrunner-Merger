import {defineStore} from "pinia";
import type {LoginResponse, User, UserStore} from "@/types/auth";
import {useGroupsStore} from "@/stores/groupsStore";
import {computed, ref } from "vue";
import {useHttp} from "@/composables/useHttp";

const defaultState = {
    user: undefined,
    accessToken: undefined,
    accessTokenExpires: undefined,
    oauthProviders: undefined,
}

export const useUserStore = defineStore("user", () => {
    const user = ref<User | undefined>(undefined);
    const accessToken = ref<string | undefined>(undefined);
    const accessTokenExpires = ref<Date | undefined>(undefined);
    const oauthProviders = ref<string[] | undefined>(undefined);

    const isAuthenticated = computed(() => {
        return !!(accessTokenExpires.value && accessTokenExpires.value > new Date());
    });

    const axios = useHttp();

    function signIn(data: LoginResponse) {
        console.log(data);
        user.value = data.user;
        accessToken.value = data.accessToken;
        accessTokenExpires.value = new Date(Date.now() + data.expiresIn * 1000);
    }

    async function refreshToken() {
        try {
            const res = await axios.post<LoginResponse>("/auth/refresh", {}, { withCredentials: true });
            signIn(res.data);
        } catch (e) {
            user.value = undefined;
            accessToken.value = undefined;
            accessTokenExpires.value = new Date(0);
            oauthProviders.value = undefined;
        }
    }

    async function getAccessToken() {
        if (accessTokenExpires.value && accessTokenExpires.value < new Date()) {
            await refreshToken();
        }
        return accessToken.value;
    }

    function logout() {
        user.value = undefined;
        accessToken.value = undefined;
        accessTokenExpires.value = undefined;
        oauthProviders.value = undefined;

        const groupsStore = useGroupsStore();
        groupsStore.clearStore();
    }

    function storeUser(u: User) {
        user.value = u;
    }

    async function fetchOAuthProviders() {
        try {
            const res = await axios.get<string[]>("/auth/oauth/providers");
            oauthProviders.value = res.data;
        } catch (e) {
            console.error(e);
        }
    }

    return {
        user,
        accessToken,
        accessTokenExpires,
        oauthProviders,
        isAuthenticated,
        signIn,
        refreshToken,
        getAccessToken,
        logout,
        storeUser,
        fetchOAuthProviders,
    };
});


/*
export const useUserStore = defineStore('user', {
    state: (): UserStore => ({...defaultState}),
    getters: {
        isAuthenticated(state) {
            return !(!state.accessTokenExpires || state.accessTokenExpires < new Date());
        },
    },
    actions: {
        signIn(data: LoginResponse) {
            console.log(data);

            this.user = data.user;
            this.accessToken = data.accessToken;
            this.accessTokenExpires = new Date(Date.now() + data.expiresIn * 1000);
        },
        async refreshToken() {
            try {
                const res = await this.axios.post<LoginResponse>('/auth/refresh', {}, {
                    withCredentials: true
                });

                this.signIn(res.data);
            } catch (e) {
                this.$state = {...defaultState, accessTokenExpires: new Date(0)};
            }
        },
        async getAccessToken() {
            if (this.accessTokenExpires && this.accessTokenExpires < new Date()) {
                await this.refreshToken();
            }

            return this.accessToken;
        },
        logout() {
            this.$state = {...defaultState};

            const groupsStore = useGroupsStore();

            groupsStore.clearStore();
        },
        storeUser(user: User) {
            this.user = user;
        },
        removeGoogleId() {
            if (this.user) {
                this.user.googleId = undefined;
            }
        },
        async fetchOAuthProviders() {
            try {
                const res = await this.axios.get<string[]>('/auth/oauth/providers');

                this.oauthProviders = res.data;
            } catch (e) {
                console.error(e);
            }
        }
    }
});*/