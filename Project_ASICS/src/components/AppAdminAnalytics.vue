<script setup>
import { onMounted, computed } from 'vue'
import useAnalyticsStore from '@/stores/analytics.js'
import { Bar } from 'vue-chartjs'
import {
  Chart as ChartJS,
  Title,
  Tooltip,
  Legend,
  BarElement,
  CategoryScale,
  LinearScale
} from 'chart.js'

ChartJS.register(Title, Tooltip, Legend, BarElement, CategoryScale, LinearScale)

const analyticsStore = useAnalyticsStore()

onMounted(async () => {
  await analyticsStore.fetchProductAnalytics()
  await analyticsStore.fetchVariantAnalytics()
})

const productChartData = computed(() => ({
  labels: analyticsStore.mostLikedProducts.map(p => p.productName),
  datasets: [
    {
      label: 'Количество лайков (Товары)',
      backgroundColor: '#001E62',
      data: analyticsStore.mostLikedProducts.map(p => p.likesCount)
    }
  ]
}))

// Автоматически разбиваем длинную строку на две части, чтобы Chart.js перенёс характеристики на новую строчку
const variantChartData = computed(() => ({
  labels: analyticsStore.mostLikedVariants.map(v => {
    const name = v.variantName || ''
    if (name.includes('(')) {
      const parts = name.split('(')
      return [parts[0].trim(), '(' + parts[1]] // Возвращаем массив: [Название, (Характеристики)]
    }
    return name
  }),
  datasets: [
    {
      label: 'Количество лайков (Вариации)',
      backgroundColor: '#10B981',
      data: analyticsStore.mostLikedVariants.map(v => v.likesCount)
    }
  ]
}))

const productChartOptions = {
  responsive: true,
  maintainAspectRatio: false,
  scales: {
    y: {
      beginAtZero: true,
      ticks: {
        stepSize: 1
      }
    }
  }
}

const variantChartOptions = {
  responsive: true,
  maintainAspectRatio: false,
  indexAxis: 'y',
  scales: {
    x: {
      beginAtZero: true,
      ticks: {
        stepSize: 1
      }
    },
    y: {
      ticks: {
        autoSkip: false,
        crossAlign: 'end',
        // Стилизуем шрифт подписей, чтобы они читались чётко
        font: {
          size: 11,
          weight: 'bold'
        }
      }
    }
  }
}
</script>

<template>
  <div class="p-6 bg-gray-50 min-h-screen">
    <h1 class="text-3xl font-black text-[rgb(0,30,98)] uppercase mb-8">Аналитика избранного</h1>

    <div class="grid grid-cols-1 xl:grid-cols-2 gap-8 mb-10 text-black">
      <div class="bg-white p-6 border-2 border-black rounded-2xl shadow-sm">
        <h2 class="text-xl font-bold text-[rgb(0,30,98)] mb-4">Самые популярные товары</h2>
        <div class="h-80 relative">
          <Bar
              v-if="analyticsStore.mostLikedProducts.length > 0"
              :data="productChartData"
              :options="productChartOptions"
          />
          <span v-else class="text-gray-400 italic flex items-center justify-center h-full">Нет данных для анализа</span>
        </div>
      </div>

      <div class="bg-white p-6 border-2 border-black rounded-2xl shadow-sm">
        <h2 class="text-xl font-bold text-[rgb(0,30,98)] mb-4">Самые популярные вариации</h2>
        <div class="h-[500px] relative w-full">
          <Bar
              v-if="analyticsStore.mostLikedVariants.length > 0"
              :data="variantChartData"
              :options="variantChartOptions"
          />
          <span v-else class="text-gray-400 italic flex items-center justify-center h-full">Нет данных для анализа</span>
        </div>
      </div>
    </div>

    <div class="grid grid-cols-1 xl:grid-cols-2 gap-8">

      <div class="bg-white border-2 border-black rounded-2xl p-6">
        <h3 class="font-black text-lg text-[rgb(0,30,98)] mb-4 uppercase">Таблица: Популярные товары</h3>
        <table class="w-full text-left border-collapse">
          <thead>
          <tr class="border-b-2 border-black text-xs uppercase text-gray-500">
            <th class="pb-2">Номер Товара</th>
            <th class="pb-2">Название</th>
            <th class="pb-2 text-right">Лайки</th>
          </tr>
          </thead>
          <tbody>
          <tr v-for="item in analyticsStore.mostLikedProducts" :key="item.productId" class="border-b border-gray-200 text-sm">
            <td class="py-3 font-mono">#{{ item.productId }}</td>
            <td class="py-3 font-semibold">{{ item.productName }}</td>
            <td class="py-3 text-right font-bold text-blue-600">{{ item.likesCount }} шт.</td>
          </tr>
          </tbody>
        </table>
      </div>

      <div class="bg-white border-2 border-black rounded-2xl p-6">
        <h3 class="font-black text-lg text-[rgb(0,30,98)] mb-4 uppercase">Таблица: Популярные вариации</h3>
        <table class="w-full text-left border-collapse">
          <thead>
          <tr class="border-b-2 border-black text-xs uppercase text-gray-500">
            <th class="pb-2">Номер Вариации</th>
            <th class="pb-2">Характеристики</th>
            <th class="pb-2 text-right">Лайки</th>
          </tr>
          </thead>
          <tbody>
          <tr v-for="item in analyticsStore.mostLikedVariants" :key="item.productVariantId" class="border-b border-gray-200 text-sm">
            <td class="py-3 font-mono">#{{ item.productVariantId }}</td>
            <td class="py-3 font-semibold">{{ item.variantName }}</td>
            <td class="py-3 text-right font-bold text-green-600">{{ item.likesCount }} шт.</td>
          </tr>
          </tbody>
        </table>
      </div>

    </div>
  </div>
</template>

<style scoped></style>