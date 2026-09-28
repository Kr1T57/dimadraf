<script setup>
import { useRouter } from 'vue-router'
import { ref, onMounted } from 'vue'
import useProductStore from '@/stores/product.js'
import useUserStore from '@/stores/user.js'
import useAnalyticsStore from '@/stores/analytics.js'
import shoesAlt from "@/assets/shoes_alt.png"

const props = defineProps({
  item: {
    type: Object,
    required: true
  }
})

const router = useRouter()
const productStore = useProductStore()
const userStore = useUserStore()
const analyticsStore = useAnalyticsStore()
const imageSrc = ref(shoesAlt)

onMounted(async () => {
  if (props.item.productVariantId) {
    const variantImage = await productStore.getVariantImage(props.item.productVariantId)
    if (variantImage && variantImage.imagePath) {
      imageSrc.value = `http://localhost:5000/imagesVariant/${variantImage.imagePath}`
      return
    }
  }
  imageSrc.value = shoesAlt
})

const goToVariant = () => {
  router.push({
    name: 'Product',
    params: { id: props.item.productId },
    query: {
      color: props.item.color,
      size: props.item.size
    }
  })
}

const removeVariantLike = (e) => {
  if (userStore.user) {
    analyticsStore.toggleVariantFavorite(userStore.user.id, props.item.productVariantId)
  }
}
</script>

<template>
  <div
      @click="goToVariant"
      class="bg-white border-4 border-black p-4 rounded-xl  hover:translate-x-1 hover:translate-y-1 hover:shadow-[4px_4px_0px_0px_rgba(0,0,0,1)] transition-all cursor-pointer flex flex-col justify-between h-full text-black"
  >
    <div class="h-48 w-full flex items-center justify-center overflow-hidden border-2 border-black border-dashed mb-4 p-2 bg-gray-50">
      <img :src="imageSrc" alt="Variant image" class="h-full object-contain mix-blend-multiply" />
    </div>

    <div class="flex-1 flex flex-col justify-between">
      <div class="flex items-start justify-between gap-2">
        <h3 class="font-black uppercase text-xl leading-tight line-clamp-2">{{ item.variantName }}</h3>

        <button
            @click.stop="removeVariantLike"
            class="p-1.5 border-2 border-black rounded-lg bg-white shadow-[2px_2px_0px_0px_rgba(0,0,0,1)] hover:translate-y-0.5 hover:shadow-[1px_1px_0px_0px_rgba(0,0,0,1)] transition-all flex items-center justify-center shrink-0"
            title="Удалить размер из избранного"
        >
          <svg
              xmlns="http://www.w3.org/2000/svg"
              class="h-5 w-5 text-red-500 fill-current"
              viewBox="0 0 24 24"
              stroke="currentColor"
              stroke-width="2.5"
          >
            <path
                stroke-linecap="round"
                stroke-linejoin="round"
                d="M4.318 6.318a4.5 4.5 0 000 6.364L12 20.364l7.682-7.682a4.5 4.5 0 00-6.364-6.364L12 7.636l-1.318-1.318a4.5 4.5 0 00-6.364 0z"
            />
          </svg>
        </button>
      </div>

      <div class="flex flex-wrap gap-2 mt-2">
        <span class="bg-emerald-100 border border-black text-xs font-black px-2 py-0.5 rounded uppercase">
          Цвет: {{ item.color }}
        </span>
        <span class="bg-blue-100 border border-black text-xs font-black px-2 py-0.5 rounded uppercase">
          Размер: {{ item.size }}
        </span>
      </div>
    </div>

    <div class="mt-4 flex items-center justify-between">
      <span class="font-mono text-sm text-gray-400">#{{ item.productVariantId }}</span>
      <span class="text-emerald-700 font-black uppercase text-xs underline">Купить этот размер →</span>
    </div>
  </div>
</template>