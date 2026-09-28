<script setup>
import useUserStore from '@/stores/user.js'
import { useTemplateRef } from 'vue'
import { useRouter } from 'vue-router'
import { useToast } from 'vue-toastification'

const login = useTemplateRef('login')
const password = useTemplateRef('password')

const toast = useToast()
const router = useRouter()
const userStore = useUserStore()
const auth = async () => {
  if (
      !login.value.value ||
      !password.value.value
  ) {
    toast.error('Заполните все поля')
    return
  }
  await userStore.authUser(login.value.value, password.value.value)
  if (!userStore.user) {
    toast.error('Неверный логин или пароль!')
    return
  }
  toast.success('Добро пожаловать!')
  if (userStore.user) {
    await router.push('/')
  }
}
</script>

<template>
  <div class="min-h-screen w-full flex items-center justify-center bg-gray-100">
    <div class="flex items-start justify-center w-200 p-5 border border-black flex-col">
      <h1 class="font-black text-3xl mt-3">Авторизация</h1>
      <span class="font-black mt-3">Почта</span>
      <input class="w-full h-10 border border-black" type="text" ref="login" />
      <span class="font-black">Пароль</span>
      <input class="w-full h-10 border border-black" type="password" ref="password" />
      <button class="w-full h-15 bg-black text-white text-2xl mt-5 w" @click="auth">Войти</button>
      <div class="flex justify-center items-center self-center flex-row mt-2">
        <h3>Нету аккаунта?</h3>
        <router-link class="text-black border-b-2 hover:text-blue-500" to="/registration"
          >Создать аккаунт</router-link
        >
      </div>
    </div>
  </div>
</template>

<style scoped></style>
