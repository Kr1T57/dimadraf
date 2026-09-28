<script setup>
import { onMounted, computed } from 'vue'
import useUserStore from "@/stores/user.js"

const userStore = useUserStore()

onMounted(async () => {
  await userStore.getAllUsers()
})

const stats = computed(() => {
  const users = userStore.allUsers || []
  return {
    total: users.length,
    admins: users.filter(u => u.role === 'Менеджер').length,
    clients: users.filter(u => u.role === 'Клиент').length
  }
})
</script>

<template>
  <div class="flex flex-col gap-8">
    <div class="flex flex-col gap-6">
      <h2 class="text-3xl font-black uppercase italic">Управление пользователями</h2>

      <div class="grid grid-cols-3 gap-4">
        <div v-for="(val, label) in { 'Всего пользователей': stats.total, 'Менеджеры': stats.admins, 'Клиенты': stats.clients }"
             :key="label"
             class="bg-white border-2 border-black p-5 rounded-xl ">
          <p class="text-[10px] font-black uppercase text-gray-400 mb-1">{{ label }}</p>
          <p class="text-2xl font-black">{{ val }}</p>
        </div>
      </div>
    </div>

    <div class="bg-white border-2 border-black rounded-2xl overflow-hidden ">
      <div class="flex items-center justify-between py-4 px-6 border-b-2 border-black bg-gray-50 font-black text-[10px] text-gray-500 uppercase tracking-widest">
        <div class="w-1/12 text-center">ID</div>
        <div class="w-1/4">ФИО Пользователя</div>
        <div class="w-1/4 text-center">Email</div>
        <div class="w-1/6 text-center">Телефон</div>
        <div class="w-1/6 text-right">Роль</div>
      </div>

      <div class="flex flex-col">
        <app-admin-user
            v-for="user in userStore.allUsers"
            :key="user.id"
            :user="user"
        />
      </div>
    </div>
  </div>
</template>