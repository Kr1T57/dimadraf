<script setup>
import { ref, onBeforeMount } from 'vue'
import useProductStore from "@/stores/product.js"
import useCategoryStore from "@/stores/category.js"
import useBrandStore from "@/stores/brand.js"
import { useToast } from "vue-toastification"

const props = defineProps(['isOpen'])
const emit = defineEmits(['close'])

const productStore = useProductStore()
const categoryStore = useCategoryStore()
const brandStore = useBrandStore()
const toast = useToast()

const newProduct = ref({
  name: '',
  description: '',
  brand: '',
  category: '',
  basePrice: 0
})

const image = ref(null)
const handleFileUpload = (event) => {
  image.value = event.target.files[0] ?? null
}

onBeforeMount(async () => {
  await brandStore.getAllBrand()
  await categoryStore.getAllCategories()
})

const addProduct = async () => {
  if (!newProduct.value.name || !newProduct.value.brand || !newProduct.value.category || !newProduct.value.basePrice || !newProduct.value.description || !image.value) {
    toast.error("Заполните основные поля!")
    return
  }

  try {
    const response = await productStore.addProduct(newProduct.value)
    const createdVariant = response.data
    await productStore.addImageProduct(image.value, { ...createdVariant })
    toast.success("Товар успешно добавлен!")
    await productStore.productFun()
    emit('close')
  } catch (e) {
    toast.error("Ошибка при добавлении")
  }
}
</script>

<template>
  <div v-if="isOpen" class="fixed inset-0 bg-black/60 flex items-center justify-center z-[1000] p-4 backdrop-blur-sm text-black">
    <div class="bg-white w-full max-w-xl border-2 border-black p-8 rounded-2xl shadow-[15px_15px_0px_0px_rgba(0,0,0,1)] max-h-[95vh] overflow-y-auto">

      <div class="flex justify-between items-center mb-6 border-b-4 border-black pb-2">
        <h2 class="font-black uppercase italic text-2xl">Новый продукт</h2>
        <button @click="emit('close')" class="text-2xl font-bold hover:rotate-90 transition-transform">✕</button>
      </div>

      <div class="flex flex-col gap-5">

        <div class="flex flex-col">
          <label class="text-[10px] font-black uppercase text-gray-400 mb-1 ml-1">Название модели</label>
          <input v-model="newProduct.name" type="text" placeholder="Например: Gel-Kayano 30" class="h-12 border-2 border-black px-4 font-bold outline-none focus:bg-yellow-50" />
        </div>

        <div class="grid grid-cols-2 gap-4">
          <div class="flex flex-col">
            <label class="text-[10px] font-black uppercase text-gray-400 mb-1 ml-1">Бренд</label>
            <select v-model="newProduct.brand" class="h-12 border-2 border-black px-3 font-bold bg-white outline-none cursor-pointer">
              <option value="">Выбрать...</option>
              <option v-for="b in brandStore.brand" :key="b.id" :value="b.name">{{ b.name }}</option>
            </select>
          </div>
          <div class="flex flex-col">
            <label class="text-[10px] font-black uppercase text-gray-400 mb-1 ml-1">Категория</label>
            <select v-model="newProduct.category" class="h-12 border-2 border-black px-3 font-bold bg-white outline-none cursor-pointer">
              <option value="">Выбрать...</option>
              <option v-for="c in categoryStore.category" :key="c.id" :value="c.name">{{ c.name }}</option>
            </select>
          </div>
        </div>

        <div class="flex flex-col">
          <label class="text-[10px] font-black uppercase text-gray-400 mb-1 ml-1">Базовая цена ($)</label>
          <input v-model="newProduct.basePrice" type="number" class="h-12 border-2 border-black px-4 font-bold outline-none" />
        </div>

        <div class="flex flex-col">
          <label class="text-[10px] font-black uppercase text-gray-400 mb-1 ml-1">Описание товара</label>
          <textarea v-model="newProduct.description" rows="4" placeholder="Введите характеристики..." class="border-2 border-black p-4 font-medium outline-none resize-none focus:bg-blue-50"></textarea>
        </div>

        <div class="flex flex-col">
          <input
              class="w-full bg-white border border-b focus:border-[rgb(0,30,98)] rounded-3xl px-6 py-4 outline-none text-lg"
              type="file"
              accept="image/*"
              @change="(e) => handleFileUpload(e)"
          />
        </div>

        <div class="flex gap-4 mt-4">
          <button @click="addProduct" class="flex-1 bg-black text-white h-14 font-black uppercase tracking-widest hover:bg-gray-800 transition-colors shadow-[6px_6px_0px_0px_rgba(0,0,0,0.2)] active:translate-y-1 active:shadow-none">
            Создать товар
          </button>
          <button @click="emit('close')" class="flex-1 border-2 border-black h-14 font-black uppercase tracking-widest hover:bg-gray-50 transition-colors">
            Отмена
          </button>
        </div>

      </div>
    </div>
  </div>
</template>