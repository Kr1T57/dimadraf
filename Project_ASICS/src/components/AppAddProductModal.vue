<script setup>
import { ref, onBeforeMount } from 'vue'
import useProductStore from "@/stores/product.js"
import useCategoryStore from "@/stores/category.js"
import useBrandStore from "@/stores/brand.js"
import { useAiStore } from "@/stores/ai.js"
import { useToast } from "vue-toastification"

const props = defineProps(['isOpen'])
const emit = defineEmits(['close'])
const productStore = useProductStore()
const categoryStore = useCategoryStore()
const brandStore = useBrandStore()
const aiStore = useAiStore()
const toast = useToast()

const newProduct = ref({
 name: '',
 description: '',
 brand: '',
 category: '',
 basePrice: 0
})

const techInput = ref('')
const image = ref(null)

const handleFileUpload = (event) => {
 image.value = event.target.files[0] ?? null
}

onBeforeMount(async () => {
 await brandStore.getAllBrand()
 await categoryStore.getAllCategories()
})

const handleGenerateDescription = async () => {
 if (!newProduct.value.name) {
 toast.error("Сначала укажите название модели!")
 return
 }
 const generated = await aiStore.generateDescription({
 name: newProduct.value.name,
 category: newProduct.value.category || 'Кроссовки',
 brand: newProduct.value.brand || 'ASICS',
 technologies: techInput.value
 })
 if (generated) {
 newProduct.value.description = generated
 }
}

const isSubmitting = ref(false)

const addProduct = async () => {
  if (!newProduct.value.name || !newProduct.value.brand || !newProduct.value.category || !newProduct.value.basePrice ||
    !newProduct.value.description || !image.value) {
    toast.error("Заполните основные поля!")
    return
  }

  if (isSubmitting.value) return
  isSubmitting.value = true

  try {
    const response = await productStore.addProduct(newProduct.value)
    if (!response?.data) throw new Error("Не удалось создать товар")

    const createdProduct = response.data
    await productStore.addImageProduct(image.value, createdProduct)
    
    toast.success("Товар успешно добавлен!")
    await productStore.productFun()
    emit('close')
  } catch (e) {
    // Если ошибка уже показана в store, не дублируем
  } finally {
    isSubmitting.value = false
  }
}
</script>
<template>
 <div v-if="isOpen" class="fixed inset-0 bg-black/60 flex items-center justify-center z-[1000] p-4 backdrop-blur-sm text-black">
 <div class="bg-white w-full max-w-xl border-2 border-black p-8 rounded-2xl shadow-[15px_15px_0px_0px_rgba(0,0,0,1)] max-h-[95vh] overflow-y-auto">
 <div class="flex justify-between items-center mb-6 border-b-4 border-black pb-2">
 <h2 class="font-black uppercase italic text-2xl">Новый продукт</h2>
 <button @click="emit('close')" class="text-2xl font-bold hover:rotate-90 transition-transform cursor-pointer">✕</button>
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

 <!-- БЛОК AI COPYWRITER -->
 <div class="p-3 bg-blue-50 border-2 border-black rounded-xl flex flex-col gap-2">
 <div class="flex justify-between items-center">
 <span class="text-xs font-black uppercase text-[rgb(0,30,98)]">
 ✨ AI Copywriter
 </span>
 <button
 type="button"
 @click="handleGenerateDescription"
 :disabled="aiStore.isGenerating"
 class="bg-[rgb(0,30,98)] text-white px-3 py-1.5 rounded-lg border-2 border-black font-black uppercase text-[10px] shadow-[2px_2px_0px_0px_rgba(0,0,0,1)] hover:bg-blue-800 disabled:bg-gray-400 cursor-pointer active:translate-y-0.5"
 >
 {{ aiStore.isGenerating ? 'Генерирую...' : 'Сгенерировать с помощью ИИ' }}
 </button>
 </div>
 <input
 v-model="techInput"
 type="text"
 placeholder="Технологии: PureGEL, FlyteFoam Blast, AHAR+..."
 class="h-9 border border-black bg-white px-3 text-xs font-bold outline-none"
 />
 </div>

 <div class="flex flex-col">
 <label class="text-[10px] font-black uppercase text-gray-400 mb-1 ml-1">Описание товара</label>
 <textarea v-model="newProduct.description" rows="4" placeholder="Введите характеристики или сгенерируйте с помощью ИИ..." class="border-2 border-black p-4 font-medium outline-none resize-none focus:bg-blue-50"></textarea>
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
 <button @click="addProduct" class="flex-1 bg-black text-white h-14 font-black uppercase tracking-widest hover:bg-gray-800 transition-colors shadow-[6px_6px_0px_0px_rgba(0,0,0,0.2)] active:translate-y-1 active:shadow-none cursor-pointer">
 Создать товар
 </button>
 <button @click="emit('close')" class="flex-1 border-2 border-black h-14 font-black uppercase tracking-widest hover:bg-gray-50 transition-colors cursor-pointer">
 Отмена
 </button>
 </div>
 </div>
 </div>
 </div>
</template>