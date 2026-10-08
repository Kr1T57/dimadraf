import { defineStore } from 'pinia'
import { ref } from 'vue'
import axios from 'axios'
import { useToast } from 'vue-toastification'
import useProductStore from '@/stores/product.js'

export const useAiStore = defineStore('ai', () => {
  const toast = useToast()
  const isGenerating = ref(false)
  const isCoachLoading = ref(false)
  const productStore = useProductStore()

  const API_KEY = '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'
  const MODEL = 'nvidia/nemotron-3-super-120b-a12b:free'

  let activeCoachAbort = null

  // Очистка от markdown, звездочек и незаконченных обрывков слов
  const cleanAndFinishText = (text) => {
    if (!text) return ''
    let cleaned = text
      .replace(/\*\*(.*?)\*\*/g, '$1')
      .replace(/\*(.*?)\*/g, '$1')
      .replace(/#{1,6}\s?/g, '')
      .replace(/\(?\bID:?\s*\d+\)?/gi, '')
      .replace(/\s{2,}/g, ' ')
      .trim()

    // Если ответ оборвался посреди предложения (нет точки/вопроса/восклицания в конце),
    // отрезаем незаконченный огрызок слова до последней точки:
    const lastPunctuation = Math.max(cleaned.lastIndexOf('.'), cleaned.lastIndexOf('!'), cleaned.lastIndexOf('?'))
    if (lastPunctuation !== -1 && lastPunctuation < cleaned.length - 1) {
      cleaned = cleaned.slice(0, lastPunctuation + 1)
    }

    return cleaned
  }

  // 1. AI Running Coach (Консультант)
  const askCoach = async (userMessage) => {
    if (activeCoachAbort) activeCoachAbort.abort()
    activeCoachAbort = new AbortController()

    isCoachLoading.value = true

    try {
      if (!productStore.products || productStore.products.length === 0) {
        await productStore.productFun()
      }

      const products = productStore.products || []
      const catalogNames = products.map(p => p.name).join(', ')

      const combinedPrompt = `ИНСТРУКЦИЯ:
Ты — спортивный консультант официального магазина ASICS ('AI Running Coach').
Твоя задача — дать емкий, профессиональный и доброжелательный ответ на русском языке (2-3 полных предложения).

ПРАВИЛА:
1. Завершай свои мысли полными предложениями. ОБЯЗАТЕЛЬНО заканчивай последнее предложение точкой.
2. Не используй markdown-разметку, звездочки (**) и решетки (#).
3. В магазине продаются ТОЛЬКО кроссовки и спортивная одежда ASICS: ${catalogNames}.
4. Если клиент спрашивает про инвентарь или активность, не связанные со спортом (шест, стриптиз, алкоголь, диван) — вежливо и с юмором поясни, что в нашем каталоге такого инвентаря нет, так как ASICS производит экипировку для бега, тренировок и активного образа жизни.

ВОПРОС КЛИЕНТА:
${userMessage.trim().slice(0, 1000)}`

      const response = await axios.post(
        'https://openrouter.ai/api/v1/chat/completions',
        {
          model: MODEL,
          messages: [
            { role: 'user', content: combinedPrompt }
          ],
          temperature: 0.3,
          max_tokens: 800 // Увеличенный лимит: предложения гарантированно дописываются до точки
        },
        {
          headers: {
            'Authorization': `Bearer ${API_KEY}`,
            'Content-Type': 'application/json'
          },
          timeout: 30000,
          signal: activeCoachAbort.signal
        }
      )

      const rawAiResponse = response.data?.choices?.[0]?.message?.content?.trim()

      if (!rawAiResponse) {
        throw new Error('ИИ прислал пустой ответ.')
      }

      const finalReply = cleanAndFinishText(rawAiResponse)

      // ПОДБОР КАРТОЧЕК:
      // Карточки прикрепляются ТОЛЬКО если ИИ РЕАЛЬНО упомянул модель в своем ответе!
      const upperReply = finalReply.toUpperCase()
      const matchedProducts = products.filter(p => {
        const pNameUpper = p.name.toUpperCase()
        if (upperReply.includes(pNameUpper)) return true
        
        const keywords = pNameUpper.replace('GEL-', '').split(' ').filter(k => k.length >= 3)
        return keywords.some(k => upperReply.includes(k))
      })

      return {
        reply: finalReply,
        // Если подходящих товаров нет (например, вопрос про шест) — карточки НЕ показываются
        recommendations: matchedProducts.slice(0, 3).map(p => ({
          productId: p.id,
          productName: p.name,
          price: p.basePrice
        }))
      }
    } catch (e) {
      if (axios.isCancel(e)) return null

      const errorMsg = e.response?.data?.error?.message || e.message
      return {
        reply: `Ошибка: ${errorMsg}`,
        recommendations: []
      }
    } finally {
      isCoachLoading.value = false
      activeCoachAbort = null
    }
  }

  // 2. AI Copywriter для менеджера
  const generateDescription = async (payload) => {
    isGenerating.value = true
    try {
      const prompt = `Напиши продающее описание товара для магазина ASICS на русском языке (ровно 3 полных предложения). Обязательно закончи точкой.
Модель: ${payload.name}, Категория: ${payload.category}, Бренд: ${payload.brand}. Особенности: ${payload.technologies || 'амортизация, легкие материалы'}.
НЕ используй кавычки, звездочки и вступительные фразы.`

      const response = await axios.post(
        'https://openrouter.ai/api/v1/chat/completions',
        {
          model: MODEL,
          messages: [
            { role: 'user', content: prompt }
          ],
          temperature: 0.3,
          max_tokens: 800
        },
        {
          headers: {
            'Authorization': `Bearer ${API_KEY}`,
            'Content-Type': 'application/json'
          },
          timeout: 25000
        }
      )

      const desc = response.data?.choices?.[0]?.message?.content?.trim()
      if (desc) {
        toast.success('Описание сгенерировано!')
        return cleanAndFinishText(desc.replace(/^["']|["']$/g, ''))
      }
      throw new Error('Пустой ответ')
    } catch (e) {
      toast.error(`Ошибка генерации: ${e.response?.data?.error?.message || e.message}`)
      return null
    } finally {
      isGenerating.value = false
    }
  }

  return {
    isGenerating,
    isCoachLoading,
    askCoach,
    generateDescription
  }
})

export default useAiStore