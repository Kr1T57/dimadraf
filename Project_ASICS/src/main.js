import { createApp } from 'vue'
import { createPinia } from 'pinia'

import router from './router/index.js'
import App from './App.vue'
import './assets/base.css'
import Toast from 'vue-toastification'
import 'vue-toastification/dist/index.css'
import AppHeaders from '@/components/AppHeaders.vue'
import AppFooters from '@/components/AppFooters.vue'
import AppProductCard from '@/components/AppProductCard.vue'
import AppCartItem from '@/components/AppCartItem.vue'
import AppCartModal from "@/components/AppCartModal.vue";
import AppAdminProducts from "@/components/AppAdminProducts.vue";
import AppEditProductModal from "@/components/AppEditProductModal.vue";
import AppEditVariantModal from "@/components/AppEditVariantModal.vue";
import AppAddVariantModal from "@/components/AppAddVariantModal.vue";
import AppAddProductModal from "@/components/AppAddProductModal.vue";
import AppOrderCard from "@/components/AppOrderCard.vue";
import AppEditStatusOrderModal from "@/components/AppEditStatusOrderModal.vue";
import AppUser from "@/components/AppUser.vue";

const toastOptions = {
  transition: 'Vue-Toastification__fade',
  maxToasts: 3,
  newestOnTop: true,
  filterBeforeCreate: (toast, toasts) => {
    if (toasts.filter((t) => t.type === toast.type).length !== 0) {
      return false
    }
    return toast
  },
}

const app = createApp(App)
app.use(Toast, toastOptions)
app.component('app-header', AppHeaders)
app.component('app-footer', AppFooters)
app.component('app-product-cart', AppProductCard)
app.component('app-cart-item', AppCartItem)
app.component('app-cart-modal',AppCartModal)
app.component('app-admin-product',AppAdminProducts)
app.component('app-edit-product',AppEditProductModal)
app.component('app-edit-variant',AppEditVariantModal)
app.component('app-add-variant',AppAddVariantModal)
app.component('app-add-product',AppAddProductModal)
app.component('app-order-card',AppOrderCard)
app.component('app-order-edit-status',AppEditStatusOrderModal)
app.component('app-admin-user',AppUser)
app.use(createPinia())
app.use(router)

app.mount('#app')
