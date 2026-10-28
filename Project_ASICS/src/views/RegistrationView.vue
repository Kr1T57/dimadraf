<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useToast } from 'vue-toastification'
import useUserStore from '@/stores/user.js'

const router = useRouter()
const userStore = useUserStore()
const toast = useToast()

const surname = ref('')
const name = ref('')
const patronymic = ref('')
const email = ref('')
const phone = ref('')
const password = ref('')
const passwordError = ref('')
const showPassword = ref(false)

const onPhoneInput = (e) => {
  const input = e.target
  let digits = input.value.replace(/\D/g, '')

  if (!digits || digits === '7' || digits === '8') {
    input.value = ''
    phone.value = ''
    return
  }

  const formatted = formatPhone(digits)
  input.value = formatted
  phone.value = formatted
}

const onPhoneKeydown = (e) => {
  const allowedKeys = ['Backspace', 'Delete', 'ArrowLeft', 'ArrowRight', 'Tab', 'Home', 'End']
  if (allowedKeys.includes(e.key) || e.ctrlKey || e.metaKey) {
    if (e.key === 'Backspace') {
      const input = e.target
      const start = input.selectionStart
      const end = input.selectionEnd

      if (start !== end) return

      if (start <= 4) {
        e.preventDefault()
        input.value = ''
        phone.value = ''
        return
      }

      const charBefore = input.value[start - 1]
      if (/\D/.test(charBefore)) {
        e.preventDefault()

        const textBefore = input.value.slice(0, start)
        let digitsBefore = textBefore.replace(/\D/g, '')
        if (digitsBefore.startsWith('7') || digitsBefore.startsWith('8')) {
          digitsBefore = digitsBefore.slice(1)
        }

        let allDigits = input.value.replace(/\D/g, '')
        if (allDigits.startsWith('7') || allDigits.startsWith('8')) {
          allDigits = allDigits.slice(1)
        }

        if (digitsBefore.length > 0) {
          const deleteIdx = digitsBefore.length - 1
          allDigits = allDigits.slice(0, deleteIdx) + allDigits.slice(deleteIdx + 1)

          const formatted = formatPhone(allDigits)
          input.value = formatted
          phone.value = formatted

          const newPos = Math.max(0, start - 2)
          input.setSelectionRange(newPos, newPos)
        }
      }
    }
    return
  }

  if (!/^\d$/.test(e.key)) {
    e.preventDefault()
  }
}

const formatPhone = (rawDigits) => {
  let digits = rawDigits.replace(/\D/g, '')
  if (digits.startsWith('7') || digits.startsWith('8')) {
    digits = digits.slice(1)
  }
  digits = digits.substring(0, 10)

  if (!digits) return ''

  let formatted = '+7 ('
  if (digits.length > 0) formatted += digits.substring(0, 3)
  if (digits.length >= 3) formatted += ') ' + digits.substring(3, 6)
  if (digits.length >= 6) formatted += '-' + digits.substring(6, 8)
  if (digits.length >= 8) formatted += '-' + digits.substring(8, 10)
  return formatted
}

const validatePassword = () => {
  const val = password.value

  if (val.length < 8) {
    passwordError.value = 'Пароль должен быть не менее 8 символов'
    return false
  }
  if (!/[A-Z]/.test(val)) {
    passwordError.value = 'Пароль должен содержать хотя бы одну заглавную латинскую букву'
    return false
  }
  if (!/[a-z]/.test(val)) {
    passwordError.value = 'Пароль должен содержать хотя бы одну строчную латинскую букву'
    return false
  }
  if (!/\d/.test(val)) {
    passwordError.value = 'Пароль должен содержать хотя бы одну цифру'
    return false
  }

  passwordError.value = ''
  return true
}

const reg = async () => {
  if (!name.value.trim() || !surname.value.trim() || !email.value.trim() || !password.value.trim()) {
    toast.error('Заполните все обязательные поля')
    return
  }

  const emailRegex = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[A-Za-z]{2,}$/
  if (!emailRegex.test(email.value.trim())) {
    toast.error('Неверный формат почты')
    return
  }

  const rawDigits = phone.value.replace(/\D/g, '')
  if (rawDigits.length !== 11) {
    toast.error('Введите номер телефона полностью')
    return
  }

  if (!validatePassword()) {
    return
  }

  const cleanPhone = '+7' + rawDigits.slice(1)

  const user = {
    id: 0,
    name: name.value.trim(),
    surname: surname.value.trim(),
    patronymic: patronymic.value.trim() || '',
    phone: cleanPhone,
    email: email.value.trim(),
    password: password.value,
    role: ''
  }

  await userStore.registerUser(user)

  if (userStore.user) {
    toast.success('Добро пожаловать!')
    await router.push('/')
  }
}
</script>

<template>
  <div class="min-h-screen w-full flex items-center justify-center bg-gray-100 p-4">
    <div class="flex items-start justify-center w-full max-w-lg p-8 border-2 border-black bg-white rounded-2xl shadow-[8px_8px_0px_0px_rgba(0,0,0,1)] flex-col">
      <h1 class="font-black text-3xl mb-6 uppercase text-[rgb(0,30,98)]">Регистрация</h1>

      <span class="font-bold text-xs uppercase text-gray-500 mb-1">Фамилия *</span>
      <input v-model="surname" class="w-full h-11 border-2 border-black px-3 font-bold mb-3 outline-none" type="text" />

      <span class="font-bold text-xs uppercase text-gray-500 mb-1">Имя *</span>
      <input v-model="name" class="w-full h-11 border-2 border-black px-3 font-bold mb-3 outline-none" type="text" />

      <span class="font-bold text-xs uppercase text-gray-500 mb-1">Отчество</span>
      <input v-model="patronymic" class="w-full h-11 border-2 border-black px-3 font-bold mb-3 outline-none" type="text" />

      <span class="font-bold text-xs uppercase text-gray-500 mb-1">Электронная почта *</span>
      <input v-model="email" class="w-full h-11 border-2 border-black px-3 font-bold mb-3 outline-none" type="email" placeholder="example@mail.com" />

      <span class="font-bold text-xs uppercase text-gray-500 mb-1">Номер телефона *</span>
      <input
        :value="phone"
        @input="onPhoneInput"
        @keydown="onPhoneKeydown"
        maxlength="18"
        class="w-full h-11 border-2 border-black px-3 font-bold mb-3 outline-none"
        type="tel"
        placeholder="+7 (999) 000-00-00"
      />

      <span class="font-bold text-xs uppercase text-gray-500 mb-1">Пароль *</span>
      <div class="relative w-full" :class="passwordError ? 'mb-1' : 'mb-6'">
        <input
          v-model="password"
          @input="validatePassword"
          class="w-full h-11 border-2 border-black px-3 pr-24 font-bold outline-none"
          :type="showPassword ? 'text' : 'password'"
          placeholder="••••••••"
        />
        <button
          type="button"
          @click="showPassword = !showPassword"
          class="absolute right-2 top-1/2 -translate-y-1/2 text-xs font-black uppercase text-gray-500 hover:text-black cursor-pointer px-2 py-1 select-none"
        >
          {{ showPassword ? 'Скрыть' : 'Показать' }}
        </button>
      </div>
      <span v-if="passwordError" class="text-red-500 text-xs font-bold mb-4">
        {{ passwordError }}
      </span>

      <button
        class="w-full h-14 bg-black text-white text-xl font-black uppercase border-2 border-black shadow-[4px_4px_0px_0px_rgba(0,0,0,0.2)] active:translate-y-0.5 hover:bg-gray-800 cursor-pointer transition-all"
        @click="reg"
      >
        Зарегистрироваться
      </button>

      <div class="flex justify-center items-center self-center flex-row gap-2 mt-6 text-sm">
        <span class="text-gray-500 font-bold">Уже есть аккаунт?</span>
        <router-link class="text-black font-black border-b-2 border-black hover:text-blue-600 transition-colors" to="/authorization">
          Войти в аккаунт
        </router-link>
      </div>
    </div>
  </div>
</template>