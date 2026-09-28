<script setup>
import AppOrderEditStatus from "@/components/AppEditStatusOrderModal.vue";
import { ref } from "vue";

const props = defineProps({
  order: { type: Object, required: true }
})

const isEditModalOpen = ref(false)
</script>

<template>
  <div class="flex items-center justify-between py-5 px-6 border-b border-gray-100 hover:bg-gray-50 transition-colors font-bold">
    <div class="w-1/6">
      <p class="text-blue-600">#{{ order.id }}</p>
      <p class="text-[9px] text-gray-400 uppercase tracking-tighter">{{ order.orderDate }}</p>
    </div>

    <div class="w-1/4 truncate text-xs">
      {{ order.user }}
    </div>

    <div class="w-1/6 text-center text-sm">
      ₽{{ order.totalAmount }}
    </div>

    <div class="w-1/6 text-center">
      <span class="px-3 py-1 rounded-full text-[9px] font-black uppercase border-2 border-black bg-yellow-300 ">
        {{ order.status }}
      </span>
    </div>

    <div class="w-1/6 text-right">
      <button @click="isEditModalOpen = true" class="p-2 hover:bg-black hover:text-white border-2 border-black rounded-lg transition-all">
        ✏️
      </button>
    </div>

    <teleport to="body">
      <app-order-edit-status
          :is-edit-modal-open="isEditModalOpen"
          :order="order"
          @close="isEditModalOpen = false"
      />
    </teleport>
  </div>
</template>