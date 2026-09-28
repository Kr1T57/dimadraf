<script setup>
import { ref, computed } from 'vue'
import useProductStore from "@/stores/product.js"
import { useToast } from "vue-toastification"

const props = defineProps(['isOpen'])
const emit = defineEmits(['close'])
const productStore = useProductStore()
const toast = useToast()

const selectedProductId = ref(null)
const selectedColor = ref('')
const selectedSize = ref('')

const selectedProduct = computed(() =>{
    if (!productStore.products || !Array.isArray(productStore.products)) return null;
    return productStore.products.find(p => p.id === selectedProductId.value)
})

const colors = computed(() =>
    selectedProduct.value ? [...new Set(selectedProduct.value.variants.map(v => v.color))] : []
)

const sizes = computed(() =>
    selectedProduct.value ? selectedProduct.value.variants.filter(v => v.color === selectedColor.value).map(v => v.size) : []
)

const currentVariant = computed(() =>
    selectedProduct.value?.variants.find(v => v.color === selectedColor.value && String(v.size) === String(selectedSize.value))
)

const save = async () => {
  if (!currentVariant.value) return
  if (currentVariant.value.stockQuantity < 0) {
    toast.error("Количество товара на складе не может быть меньше 0!")
    return
  }
  await productStore.editProductVariant(currentVariant.value)
  toast.success("Количество обновлено")
}

const remove = async () => {
  if (!currentVariant.value) return
  if (confirm("Удалить эту вариацию?")) {
    await productStore.deleteProductVariant(currentVariant.value.id)
    toast.success("Вариация удалена")
    await productStore.productFun()
    emit('close')
  }
}
</script>

<template>
  <div class="fixed inset-0 bg-black/60 flex items-center justify-center z-[1000] p-4 backdrop-blur-sm text-black">
    <div class="bg-white w-full max-w-md border-2 border-black p-6 rounded-2xl shadow-[10px_10px_0px_0px_rgba(0,0,0,1)]">
      <h2 class="font-black uppercase mb-4 italic">Управление остатками</h2>

      <div class="flex flex-col gap-4">
        <select v-model="selectedProductId" class="h-10 border-2 border-black px-2 font-bold">
          <option :value="null">Выберите товар...</option>
          <option v-for="p in productStore.products" :key="p.id" :value="p.id">{{ p.name }}</option>
        </select>

        <div v-if="selectedProductId" class="grid grid-cols-2 gap-2">
          <select v-model="selectedColor" class="h-10 border-2 border-black px-2">
            <option value="">Цвет</option>
            <option v-for="c in colors" :key="c" :value="c">{{ c }}</option>
          </select>
          <select v-model="selectedSize" class="h-10 border-2 border-black px-2">
            <option value="">Размер</option>
            <option v-for="s in sizes" :key="s" :value="s">{{ s }}</option>
          </select>
        </div>

        <div v-if="currentVariant" class="bg-gray-100 p-4 border-2 border-black border-dashed">
          <label class="text-[10px] font-bold uppercase">Количество на складе</label>
          <input v-model="currentVariant.stockQuantity" type="number" min="0" class="w-full h-10 border-2 border-black px-3 mt-1" />

          <div class="flex gap-2 mt-4">
            <button @click="save" class="flex-1 bg-black text-white py-2 font-bold uppercase text-[10px]">Сохранить</button>
            <button @click="remove" class="flex-1 bg-red-600 text-white py-2 font-bold uppercase text-[10px]">Удалить</button>
          </div>
        </div>
      </div>

      <button @click="emit('close')" class="w-full mt-4 text-[10px] font-bold uppercase underline">Закрыть</button>
    </div>
  </div>
</template>