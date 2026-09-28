<script setup>
defineProps({
  user: { type: Object, required: true }
})
</script>

<template>
  <div class="flex items-center justify-between py-5 px-6 border-b border-gray-100 hover:bg-gray-50 transition-colors">
    <div class="w-1/12 text-center font-black text-gray-300">
      #{{ user.id }}
    </div>

    <div class="w-1/4 flex items-center gap-3">
      <div class="w-10 h-10 bg-black rounded-full flex items-center justify-center text-white font-black text-xs shrink-0">
        {{ user.name[0] }}{{ user.surname[0] }}
      </div>
      <div class="flex flex-col">
        <p class="font-black text-sm leading-tight">{{ user.surname }} {{ user.name }}</p>
        <p class="text-[10px] text-gray-400 font-bold uppercase">{{ user.patronymic || '—' }}</p>
      </div>
    </div>

    <div class="w-1/4 text-center">
      <p class="font-bold text-xs truncate text-blue-600">{{ user.email }}</p>
    </div>

    <div class="w-1/6 text-center">
      <p class="font-black text-xs">{{ user.phone }}</p>
    </div>

    <div class="w-1/6 text-right">
      <span :class="[
        'px-3 py-1 rounded-full text-[9px] font-black uppercase border-2 border-black shadow-[2px_2px_0px_0px_rgba(0,0,0,1)]',
        user.role === 'Менеджер' ? 'bg-purple-400' : 'bg-green-300'
      ]">
        {{ user.role }}
      </span>
    </div>
  </div>
</template>