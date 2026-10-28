<script setup>
import { ref, useTemplateRef } from 'vue'
import { useRouter } from 'vue-router'
import { useToast } from 'vue-toastification'
import useOrderStore from '@/stores/order.js'
import useUserStore from '@/stores/user.js'
import useCartItemStore from '@/stores/cartItems.js'

const props = defineProps({
  isModalOpen: {
    type: Boolean,
    default: false,
  },
  totalPrice: {
    type: Number,
    required: true,
  },
})

const emit = defineEmits(['closeModal'])

const router = useRouter()
const toast = useToast()
const orderStore = useOrderStore()
const userStore = useUserStore()
const cartStore = useCartItemStore()

const city = useTemplateRef('city')
const street = useTemplateRef('street')
const numberHouse = useTemplateRef('numberHouse')
const postalIndex = useTemplateRef('postalIndex')

const isLoading = ref(false)

const addOrder = async () => {
  const cityVal = city.value?.value?.trim()
  const streetVal = street.value?.value?.trim()
  const houseVal = numberHouse.value?.value?.trim()
  const postalVal = postalIndex.value?.value?.trim()

  if (!cityVal || !streetVal || !houseVal || !postalVal) {
    toast.error('Заполните все поля')
    return
  }

  if (isLoading.value) return
  isLoading.value = true

  try {
    const order = {
      id: 0,
      user: userStore.user.email,
      orderDate: '',
      status: 'Ожидает',
      totalAmount: props.totalPrice,
      city: cityVal,
      street: streetVal,
      house: houseVal,
      postalCode: postalVal,
    }

    await orderStore.addOrder(order)
    if (!orderStore.order?.id) return

    await orderStore.addOrderItem(userStore.user.id, orderStore.order.id)
    cartStore.clearAllCart()
    toast.success('Успешно оформлено')

    emit('closeModal')
    await router.push({ name: 'Catalog' })
  } finally {
    isLoading.value = false
  }
}
</script>

<template>
  <div
      v-if="isModalOpen"
      class="fixed inset-0 bg-black/50 flex items-center justify-center z-[999] p-4"
  >
    <div
        class="bg-white w-full max-w-lg border-2 border-black p-8 flex flex-col relative rounded-2xl shadow-[10px_10px_0px_0px_rgba(0,0,0,1)]"
    >
      <button
          :disabled="isLoading"
          @click="emit('closeModal')"
          class="absolute top-4 right-4 text-2xl font-bold cursor-pointer disabled:opacity-30"
      >
        ✕
      </button>

      <h1 class="font-black text-3xl mb-6 uppercase text-[rgb(0,30,98)]">Заполните адрес</h1>

      <div class="flex flex-col gap-4">
        <div class="flex flex-col">
          <label class="text-xs uppercase mb-1 text-[rgb(0,30,98)]">Город</label>
          <input
              :disabled="isLoading"
              ref="city"
              class="w-full h-12 border border-black px-3 focus:bg-gray-50 outline-none disabled:bg-gray-100"
              type="text"
          />
        </div>

        <div class="flex flex-col">
          <label class="text-xs uppercase mb-1 text-[rgb(0,30,98)]">Улица</label>
          <input
              :disabled="isLoading"
              ref="street"
              class="w-full h-12 border border-black px-3 focus:bg-gray-50 outline-none disabled:bg-gray-100"
              type="text"
          />
        </div>

        <div class="flex flex-col">
          <label class="text-xs uppercase mb-1 text-[rgb(0,30,98)]">Номер дома</label>
          <input
              :disabled="isLoading"
              ref="numberHouse"
              class="w-full h-12 border border-black px-3 focus:bg-gray-50 outline-none disabled:bg-gray-100"
              type="number"
              maxlength="3"
              max="999"
              min="1"
          />
        </div>

        <div class="flex flex-col">
          <label class="text-xs uppercase mb-1 text-[rgb(0,30,98)]">Почтовый индекс</label>
          <input
              :disabled="isLoading"
              ref="postalIndex"
              class="w-full h-12 border border-black px-3 focus:bg-gray-50 outline-none disabled:bg-gray-100"
              type="number"
              maxlength="6"
              max="999999"
              min="100000"
          />
        </div>

        <div class="flex flex-row justify-between gap-3 mt-6">
          <button
              :disabled="isLoading"
              @click="emit('closeModal')"
              class="w-full h-14 rounded-xl border border-black bg-blue-50 text-[rgb(0,30,98)] text-xl uppercase hover:bg-gray-100 cursor-pointer disabled:opacity-30 disabled:cursor-not-allowed"
          >
            Отмена
          </button>

          <button
              :disabled="isLoading"
              @click="addOrder"
              class="w-full h-14 rounded-xl bg-[rgb(0,30,98)] text-white font-black text-xl uppercase hover:bg-blue-900 disabled:bg-gray-400 disabled:cursor-not-allowed cursor-pointer flex items-center justify-center gap-3 transition-colors"
          >
            <svg
                v-if="isLoading"
                class="animate-spin h-6 w-6 text-white"
                xmlns="http://www.w3.org/2000/svg"
                fill="none"
                viewBox="0 0 24 24"
            >
              <circle
                  class="opacity-25"
                  cx="12"
                  cy="12"
                  r="10"
                  stroke="currentColor"
                  stroke-width="4"
              ></circle>
              <path
                  class="opacity-75"
                  fill="currentColor"
                  d="M4 12a8 8 0 018-8v8H4z"
              ></path>
            </svg>
            <span>{{ isLoading ? 'Оформление...' : 'Оформить' }}</span>
          </button>
        </div>
      </div>
    </div>
  </div>
</template>
