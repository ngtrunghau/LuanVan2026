<template>
  <div class="main-wrapper">
    <layoutheader :class="{ 'header-space': showHeaderSpace }" ref="header" />

    <div class="content" style="padding-top: 150px;">
      <div class="container">
        <div class="row">
          <div class="col-md-7 col-lg-9 col-xl-9">
            <div class="card">
              <div class="card-body product-description">
                <div class="doctor-widget">
                  <div class="doc-info-left">
                    <div class="doctor-img1" v-if="this.model.imageUrl">
                      <img
                        :src="this.model.imageUrl"
                        class="img-fluid"
                        alt="User Image"
                      />
                    </div>
                    <div class="doctor-img1" v-else>
                      <img
                        src="@/assets/img/caulong/logo/logoNTH_removeBackground.png"
                        class="img-fluid"
                        alt="User Image"
                      />
                    </div>
                    <div class="doc-info-cont">
                      <h4 class="doc-name mb-2">{{ this.model.name }}</h4>
                      <!-- <p>
                        Thương hiệu: VNB
                      </p>
                      <p>
                        Tuy nhiên điểm khác biệt ở V200 Xanh là ngoài màu vợt ra thì vợt nặng đầu hơn so với V200 Đỏ. 
                        Đểm này giúp V200 đối với người mới chơi và người luyện tập thi đấu là tập thêm được lực cổ tay, 
                        hỗ trợ đập cầu đi cắm hơn, nhanh và mạnh hơn. Còn đối với người thích đánh thiên công thì vợt càng 
                        tăng thêm sức mạnh cho những pha đập cầu. Sợi carbon chắc chắc và bền giúp vợt nhanh chóng quay lại 
                        trạng thái ổn định để chuẩn bị tốt nhất cho pha đánh cầu sau. 
                      </p> -->
                      <div class="feature-product pt-4">
                        <span>Ưu đãi:</span>
                        <ul>
                          <li>
                            <img src="@/assets/img/caulong/tick.png" alt="tick" class="img-tick">
                            Tặng 2 Quấn cán vợt Cầu Lông
                          </li>
                          <li>
                            <img src="@/assets/img/caulong/tick.png" alt="tick" class="img-tick">
                            Sản phẩm cam kết chính hãng
                          </li>
                          <li>
                            <img src="@/assets/img/caulong/tick.png" alt="tick" class="img-tick">
                            Một số sản phẩm sẽ được tặng bao đơn hoặc bao nhung bảo vệ vợt
                          </li>
                          <li>
                            <img src="@/assets/img/caulong/tick.png" alt="tick" class="img-tick">
                            Thanh toán sau khi kiểm tra và nhận hàng (Giao khung vợt)
                          </li>
                          <li>
                            <img src="@/assets/img/caulong/tick.png" alt="tick" class="img-tick">
                            Bảo hành chính hãng theo nhà sản xuất (Trừ hàng nội địa, xách tay)
                          </li>
                        </ul>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
            <SanPhamDetail :detail="this.model"></SanPhamDetail>
          </div>
          <div class="col-md-5 col-lg-3 col-xl-3 theiaStickySidebar">
            <!-- Right Details -->
            <div class="stickysidebar">
              <SanPhamSidebar :sidebar="this.model"></SanPhamSidebar>
            </div>
            <!-- /Right Details -->
          </div>
        </div>
      </div>
    </div>
    <footerHome></footerHome>
  </div>
</template>
<script setup>
import { getCurrentInstance, reactive, toRefs, watch } from "vue";
import Loading from "vue3-loading-overlay";
import Paginate from "vuejs-paginate-next";
import 'vue-multiselect/dist/vue-multiselect.css';
import Treeselect from 'vue3-treeselect';
import { sanPhamModel } from "@/models/sanPhamModel";
import '@vuepic/vue-datepicker/dist/main.css';
import { notifyModel } from "@/models/notifyModel";
import { Form, Field } from "vee-validate";
import * as Yup from "yup";
defineOptions({
  name: "admin/page"
});
const {
  proxy
} = getCurrentInstance();
const state = reactive({
  title: "CHI TIẾT SẢN PHẨM",
  model: sanPhamModel.baseJson(),
  urlFile: `${process.env.VUE_APP_API_URL}files/view/`,
  listLoai: []
});
const {
  title,
  model,
  urlFile,
  listLoai
} = toRefs(state);
const schema = Yup.object().shape({
  name: Yup.string().required("Tên sản phẩm không được bỏ trống !")
});
async function getListLoai() {
  await proxy.$store.dispatch("loaiStore/getAllCustomer").then(res => {
    if (res != null && res.code === 0) {
      state.listLoai = res.data || [];
    }
  });
}
async function handleInfo() {
  const params = {
    id: proxy.$route.params.id
  };
  await proxy.$store.dispatch("sanPhamStore/getByIdCustomer", params).then(res => {
    //  console.log("ID: ", res);
    if (res.code === 0) {
      console.log(res);
      state.model = sanPhamModel.getJson(res.data);
      state.model.categories = state.listLoai.find(cat => cat.id === res.data.categoriesId) || null;
      // this.$refs.form.setFieldValue('fileImage', res.data.fileImage || null);
    } else {
      proxy.$store.dispatch("snackBarStore/addNotify", {
        message: res.message,
        code: res.code
      });
    }
  });
}
getListLoai();
handleInfo();
</script>

