<template>
  <section class="service-section section-list-service pt-60 pb-60">
    <div class="container">
      <div class="title_modules">
        <h2> 
          <a href="san-pham" title="#Sản phẩm mới"><span>SẢN PHẨM MỚI</span></a>
        </h2>
      </div>
      
      <b-card no-body v-if="listLoai.length > 0">
        <b-tabs card>
          <b-tab 
            v-for="loai in listLoai" 
            :key="loai.id"
            :title="loai.name"
            :active="loai.id === listLoai[0].id"
          >
            <b-card-text>
              <ProductSwiper 
                :products="getProductsByCategory(loai.id)"
                :category-name="loai.name"
              />
            </b-card-text>
          </b-tab>
        </b-tabs>
      </b-card>
      
      <div v-else class="text-center py-4">
        <p>Đang tải danh mục...</p>
      </div>
    </div>
  </section>
  <section class="lib-section-4">	
    <div class="section_banner section_large_banner">
      <div class="container">
        <router-link
            :to="{
              path: `/`,
            }"
        >
        <img class="img-responsive image-line" src="@/assets/img/caulong/core/banner-section.png" alt="large-banner">
        </router-link>
      </div>
    </div>
  </section>

</template>

<script>
import ProductSwiper from '@/views/frontend/pages/home/ProductSwiper.vue';

export default {
  components: {
    ProductSwiper
  },
  data() {
    return {
      list: [],       // Danh sách sản phẩm
      listLoai: [],   // Danh sách loại từ API
    };
  },
  methods: {
    // Lấy danh sách sản phẩm
    async getData() {
      await this.$store.dispatch("sanPhamStore/getAllCustomer").then(res => {
        this.list = res.data || [];
      });
    },
    
    // Lấy danh sách loại
    async getListLoai() {
      await this.$store.dispatch("loaiStore/getAllCustomer").then((res) => {
        if (res != null && res.code === 0) {
          this.listLoai = res.data || [];
        }
      });
    },
    
    // Lọc sản phẩm theo categoryId
    getProductsByCategory(categoryId) {
      return this.list.filter(item => item.categoriesId == categoryId);
    }
  },
  created() {
    this.getData();
    this.getListLoai();
  }
};
</script>