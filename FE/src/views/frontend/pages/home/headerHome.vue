<template>
  <!-- Home Banner -->
  <section class="banner-section">
    <div class="banner-slider">
      <swiper
        :spaceBetween="0"
        :navigation="false"
        :pagination="false"
        :loop="true"
        :autoplay="{ delay: 5000, disableOnInteraction: false }"
        :modules="modules"
      >
        <swiper-slide class="slide-news">
          <img
              src="@/assets/img/caulong/core/banner.png" alt="Image"
              style="width: 100%"
            />
        </swiper-slide>
        <swiper-slide class="slide-news">
          <img
              src="@/assets/img/caulong/core/banner-1.png" alt="Image"
              style="width: 100%"
            />
        </swiper-slide>
        <swiper-slide class="slide-news">
          <img
              src="@/assets/img/caulong/core/banner-2.png" alt="Image"
              style="width: 100%"
            />
        </swiper-slide>
      </swiper>
    </div>
  </section>
  <!-- /Home Banner -->


  <div class="section_services pt-60 pb-60">
    <div class="container">
      <div class="row promo-box">
        <div class="col-lg-3 col-md-3 col-sm-6 col-6">
          <div class="promo-item">
            <div class="icon">
              <img width="50" height="50" class="lazyload loaded" src="https://cdn.shopvnb.com/themes/images/policy_image_2.svg" data-src="https://cdn.shopvnb.com/themes/images/policy_image_2.svg" alt="Vận chuyển toàn quốc" data-was-processed="true">
            </div>
            <div class="info">
              Vận chuyển <span>TOÀN QUỐC</span> <br> Thanh toán khi nhận hàng
            </div>
          </div>
        </div>

        <div class="col-lg-3 col-md-3 col-sm-6 col-6">
          <div class="promo-item">
            <div class="icon">
              <img width="50" height="50" class="lazyload loaded" src="https://cdn.shopvnb.com/themes/images/policy_image_1.svg" data-src="https://cdn.shopvnb.com/themes/images/policy_image_1.svg" alt="" data-was-processed="true">
            </div>
            <div class="info">
              <span>Bảo đảm chất lượng</span> <br>Sản phẩm bảo đảm chất lượng.
            </div>
          </div>
        </div>

        <div class="col-lg-3 col-md-3 col-sm-6 col-6">
          <div class="promo-item">
            <div class="icon">
              <img width="50" height="50" class="lazyload loaded" src="https://cdn.shopvnb.com/themes/images/thanh_toan.svg" data-src="https://cdn.shopvnb.com/themes/images/thanh_toan.svg" alt="" data-was-processed="true">
            </div>
            <div class="info">
              Tiến hành <span>THANH TOÁN</span> <br> Với nhiều <span>PHƯƠNG THỨC</span>
            </div>
          </div>
        </div>

        <div class="col-lg-3 col-md-3 col-sm-6 col-6">
          <div class="promo-item">
            <div class="icon">
              <img width="50" height="50" class="lazyload loaded" src="https://cdn.shopvnb.com/themes/images/doi_san_pham.svg" data-src="https://cdn.shopvnb.com/themes/images/doi_san_pham.svg" alt="" data-was-processed="true">
            </div>
            <div class="info">
              <span>Đổi sản phẩm mới</span><br> nếu sản phẩm lỗi
            </div>
          </div>
        </div>

      </div>
    </div>
  </div>
  

  <section id="section-2" class="section-brand-history pb-60 pt-60">
    <div class="container">
      <div class="row">
        <div class="col-lg-6 col-md-6 col-12">
          <div class="brand-history-content">
            <h1>Bạn là một người đam mê cầu lông</h1>
            <p>
              Với xuất phát từ những người chơi cầu lông và đam mê cầu lông, ShopNTH hiểu được sự cần thiết của dụng cụ cầu lông 
              chính hãng phục vụ cho người chơi sẽ mang tới những cảm giác thích thú nhất cho những người đam mê bộ môn này.
            </p>
            <p>
              Với hệ thống Cửa Hàng cầu lông trên toàn quốc sẽ giúp người chơi có địa điểm uy tín và đa dạng sản phẩm. 
              Quý Khách có thể yên tâm lựa chọn cho mình những dụng cụ, phụ kiện phù hợp với trình độ và sở thích của 
              mình để cùng cháy với đam mê Cầu Lông.
            </p>
            <p class="slogan">ShopNTH Đồng Hành Cùng Đam Mê!</p>
          </div>
        </div>
        <div class="col-lg-6 col-md-6 col-12">
          <div class="brand-history-thumb">
            <a class="popup-video" href="">
                <img class=" ls-is-cached lazyloaded" src="@/assets/img/caulong/core/banner2.jpg" data-src="@/assets/img/caulong/core/banner2.jpg" alt="Câu chuyện thương hiệu">
            </a>
          </div>
        </div>
      </div>
    </div>
  </section>
</template>

<script setup>
import { getCurrentInstance, onMounted, reactive, toRefs, watch } from "vue";
import { ref } from "vue";
const currentDate = ref(new Date());
import AOS from "aos";
import "aos/dist/aos.css";
import { Swiper, SwiperSlide } from 'swiper/vue';

// Import Swiper styles
// Import Swiper styles
import 'swiper/css';
import 'swiper/css/pagination';
// import required modules
// import required modules
import { Navigation, Pagination, Autoplay } from 'swiper/modules';
const props = defineProps({
  header: {
    type: Object
  }
});
const {
  proxy
} = getCurrentInstance();
const state = reactive({
  modules: [Navigation, Pagination, Autoplay],
  startdate: currentDate,
  list: [],
  url: `${process.env.VUE_APP_API_URL}files/view/`,
  urlFile: `${process.env.VUE_APP_API_URL}files/view`
});
const {
  modules,
  startdate,
  list,
  url,
  urlFile
} = toRefs(state);
watch(() => proxy.$props, val => {
  state.list = val.header;
  //  console.log("LOG WATCH LIST ", this.list )
}, {
  deep: true
});
onMounted(() => {
  proxy.$nextTick(() => {
    AOS.init();
  });
});
</script>
<style>
.carousel__pagination{
  position: absolute;
  bottom: 10px;
  left: 50%;
  transform: translateX(-50px);
}

.carousel__pagination-button--active::after{
  background-color: #F5E7B2 !important;
  width: 10px !important;
  height: 10px !important;
  border-radius: 5px !important;
}

.carousel__pagination-button::after{
  width: 10px;
  height: 10px;
  border-radius: 50%;
  background-color: #F5E7B25d;
}
</style>

