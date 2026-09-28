<script setup>
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
  <div class="flex min-h-screen bg-gray-50">

    <aside class="w-64 bg-black text-white flex flex-col pt-8 px-6 ">
      <div class="mb-12">
        <h1 class="text-xl font-black tracking-widest">Панель администратора</h1>
      </div>

      <nav class="flex flex-col gap-6 font-bold text-sm tracking-wider">
        <router-link to="/" class="flex items-center gap-3 text-gray-400 hover:text-white transition-colors">
          <span>Главная</span>
        </router-link>

        <router-link :to="{ name: 'admin-products' }" active-class="text-white" class="text-gray-400 hover:text-white transition-colors">
          <span>Товары</span>
        </router-link>

        <router-link :to="{ name: 'admin-orders' }" active-class="text-white" class="text-gray-400 hover:text-white transition-colors">
          <span>Заказы</span>
        </router-link>

        <router-link :to="{ name: 'admin-users' }" active-class="text-white" class="text-gray-400 hover:text-white transition-colors">
          <span>Пользователи</span>
        </router-link>

        <router-link :to="{ name: 'admin-analytics' }" active-class="text-white" class="text-gray-400 hover:text-white transition-colors">
          <span>Аналитика</span>
        </router-link>
      </nav>
    </aside>

    <main class="flex-1 p-10 overflow-x-hidden">
      <router-view />
    </main>
  </div>
</template>

<style scoped>

</style>