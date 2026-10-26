import { createRouter, createWebHistory } from 'vue-router'
import HomeView from '@/views/HomeView.vue'
import CatalogView from '@/views/CatalogView.vue'
import AuthorizationView from '@/views/AuthorizationView.vue'
import RegistrationView from '@/views/RegistrationView.vue'
import ProfileView from '@/views/ProfileView.vue'
import ProductView from '@/views/ProductView.vue'
import CartView from '@/views/CartView.vue'
import AdminView from "@/views/AdminView.vue";
import FavoriteView from "@/views/FavoriteView.vue";

import useUserStore from '@/stores/user.js'
import CompareView from "@/views/CompareView.vue";

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      component: HomeView,
      name: 'Home',
    },
    {
      path: '/catalog',
      component: CatalogView,
      name: 'Catalog',
    },
    {
      path: '/authorization',
      component: AuthorizationView,
      name: 'Authorization',
    },
    {
      path: '/registration',
      component: RegistrationView,
      name: 'Registration',
    },
    {
      path: '/profile',
      component: ProfileView,
      name: 'Profile',
    },
    {
      path: '/product/:id',
      component: ProductView,
      name: 'Product',
    },
    {
      path: '/cart',
      component: CartView,
      name: 'Cart',
    },
    {
      path: '/favorite',
      component: FavoriteView,
      name: 'Favorite',
    },
    {
      path: '/admin',
      component: () => import('@/views/AdminView.vue'),
      redirect: to => {
        // 2. Достаем полные данные пользователя из Pinia
        const userStore = useUserStore()
        const user = userStore.user

        if (user) {
          if (user.role === 'Менеджер') {
            return { name: 'admin-orders' }
          }
          if (user.role === 'Кладовщик') {
            return { name: 'admin-products' }
          }
        }
        return { name: 'Home' }
      },
      children: [
        {
          path: 'products',
          name: 'admin-products',
          component: () => import('@/components/AppAdminProductsWindow.vue'),
        },
        {
          path: 'orders',
          name: 'admin-orders',
          component: () => import('@/components/AppAdminOrdersWindow.vue'),
        },
        {
          path: 'users',
          name: 'admin-users',
          component: () => import('@/components/AppAdminUsersWindow.vue'),
        },
        {
          path: 'analytics',
          name: 'admin-analytics',
          component: () => import('@/components/AppAdminAnalytics.vue')
        }
      ]
    },
    {
      path: '/compare',
      component: CompareView,
      name: 'Compare',
    }
  ],
  scrollBehavior(to, from, savedPosition) {
    if (savedPosition) {
      return savedPosition
    }
    return { top: 0, behavior: 'smooth' }
  }
})

export default router
