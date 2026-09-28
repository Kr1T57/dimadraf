<script setup>
import {onBeforeMount, ref} from 'vue'
import useProductStore from "@/stores/product.js"
import { useToast } from "vue-toastification"
import useColorStore from "@/stores/color.js";

const emit = defineEmits(['close'])
const productStore = useProductStore()
const toast = useToast()
const colorStore = useColorStore()
const newVariant = ref({
  productId: null,
  color: null,
  size: '',
  stockQuantity: 0
})

const image = ref(null)
const handleFileUpload = (event) => {
  image.value = event.target.files[0] ?? null
}

onBeforeMount(async () => {
  if (colorStore.color.length === 0) {
    await colorStore.getAllColors()
  }
})
const add = async () => {
  if (!newVariant.value.productId || !newVariant.value.color || !newVariant.value.size || !newVariant.value.stockQuantity || !image.value){
    toast.error("Заполните поля")
    return
  }
  const response = await productStore.addProductVariant(newVariant.value)
  const createdVariant = response.data
  await productStore.addImageVariant(image.value, { ...createdVariant })
  toast.success("Вариация успешно добавлена")
  await productStore.productFun()
  emit('close')
}
</script>

<template>
  <div class="fixed inset-0 bg-black/60 flex items-center justify-center z-[1000] p-4 backdrop-blur-sm text-black">
    <div class="bg-white w-full max-w-md border-2 border-black p-6 rounded-2xl ">
      <h2 class="font-black uppercase mb-4 italic text-blue-600">Новая вариация</h2>

      <div class="flex flex-col gap-3">
        <select v-model="newVariant.productId" class="h-10 border-2 border-black px-2 font-bold">
          <option :value="null">Выберите товар...</option>
          <option v-for="p in productStore.products" :key="p.id" :value="p.id">{{ p.name }}</option>
        </select>

        <select v-model="newVariant.color" class="h-10 border-2 border-black px-2 font-bold">
          <option :value="null">Выберите цвет...</option>
          <option v-for="color in colorStore.color" :key="color.id" :value="color.name">{{ color.name }}</option>
        </select>

        <input v-model="newVariant.size" placeholder="Размер (например, 42)" class="h-10 border-2 border-black px-3" />
        <input v-model="newVariant.stockQuantity" type="number" placeholder="Количество" class="h-10 border-2 border-black px-3" />

        <input
            class="w-full bg-white border border-b focus:border-[rgb(0,30,98)] rounded-3xl px-6 py-4 outline-none text-lg"
            type="file"
            accept="image/*"
            @change="(e) => handleFileUpload(e)"
        />

        <button @click="add" class="bg-blue-600 text-white py-3 font-black uppercase text-xs mt-2 border-2 border-black  active:translate-y-0.5 transition-all">
          Добавить в базу
        </button>
      </div>

      <button @click="emit('close')" class="w-full mt-4 text-[10px] font-bold uppercase underline">Отмена</button>
    </div>
  </div>
</template>