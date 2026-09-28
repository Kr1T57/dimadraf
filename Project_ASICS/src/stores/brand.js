import {defineStore} from "pinia";
import {ref} from "vue";
import {useToast} from "vue-toastification";
import axios from "axios";

const useBrandStore = defineStore('brand',() => {
    const brand = ref([])
    const toast = useToast()
    const getAllBrand = async () => {
        await axios
            .get('http://localhost:5000/api/brand/GetAllBrands')
            .then((res) => {brand.value = res.data})
            .catch((err) => toast.error(err.response.data))
    }
    return {
        brand,
        getAllBrand,
    }
})
export default useBrandStore;