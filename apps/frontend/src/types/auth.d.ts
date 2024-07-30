export interface User {
    id: string;
    email: string;
    username: string;
}

export interface UserStore {
    user?: User;
    accessToken?: string;
    accessTokenExpires?: Date;
}

export interface LoginResponse {
    accessToken: string;
    expiresIn: number;
    user: User;
}