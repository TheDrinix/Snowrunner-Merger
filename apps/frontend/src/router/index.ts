import { createRouter, createWebHistory } from 'vue-router'
import HomeView from '../views/HomeView.vue'
import LoginView from "@/views/auth/LoginView.vue";
import RegisterView from "@/views/auth/RegisterView.vue";
import {useUserStore} from "@/stores/userStore";
import {useGroupsStore} from "@/stores/groupsStore";
import GroupView from "@/views/groups/GroupView.vue";

const router = createRouter({
    history: createWebHistory(import.meta.env.BASE_URL),
    routes: [
        {
            path: '/',
            name: 'home',
            component: HomeView,
            meta: {
                bypassAuth: true
            }
        },
        {
            path: '/auth/google/callback',
            name: 'google-callback',
            component: () => import('../views/auth/GoogleCallbackView.vue')
        },
        {
            path: '/auth/register-confirm',
            name: 'register-confirm',
            component: () => import('../views/auth/RegisterConfirmationView.vue')
        },
        {
            path: '/auth',
            name: 'auth',
            component: () => import('../views/AuthView.vue'),
            meta: {
              bypassAuth: true
            },
            children: [
                {
                    path: 'login',
                    name: 'login',
                    component: LoginView
                },
                {
                    path: 'register',
                    name: 'register',
                    component: RegisterView
                }
            ]
        },
        {
            path: '/groups',
            name: 'groups',
            component: () => import('../views/groups/GroupsView.vue')
        },
        {
            path: '/groups/create',
            name: 'group-create',
            component: () => import('../views/groups/CreateGroupView.vue')
        },
        {
            path: '/group/:id',
            children: [
                {
                    path: '',
                    name: 'group',
                    component: GroupView
                },
                {
                    path: 'manage',
                    name: 'group-manage',
                   component: () => import('../views/groups/GroupManageView.vue'),
                    meta: {
                        validateGroupOwner: true
                    }
                },
                {
                    path: 'merge',
                    name: 'group-save-merge',
                    component: () => import('../views/groups/SaveMergeView.vue'),
                },
                {
                    path: 'upload',
                    name: 'group-save-upload',
                    component: () => import('../views/groups/SaveUploadView.vue'),
                    meta: {
                        validateGroupOwner: true
                    }
                }
            ],
        }
    ]
})


router.beforeEach(async (to, from, next) => {
    const userStore = useUserStore();

    if (!userStore.accessTokenExpires) {
        await userStore.refreshToken();
    }

    if (!to.meta.bypassAuth) {
        if (!userStore.isAuthenticated) {
            next({ name: 'login' });
        }
    }

    if (to.meta.validateGroupOwner) {
        const groupId = to.params.id as string;
        const groupsStore = useGroupsStore();

        // TODO: Load user's groups if they haven't been loaded yet

        if (!groupsStore.isGroupOwner(groupId)) {
            next({ name: 'group', params: { id: groupId } });
        }
    }

    next();
})


export default router
