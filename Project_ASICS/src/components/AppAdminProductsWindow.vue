<script setup>

import AppAdminProduct from "@/components/AppAdminProducts.vue";
import AppAddProduct from "@/components/AppAddProductModal.vue";
import AppAddVariant from "@/components/AppAddVariantModal.vue";
import AppEditVariant from "@/components/AppEditVariantModal.vue";
import useProductStore from "@/stores/product.js";
import {onBeforeMount, ref} from "vue";

const productStore = useProductStore()
const isEditVariantOpen = ref(false)
const isAddVariantOpen = ref(false)
const isAddProductOpen = ref(false)
onBeforeMount(async () => {
  await productStore.productFun()
})
</script>

<template>
  <div class="flex justify-between items-center mb-10">
    <h2 class="text-3xl font-black uppercase ">Товары</h2>

    <div class="flex gap-3">
      <div class="flex border-2 border-black rounded-xl overflow-hidden ">
        <button @click="isEditVariantOpen = true" class="bg-white text-black px-4 py-3 font-bold text-[10px] uppercase hover:bg-gray-100 border-r-2 border-black transition-colors">
          Управление остатками
        </button>
        <button @click="isAddVariantOpen = true" class="bg-white text-black px-4 py-3 font-bold text-[10px] uppercase hover:bg-gray-100 transition-colors">
          Добавить вариацию
        </button>
      </div>

      <button class="bg-black text-white px-6 py-3 rounded-xl font-black text-xs uppercase hover:bg-gray-800 transition-all"
              @click="isAddProductOpen = true">
        + Добавить товар
      </button>
    </div>
  </div>

  <div  class="bg-white border border-gray-200 rounded-lg shadow-sm">

    <div  class="flex items-center justify-between py-4 px-6 border-b-2 border-gray-100 font-bold text-xs text-gray-400 uppercase tracking-wider">
      <div class="w-2/5">Фотография & Название</div>
      <div class="w-1/4">Категория</div>
      <div class="w-1/6">Цена</div>
      <div class="w-1/6 text-right">Действия</div>
    </div>

    <div class="flex flex-col">
      <app-admin-product
          v-for="product in productStore.products"
          :key="product.id"
          :product="product"
      />
    </div>
  </div>
  <teleport to="body">
    <app-edit-variant v-if="isEditVariantOpen" :is-open="isEditVariantOpen" @close="isEditVariantOpen = false" />
    <app-add-variant v-if="isAddVariantOpen" :is-open="isAddVariantOpen" @close="isAddVariantOpen = false" />
    <app-add-product v-if="isAddProductOpen" :is-open="isAddProductOpen" @close="isAddProductOpen = false"/>
  </teleport>
</template>

<style scoped>

</style>