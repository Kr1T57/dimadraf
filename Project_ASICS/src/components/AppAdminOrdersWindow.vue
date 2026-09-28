<script setup>
import { onMounted, ref, computed } from 'vue'
import useOrderStore from "@/stores/order.js"
import AppOrderCard from "@/components/AppOrderCard.vue";
import AppOrderEditStatus from "@/components/AppEditStatusOrderModal.vue";


const orderStore = useOrderStore()

onMounted(async () => {
  await orderStore.getAllOrders()
})

const stats = computed(() => {
  const orders = orderStore.order || []
  return {
    total: orders.length,
    new: orders.filter(o => o.status === 'Ожидает').length,
    processing: orders.filter(o => o.status === 'В обработке' || o.status === 'Отправлен').length,
    delivered: orders.filter(o => o.status === 'Доставлен').length
  }
})

</script>

<template>
  <div class="flex flex-col gap-8">
    <div class="flex flex-col gap-6">
      <h2 class="text-3xl font-black uppercase italic">Управление заказами</h2>

      <div class="grid grid-cols-4 gap-4">
        <div v-for="(val, label) in { 'Всего заказов': stats.total, 'Ожидают': stats.new, 'В работе': stats.processing, 'Завершено': stats.delivered }"
             :key="label"
             class="bg-white border-2 border-black p-5 rounded-xl ">
          <p class="text-[10px] font-black uppercase text-gray-400 mb-1">{{ label }}</p>
          <p class="text-2xl font-black">{{ val }}</p>
        </div>
      </div>
    </div>

    <div class="bg-white border-2 border-black rounded-2xl overflow-hidden shadow-[10px_10px_0px_0px_rgba(0,0,0,1)]">
      <div class="flex items-center justify-between py-4 px-6 border-b-2 border-black bg-gray-50 font-black text-[10px] text-gray-500 uppercase tracking-widest">
        <div class="w-1/6">ID / Дата</div>
        <div class="w-1/4">Пользователь</div>
        <div class="w-1/6 text-center">Сумма</div>
        <div class="w-1/6 text-center">Статус</div>
        <div class="w-1/6 text-right">Действия</div>
      </div>

      <div class="flex flex-col">
        <app-order-card
            v-for="order in orderStore.order"
            :key="order.id"
            :order="order"
        />
      </div>
    </div>

  </div>
</template>