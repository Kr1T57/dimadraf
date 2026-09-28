<script setup>
import { ref } from 'vue'
import AppProfileOrderItem from './AppProfileOrderItem.vue'

const props = defineProps({
  order: {
    type: Object,
    required: true
  }
})

const isExpanded = ref(false)
</script>

<template>
  <div class="w-full border border-black p-4 bg-white flex flex-col mb-4">
    <div class="flex items-center justify-between w-full">
      <div class="flex flex-col gap-1">
        <span class="font-black text-lg text-[rgb(0,30,98)] uppercase">Заказ #ASICS-{{ order.orderId }}</span>
        <span class="text-xs text-gray-500 font-medium">Дата доставки: {{ order.orderDate }}</span>
      </div>

      <div class="flex items-center gap-6">
        <div class="text-right">
          <span class="text-xs text-gray-400 uppercase font-bold block">Сумма заказа</span>
          <span class="font-black text-xl text-black">{{ order.totalAmount }} ₽</span>
        </div>

        <button
            @click="isExpanded = !isExpanded"
            class="text-2xl font-black text-[rgb(0,30,98)] hover:text-blue-600 cursor-pointer transition-transform duration-200"
            :class="{ 'rotate-180': isExpanded }"
        >
          ▼
        </button>
      </div>
    </div>

    <div v-if="isExpanded" class="mt-4 pt-4 border-t border-dashed border-gray-300 flex flex-col gap-2">
      <h4 class="text-xs font-black uppercase text-gray-400 mb-2 tracking-wider">Купленные товары:</h4>

      <AppProfileOrderItem
          v-for="item in order.items"
          :key="item.productVariantId"
          :item="item"
      />
    </div>
  </div>
</template>

<style scoped>
.rotate-180 {
  transform: rotate(180deg);
}
</style>