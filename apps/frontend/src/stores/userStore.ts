import {defineStore} from "pinia";
import type {LoginResponse, UserStore} from "@/types/auth";
import {useGroupsStore} from "@/stores/groupsStore";

const defaultState = {
    user: undefined,
    accessToken: undefined,
    accessTokenExpires: undefined
}

export const useUserStore = defineStore('user', {
    state: (): UserStore => ({...defaultState}),
    getters: {
        isAuthenticated(state) {
            return !(!state.accessTokenExpires || state.accessTokenExpires < new Date());
        }
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
        }
    }
});