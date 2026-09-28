<script setup>

import useUserStore from "@/stores/user.js";
import {onMounted} from "vue";

const userStore = useUserStore()

onMounted(async () => {
  if (localStorage.getItem('user')) {
    try {
      const localStorageUser = JSON.parse(localStorage.getItem('user') ?? '')

      await userStore.authUser(localStorageUser.login, localStorageUser.password)
    } catch (e) {
      console.error("Ошибка автоавторизации:", e)
      localStorage.removeItem('user')
    }
  }
})
</script>

<template>
  <div>
    <app-header></app-header>
    <router-view></router-view>
    <app-footer></app-footer>
  </div>
</template>

<style scoped></style>
