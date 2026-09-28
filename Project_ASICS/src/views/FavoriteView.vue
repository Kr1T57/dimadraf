<script setup>
import {onMounted, watch} from 'vue'
import useAnalyticsStore from '@/stores/analytics.js'
import useUserStore from '@/stores/user.js'
import AppFavoritesProduct from '@/components/AppFavoritesProduct.vue'
import AppFavoriteVariant from '@/components/AppFavoritesVariant.vue'

const analyticsStore = useAnalyticsStore()
const userStore = useUserStore()

const loadData = async (userId) => {
  await analyticsStore.fetchUserFavorites(userId)
}

onMounted(async () => {
  if (userStore.user) {
    await loadData(userStore.user.id)
  }
watch(
    () => userStore.user,
    async (newUser) => {
      if (newUser) {
        await loadData(newUser.id)
      }
    }
)
})
</script>

<template>
  <div class="p-8 bg-gray-50 min-h-screen text-black">
    <div class="max-w-7xl mx-auto">
      <h1 class="text-5xl font-black text-[rgb(0,30,98)] uppercase mb-2 italic tracking-tighter">
        Мое Избранное
      </h1>
      <p class="font-bold text-gray-500 uppercase text-xs mb-8">
        Здесь хранятся товары и размеры, которые вы отложили
      </p>

      <div class="mb-12">
        <h2 class="text-2xl font-black uppercase mb-4 flex items-center gap-2">
          <span>Отложенные модели</span>
        </h2>

        <div
            v-if="analyticsStore.userFullProducts && analyticsStore.userFullProducts.length > 0"
            class="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-6"
        >
          <AppFavoritesProduct
              v-for="item in analyticsStore.userFullProducts"
              :key="item.productId"
              :item="item"
          />
        </div>
        <div v-else class="border-4 border-black border-dashed p-8 text-center rounded-xl bg-white shadow-[4px_4px_0px_0px_rgba(0,0,0,1)]">
          <p class="text-gray-400 italic font-black uppercase text-sm">Вы пока не добавляли модели целиком</p>
        </div>
      </div>

      <div>
        <h2 class="text-2xl font-black uppercase mb-4 flex items-center gap-2">
          <span>Конкретные размеры и цвета</span>
        </h2>

        <div
            v-if="analyticsStore.userFullVariants && analyticsStore.userFullVariants.length > 0"
            class="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-6"
        >
          <AppFavoriteVariant
              v-for="item in analyticsStore.userFullVariants"
              :key="item.productVariantId"
              :item="item"
          />
        </div>
        <div v-else class="border-4 border-black border-dashed p-8 text-center rounded-xl bg-white shadow-[4px_4px_0px_0px_rgba(0,0,0,1)]">
          <p class="text-gray-400 italic font-black uppercase text-sm">Вы пока не откладывали конкретные размеры</p>
        </div>
      </div>

    </div>
  </div>
</template>