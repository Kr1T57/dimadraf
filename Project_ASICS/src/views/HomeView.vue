<script setup>
import { useRouter } from 'vue-router'
const router = useRouter()
</script>

<template>
  <div
    class="main-banner h-screen bg-cover bg-center bg-no-repeat flex items-center justify-start px-10 pl-20"
  >
    <div class="flex space-x-6 flex-col">
      <div class="max-w-40 flex flex-wrap font-bold text-5xl">
        <h1 class="text-blue-950 flex items-center">Производительность и стиль</h1>
      </div>
      <div class="max-w-120 flex flex-wrap text-xl mt-5">
        <span class="text-blue-950"
          >Высококачественное спортивное снаряжение, разработанное для спортсменов, стремящихся к
          совершенству.</span
        >
      </div>
      <div class="flex justify-start h-12 w-50 mt-5">
        <button
          class="bg-[rgb(0,30,98)] rounded-2xl p-2 hover:cursor-pointer text-white"
          @click="router.push({ name: 'Catalog' })"
        >
          Купить сейчас
        </button>
      </div>
    </div>
  </div>
  <div class="flex items-center space-x-6 justify-center flex-col gap-5 mt-5 text-black">
    <h2 class="">Рекомендуемые категории</h2>
    <div class="flex items-center justify-between gap-5">
      <div class="flex items-center flex-col">
        <img
          src="../assets/volleyball.png"
          alt="volleyball"
          class="h-100 w-100 bg-cover bg-center bg-no-repeat flex items-center justify-center"
        />
        <router-link to="/catalog">
          <div class="flex items-center flex-col text-10 font-bold">
            <h2>Волейбольные</h2>
            <h2>Кроссовки</h2>
          </div>
        </router-link>
      </div>
      <div class="flex items-center flex-col">
        <img
          src="../assets/trousers.png"
          alt="volleyball"
          class="h-100 w-100 bg-cover bg-center bg-no-repeat flex items-center justify-center"
        />
        <router-link to="/catalog">
          <div class="flex items-center flex-col text-10 font-bold">
            <h2>Спортивный</h2>
            <h2>Стиль</h2>
          </div>
        </router-link>
      </div>
      <div class="flex items-center flex-col">
        <img
          src="../assets/tennis.png"
          alt="volleyball"
          class="h-100 w-100 bg-cover bg-center bg-no-repeat flex items-center justify-center"
        />
        <router-link to="/catalog">
          <div class="flex items-center flex-col text-10 font-bold">
            <h2>Теннисная</h2>
            <h2>Форма</h2>
          </div>
        </router-link>
      </div>
    </div>
  </div>
</template>

<style scoped>
.main-banner {
  background-image: url('../assets/ad_banner.png');
}
</style>
