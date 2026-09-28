<script setup>
import AppCartItem from '@/components/AppCartItem.vue'
import { computed, onBeforeMount, ref, useTemplateRef } from 'vue'
import useCartItemStore from '@/stores/cartItems.js'
import useUserStore from '@/stores/user.js'
import { useRoute, useRouter } from 'vue-router'
import useProductStore from '@/stores/product.js'
import useOrderStore from '@/stores/order.js'
import { useToast } from 'vue-toastification'
import AppCartModal from "@/components/AppCartModal.vue";

const userStore = useUserStore()
const router = useRouter()
const route = useRoute()
const cartItemStore = useCartItemStore()
const productStore = useProductStore()
const orderStore = useOrderStore()
const toast = useToast()

const isModalOpen = ref(false)


onBeforeMount(async () => {
  if (userStore.user && userStore.user.id) {
    await cartItemStore.getCartItems(userStore.user.id)
  } else {
    console.error('Юзер не авторизован')
  }
})

const cartItemRef = useTemplateRef('cartItemRef')
const totalPrice = computed(() => Math.round(cartItemStore.cartTotalPrice * 100) / 100)
</script>

<template>
  <div class="container mx-auto px-6 py-10">
    <h1 class="text-3xl font-black uppercase mb-8 italic">Корзина</h1>

    <div class="flex flex-col lg:flex-row gap-10">
      <div class="grow lg:w-2/3">
        <div v-if="cartItemStore.currentCartItem.length > 0">
          <app-cart-item
            v-for="item in cartItemStore.currentCartItem"
            :key="item.id"
            :cart-item="item" />
<!--            ref="cartItemRef"-->

        </div>
        <div v-else class="text-center py-20 border-2 border-dashed border-gray-200 rounded-xl">
          <p class="text-gray-400 text-xl">Ваша корзина пока пуста</p>
        </div>
      </div>

      <div class="lg:w-1/3" v-if="cartItemStore.currentCartItem.length > 0">
        <div class="border border-black p-6 rounded-xl bg-white sticky top-10">
          <h2 class="text-2xl font-bold uppercase mb-6">Заказ</h2>

          <div class="flex flex-col gap-4 border-b border-gray-200 pb-6 mb-6 text-lg">
            <div class="flex justify-between">
              <span class="text-gray-600">Примерка: </span>
              <span class="font-bold text-green-600">бесплатно</span>
            </div>
            <div class="flex justify-between">
              <span class="text-gray-600">Доставка: </span>
              <span class="font-bold text-green-600">бесплатно</span>
            </div>
          </div>

          <div class="flex justify-between items-center mb-8">
            <span class="text-xl font-bold uppercase">Итог</span>
            <span class="text-2xl font-black italic">{{ totalPrice }} ₽</span>
          </div>

          <button
            class="w-full bg-black text-white py-4 rounded-lg font-bold uppercase hover:bg-gray-800 transition-all mb-4"
            @click="isModalOpen = true"
          >
            Оформить заказ
          </button>

          <router-link
            to="/catalog"
            class="block text-center text-sm font-bold uppercase underline hover:no-underline"
          >
            Продолжить покупки
          </router-link>
        </div>
      </div>
    </div>
    <Teleport to="body">
      <app-cart-modal :totalPrice="totalPrice" :is-modal-open="isModalOpen" @close-modal="isModalOpen = false"/>
    </Teleport>
  </div>
</template>

<style scoped></style>
