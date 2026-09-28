<script setup>
import useProductStore from '@/stores/product.js'
import { onBeforeMount, ref, computed } from 'vue'
import router from '@/router/index.js'
import AppProductCart from '@/components/AppProductCard.vue'

const productStore = useProductStore()

const selectedCategory = ref('Все')
const currentSort = ref('noSort')

onBeforeMount(async () => {
  await productStore.productFun()
})

const filteredAndSortedProducts = computed(() => {
  if (!productStore.products) return []

  let result = [...productStore.products]

  if (selectedCategory.value !== 'Все') {
    result = result.filter(product => product.category === selectedCategory.value)
  }

  if (currentSort.value === 'sortAsc') {
    result.sort((a, b) => a.basePrice - b.basePrice)
  } else if (currentSort.value === 'sortDesc') {
    result.sort((a, b) => b.basePrice - a.basePrice)
  }

  return result
})
</script>

<template>
  <div class="p-10 flex justify-start  min-h-screen gap-6">
    <div class="flex justify-start flex-col border border-black mt-8 h-fit bg-white w-56 p-5 shadow-sm">

      <div class="flex justify-start flex-col items-start text-xl">
        <h1 class="font-black text-2xl mb-3 text-[rgb(0,30,98)] uppercase tracking-tight">Категории</h1>

        <button
            @click="selectedCategory = 'Все'"
            class="mt-1 text-left w-full font-bold transition-colors cursor-pointer"
            :class="selectedCategory === 'Все' ? 'text-blue-600 underline' : 'text-gray-700 hover:text-blue-500'"
        >
          Все
        </button>
        <button
            @click="selectedCategory = 'Кроссовки'"
            class="mt-1 text-left w-full font-bold transition-colors cursor-pointer"
            :class="selectedCategory === 'Кроссовки' ? 'text-blue-600 underline' : 'text-gray-700 hover:text-blue-500'"
        >
          Кроссовки
        </button>
        <button
            @click="selectedCategory = 'Верх'"
            class="mt-1 text-left w-full font-bold transition-colors cursor-pointer"
            :class="selectedCategory === 'Верх' ? 'text-blue-600 underline' : 'text-gray-700 hover:text-blue-500'"
        >
          Верх
        </button>
        <button
            @click="selectedCategory = 'Носки'"
            class="mt-1 text-left w-full font-bold transition-colors cursor-pointer"
            :class="selectedCategory === 'Носки' ? 'text-blue-600 underline' : 'text-gray-700 hover:text-blue-500'"
        >
          Носки
        </button>
      </div>

      <div class="flex justify-start flex-col mt-8 border-t border-gray-300 pt-4">
        <h1 class="font-black text-2xl text-[rgb(0,30,98)] uppercase tracking-tight">Сортировка</h1>
        <select
            v-model="currentSort"
            name="selectSort"
            class="mt-3 w-full border border-black p-2 bg-gray-50 outline-none font-bold"
        >
          <option value="noSort">Без сортировки</option>
          <option value="sortAsc">По возрастанию</option>
          <option value="sortDesc">По убыванию</option>
        </select>
      </div>
    </div>

    <div class="flex items-start justify-start flex-wrap gap-4 mt-8 flex-1">
      <template v-if="filteredAndSortedProducts.length > 0">
        <app-product-cart
            v-for="product in filteredAndSortedProducts"
            :key="product.id"
            :product="product"
            @click="
            router.push({
              name: 'Product',
              params: {
                id: product.id,
              },
            })
          "
        ></app-product-cart>
      </template>

      <div v-else class="text-gray-500 italic text-xl mt-10 ml-6">
        Товары в категории "{{ selectedCategory }}" не найдены.
      </div>
    </div>
  </div>
</template>

<style scoped></style>