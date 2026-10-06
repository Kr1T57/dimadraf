<script setup>
import { ref, nextTick } from 'vue'
import { useRouter } from 'vue-router'
import { useAiStore } from '@/stores/ai.js'

const router = useRouter()
const aiStore = useAiStore()

const isOpen = ref(false)
const userMessage = ref('')
const messagesContainer = ref(null)

const messages = ref([
  {
    role: 'assistant',
    text: 'Привет! Я твой персональный ИИ-ассистент от ASICS. Напишите свой вес, дистанцию, покрытие или особенности стопы. Если есть особые пожелания к одежде или кроссовкам — например, нужен влагозащитный верх или вентилируемые ткани — обязательно укажите их!',
    recommendations: []
  }
])

const scrollToBottom = async () => {
  await nextTick()
  if (messagesContainer.value) {
    messagesContainer.value.scrollTop = messagesContainer.value.scrollHeight
  }
}

const sendMessage = async () => {
  const text = userMessage.value.trim()
  // Блокируем пустые запросы или спам во время загрузки
  if (!text || aiStore.isCoachLoading) return

  messages.value.push({ role: 'user', text })
  userMessage.value = ''
  scrollToBottom()

  const data = await aiStore.askCoach(text)
  if (data) {
    messages.value.push({
      role: 'assistant',
      text: data.reply,
      recommendations: data.recommendations || []
    })
  }
  scrollToBottom()
}

const openProduct = (productId) => {
  isOpen.value = false
  router.push({ name: 'Product', params: { id: productId } })
}
</script>

<template>
  <div class="fixed bottom-6 right-6 z-[9999] flex flex-col items-end">
    <!-- Окно чата -->
    <div
      v-if="isOpen"
      class="bg-white border-2 border-black w-88 sm:w-[420px] h-[520px] rounded-2xl shadow-[10px_10px_0px_0px_rgba(0,0,0,1)] flex flex-col mb-4 overflow-hidden"
    >
      <!-- Шапка -->
      <div class="bg-[rgb(0,30,98)] text-white p-4 flex justify-between items-center border-b-2 border-black">
        <div class="flex items-center gap-2">
          <span class="text-2xl">🏃‍♂️</span>
          <div>
            <h3 class="font-black uppercase tracking-tight text-sm">ИИ-ассистент</h3>
            <p class="text-[10px] text-gray-300 uppercase">Подбор экипировки ASICS</p>
          </div>
        </div>
        <button
          @click="isOpen = false"
          class="text-white hover:text-yellow-300 font-black text-xl px-2 transition-colors cursor-pointer"
        >
          ✕
        </button>
      </div>

      <!-- Лента сообщений -->
      <div ref="messagesContainer" class="flex-1 p-4 overflow-y-auto bg-gray-50 flex flex-col gap-3">
        <div
          v-for="(msg, idx) in messages"
          :key="idx"
          :class="['flex flex-col max-w-[85%]', msg.role === 'user' ? 'self-end items-end' : 'self-start items-start']"
        >
          <div
            :class="[
              'p-3 rounded-xl border-2 border-black text-xs font-medium leading-relaxed',
              msg.role === 'user'
                ? 'bg-yellow-300 text-black shadow-[3px_3px_0px_0px_rgba(0,0,0,1)]'
                : 'bg-white text-black shadow-[3px_3px_0px_0px_rgba(0,0,0,1)]'
            ]"
          >
            {{ msg.text }}
          </div>

          <!-- Рекомендованные карточки товаров -->
          <div v-if="msg.recommendations && msg.recommendations.length > 0" class="mt-2 flex flex-col gap-2 w-full">
            <div
              v-for="prod in msg.recommendations"
              :key="prod.productId"
              @click="openProduct(prod.productId)"
              class="bg-white border-2 border-black p-2.5 rounded-lg shadow-[3px_3px_0px_0px_rgba(0,30,98,1)] hover:bg-blue-50 cursor-pointer transition-all flex justify-between items-center"
            >
              <div>
                <p class="font-black text-xs uppercase text-[rgb(0,30,98)]">{{ prod.productName }}</p>
                <p class="text-[10px] text-gray-500 font-bold">₽{{ prod.price }}</p>
              </div>
              <span class="text-xs font-black text-blue-600 uppercase underline">Смотреть →</span>
            </div>
          </div>
        </div>

        <div v-if="aiStore.isCoachLoading" class="self-start bg-white border-2 border-black p-2.5 rounded-xl shadow-[3px_3px_0px_0px_rgba(0,0,0,1)]">
          <span class="text-xs font-bold text-gray-500 animate-pulse">Тренер анализирует каталог...</span>
        </div>
      </div>

      <!-- Ввод сообщения с защитой от спама -->
      <div class="p-3 bg-white border-t-2 border-black flex gap-2">
        <input
          v-model="userMessage"
          @keyup.enter="sendMessage"
          :disabled="aiStore.isCoachLoading"
          maxlength="1000"
          placeholder="Вес 85 кг, бегаю 10 км по асфальту..."
          class="flex-1 h-11 border-2 border-black px-3 text-xs font-bold outline-none focus:bg-yellow-50 disabled:bg-gray-100"
        />
        <button
          @click="sendMessage"
          :disabled="aiStore.isCoachLoading || !userMessage.trim()"
          class="bg-black text-white px-4 h-11 border-2 border-black font-black uppercase text-xs hover:bg-gray-800 disabled:bg-gray-300 disabled:cursor-not-allowed active:translate-y-0.5 cursor-pointer"
        >
          ➤
        </button>
      </div>
    </div>

    <!-- Кнопка вызова виджета -->
    <button
      @click="isOpen = !isOpen"
      class="bg-black text-white hover:bg-[rgb(0,30,98)] border-2 border-black p-3.5 rounded-2xl shadow-[5px_5px_0px_0px_rgba(0,0,0,1)] hover:translate-x-0.5 hover:translate-y-0.5 transition-all flex items-center gap-2 font-black uppercase text-xs tracking-wider cursor-pointer"
    >
      <span class="text-lg">🤖</span>
      <span class="hidden sm:inline">ИИ-ассистент от ASICS</span>
    </button>
  </div>
</template>