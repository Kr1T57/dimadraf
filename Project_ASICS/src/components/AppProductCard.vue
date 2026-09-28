<script setup>
import {computed, onBeforeMount, ref} from "vue";
import useProductStore from "@/stores/product.js";
import shoesAlt from "@/assets/shoes_alt.png";
import useAnalyticsStore from "@/stores/analytics.js";
import useUserStore from "@/stores/user.js";

const props = defineProps({
  product: {
    type: Object,
    required: true,
  },
})

const productStore = useProductStore();
const userStore = useUserStore();
const analyticsStore = useAnalyticsStore();

const productImage = ref(null)

const imageSrc = ref('')

onBeforeMount(async () => {
  productImage.value = await productStore.getProductImage(props.product.id)
  if (productImage.value && productImage.value.imageUrl) {
    imageSrc.value = `http://localhost:5000/images/${productImage.value.imageUrl}`
  } else {
    imageSrc.value = shoesAlt
  }
  if (userStore.user && analyticsStore.userProductFavorites.length === 0) {
    await analyticsStore.fetchUserFavorites(userStore.user.id)
  }
})

const handleImageError = () => {
  imageSrc.value = shoesAlt
}

const isLiked = computed(() => {
  return analyticsStore.userProductFavorites.includes(props.product.id)
})
const handleLikeClick = async () => {
  if (!userStore.user) {
    alert("Для добавления в избранное необходимо авторизоваться!")
    return
  }
  await analyticsStore.toggleProductFavorite(userStore.user.id, props.product.id)
}
</script>

<template>
  <div
    class="flex justify-start flex-col border border-black rounded-xl overflow-hidden bg-white p-3 m-2 relative"
  >
    <img :src="imageSrc" alt="logo" class="w-50 h-50 object-contain p-2" @error="handleImageError" />
    <h2>{{ product.name }}</h2>
    <span>{{ product.category }}</span>
    <p>{{ product.basePrice }} ₽</p>
    <button
        @click.stop="handleLikeClick"
        class="absolute bottom-3 right-3 p-2 rounded-full border border-gray-200 hover:bg-gray-100 transition-colors bg-white shadow-sm"
    >
      <svg
          xmlns="http://www.w3.org/2000/svg"
          :class="['h-6 w-6 transition-colors', isLiked ? 'text-red-500 fill-current' : 'text-gray-400']"
          fill="none"
          viewBox="0 0 24 24"
          stroke="currentColor"
      >
        <path
            stroke-linecap="round"
            stroke-linejoin="round"
            stroke-width="2"
            d="M4.318 6.318a4.5 4.5 0 000 6.364L12 20.364l7.682-7.682a4.5 4.5 0 00-6.364-6.364L12 7.636l-1.318-1.318a4.5 4.5 0 00-6.364 0z"
        />
      </svg>
    </button>
  </div>
</template>

<style scoped></style>
