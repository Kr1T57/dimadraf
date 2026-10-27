<script setup>
import {useTemplateRef} from "vue";
import useOrderStore from "@/stores/order.js";
import {useToast} from "vue-toastification";
import useUserStore from "@/stores/user.js";
import useCartItemStore from "@/stores/cartItems.js";

const props = defineProps({
  isModalOpen: {
    type: Boolean,
    default: false,
  },
  totalPrice: {
    type: Number,
    required: true,
  }
})

const emit = defineEmits(['closeModal'])
const toast = useToast()


const orderStore = useOrderStore()
const userStore = useUserStore()
const cartStore = useCartItemStore()

const city = useTemplateRef('city')
const street = useTemplateRef('street')
const numberHouse = useTemplateRef('numberHouse')
const postalIndex = useTemplateRef('postalIndex')

const addOrder = async () => {
  const cityVal = city.value.value.trim()
  const streetVal = street.value.value.trim()
  const houseVal = numberHouse.value.value.trim()
  const postalVal = postalIndex.value.value.trim()

  if (!cityVal || !streetVal || !houseVal || !postalVal) {
    toast.error('Заполните все поля')
    return
  }

  if (cityVal.length < 2) {
    toast.error('Название города слишком короткое')
    return
  }
  if (streetVal.length < 2) {
    toast.error('Название улицы слишком короткое')
    return
  }

  if (parseInt(houseVal) === 0) {
    toast.error('Номер дома не может быть равен нулю')
    return
  }
  if (String(postalIndex.value.value).length !== 6) {
    toast.error('Почтовый индекс должен состоять ровно из 6 цифр')
    return
  }
  const order = {
    id: 0,
    user: userStore.user.email,
    orderDate: '',
    status: 'Ожидает',
    totalAmount: props.totalPrice,
    city: city.value.value,
    street: street.value.value,
    house: numberHouse.value.value,
    postalCode: postalIndex.value.value,
  }
  await orderStore.addOrder(order)
  emit('closeModal')
  if (!orderStore.order) {
    return
  }
  await orderStore.addOrderItem(userStore.user.id,orderStore.order.id)
  cartStore.clearAllCart()
  toast.success('Успешно оформлено')

}
</script>

<template>
  <div
      v-if="isModalOpen"
      class="fixed inset-0 bg-black/50 flex items-center justify-center z-999 p-4"
  >
    <div
        class="bg-white w-full max-w-lg border-2 border-black p-8 flex flex-col relative rounded-2xl"
    >
      <button
          @click="emit('closeModal')
"
          class="absolute top-4 right-4 text-2xl font-bold cursor-pointer"
      >
        ✕
      </button>

      <h1 class="font-black text-3xl mb-6 uppercase text-[rgb(0,30,98)]">Заполните адрес</h1>

      <div class="flex flex-col gap-4">
        <div class="flex flex-col">
          <label class="text-xs uppercase mb-1 text-[rgb(0,30,98)]">Город</label>
          <input
              ref="city"
              class="w-full h-12 border border-black px-3 focus:bg-gray-50 outline-none"
              type="text"
              maxlength="50"
              placeholder="Например: Москва"
              @input="e => e.target.value = e.target.value.replace(/[^a-zA-Zа-яА-ЯёЁ\s-]/g, '')"
          />
        </div>

        <div class="flex flex-col">
          <label class="text-xs uppercase mb-1 text-[rgb(0,30,98)]">Улица</label>
          <input
              ref="street"
              class="w-full h-12 border border-black px-3 focus:bg-gray-50 outline-none"
              type="text"
              maxlength="100"
          />
        </div>

        <div class="flex flex-col">
          <label class="text-xs uppercase mb-1 text-[rgb(0,30,98)]">Номер дома</label>
          <input
              ref="numberHouse"
              class="w-full h-12 border border-black px-3 focus:bg-gray-50 outline-none"
              type="text"
              maxlength="4"
              @input="e => e.target.value = e.target.value.replace(/\D/g, '')"
          />
        </div>

        <div class="flex flex-col">
          <label class="text-xs uppercase mb-1 text-[rgb(0,30,98)]">Почтовый индекс</label>
          <input
              ref="postalIndex"
              class="w-full h-12 border border-black px-3 focus:bg-gray-50 outline-none"
              type="text"
              maxlength="6"
              @input="e => e.target.value = e.target.value.replace(/\D/g, '')"
          />
        </div>

        <div class="flex flex-row justify-between gap-3 mt-6">
          <button
              @click="emit('closeModal')
"
              class="w-full h-14 rounded-xl border border-black bg-blue-50 text-[rgb(0,30,98)] text-xl uppercase hover:bg-gray-100 cursor-pointer"
          >
            Отмена
          </button>
          <button
              @click="addOrder"
              class="w-full h-14 rounded-xl bg-[rgb(0,30,98)] text-white font-black text-xl uppercase hover:bg-gray-800 cursor-pointer"
          >
            Оформить
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>

</style>