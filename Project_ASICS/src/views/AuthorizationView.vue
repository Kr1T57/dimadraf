<script setup>
import useUserStore from '@/stores/user.js'
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useToast } from 'vue-toastification'

const login = ref('')
const password = ref('')
const showPassword = ref(false)

const toast = useToast()
const router = useRouter()
const userStore = useUserStore()

const auth = async () => {
  if (!login.value.trim() || !password.value.trim()) {
    toast.error('Заполните все поля')
    return
  }
  await userStore.authUser(login.value.trim(), password.value.trim())
  if (!userStore.user) {
    toast.error('Неверный логин или пароль!')
    return
  }
  toast.success('Добро пожаловать!')
  await router.push('/')
}
</script>

<template>
  <div class="min-h-screen w-full flex items-center justify-center bg-gray-100 p-4">
    <div class="flex items-start justify-center w-full max-w-lg p-8 border-2 border-black bg-white rounded-2xl shadow-[8px_8px_0px_0px_rgba(0,0,0,1)] flex-col">
      <h1 class="font-black text-3xl mb-6 uppercase text-[rgb(0,30,98)]">Авторизация</h1>
      
      <span class="font-bold text-xs uppercase text-gray-500 mb-1">Почта</span>
      <input
        v-model="login"
        class="w-full h-11 border-2 border-black px-3 font-bold mb-4 outline-none"
        type="text"
        placeholder="example@mail.com"
      />

      <span class="font-bold text-xs uppercase text-gray-500 mb-1">Пароль</span>
      <div class="relative w-full mb-6">
        <input
          v-model="password"
          class="w-full h-11 border-2 border-black px-3 pr-24 font-bold outline-none"
          :type="showPassword ? 'text' : 'password'"
          placeholder="••••••••"
          @keyup.enter="auth"
        />
        <button
          type="button"
          @click="showPassword = !showPassword"
          class="absolute right-2 top-1/2 -translate-y-1/2 text-xs font-black uppercase text-gray-500 hover:text-black cursor-pointer px-2 py-1 select-none"
        >
          {{ showPassword ? 'Скрыть' : 'Показать' }}
        </button>
      </div>

      <button
        class="w-full h-14 bg-black text-white text-xl font-black uppercase border-2 border-black shadow-[4px_4px_0px_0px_rgba(0,0,0,0.2)] active:translate-y-0.5 hover:bg-gray-800 cursor-pointer transition-all"
        @click="auth"
      >
        Войти
      </button>

      <div class="flex justify-center items-center self-center flex-row gap-2 mt-6 text-sm">
        <span class="text-gray-500 font-bold">Нет аккаунта?</span>
        <router-link class="text-black font-black border-b-2 border-black hover:text-blue-600 transition-colors" to="/registration">
          Создать аккаунт
        </router-link>
      </div>
    </div>
  </div>
</template>