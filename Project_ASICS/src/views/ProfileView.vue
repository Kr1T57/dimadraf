<script setup>
import { ref, watch } from 'vue'
import { useRouter } from 'vue-router'
import useUserStore from '@/stores/user.js'
import useOrderStore from '@/stores/order.js'
import { useToast } from 'vue-toastification'
import AppProfileOrder from '@/components/AppProfileOrder.vue'

const router = useRouter()
const userStore = useUserStore()
const orderStore = useOrderStore()
const toast = useToast()

const currentTab = ref('data')

const form = ref({
  name: '',
  surname: '',
  patronymic: '',
  email: '',
  phone: ''
})

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
        form.value.phone = ''
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
          form.value.phone = formatted

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

const onPhoneInput = (e) => {
  const input = e.target
  let digits = input.value.replace(/\D/g, '')

  if (!digits || digits === '7' || digits === '8') {
    input.value = ''
    form.value.phone = ''
    return
  }

  const formatted = formatPhone(digits)
  input.value = formatted
  form.value.phone = formatted
}

const syncForm = () => {
  if (userStore.user) {
    form.value.name = userStore.user.name || ''
    form.value.surname = userStore.user.surname || ''
    form.value.patronymic = userStore.user.patronymic || ''
    form.value.email = userStore.user.email || ''

    let p = (userStore.user.phone || '').replace(/\D/g, '')
    if (p.startsWith('7') || p.startsWith('8')) p = p.slice(1)
    let formatted = '+7 ('
    if (p.length > 0) formatted += p.substring(0, 3)
    if (p.length >= 3) formatted += ') ' + p.substring(3, 6)
    if (p.length >= 6) formatted += '-' + p.substring(6, 8)
    if (p.length >= 8) formatted += '-' + p.substring(8, 10)
    form.value.phone = formatted
  }
}

watch(
  () => userStore.user,
  (user) => {
    if (!user?.id) return
    syncForm()
    orderStore.getUserOrderHistory(user.id)
  },
  { immediate: true }
)

const saveProfile = async () => {
  if (!form.value.name.trim() || !form.value.surname.trim() || !form.value.email.trim()) {
    toast.error('Заполните обязательные поля')
    return
  }

  const rawPhone = form.value.phone.replace(/\D/g, '')
  if (rawPhone.length !== 11) {
    toast.error('Введите номер телефона полностью')
    return
  }

  const cleanPhone = '+7' + rawPhone.slice(1)

  const updatedData = {
    ...userStore.user,
    name: form.value.name.trim(),
    surname: form.value.surname.trim(),
    patronymic: form.value.patronymic?.trim() || '',
    email: form.value.email.trim(),
    phone: cleanPhone
  }

  const success = await userStore.editUser(updatedData)
  if (success) {
    syncForm()
  }
}

const logout = () => {
  userStore.logout()
}
</script>

<template>
  <div class="min-h-screen bg-gray-50 py-12 px-4 sm:px-6 text-black font-[Franklin]">
    <div class="max-w-4xl mx-auto flex flex-col items-center">
      <!-- Заголовок по центру -->
      <h1 class="text-4xl font-black uppercase italic tracking-tighter text-[rgb(0,30,98)] mb-8 text-center">
        Личный кабинет
      </h1>

      <!-- Вкладки по центру -->
      <div class="flex justify-center gap-4 border-b-2 border-black pb-4 mb-8 w-full max-w-xl">
        <button
          @click="currentTab = 'data'"
          :class="[
            'px-6 py-2.5 font-black uppercase text-xs sm:text-sm border-2 border-black rounded-xl transition-all cursor-pointer',
            currentTab === 'data'
              ? 'bg-[rgb(0,30,98)] text-white shadow-[3px_3px_0px_0px_rgba(0,0,0,1)]'
              : 'bg-white text-black hover:bg-gray-100'
          ]"
        >
          Персональные данные
        </button>

        <button
          @click="currentTab = 'orders'"
          :class="[
            'px-6 py-2.5 font-black uppercase text-xs sm:text-sm border-2 border-black rounded-xl transition-all cursor-pointer flex items-center gap-2',
            currentTab === 'orders'
              ? 'bg-[rgb(0,30,98)] text-white shadow-[3px_3px_0px_0px_rgba(0,0,0,1)]'
              : 'bg-white text-black hover:bg-gray-100'
          ]"
        >
          История заказов
          <!-- Счетчик в стиле сайта (белый с темно-синим текстом) -->
          <span
            :class="[
              'text-xs px-2 py-0.5 rounded-full border border-black font-black',
              currentTab === 'orders'
                ? 'bg-white text-[rgb(0,30,98)]'
                : 'bg-[rgb(0,30,98)] text-white'
            ]"
          >
            {{ orderStore.orderHistory?.length || 0 }}
          </span>
        </button>
      </div>

      <!-- Вкладка 1: Редактирование профиля (ровно по центру) -->
      <div
        v-if="currentTab === 'data'"
        class="w-full max-w-xl bg-white border-2 border-black p-8 rounded-2xl shadow-[8px_8px_0px_0px_rgba(0,0,0,1)]"
      >
        <h2 class="text-2xl font-black uppercase mb-6 text-[rgb(0,30,98)] text-center">
          Редактирование профиля
        </h2>
        <div class="flex flex-col gap-4">
          <div>
            <label class="text-xs font-bold uppercase text-gray-500">Фамилия *</label>
            <input v-model="form.surname" class="w-full h-11 border-2 border-black px-3 mt-1 font-bold outline-none" />
          </div>
          <div>
            <label class="text-xs font-bold uppercase text-gray-500">Имя *</label>
            <input v-model="form.name" class="w-full h-11 border-2 border-black px-3 mt-1 font-bold outline-none" />
          </div>
          <div>
            <label class="text-xs font-bold uppercase text-gray-500">Отчество</label>
            <input v-model="form.patronymic" class="w-full h-11 border-2 border-black px-3 mt-1 font-bold outline-none" />
          </div>
          <div>
            <label class="text-xs font-bold uppercase text-gray-500">Электронная почта *</label>
            <input v-model="form.email" type="email" class="w-full h-11 border-2 border-black px-3 mt-1 font-bold outline-none" />
          </div>
          <div>
          <label class="text-xs font-bold uppercase text-gray-500">Номер телефона *</label>
          <input
            :value="form.phone"
            @input="onPhoneInput"
            @keydown="onPhoneKeydown"
            maxlength="18"
            class="w-full h-11 border-2 border-black px-3 mt-1 font-bold outline-none"
            type="tel"
            placeholder="+7 (999) 000-00-00"
          />
        </div>

          <div class="flex gap-4 mt-6">
            <button
              @click="saveProfile"
              class="flex-1 bg-black text-white py-3.5 font-black uppercase text-sm border-2 border-black shadow-[4px_4px_0px_0px_rgba(0,0,0,0.2)] active:translate-y-0.5 cursor-pointer hover:bg-gray-800 transition-colors"
            >
              Сохранить данные
            </button>
            <button
              @click="logout"
              class="border-2 border-red-600 text-red-600 py-3.5 px-6 font-black uppercase text-sm hover:bg-red-50 cursor-pointer transition-colors"
            >
              Выйти
            </button>
          </div>
        </div>
      </div>

      <!-- Вкладка 2: История заказов -->
      <div v-if="currentTab === 'orders'" class="w-full max-w-3xl">
        <div v-if="orderStore.orderHistory && orderStore.orderHistory.length > 0" class="flex flex-col gap-4">
          <AppProfileOrder
            v-for="order in orderStore.orderHistory"
            :key="order.orderId"
            :order="order"
          />
        </div>
        <div v-else class="border-4 border-black border-dashed p-12 text-center rounded-2xl bg-white shadow-[6px_6px_0px_0px_rgba(0,0,0,1)]">
          <p class="text-gray-400 font-bold uppercase text-sm">У вас пока нет доставленных заказов</p>
        </div>
      </div>
    </div>
  </div>
</template>
