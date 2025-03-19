export interface User {
    id: string;
    email: string;
    username: string;
    googleId?: string;
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

export interface GoogleLinkingTokenData {
    token: string;
    expiresAt: Date;
    user: User;
}

export interface GoogleAccountCompletionTokenData {
    token: string;
    expiresAt: Date;
    email: string;
}

export type GoogleLoginRes =
    | { tokenType: 1; data: LoginResponse }
    | { tokenType: 2; data: GoogleLinkingTokenData }
    | { tokenType: 4; data: GoogleAccountCompletionTokenData };