<script setup>
import useCartItemStore from '@/stores/cartItems.js'
import useUserStore from '@/stores/user.js'
import useProductStore from "@/stores/product.js"
import { computed, ref, onBeforeMount } from 'vue'
import shoesAlt from "@/assets/shoes_alt.png"

const props = defineProps({
  cartItem: {
    type: Object,
    required: true,
  },
})
const cartItemStore = useCartItemStore()
const userStore = useUserStore()
const productStore = useProductStore()

const imageSrc = ref('')

const handleImageError = () => {
  imageSrc.value = shoesAlt
}
onBeforeMount(async () => {
  const variantImage = await productStore.getVariantImage(props.cartItem.productVariantId)

  if (variantImage && variantImage.imagePath) {
    imageSrc.value = `http://localhost:5000/imagesVariant/${variantImage.imagePath}`
  } else {
    imageSrc.value = shoesAlt
  }
})

const prepareCartData = () => {
  return {
    userId: Number(props.cartItem.userId),
    productVariantId: Number(props.cartItem.productVariantId),
    quantity: 1,
    productName: props.cartItem.productName || '',
    color: props.cartItem.color || '',
    size: parseFloat(props.cartItem.size) || 0,
  }
}
const addToCart = async () => {
  const cart = prepareCartData()

  if (!isNaN(cart.userId)) {
    await cartItemStore.addToCart(cart)
    await cartItemStore.getCartItems(cart.userId)
  }
}

const minusToCart = async () => {
  const cart = prepareCartData()

  if (!isNaN(cart.userId)) {
    await cartItemStore.minusToCart(cart)
    await cartItemStore.getCartItems(cart.userId)
  }
}
const deleteCartItem = async () => {
  await cartItemStore.removeCart(props.cartItem.id)
  await cartItemStore.getCartItems(props.cartItem.userId)
}
const totalPrice = computed(() => {
  const price = Number(props.cartItem.price !== undefined ? props.cartItem.price : props.cartItem.Price) || 0
  const quantity = Number(props.cartItem.quantity !== undefined ? props.cartItem.quantity : props.cartItem.Quantity) || 0

  return Math.round(price * quantity * 100) / 100
})
defineExpose({
  totalPrice,
})
</script>

<template>
  <div
    class="flex items-center justify-between border border-gray-200 rounded-xl p-4 bg-white shadow-sm mb-4 w-full"
  >
    <div class="flex items-center gap-6">
      <div
        class="w-32 h-32 bg-gray-100 rounded-lg flex items-center justify-center overflow-hidden">
        <img :src="imageSrc" alt="shoes" class="object-contain w-full h-full mix-blend-multiply" @error="handleImageError" />
      </div>

      <div class="flex flex-col gap-1">
        <h2 class="text-xl font-bold uppercase tracking-tight">{{ cartItem.productName }}</h2>
        <p class="text-gray-500 text-sm">
          Цвет: <span class="text-black font-medium">{{ cartItem.color }}</span>
        </p>
        <p class="text-gray-500 text-sm">
          Размер: <span class="text-black font-medium">{{ cartItem.size }}</span>
        </p>
      </div>
    </div>

    <div class="flex flex-col items-end justify-between h-32">
      <button
        class="text-gray-400 hover:text-red-500 transition-colors text-xl"
        @click="deleteCartItem"
      >
        <span class="material-icons">🗑️</span>
      </button>

      <div class="flex items-center gap-6 flex-col">
        <div class="flex items-center border border-black rounded-md px-2 py-1 gap-4">
          <button
            class="text-xl font-medium hover:text-gray-500"
            v-if="userStore.user"
            @click="minusToCart"
          >
            -
          </button>
          <span class="text-lg font-semibold min-w-5 text-center">{{ cartItem.quantity }}</span>
          <button
            class="text-xl font-medium hover:text-gray-500"
            v-if="userStore.user"
            @click="addToCart"
          >
            +
          </button>
        </div>
        <p class="text-xl font-black">{{ totalPrice }} ₽</p>
      </div>
    </div>
  </div>
</template>

<style scoped></style>
