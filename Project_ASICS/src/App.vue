<script setup>
import useUserStore from "@/stores/user.js";
import { onMounted } from "vue";
import AppAiCoachWidget from "@/components/AppAiCoachWidget.vue";

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
 <!-- :key="$route.fullPath" заставляет Vue перезагружать карточку при переходе на другой товар -->
 <router-view :key="$route.fullPath"></router-view>
 <app-footer></app-footer>
 <app-ai-coach-widget />
 </div>
</template>

<style scoped></style>