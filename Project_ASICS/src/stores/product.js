import { defineStore } from 'pinia'
import { ref } from 'vue'
import axios from 'axios'
import { useToast } from 'vue-toastification'

const toast = useToast()

const useProductStore = defineStore('product', () => {
  const products = ref([])
  const currentProduct = ref(null)

  async function productFun() {
    await axios
      .get('http://localhost:5000/api/product/GetProductAll')
      .then((res) => (products.value = res.data))
      .catch((err) => toast.error(err.response.data))
  }
  const getProduct = async (id) => {
    let getProductForSum
    await axios
      .get(`http://localhost:5000/api/product/GetProduct/${id}`)
      .then((res) => {
        currentProduct.value = res.data
        getProductForSum = res.data
      })
      .catch((err) => toast.error(err.response.data))
    return getProductForSum
  }
  const editProduct = async (productData) => {
    await  axios
    .put('http://localhost:5000/api/product/EditProduct', productData)
    .then(async () => {
      await productFun()
    })
    .catch((err) => toast.error(err.response.data))
  }
  const editProductVariant = async (productVariantData) => {
    await  axios
    .put('http://localhost:5000/api/product/EditProductVariant', productVariantData)
    .then(async () => {
      await productFun()
    })
    .catch((err) => toast.error(err.response.data))
  }
  const deleteProduct = async (id) => {
    await axios
    .delete(`http://localhost:5000/api/product/DeleteProduct/${id}`)
    .catch((err) => toast.error(err.response.data))
  }
  const addProductVariant = async (variantData) => {
    return await axios.post('http://localhost:5000/api/productvariant/AddProductVariant', variantData)
        .catch((err) => toast.error(err.response.data))
  }
  const addProduct = async (productData) => {
    return await axios.post('http://localhost:5000/api/product/AddProduct', productData)
        .catch((err) => toast.error(err.response.data))
  }

  const deleteProductVariant = async (id) => {
    await axios.delete(`http://localhost:5000/api/productvariant/DeleteProductVariant/${id}`)
        .catch((err) => toast.error(err.response.data))
  }

  const addImageProduct = async (file, productData) => {
    const formData = new FormData()
    formData.append('file', file)
    formData.append('productId', productData.id)
    formData.append('category', productData.category)
    formData.append('name', productData.name)

    return await axios.post('http://localhost:5000/api/image/UpLoadProductImage', formData, {
      headers: { 'Content-Type': 'multipart/form-data' }
    }).catch((err) => {
      const errorMsg = err.response?.data?.title || err.response?.data || 'Ошибка загрузки фото товара'
      toast.error(typeof errorMsg === 'string' ? errorMsg : 'Ошибка валидации фото')
      throw err
    })
  }

  const addImageVariant = async (file, variantData) => {
    const formData = new FormData()
    formData.append('file', file)
    formData.append('variantId', variantData.id)
    formData.append('color', variantData.color)
    formData.append('size', variantData.size)

    return await axios.post('http://localhost:5000/api/image/UpLoadProductVariantImage', formData, {
      headers: { 'Content-Type': 'multipart/form-data' }
    }).catch((err) => {
      const errorMsg = err.response?.data?.title || err.response?.data || 'Ошибка загрузки фото вариации'
      toast.error(typeof errorMsg === 'string' ? errorMsg : 'Ошибка валидации фото')
      throw err
    })
  }

  const getProductImage = async (productId) => {
    let productImage = null
    await axios
        .get(`http://localhost:5000/api/image/GetProductImage/${productId}`)
        .then((res) => {
          productImage = res.data
        })
        .catch((err) => {
          console.warn(`У товара с ID ${productId} отсутствует изображение в БД.`);
        })

    return productImage
  }

  const getVariantImage = async (variantId) => {
    let variantImage = null
    await axios
        .get(`http://localhost:5000/api/image/GetVariantImage/${variantId}`)
        .then((res) => {
          variantImage = res.data
        })
        .catch((err) => {
          console.warn(`У вариации с ID ${variantId} отсутствует изображение в БД.`);
        })

    return variantImage
  }

  const getProductImageByVariant = async (variantId) => {
    let productImage = null
    await axios
        .get(`http://localhost:5000/api/image/GetProductImageByVariant/${variantId}`)
        .then((res) => {
          productImage = res.data
        })
        .catch((err) => {
          console.warn(`Не удалось загрузить картинку товара для вариации ID ${variantId}`);
        })

    return productImage
  }
  return {
    products,
    productFun,
    currentProduct,
    getProduct,
    editProduct,
    editProductVariant,
    deleteProduct,
    addProductVariant,
    deleteProductVariant,
    addProduct,
    addImageVariant,
    addImageProduct,
    getProductImage,
    getVariantImage,
    getProductImageByVariant,
  }
})

export default useProductStore
