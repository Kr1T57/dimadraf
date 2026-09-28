import { defineStore } from 'pinia'
import {computed, ref} from 'vue'
import { useToast } from 'vue-toastification'
import axios from 'axios'

const useCartItemStore = defineStore('cartItem', () => {
  const cartItem = ref(null)
  const currentCartItem = ref([])
  const toast = useToast()
  const addToCart = async (cartData) => {
    try {
      const res = await axios.post('http://localhost:5000/api/cartItem/AddCartItem', cartData)
      cartItem.value = res.data
      return true
    } catch (err) {
      toast.error(err.response?.data || 'Ошибка при добавлении')
      return false
    }
  }
  const minusToCart = async (cartData) => {
    try {
      const res = await axios.put('http://localhost:5000/api/cartItem/SubtractCartItem', cartData)
      cartItem.value = res.data
      return true
    } catch (err) {
      toast.error(err.response?.data || 'Ошибка при уменьшении количества')
      return false
    }
  }
  const cartTotalPrice = computed(() => {
    return currentCartItem.value.reduce((sum, item) => {
      const price = Number(item.price !== undefined ? item.price : item.Price) || 0
      const quantity = Number(item.quantity !== undefined ? item.quantity : item.Quantity) || 0

      return sum + (price * quantity)
    }, 0)
  })
  const getCartItems = async (id) => {
    await axios
      .get(`http://localhost:5000/api/cartItem/GetCartItem/${id}`)
      .then((res) => {
        currentCartItem.value = res.data
      })
      .catch((err) => toast.error(err.response.data))
  }
  const getCart = async (id) => {
    let cartSum
    await axios
      .get(`http://localhost:5000/api/cartItem/GetCartForSum/${id}`)
      .then((res) => {
        cartSum = res.data
      })
      .catch((err) => toast.error(err.response.data))
    return cartSum
  }
  const removeCart = async (id) => {
    await axios
      .delete(`http://localhost:5000/api/cartItem/DeleteCartItem/${id}`)
      .catch((err) => toast.error(err.response.data))
  }
  const clearAllCart = () =>{
    cartItem.value = null
    currentCartItem.value = []
  }
  return {
    cartItem,
    addToCart,
    currentCartItem,
    getCartItems,
    minusToCart,
    getCart,
    removeCart,
    clearAllCart,
    cartTotalPrice,
  }
})
export default useCartItemStore
