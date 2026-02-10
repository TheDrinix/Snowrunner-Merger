import type { RouteLocationRaw } from "vue-router";

export interface NavigationLink {
    name: string;
    to: RouteLocationRaw
}