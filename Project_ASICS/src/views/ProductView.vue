<script setup>
import { useRoute, useRouter } from 'vue-router'
import useProductStore from '@/stores/product.js'

import {computed, onBeforeMount, ref, watch} from 'vue'
import useCartItemStore from '@/stores/cartItems.js'
import { useToast } from 'vue-toastification'
import useUserStore from '@/stores/user.js'
import shoesAlt from "@/assets/shoes_alt.png"
import useAnalyticsStore from "@/stores/analytics.js";

const route = useRoute()
const router = useRouter()
const userStore = useUserStore()
const productStore = useProductStore()
const cartItemStore = useCartItemStore()
const analyticsStore = useAnalyticsStore()
const toast = useToast()

const selectedColorName = ref(null)
const selectedSize = ref(null)

const imageSrc = ref('')

const handleImageError = () => {
  imageSrc.value = shoesAlt
}

const addToCart = async () => {
  if (!userStore.user) {
    toast.info('Для добавление в корзину нужно войти в аккаунт')
    await router.push('/authorization')
    return
  }
  if (!selectedVariant.value) {
    toast.error('Пожалуйста, выберите цвет и размер')
    return
  }

  if (!selectedColorName.value) {
    toast.error('Пожалуйста выберите цвет и размер')
    return
  }
  const cart = {
    id: 0,
    userId: userStore.user.id,
    productVariantId: selectedVariant.value.id,
    quantity: 1,
    productName: '',
    color: '',
    size: 0,
  }
  const isSuccess = await cartItemStore.addToCart(cart)

  if (isSuccess) {
    toast.success('Товар успешно добавлен в корзину')
  }
}

onBeforeMount(async () => {
  await productStore.getProduct(Number(route.params.id))

  const variants = productStore.currentProduct?.variants || []
  if (variants.length > 0) {
    const queryColor = route.query.color
    const querySize = route.query.size

    if (queryColor && querySize) {
      selectedColorName.value = String(queryColor)
      selectedSize.value = Number(querySize)
    } else {
      selectedColorName.value = variants[0].color
      selectedSize.value = variants[0].size
    }

    await updateVariantImage()
  }
  if (userStore.user) {
    await analyticsStore.fetchUserFavorites(userStore.user.id)
  }
})

const availableColors = computed(() => {
  const variants = productStore.currentProduct?.variants || []
  const uniqueColorNames = [...new Set(variants.map((v) => v.color))]

  return uniqueColorNames.map((name) => ({
    id: name,
    name: name,
  }))
})

const availableSizes = computed(() => {
  const variants = productStore.currentProduct?.variants || []

  const filteredByColor = variants.filter((v) => v.color === selectedColorName.value)

  const uniqueSizes = [...new Set(filteredByColor.map((v) => v.size))]

  return uniqueSizes.sort((a, b) => a - b)
})

const selectedVariant = computed(() => {
  const variants = productStore.currentProduct?.variants || []
  return variants.find(
    (v) => v.color === selectedColorName.value && String(v.size) === String(selectedSize.value),
  )
})

const currentColorName = computed(() => selectedColorName.value || '')

const isVariantLiked = computed(() => {
  if (!selectedVariant.value) return false
  return analyticsStore.userVariantFavorites.includes(selectedVariant.value.id)
})

const handleVariantLikeClick = async () => {
  if (!userStore.user) {
    toast.info('Для добавления в избранное войдите в аккаунт')
    return
  }
  if (!selectedVariant.value) {
    toast.error('Сначала выберите цвет и размер товара')
    return
  }
  await analyticsStore.toggleVariantFavorite(userStore.user.id, selectedVariant.value.id)
}

watch(selectedColorName, (newColor) => {
  if (newColor) {
    const variants = productStore.currentProduct?.variants || []
    const filteredByColor = variants.filter((v) => v.color === newColor)

    if (filteredByColor.length > 0) {
      const sortedSizes = filteredByColor.map((v) => v.size).sort((a, b) => a - b)
      selectedSize.value = sortedSizes[0]
    }
  }
})

const updateVariantImage = async () => {
  if (selectedVariant.value) {
    const variantImage = await productStore.getVariantImage(selectedVariant.value.id)

    if (variantImage && variantImage.imagePath) {
      imageSrc.value = `http://localhost:5000/imagesVariant/${variantImage.imagePath}`
    } else {
      imageSrc.value = shoesAlt
    }
  } else {
    imageSrc.value = shoesAlt
  }
}
watch(selectedVariant, async () => {
  await updateVariantImage()
})
</script>

<template>
  <div
    v-if="productStore.currentProduct"
    class="flex flex-row min-h-screen p-10 md:p-20 w-full bg-white gap-16"
  >
    <div class="flex flex-col gap-6">
      <div class="border-4 border-black p-2 shadow-[12px_12px_0px_0px_rgba(0,0,0,1)] bg-white">
        <img
            :src="imageSrc"
            alt="Main Product"
            class="h-[500px] w-[500px] object-contain mix-blend-multiply"
            @error="handleImageError"
        />
      </div>
    </div>

    <div class="flex flex-col flex-1 max-w-2xl gap-8">
      <div>
        <h4 class="uppercase font-black text-blue-800 tracking-tighter text-lg">
          Asics / Training
        </h4>
        <div class="flex items-center justify-between gap-4 mt-2">
          <h1 class="font-black text-6xl uppercase leading-none mt-2">
            {{ productStore.currentProduct.name }}
          </h1>
          <button
              @click="handleVariantLikeClick"
              class="p-3 border-4 border-black rounded-xl bg-white shadow-[4px_4px_0px_0px_rgba(0,0,0,1)] hover:translate-y-0.5 hover:shadow-[2px_2px_0px_0px_rgba(0,0,0,1)] transition-all flex items-center justify-center shrink-0"
              title="Добавить этот размер в избранное"
          >
            <svg
                xmlns="http://www.w3.org/2000/svg"
                :class="['h-8 w-8 transition-colors', isVariantLiked ? 'text-red-500 fill-current' : 'text-gray-400']"
                fill="none"
                viewBox="0 0 24 24"
                stroke="currentColor"
            >
              <path
                  stroke-linecap="round"
                  stroke-linejoin="round"
                  stroke-width="2.5"
                  d="M4.318 6.318a4.5 4.5 0 000 6.364L12 20.364l7.682-7.682a4.5 4.5 0 00-6.364-6.364L12 7.636l-1.318-1.318a4.5 4.5 0 00-6.364 0z"
              />
            </svg>
          </button>
        </div>
        <p class="text-4xl font-bold mt-4 text-[rgb(0,30,98)]">
          ₽{{ productStore.currentProduct.basePrice }}
        </p>
      </div>

      <hr class="border-t-4 border-black" />

      <div class="flex flex-col gap-2">
        <span class="font-black uppercase text-xl">Описание</span>
        <p class="text-lg font-medium leading-tight">
          {{
            productStore.currentProduct.description ||
            'Кроссовки обеспечивают отличную поддержку и амортизацию для интенсивных тренировок в зале и на улице.'
          }}
        </p>
      </div>

      <div class="flex flex-col gap-4">
        <span class="font-black uppercase text-sm italic"
          >Выбранный цвет: {{ currentColorName }}</span
        >
        <div class="flex gap-3">
          <button
            v-for="color in availableColors"
            :key="color.id"
            @click="selectedColorName = color.id"
            :class="[
              'px-6 py-2 border-2 border-black font-black uppercase transition-all',
              selectedColorName === color.id
                ? 'bg-black text-white translate-y-1 shadow-none'
                : 'bg-white shadow-[4px_4px_0px_0px_rgba(0,0,0,1)] hover:bg-gray-50',
            ]"
          >
            {{ color.name }}
          </button>
        </div>
      </div>

      <div class="flex flex-col gap-4">
        <span class="font-black uppercase text-sm italic">Размер: {{ selectedSize }}</span>
        <div class="flex gap-2">
          <button
            v-for="size in availableSizes"
            :key="size"
            @click="selectedSize = size"
            :class="[
              'w-14 h-14 border-2 border-black font-black transition-all flex items-center justify-center',
              String(selectedSize) === String(size)
                ? 'bg-[rgb(0,30,98)] text-white'
                : 'bg-white hover:border-blue-800',
            ]"
          >
            {{ size }}
          </button>
        </div>
      </div>

      <div class="mt-6">
        <button class="group relative w-full h-20 bg-black border-2 border-black">
          <div
            class="absolute inset-0 bg-[rgb(0,30,98)] translate-x-2 translate-y-2 group-hover:translate-x-0 group-hover:translate-y-0 transition-transform"
          ></div>
          <div
            class="absolute inset-0 bg-black flex items-center justify-center gap-4 text-white font-black text-2xl uppercase border-2 border-black"
            @click="addToCart"
          >
            <svg
              xmlns="http://www.w3.org/2000/svg"
              class="h-8 w-8"
              fill="none"
              viewBox="0 0 24 24"
              stroke="currentColor"
            >
              <path
                stroke-linecap="round"
                stroke-linejoin="round"
                stroke-width="3"
                d="M12 4v16m8-8H4"
              />
            </svg>
            Добавить в корзину
          </div>
        </button>
      </div>

      <div class="grid grid-cols-2 gap-4 mt-4 font-bold text-sm uppercase">
        <div class="flex items-center gap-2">✓ Оригинальный товар</div>
        <div class="flex items-center gap-2">✓ Быстрая доставка</div>
        <div class="flex items-center gap-2">✓ Гарантия качества</div>
        <div class="flex items-center gap-2">✓ Примерка перед покупкой</div>
      </div>
    </div>
  </div>

  <div v-else class="min-h-screen flex items-center justify-center bg-gray-100">
    <div class="text-center">
      <h2 class="text-4xl font-black uppercase animate-pulse">Загружаем данные...</h2>
      <p class="mt-2 font-bold text-gray-500 italic">Связываемся с сервером Asics</p>
    </div>
  </div>
</template>

<style scoped></style>
