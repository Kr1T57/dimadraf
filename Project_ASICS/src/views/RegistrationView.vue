<script setup>
import { useRouter } from 'vue-router'
import useUserStore from '@/stores/user.js'
import { useTemplateRef } from 'vue'
import { useToast } from 'vue-toastification'

const name = useTemplateRef('name')
const surname = useTemplateRef('surname')
const patronymic = useTemplateRef('patronymic')
const phone = useTemplateRef('phone')
const email = useTemplateRef('login')
const password = useTemplateRef('password')

const toast = useToast()
const router = useRouter()
const userStore = useUserStore()

const regexValidation = (value, type) => {
  switch (type) {
    case 'password':
      return String(value).match(/(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).{8,}/)
    case 'email':
      return String(value).match(/^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[A-Za-z]/)
    case 'phone':
      return String(value).match(/^(?:\+7|8|7)?(\d{10})$/)
  }
}

const reg = async () => {
  if (
    !name.value.value ||
    !surname.value.value ||
    !phone.value.value ||
    !email.value.value ||
    !password.value.value
  ) {
    toast.error('Заполните все поля')
    return
  }
  if (!regexValidation(email.value.value, 'email')) {
    toast.error('Неверный формат почты')
    return
  }
  if (!regexValidation(password.value.value, 'password')) {
    toast.error(
      'Пароль должен быть не менее 8 символов, содержать нижний и верхний регистр и число',
    )
    return
  }
  if (!regexValidation(phone.value.value, 'phone')) {
    toast.error('Неверный формат номера телефона')
    return
  }
  const user = {
    id: 0,
    name: name.value.value,
    surname: surname.value.value,
    patronymic: patronymic.value.value,
    phone: phone.value.value,
    email: email.value.value,
    password: password.value.value,
    role: '',
  }
  await userStore.registerUser(user)

  toast.success('Добро пожаловать!')

  if (userStore.user) {
    await router.push('/')
  }
}
</script>

<template>
  <div class="min-h-screen w-full flex items-center justify-center bg-gray-100">
    <div class="flex items-start justify-center w-200 p-5 border border-black flex-col">
      <h1 class="font-black text-3xl mt-3">Регистрация</h1>
      <span class="font-black mt-3">Фамилия</span>
      <input class="w-full h-10 border border-black" type="text" ref="surname" />
      <span class="font-black mt-3">Имя</span>
      <input class="w-full h-10 border border-black" type="text" ref="name" />
      <span class="font-black mt-3">Отчество</span>
      <input class="w-full h-10 border border-black" type="text" ref="patronymic" />
      <span class="font-black mt-3">Почта</span>
      <input class="w-full h-10 border border-black" type="email" ref="login" />
      <span class="font-black mt-3">Номер телефона</span>
      <input class="w-full h-10 border border-black" type="tel" ref="phone" />
      <span class="font-black">Пароль</span>
      <input class="w-full h-10 border border-black" type="password" ref="password" />
      <button class="w-full h-15 bg-black text-white text-2xl mt-5 w" @click="reg">
        Зарегистрироваться
      </button>
      <div class="flex justify-center items-center self-center flex-row mt-2">
        <h3>Есть аккаунта?</h3>
        <router-link class="text-black border-b-2 hover:text-blue-500" to="/authorization"
          >Войти в аккаунт</router-link
        >
      </div>
    </div>
  </div>
</template>

<style scoped></style>
