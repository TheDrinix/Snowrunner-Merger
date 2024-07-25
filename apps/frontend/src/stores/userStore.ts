import {defineStore} from "pinia";
import type {LoginResponse, UserStore} from "@/types/auth";

const defaultState = {
    isAuthenticated: false,
    user: undefined,
    accessToken: undefined,
    accessTokenExpires: undefined
}

export const useUserStore = defineStore('user', {
    state: (): UserStore => ({...defaultState}),
    getters: {
    },
    actions: {
        signIn(data: LoginResponse) {
            console.log(data);

            this.isAuthenticated = true;
            this.user = data.user;
            this.accessToken = data.accessToken;
            this.accessTokenExpires = new Date(Date.now() + data.expiresIn * 1000);
        },
        async refreshToken() {
            const res = await this.axios.post<LoginResponse>('/auth/refresh', {}, {
                withCredentials: true
            });

            if (res.status === 200) {
                this.signIn(res.data);
            } else {
                this.$state = {...defaultState};
            }
        },
        async getAccessToken() {
            if (this.accessTokenExpires && this.accessTokenExpires < new Date()) {
                await this.refreshToken();
            }

            return this.accessToken;
        }
    }
});