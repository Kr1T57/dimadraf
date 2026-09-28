import {defineStore} from "pinia";
import axios from "axios";
import {ref} from "vue";
import {useToast} from "vue-toastification";

const useOrderStore = defineStore('order', () => {
    const order = ref(null)
    const toast = useToast()
    const orderHistory = ref([])
    const addOrder = async (orderData) => {
        await axios
            .post('http://localhost:5000/api/Order/AddOrder', orderData)
            .then((res) => {
                order.value = res.data
            })
            .catch((err) => toast.error(err.response?.data))
    }
    const addOrderItem = async (userId, OrderId) => {
        await axios
            .postForm('http://localhost:5000/api/order/addOrderItem', {userId, OrderId})
            .catch((err) => toast.error(err.response.data))
    }
    const  getAllOrders = async () => {
        await axios
        .get(`http://localhost:5000/api/order/GetAllOrders`)
        .then((res) => {
            order.value = res.data
        })
        .catch((err) => toast.error(err.response?.data))
    }
    const editOrderStatus = async (orderData) =>{
        await axios
        .put('http://localhost:5000/api/order/EditStatusOrder', orderData)
        .then(async () => {
            await getAllOrders()
        })
        .catch((err) => toast.error(err.response.data))
    }
    const getUserOrderHistory = async (userId) => {
        await axios
            .get(`http://localhost:5000/api/Order/GetUserOrderHistory/${userId}`)
            .then((res) => {
                orderHistory.value = res.data
            })
            .catch((err) => toast.error(err.response?.data || 'Ошибка загрузки истории заказов'))
    }
    return {
        order,
        addOrder,
        orderHistory,
        addOrderItem,
        editOrderStatus,
        getAllOrders,
        getUserOrderHistory
    }
})

export default useOrderStore