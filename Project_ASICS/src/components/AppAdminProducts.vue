<script setup>
import useProductStore from "@/stores/product.js";
import { ref, onBeforeMount } from "vue";
import AppEditProductModal from "@/components/AppEditProductModal.vue";
import {useToast} from "vue-toastification";
import shoesAlt from "@/assets/shoes_alt.png";

const props = defineProps({
  product: { type: Object, required: true }
})
const isModalOpen = ref(false)
const productStore = useProductStore()
const toast = useToast()

const productImage = ref(null)
const imageSrc = ref('')
onBeforeMount(async () => {
  productImage.value = await productStore.getProductImage(props.product.id)
  if (productImage.value && productImage.value.imageUrl) {
    imageSrc.value = `http://localhost:5000/images/${productImage.value.imageUrl}`
  } else {
    imageSrc.value = shoesAlt
  }
})

const handleImageError = () => {
  imageSrc.value = shoesAlt
}

const confirmDelete = async () => {
  if (confirm(`Удалить товар "${props.product.name}"?`)) {
    await productStore.deleteProduct(props.product.id)
    toast.success("Товар удален")
    await productStore.productFun()
  }
}
</script>

<template>
  <div class="flex items-center justify-between py-4 border-b border-gray-200 hover:bg-gray-50 transition-colors px-6">

    <div class="flex items-center gap-4 w-2/5">
      <div class="w-16 h-16 bg-gray-100 rounded flex items-center justify-center overflow-hidden shrink-0 border border-gray-200">
        <img :src="imageSrc" alt="product" class="object-contain w-full h-full" @error="handleImageError" />
      </div>
      <span class="font-bold text-gray-900">{{ product.name }}</span>
    </div>

    <div class="w-1/4 text-gray-600 font-medium text-sm uppercase">
      {{ product.category }}
    </div>

    <div class="w-1/6 font-bold text-gray-900">
      ₽{{ product.basePrice }}
    </div>

    <div class="w-1/6 flex justify-end gap-4 text-gray-500">
      <button class="hover:text-black transition-colors hover:cursor-pointer" title="Редактировать" @click="isModalOpen = true">
        <span class="material-icons" >✏️</span>
      </button>
      <button class="hover:text-red-500 transition-colors hover:cursor-pointer" title="Удалить" @click="confirmDelete">
        <span class="material-icons">🗑️</span>
      </button>
    </div>
    <teleport to="body">
      <app-edit-product-modal
          :is-modal-open="isModalOpen"
          :product="product"
          @close-modal="isModalOpen = false"
      />
    </teleport>
  </div>
</template>