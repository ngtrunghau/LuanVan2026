<template>
  <!-- Footer -->
  <footer class="footer footer-one footer-ten">
    <div class="footer-top">
      <div class="container">
        <div class="footer-details">
          <div class="row-footer">
            <div class="" style="display: flex; justify-content: center; align-items: center;">
              <div class="footer-widget footer-about">
                <div class="footer-logo">
                  <img src="@/assets/img/caulong/logo/logo_HBTShop-removebg.png" alt="logo" />
                </div>
                <div class="footer-about-content" v-if ="this.listFooter">
                  <p style="text-align: center; padding-bottom: 5px; font-size: 14px">
                    ShopHBT - Hệ Thống Shop Cầu Lông
                  </p>
                </div>
                <!-- <div class="footer-about-content" style="text-align: center; color: #5B2F39;">
                  <span style="text-align: center; padding-bottom: 5px; font-size: 14px;">
                    NƠI HỘI TỤ TÂM - TÀI - ĐỨC
                  </span>
                </div> -->
              </div>
            </div>
            <div class="">
              <div class="" style="font-size: 14px; line-height: 24px;">
                <div class="" style="padding: 0;">
                  <div class="name-footer">ShopHBT - Hệ Thống Shop Cầu Lông</div>
                  <div class="title-footer">
                   <i class="fa fa-solid fa-location-dot me-1" style="color: #BE8B43"></i>
                    Số 123, QL30, phường 1, TP. Cao Lãnh, tỉnh Đồng Tháp
                  </div>
                  <div class="title-footer">
                   <i class="fa fa-solid fa-phone me-1" style="color: #BE8B43"></i>
                    Hotline: 0123 456 789
                  </div>
                  <div class="title-footer">
                   <i class="fa fa-solid fa-phone me-1" style="color: #BE8B43"></i>
                    Hợp tác kinh doanh: 0123 456 789 (Trung Hậu)
                  </div>
                </div>
              </div>
            </div>

          </div>
        </div>
      </div>
    </div>
    <div class="footer-bottom">
      <div class="container">
        <!-- Copyright -->
        <div class="copyright">
          <div class="copyright-text">
            <p class="mb-0" style="color: #000;">
              © {{ new Date().getFullYear() }} Bản quyền thuộc về Trung Hậu
            </p>
          </div>
        </div>
        <!-- /Copyright -->
      </div>
    </div>
  </footer>
  <!-- /Footer -->
</template>
<script>
import AOS from "aos";
import "aos/dist/aos.css";
export default {
  data() {
    return {
      url : `${process.env.VUE_APP_API_URL}files/view/`,
      urlFile:`${process.env.VUE_APP_API_URL}files/view`,
    };
  },
  props: {
    footer: { type : Object}
  },
  mounted() {
    this.$nextTick(() => {
      AOS.init();
    });
  },
  created() {
  },
  // watch:{
  //   '$props':{
  //     handler: function (val) {
  //       this.listFooter = val.footer;
  //     },
  //     deep: true
  //   }
  // },
  methods: {
  },
};

</script>

<style>

</style>
