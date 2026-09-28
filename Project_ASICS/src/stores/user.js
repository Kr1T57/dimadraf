import { defineStore } from 'pinia'
import { ref } from 'vue'
import axios from 'axios'
import { useToast } from 'vue-toastification'
import useAnalyticsStore from "@/stores/analytics.js"
import router from "@/router/index.js";

const useUserStore = defineStore('user', () => {
  const user = ref(null)
  const allUsers = ref([])
  const toast = useToast()
  const analyticsStore = useAnalyticsStore()
  const authUser = async (login, password) => {
    await axios
      .postForm('http://localhost:5000/api/user/Authorization', {
        login: login,
        password: password,
      })
        .then((res) => {
          user.value = res.data

          localStorage.setItem('user', JSON.stringify({
            login: login,
            password: password
          }))
        })
      .catch((err) => toast.error(err.response.data))
  }
  const registerUser = async (userData) => {
    await axios
      .post('http://localhost:5000/api/user/Registration', userData)
        .then((res) => {
          user.value = res.data

          localStorage.setItem('user', JSON.stringify({
            login: userData.login,
            password: userData.password
          }))
        })
      .catch((err) => toast.error(err.response.data))
  }
  const editUser = async (userData) => {
    try {
      const res = await axios.put('http://localhost:5000/api/user/EditUser', userData)
      user.value = res.data
      toast.success('Успешно обновлено')
      return true
    } catch (err) {
      toast.error(err.response?.data || 'Ошибка при обновлении данных')
      return false
    }
  }
  const getAllUsers = async () => {
    await axios
        .get('http://localhost:5000/api/user/GetAllUsers')
        .then((res) => {allUsers.value = res.data})
        .catch((err) => toast.error(err.response.data))
  }
  const logout = () => {
    user.value = null
    localStorage.removeItem('user')
    analyticsStore.clearFavorites()
    toast.success('Вы успешно вышли из аккаунта')
    router.push({ name: 'Home' })
  }
  return {
    user,
    authUser,
    registerUser,
    editUser,
    getAllUsers,
    allUsers,
    logout,
  }
})
export default useUserStore
