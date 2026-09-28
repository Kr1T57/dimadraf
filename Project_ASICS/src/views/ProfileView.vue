<script setup>
import { useRouter } from 'vue-router'
import useUserStore from '@/stores/user.js'
import { ref, useTemplateRef, onMounted } from 'vue'
import useOrderStore from '@/stores/order.js'
import { useToast } from 'vue-toastification'
import AppProfileOrder from '@/components/AppProfileOrder.vue'

const router = useRouter()
const userStore = useUserStore()
const orderStore = useOrderStore()
const isModalOpen = ref(false)

const name = useTemplateRef('name')
const surname = useTemplateRef('surname')
const patronymic = useTemplateRef('patronymic')
const phone = useTemplateRef('phone')
const email = useTemplateRef('login')

const toast = useToast()

onMounted(async () => {
  if (userStore.user && userStore.user.id) {
    await orderStore.getUserOrderHistory(userStore.user.id)
  }
})

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

const ed = async () => {
  const trimmedName = name.value.value.trim()
  const trimmedSurname = surname.value.value.trim()
  const trimmedPatronymic = patronymic.value.value ? patronymic.value.value.trim() : ''
  const trimmedEmail = email.value.value.trim()
  const trimmedPhone = phone.value.value.trim()

  if (!trimmedName || !trimmedSurname || !trimmedPhone || !trimmedEmail) {
    toast.error('Заполните все поля')
    return
  }
  if (!regexValidation(trimmedEmail, 'email')) {
    toast.error('Неверный формат почты')
    return
  }
  if (!regexValidation(trimmedPhone, 'phone')) {
    toast.error('Неверный формат номера телефона')
    return
  }

  const updatedData = {
    ...userStore.user,
    name: trimmedName,
    surname: trimmedSurname,
    patronymic: trimmedPatronymic,
    email: trimmedEmail,
    phone: trimmedPhone
  }
  const success = await userStore.editUser(updatedData)
  if (success) {
    isModalOpen.value = false
  }
}
const logout = () => {
  userStore.logout()
}
</script>

<template>
  <div class="p-10 flex items-start justify-start bg-gray-100 gap-10 min-h-screen">
    <div class="relative mt-10">
      <label class="absolute top-0 left-5 -translate-y-1/2 bg-gray-100 px-2">Мой профиль</label>
      <div
        class="flex items-start justify-center w-130 p-5 gap-1 border border-black flex-col"
        id="mainDiv"
      >
        <div class="flex items-center justify-between w-full mb-4">
          <h1 class="font-black text-3xl mt-3 text-[rgb(0,30,98)]">Персональные данные</h1>
          <button
            class="font-bold text-3xl hover:text-blue-500 cursor-pointer"
            @click="isModalOpen = true"
          >
            ✎
          </button>
        </div>
        <h2 class="font-black mt-3 text-[rgb(0,30,98)]">Имя</h2>
        <span
          class="w-full h-10 flex items-center px-3 border border-black"
          v-if="userStore.user"
          >{{ userStore.user.name }}</span
        >
        <h2 class="font-black mt-3 text-[rgb(0,30,98)]">Фамилия</h2>
        <span
          class="w-full h-10 flex items-center px-3 border border-black"
          v-if="userStore.user"
          >{{ userStore.user.surname }}</span
        >
        <h2 class="font-black mt-3 text-[rgb(0,30,98)]">Отчество</h2>
        <span
          class="w-full h-10 flex items-center px-3 border border-black"
          v-if="userStore.user"
          >{{ userStore.user?.patronymic || 'Отсутствует' }}</span
        >
        <h2 class="font-black text-[rgb(0,30,98)]">Почта</h2>
        <span
          class="w-full h-10 flex items-center px-3 border border-black"
          v-if="userStore.user"
          >{{ userStore.user.email }}</span
        >
        <h2 class="font-black text-[rgb(0,30,98)]">Телефон</h2>
        <span
          class="w-full h-10 flex items-center px-3 border border-black"
          v-if="userStore.user"
          >{{ userStore.user.phone }}</span
        >

        <button
            @click="logout"
            class="w-full h-12 border border-red-600 text-red-600 font-black text-lg uppercase rounded-xl hover:bg-red-50 cursor-pointer transition-colors mt-6"
        >
          Выйти из аккаунта
        </button>
      </div>
    </div>
    <div class="flex items-start justify-center w-150 p-5 border border-black flex-col mt-10">
      <h1 class="font-black text-3xl mt-3 text-[rgb(0,30,98)]">История заказов</h1>
      <div class="w-full flex flex-col">
        <template v-if="orderStore.orderHistory && orderStore.orderHistory.length > 0">
          <AppProfileOrder
              v-for="orderItem in orderStore.orderHistory"
              :key="orderItem.OrderId"
              :order="orderItem"
          />
        </template>

        <span v-else class="text-gray-500 italic">История пока пуста (нет доставленных заказов)</span>
      </div>
    </div>
    <Teleport to="body">
      <div
        v-if="isModalOpen"
        class="fixed inset-0 bg-black/50 flex items-center justify-center z-[999] p-4"
      >
        <div
          class="bg-white w-full max-w-lg border-2 border-black p-8 flex flex-col relative rounded-2xl"
        >
          <button
            @click="isModalOpen = false"
            class="absolute top-4 right-4 text-2xl font-bold cursor-pointer"
          >
            ✕
          </button>

          <h1 class="font-black text-3xl mb-6 uppercase text-[rgb(0,30,98)]">Изменить данные</h1>

          <div class="flex flex-col gap-4">
            <div class="flex flex-col">
              <label class="text-xs uppercase mb-1 text-[rgb(0,30,98)]">Имя</label>
              <input
                :value="userStore.user.name"
                ref="name"
                class="w-full h-12 border border-black px-3 focus:bg-gray-50 outline-none"
                type="text"
              />
            </div>

            <div class="flex flex-col">
              <label class="text-xs uppercase mb-1 text-[rgb(0,30,98)]">Фамилия</label>
              <input
                :value="userStore.user.surname"
                ref="surname"
                class="w-full h-12 border border-black px-3 focus:bg-gray-50 outline-none"
                type="text"
              />
            </div>

            <div class="flex flex-col">
              <label class="text-xs uppercase mb-1 text-[rgb(0,30,98)]">Отчество</label>
              <input
                :value="userStore.user.patronymic"
                ref="patronymic"
                class="w-full h-12 border border-black px-3 focus:bg-gray-50 outline-none"
                type="text"
              />
            </div>

            <div class="flex flex-col">
              <label class="text-xs uppercase mb-1 text-[rgb(0,30,98)]">Почта</label>
              <input
                :value="userStore.user.email"
                ref="login"
                class="w-full h-12 border border-black px-3 focus:bg-gray-50 outline-none"
                type="text"
              />
            </div>

            <div class="flex flex-col">
              <label class="text-xs uppercase mb-1 text-[rgb(0,30,98)]">Телефон</label>
              <input
                :value="userStore.user.phone"
                ref="phone"
                class="w-full h-12 border border-black px-3 focus:bg-gray-50 outline-none"
                type="text"
              />
            </div>

            <div class="flex flex-row justify-between gap-3 mt-6">
              <button
                @click="isModalOpen = false"
                class="w-full h-14 rounded-xl border border-black bg-blue-50 text-[rgb(0,30,98)] text-xl uppercase hover:bg-gray-100 cursor-pointer"
              >
                Отмена
              </button>
              <button
                @click="ed"
                class="w-full h-14 rounded-xl bg-[rgb(0,30,98)] text-white font-black text-xl uppercase hover:bg-gray-800 cursor-pointer"
              >
                Сохранить
              </button>
            </div>
          </div>
        </div>
      </div>
    </Teleport>
  </div>
</template>

<style scoped></style>
