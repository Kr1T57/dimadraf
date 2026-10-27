<script setup>
import { watch } from 'vue'
import { useRouter } from 'vue-router'
import useUserStore from '@/stores/user.js'
import useCartItemStore from '@/stores/cartItems.js'
import useAnalyticsStore from '@/stores/analytics.js'

const router = useRouter()
const userStore = useUserStore()
const cartStore = useCartItemStore()
const analyticsStore = useAnalyticsStore()

const syncUserData = (user) => {
  if (!user?.id) return
  cartStore.getCartItems(user.id)
  analyticsStore.fetchUserFavorites(user.id)
}

watch(
    () => userStore.user,
    (user) => {
      if (!user?.id) {
        cartStore.clearAllCart()
        analyticsStore.clearFavorites()
        return
      }
      cartStore.getCartItems(user.id)
      analyticsStore.fetchUserFavorites(user.id)
    },
    { immediate: true }
)
</script>

<template>
  <header class="bg-white shadow-md">
    <nav class="container mx-auto px-6 py-4 flex items-center justify-between">
      <div class="flex items-center justify-between gap-3">
        <router-link to="/">
          <img src="../assets/asics-seeklogo.png" alt="Logo" class="h-15 w-40" />
        </router-link>
      </div>

      <div class="flex items-center space-x-6 text-gray-600">
        <router-link class="hover:text-blue-500" to="/catalog">Товары</router-link>
        <router-link class="hover:text-blue-500" to="/compare" v-if="userStore.user">Сравнение</router-link>
        <router-link class="hover:text-blue-500" to="/profile" v-if="userStore.user">Мой профиль</router-link>
        <router-link
            class="hover:text-blue-500"
            to="/admin"
            v-if="userStore.user?.role === 'Менеджер' || userStore.user?.role === 'Кладовщик'"
        >
          Услуги
        </router-link>
      </div>

      <div class="flex items-center space-x-6 text-gray-600">
        <!-- Кнопка Избранного -->
        <button
            v-if="userStore.user"
            class="relative flex items-center justify-center p-2 hover:scale-110 transition-transform cursor-pointer"
            @click="router.push({ name: 'Favorite' })"
            title="Избранное"
        >
          <span class="text-2xl">♡</span>
          <span
              v-if="analyticsStore.totalFavoritesCount > 0"
              class="absolute -top-1 -right-1 bg-red-500 text-white text-[10px] font-black w-5 h-5 rounded-full flex items-center justify-center border border-black"
          >
            {{ analyticsStore.totalFavoritesCount }}
          </span>
        </button>

        <!-- Кнопка Корзины -->
        <router-link to="/cart" v-if="userStore.user" class="relative flex items-center justify-center p-2 cursor-pointer">
          <span class="text-2xl">🛒</span>
          <span
              v-if="cartStore.cartTotalQuantity > 0"
              class="absolute -top-1 -right-1 bg-[rgb(0,30,98)] text-white text-[10px] font-black w-5 h-5 rounded-full flex items-center justify-center border border-black"
          >
            {{ cartStore.cartTotalQuantity }}
          </span>
        </router-link>

        <span v-if="userStore.user" class="font-bold text-sm text-black">
          {{ userStore.user.name }} {{ userStore.user.surname }}
        </span>

        <button
            class="bg-black text-white px-4 py-2 rounded-full text-sm font-bold cursor-pointer hover:bg-gray-800"
            @click="router.push({ name: 'Authorization' })"
            v-if="!userStore.user"
        >
          Войти
        </button>
      </div>
    </nav>
  </header>
</template>