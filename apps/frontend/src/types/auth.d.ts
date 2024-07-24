export interface User {
    email: string;
    username: string;
}

export interface UserStore {
    isAuthenticated: boolean;
    user?: User;
    accessToken?: string;
    accessTokenExpires?: Date;
}

export interface LoginResponse {
    accessToken: string;
    expiresIn: number;
    user: User;
}