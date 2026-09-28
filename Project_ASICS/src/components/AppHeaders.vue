<script setup>
import { useRouter } from 'vue-router'
import useUserStore from '@/stores/user.js'
import user from '@/stores/user.js'

const router = useRouter()
const userStore = useUserStore()
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
        <router-link class="hover:text-blue-500" to="/profile" v-if="userStore.user"
          >Мой профиль</router-link
        >
        <router-link class="hover:text-blue-500" to="/admin" v-if="userStore.user?.role === 'Менеджер'"
          >Услуги</router-link
        >
      </div>
      <div class="flex items-center space-x-6 text-gray-600">
        <button
            v-if="userStore.user"
            class="flex items-center gap-3 text-4xl  hover:scale-110 transition-transform"
            @click="router.push({ name: 'Favorite' })"
            title="Избранное"
        >
          ♡
        </button>
        <router-link to="/cart">
          <button
            class="flex items-center gap-3 text-2xl"
            v-if="userStore.user"
            @click="router.push({ name: 'Cart' })"
          >
            🛒
          </button>
        </router-link>
        <span v-if="userStore.user">{{ userStore.user.name }} {{ userStore.user.surname }}</span>
        <button
          class="bg-black text-white px-4 py-2 rounded-full text-1xl"
          @click="router.push({ name: 'Authorization' })"
          v-if="!userStore.user"
        >
          Войти
        </button>
      </div>
    </nav>
  </header>
</template>

<style scoped></style>
