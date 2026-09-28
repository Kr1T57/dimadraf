import {defineStore} from "pinia";
import { ref } from 'vue'
import axios from 'axios'
import { useToast } from 'vue-toastification'

const useAnalyticsStore = defineStore('analytics', () => {
    const mostLikedProducts = ref([])
    const mostLikedVariants = ref([])
    const userProductFavorites = ref([])
    const userVariantFavorites = ref([])
    const userFullProducts = ref([])
    const userFullVariants = ref([])
    const toast = useToast()

    const fetchProductAnalytics = async () => {
        try {
            const res = await axios.get('http://localhost:5000/api/Analytics/GetMostLikedProducts')
            mostLikedProducts.value = res.data
        } catch (err) {
            toast.error('Ошибка при загрузке аналитики товаров')
        }
    }

    const fetchVariantAnalytics = async () => {
        try {
            const res = await axios.get('http://localhost:5000/api/Analytics/GetMostLikedVariants')
            mostLikedVariants.value = res.data
        } catch (err) {
            toast.error('Ошибка при загрузке аналитики вариаций')
        }
    }
    const fetchUserFavorites = async (userId) => {
        try {
            const prodRes = await axios.get(`http://localhost:5000/api/Analytics/GetUserProductFavorites/${userId}`)
            userProductFavorites.value = prodRes.data

            const varRes = await axios.get(`http://localhost:5000/api/Analytics/GetUserVariantFavorites/${userId}`)
            userVariantFavorites.value = varRes.data

            const fullProdRes = await axios.get(`http://localhost:5000/api/Analytics/GetActualUserProductFavorites/${userId}`)
            userFullProducts.value = fullProdRes.data

            const fullVarRes = await axios.get(`http://localhost:5000/api/Analytics/GetActualUserVariantFavorites/${userId}`)
            userFullVariants.value = fullVarRes.data
        } catch (err) {
            console.error('Не удалось загрузить избранное пользователя')
        }
    }

    const toggleProductFavorite = async (userId, productId) => {
        try {
            const res = await axios.post('http://localhost:5000/api/Analytics/ToggleProductFavorite', {
                id: 0,
                userId: userId,
                productId: productId
            })

            if (res.data.isLiked) {
                userProductFavorites.value.push(productId)
                toast.success('Товар добавлен в избранное')
            } else {
                userProductFavorites.value = userProductFavorites.value.filter(id => id !== productId)
                userFullProducts.value = userFullProducts.value.filter(p => p.productId !== productId)
                toast.info('Товар удален из избранного')
            }
        } catch (err) {
            toast.error('Не удалось изменить статус избранного')
        }
    }

    const toggleVariantFavorite = async (userId, variantId) => {
        try {
            const res = await axios.post('http://localhost:5000/api/Analytics/ToggleVariantFavorite', {
                id: 0,
                userId: userId,
                productVariantId: variantId
            })

            if (res.data.isLiked) {
                userVariantFavorites.value.push(variantId)
                toast.success('Размер добавлен в избранное')
            } else {
                userVariantFavorites.value = userVariantFavorites.value.filter(id => id !== variantId)
                userFullVariants.value = userFullVariants.value.filter(v => v.productVariantId !== variantId)
                toast.info('Размер удален из избранного')
            }
        } catch (err) {
            toast.error('Не удалось изменить статус избранного для вариации')
        }
    }
    const clearFavorites = () => {
        userProductFavorites.value = []
        userVariantFavorites.value = []
    }

    return {
        mostLikedProducts,
        mostLikedVariants,
        userProductFavorites,
        userVariantFavorites,
        userFullProducts,
        userFullVariants,
        fetchProductAnalytics,
        fetchVariantAnalytics,
        fetchUserFavorites,
        toggleProductFavorite,
        toggleVariantFavorite,
        clearFavorites
    }
})

export default useAnalyticsStore