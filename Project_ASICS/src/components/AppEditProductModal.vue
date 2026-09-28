<script setup>
import {ref, computed, watch, onMounted} from "vue";
import { useToast } from "vue-toastification";
import useProductStore from "@/stores/product.js";
import useCategoryStore from "@/stores/category.js";

const props = defineProps({
  isModalOpen: Boolean,
  product: Object
})
const emit = defineEmits(['closeModal'])
const toast = useToast()
const productStore = useProductStore()
const categoryStore = useCategoryStore()

onMounted(async () => {
  if (categoryStore.category.length === 0) {
    await categoryStore.getAllCategories()
  }
})
const save = async () => {
  try {
    await productStore.editProduct(props.product)
    toast.success("Данные успешно сохранены!")
    emit('closeModal')
    await productStore.productFun()
  } catch (e) {
    toast.error("Ошибка при сохранении")
  }
}
</script>

<template>
  <div v-if="isModalOpen" class="fixed inset-0 bg-black/60 flex items-center justify-center z-[1000] p-4 backdrop-blur-sm text-black">
    <div class="bg-white w-full max-w-lg border-2 border-black p-8 flex flex-col relative rounded-2xl shadow-[10px_10px_0px_0px_rgba(0,0,0,1)]">

      <button @click="emit('closeModal')" class="absolute top-4 right-4 text-xl font-bold">✕</button>

      <h1 class="font-black text-xl mb-6 uppercase italic border-b-2 border-black pb-2">Редактирование</h1>

      <div class="flex flex-col gap-4">
        <div class="flex flex-col">
          <label class="text-[10px] font-black uppercase text-gray-400">Название</label>
          <input v-model="product.name" class="h-10 border-2 border-black px-3 font-bold outline-none focus:bg-yellow-50" />
        </div>

        <div class="flex flex-col">
          <label class="text-[10px] font-black uppercase text-gray-400">Категория</label>
          <select v-model="product.category" class="h-10 border-2 border-black px-3 font-bold bg-white outline-none">
            <option v-for="cat in categoryStore.category" :key="cat.id" :value="cat.name">
              {{ cat.name }}
            </option>
          </select>
        </div>

        <div class="flex flex-col">
          <label class="text-[10px] font-black uppercase text-gray-400">Цена ($)</label>
          <input v-model="product.basePrice" type="number" class="h-10 border-2 border-black px-3 font-bold outline-none" />
        </div>

        <div class="flex flex-col">
          <label class="text-[10px] font-black uppercase text-gray-400">Описание</label>
          <textarea v-model="product.description" class="h-24 border-2 border-black p-3 font-medium outline-none resize-none" />
        </div>

        <div class="flex gap-3 mt-4">
          <button @click="save" class="flex-1 h-12 bg-black text-white font-black uppercase hover:bg-gray-800 transition-all">
            Сохранить
          </button>
          <button @click="emit('closeModal')" class="flex-1 h-12 border-2 border-black font-black uppercase hover:bg-gray-100 transition-all">
            Отмена
          </button>
        </div>
      </div>
    </div>
  </div>
</template>