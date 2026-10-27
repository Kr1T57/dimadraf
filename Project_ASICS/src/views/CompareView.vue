<script setup>
import { ref, computed, watch, onMounted, onUnmounted } from 'vue'
import { useRouter } from 'vue-router'
import useProductStore from '@/stores/product.js'
import useUserStore from '@/stores/user.js'
import useAnalyticsStore from '@/stores/analytics.js'
import shoesAlt from '@/assets/shoes_alt.png'

const router = useRouter()
const productStore = useProductStore()
const userStore = useUserStore()
const analyticsStore = useAnalyticsStore()

const selectedCategory = ref('')
const startIndex = ref(0)
const visibleCount = ref(3)
const productImages = ref({})

const updateVisibleCount = () => {
  const width = window.innerWidth
  if (width < 640) visibleCount.value = 1
  else if (width < 1024) visibleCount.value = 2
  else if (width < 1440) visibleCount.value = 3
  else visibleCount.value = 4
}

// 1 & 3. Объединяем избранные товары и вариации без дубликатов
const comparedProducts = computed(() => {
  if (!userStore.user || !productStore.products?.length) return []

  const productFavIds = new Set(analyticsStore.userProductFavorites || [])
  const variantFavIds = new Set(analyticsStore.userVariantFavorites || [])

  return productStore.products.filter(p => {
    if (productFavIds.has(p.id)) return true
    if (p.variants?.some(v => variantFavIds.has(v.id))) return true
    return false
  })
})

// Категории с количеством товаров
const categoriesWithCount = computed(() => {
  const counts = {}
  for (const product of comparedProducts.value) {
    const cat = product.category || 'Без категории'
    counts[cat] = (counts[cat] || 0) + 1
  }
  return Object.entries(counts).map(([name, count]) => ({ name, count }))
})

// 6. По умолчанию выбираем первую доступную категорию
watch(categoriesWithCount, (cats) => {
  if (cats.length === 0) {
    selectedCategory.value = ''
    return
  }
  if (!selectedCategory.value || !cats.some(c => c.name === selectedCategory.value)) {
    selectedCategory.value = cats[0].name
  }
}, { immediate: true })

// 2. Товары только выбранной категории
const categoryProducts = computed(() => {
  if (!selectedCategory.value) return []
  return comparedProducts.value.filter(p => p.category === selectedCategory.value)
})

// DNS-карусель / сдвиг товаров
const maxStartIndex = computed(() => Math.max(0, categoryProducts.value.length - visibleCount.value))
const canPrev = computed(() => startIndex.value > 0)
const canNext = computed(() => startIndex.value < maxStartIndex.value)

const prev = () => {
  if (canPrev.value) startIndex.value--
}

const next = () => {
  if (canNext.value) startIndex.value++
}

const visibleProducts = computed(() => {
  return categoryProducts.value.slice(startIndex.value, startIndex.value + visibleCount.value)
})

// 7. Загрузка основных изображений
const loadImages = async (products) => {
  for (const p of products) {
    if (productImages.value[p.id]) continue
    const imgData = await productStore.getProductImage(p.id)
    productImages.value[p.id] = imgData?.imageUrl
        ? `http://localhost:5000/images/${imgData.imageUrl}`
        : shoesAlt
  }
}

watch(categoryProducts, (products) => {
  startIndex.value = 0
  loadImages(products)
}, { immediate: true })

// 1. Проверка наличия хотя бы одной вариации на складе
const isInStock = (product) => {
  return product.variants?.some(v => Number(v.stockQuantity) > 0) ?? false
}

// 8. Переход на страницу товара по клику на фото
const goToProduct = (productId) => {
  router.push({ name: 'Product', params: { id: productId } })
}

onMounted(async () => {
  updateVisibleCount()
  window.addEventListener('resize', updateVisibleCount)

  if (productStore.products.length === 0) {
    await productStore.productFun()
  }
  if (userStore.user) {
    await analyticsStore.fetchUserFavorites(userStore.user.id)
  }
})

onUnmounted(() => {
  window.removeEventListener('resize', updateVisibleCount)
})

watch(() => userStore.user, async (newUser) => {
  if (newUser) {
    await analyticsStore.fetchUserFavorites(newUser.id)
  }
})
</script>

<template>
  <div class="p-8 bg-gray-50 min-h-screen text-black">
    <div class="max-w-7xl mx-auto">
      <h1 class="text-4xl font-black text-[rgb(0,30,98)] uppercase mb-2 italic tracking-tighter">
        Сравнение товаров
      </h1>
      <p class="font-bold text-gray-500 uppercase text-xs mb-8">
        Сравнение характеристик моделей из вашего избранного
      </p>

      <!-- 5. Плашка при отсутствии товаров -->
      <div
          v-if="comparedProducts.length === 0"
          class="border-4 border-black border-dashed p-12 text-center rounded-2xl bg-white shadow-[6px_6px_0px_0px_rgba(0,0,0,1)] max-w-xl mx-auto my-16"
      >
        <h2 class="text-2xl font-black uppercase text-[rgb(0,30,98)] mb-2">
          Нет товаров для сравнения
        </h2>
        <p class="text-gray-500 font-bold text-xs uppercase mb-6">
          Добавьте товары или их размеры в избранное, чтобы сравнить характеристики
        </p>
        <router-link
            to="/catalog"
            class="inline-block bg-black text-white px-6 py-3 font-black uppercase text-xs rounded-xl hover:bg-gray-800 shadow-[4px_4px_0px_0px_rgba(0,0,0,1)] active:translate-y-0.5 transition-all"
        >
          Перейти в каталог
        </router-link>
      </div>

      <!-- Основной блок сравнения -->
      <div v-else class="flex flex-col lg:flex-row gap-8 items-start">
        <!-- Левая колонка: Категории -->
        <aside class="w-full lg:w-64 shrink-0 bg-white border-2 border-black p-5 rounded-2xl shadow-[6px_6px_0px_0px_rgba(0,0,0,1)]">
          <h2 class="font-black text-base uppercase tracking-tight text-[rgb(0,30,98)] mb-4 border-b-2 border-black pb-2">
            Категории
          </h2>
          <div class="flex flex-col gap-2">
            <button
                v-for="cat in categoriesWithCount"
                :key="cat.name"
                @click="selectedCategory = cat.name"
                :class="[
                'w-full text-left px-3 py-2.5 rounded-xl font-black text-xs uppercase flex justify-between items-center transition-all border-2 border-black cursor-pointer',
                selectedCategory === cat.name
                  ? 'bg-[rgb(0,30,98)] text-white shadow-[2px_2px_0px_0px_rgba(0,0,0,1)]'
                  : 'bg-gray-50 text-black hover:bg-gray-100 shadow-[2px_2px_0px_0px_rgba(0,0,0,0.5)]'
              ]"
            >
              <span>{{ cat.name }}</span>
              <span
                  :class="[
                  'px-2 py-0.5 rounded-full text-[10px] font-bold border border-black',
                  selectedCategory === cat.name ? 'bg-white text-black' : 'bg-gray-200 text-gray-800'
                ]"
              >
                {{ cat.count }}
              </span>
            </button>
          </div>
        </aside>

        <!-- Правая колонка: Таблица сравнения с DNS-стрелками -->
        <main class="flex-1 w-full bg-white border-2 border-black rounded-2xl p-6 shadow-[8px_8px_0px_0px_rgba(0,0,0,1)] relative overflow-hidden">
          <div class="flex justify-between items-center mb-4">
            <span class="text-xs font-bold uppercase text-gray-400">
              Показано: {{ startIndex + 1 }}-{{ Math.min(startIndex + visibleCount, categoryProducts.length) }} из {{ categoryProducts.length }}
            </span>
          </div>

          <div class="relative overflow-x-auto">
            <!-- Стрелки переключения (DNS style) -->
            <button
                v-if="canPrev"
                @click="prev"
                class="absolute left-38 sm:left-46 top-24 -translate-y-1/2 z-20 w-10 h-10 bg-white border-2 border-black rounded-full flex items-center justify-center font-black text-xl hover:bg-black hover:text-white shadow-[3px_3px_0px_0px_rgba(0,0,0,1)] active:translate-y-0 transition-all cursor-pointer"
                title="Предыдущие товары"
            >
              ‹
            </button>
            <button
                v-if="canNext"
                @click="next"
                class="absolute right-2 top-24 -translate-y-1/2 z-20 w-10 h-10 bg-white border-2 border-black rounded-full flex items-center justify-center font-black text-xl hover:bg-black hover:text-white shadow-[3px_3px_0px_0px_rgba(0,0,0,1)] active:translate-y-0 transition-all cursor-pointer"
                title="Следующие товары"
            >
              ›
            </button>

            <table class="w-full text-left border-collapse table-fixed">
              <thead>
              <tr class="border-b-2 border-black">
                <th class="w-36 sm:w-44 p-3 bg-gray-50 font-black text-xs uppercase text-gray-500 align-top">
                  Товар
                </th>
                <th
                    v-for="p in visibleProducts"
                    :key="p.id"
                    class="p-4 align-top text-center border-l-2 border-gray-200"
                >
                  <!-- Фото товара с переходом -->
                  <div
                      @click="goToProduct(p.id)"
                      class="h-44 w-full flex items-center justify-center p-2 bg-gray-50 border-2 border-black rounded-xl cursor-pointer hover:shadow-[4px_4px_0px_0px_rgba(0,0,0,1)] hover:-translate-y-0.5 transition-all group overflow-hidden"
                      title="Перейти к товару"
                  >
                    <img
                        :src="productImages[p.id] || shoesAlt"
                        :alt="p.name"
                        class="h-full object-contain mix-blend-multiply group-hover:scale-105 transition-transform"
                        @error="(e) => e.target.src = shoesAlt"
                    />
                  </div>
                </th>
              </tr>
              </thead>
              <tbody>
              <!-- Имя -->
              <tr class="border-b border-gray-200">
                <td class="p-3 bg-gray-50 font-black text-xs uppercase text-gray-500">Название</td>
                <td
                    v-for="p in visibleProducts"
                    :key="p.id"
                    class="p-4 border-l-2 border-gray-200 font-black text-sm uppercase text-[rgb(0,30,98)]"
                >
                  {{ p.name }}
                </td>
              </tr>

              <!-- Бренд -->
              <tr class="border-b border-gray-200">
                <td class="p-3 bg-gray-50 font-black text-xs uppercase text-gray-500">Бренд</td>
                <td
                    v-for="p in visibleProducts"
                    :key="p.id"
                    class="p-4 border-l-2 border-gray-200 font-bold text-sm"
                >
                  {{ p.brand }}
                </td>
              </tr>

              <!-- Цена -->
              <tr class="border-b border-gray-200">
                <td class="p-3 bg-gray-50 font-black text-xs uppercase text-gray-500">Цена</td>
                <td
                    v-for="p in visibleProducts"
                    :key="p.id"
                    class="p-4 border-l-2 border-gray-200 font-black text-base text-black"
                >
                  {{ p.basePrice }} ₽
                </td>
              </tr>

              <!-- Наличие -->
              <tr class="border-b border-gray-200">
                <td class="p-3 bg-gray-50 font-black text-xs uppercase text-gray-500">Наличие</td>
                <td
                    v-for="p in visibleProducts"
                    :key="p.id"
                    class="p-4 border-l-2 border-gray-200"
                >
                    <span
                        v-if="isInStock(p)"
                        class="inline-block px-2.5 py-1 rounded-md text-xs font-black uppercase bg-green-100 text-green-700 border border-green-500"
                    >
                      В наличии
                    </span>
                  <span
                      v-else
                      class="inline-block px-2.5 py-1 rounded-md text-xs font-black uppercase bg-red-100 text-red-600 border border-red-400"
                  >
                      Нет в наличии
                    </span>
                </td>
              </tr>

              <!-- Описание -->
              <tr>
                <td class="p-3 bg-gray-50 font-black text-xs uppercase text-gray-500 align-top">Описание</td>
                <td
                    v-for="p in visibleProducts"
                    :key="p.id"
                    class="p-4 border-l-2 border-gray-200 align-top text-xs text-gray-600 leading-relaxed font-medium"
                >
                  {{ p.description }}
                </td>
              </tr>
              </tbody>
            </table>
          </div>
        </main>
      </div>
    </div>
  </div>
</template>