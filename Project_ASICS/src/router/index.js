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
      redirect: { name: 'admin-products' },
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
