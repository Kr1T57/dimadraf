<script setup>
import { useToast } from "vue-toastification"
import useOrderStore from "@/stores/order.js"

const props = defineProps({
  order: { type: Object, required: true },
  isEditModalOpen: { type: Boolean, default: false }
})

const emit = defineEmits(['close'])
const orderStore = useOrderStore()
const toast = useToast()

const statuses = ['Ожидает', 'В обработке', 'Отправлен', 'Доставлен', 'Отменен']

const saveStatus = async () => {
  try {
    await orderStore.editOrderStatus(props.order)
    toast.success("Статус успешно обновлен!")
    emit('close')
  } catch (e) {
    toast.error("Ошибка при обновлении")
  }
}
</script>

<template>
  <div v-if="isEditModalOpen" class="fixed inset-0 bg-black/60 flex items-center justify-center z-[1000] p-4 backdrop-blur-sm text-black">
    <div class="bg-white w-full max-w-md border-2 border-black p-8 flex flex-col relative rounded-2xl shadow-[15px_15px_0px_0px_rgba(0,0,0,1)]">

      <button @click="emit('close')" class="absolute top-4 right-4 text-xl font-bold hover:scale-110 transition-transform">✕</button>

      <h1 class="font-black text-xl mb-6 uppercase italic border-b-2 border-black pb-2">Заказ #{{ order.id }}</h1>

      <div class="flex flex-col gap-6">
        <div class="bg-gray-50 border-2 border-black border-dashed p-4 flex flex-col gap-2">
          <div>
            <label class="text-[9px] font-black uppercase text-gray-400">Клиент</label>
            <p class="text-xs font-bold">{{ order.user }}</p>
          </div>
          <div>
            <label class="text-[9px] font-black uppercase text-gray-400">Адрес доставки</label>
            <p class="text-xs font-bold leading-tight">
              г. {{ order.city }}, ул. {{ order.street }}, д. {{ order.house }}<br>
              <span class="text-gray-500 italic">Индекс: {{ order.postalCode }}</span>
            </p>
          </div>
        </div>

        <div class="flex flex-col">
          <label class="text-[10px] font-black uppercase text-gray-400 mb-1">Изменить статус</label>
          <select v-model="order.status" class="h-12 border-2 border-black px-4 font-black bg-white outline-none focus:bg-yellow-50 transition-colors cursor-pointer">
            <option v-for="status in statuses" :key="status" :value="status">
              {{ status }}
            </option>
          </select>
        </div>

        <div class="flex gap-3 mt-2">
          <button @click="saveStatus" class="flex-1 h-14 bg-black text-white font-black uppercase hover:bg-gray-800 transition-all shadow-[4px_4px_0px_0px_rgba(0,0,0,0.3)]">
            Сохранить
          </button>
          <button @click="emit('close')" class="flex-1 h-14 border-2 border-black font-black uppercase hover:bg-gray-100 transition-all">
            Отмена
          </button>
        </div>
      </div>
    </div>
  </div>
</template>