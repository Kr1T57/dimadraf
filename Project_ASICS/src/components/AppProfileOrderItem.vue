<script setup>
import useProductStore from "@/stores/product.js"
import { ref, onBeforeMount } from 'vue'
import shoesAlt from "@/assets/shoes_alt.png"


const props = defineProps({
  item: {
    type: Object,
    required: true,
  },
})

const productStore = useProductStore()
const imageSrc = ref('')

const handleImageError = () => {
  imageSrc.value = shoesAlt
}

onBeforeMount(async () => {
  const variantImage = await productStore.getVariantImage(props.item.productVariantId)

  if (variantImage && variantImage.imagePath) {
    imageSrc.value = `http://localhost:5000/imagesVariant/${variantImage.imagePath}`
  } else {
    imageSrc.value = shoesAlt
  }
})
</script>

<template>
  <div class="flex items-center justify-between border border-gray-100 rounded-xl p-3 bg-gray-50 mb-2 w-full">
    <div class="flex items-center gap-4">
      <div class="w-20 h-20 bg-white rounded-lg flex items-center justify-center overflow-hidden border border-gray-200">
        <img :src="imageSrc" alt="shoes" class="object-contain w-full h-full mix-blend-multiply" @error="handleImageError" />
      </div>

      <div class="flex flex-col gap-0.5">
        <h3 class="text-base font-bold uppercase tracking-tight text-black">{{ item.productName }}</h3>
        <p class="text-gray-500 text-xs">
          Цвет: <span class="text-black font-medium">{{ item.color }}</span>
        </p>
        <p class="text-gray-500 text-xs">
          Размер: <span class="text-black font-medium">{{ item.size }}</span>
        </p>
        <p class="text-gray-400 text-xs mt-1">
          Количество: <span class="text-black font-bold">{{ item.quantity }} шт.</span>
        </p>
      </div>
    </div>

    <div class="text-right">
      <p class="text-gray-400 text-xs">{{ item.price }} ₽ / шт.</p>
      <p class="text-lg font-black text-[rgb(0,30,98)] mt-1">{{ item.price * item.quantity }} ₽</p>
    </div>
  </div>
</template>