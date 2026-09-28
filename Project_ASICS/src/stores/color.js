import {defineStore} from "pinia";
import {ref} from "vue";
import {useToast} from "vue-toastification";
import axios from "axios";

const useColorStore = defineStore('color',() => {
    const color = ref([])
    const toast = useToast()
    const getAllColors = async () => {
        await axios
        .get('http://localhost:5000/api/color/GetAllColor')
        .then((res) => {color.value = res.data})
        .catch((err) => toast.error(err.response.data))
    }
    return {
        color,
        getAllColors,
    }
})
export default useColorStore;