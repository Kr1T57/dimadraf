import {defineStore} from "pinia";
import {ref} from "vue";
import {useToast} from "vue-toastification";
import axios from "axios";

const useCategoryStore =  defineStore('category',() =>{
    const category = ref([])
    const toast = useToast()
    const getAllCategories = async () => {
        if (category.value && category.value.length > 0) return;
        await  axios
        .get(`http://localhost:5000/api/category/GetAllCategory`)
        .then((res) => (category.value = res.data))
        .catch((err) => toast.error(err.response.data))
    }
    return {
        category,
        getAllCategories,
    }
})
export default useCategoryStore;