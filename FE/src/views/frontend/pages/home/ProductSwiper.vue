<template>
    <div class="listing-service">
      <swiper
        :modules="modules"
        :slides-per-view="1"
        :space-between="20"
        :pagination="{ clickable: true }"
        :navigation="true"
        :breakpoints="{
          576: { slidesPerView: 2 },
          768: { slidesPerView: 3 },
          1024: { slidesPerView: 4 }
        }"
      >
        <swiper-slide v-for="product in products" :key="product.id">
          <router-link :to="`/san-pham-chi-tiet/${product.id}`">
            <div class="service-item">
              <div class="service-thumb">
                <img 
                  :src="product.imageUrl" 
                  :alt="product.name"
                  class="product-image"
                >
              </div>
              <div class="service-content">
                <span>{{ categoryName }}</span>
                <h3>{{ product.name }}</h3>
                <p class="service-excerpt">{{ formatCurrency(product.price) }}</p>
              </div>
            </div>
          </router-link>
        </swiper-slide>
      </swiper>
    </div>
  </template>
  
  <script setup>
import { Swiper, SwiperSlide } from 'swiper/vue';
import { Pagination, Navigation } from 'swiper/modules';
import 'swiper/css';
import 'swiper/css/pagination';
import 'swiper/css/navigation';
const props = defineProps({
  products: Array,
  categoryName: String
});
function formatCurrency(value) {
  return value ? value.toLocaleString("vi-VN") + "đ" : "0đ";
}
</script>
  
  <style scoped>
  .product-image {
    width: 100%;
    object-fit: cover;
    border-radius: 8px;
  }
</style>