<template>
  <div class="main-wrapper home-ten" style="position: relative;">
    <!-- <div class="social-icon">
      <a href="" target="_blank">
        <img src="@/assets/img/phuongthanh/icon-fb.png" alt="Image" class="icon-footer">
      </a>
      <a href="" target="_blank">
        <img src="@/assets/img/phuongthanh/icon-insta.png" alt="Image" class="icon-footer">
      </a>
      <a href="" target="_blank">
        <img src="@/assets/img/phuongthanh/icon-youtube.png" alt="Image" class="icon-footer" style="border-radius: 5px">
      </a>
      <a href="" target="_blank">
        <img src="@/assets/img/phuongthanh/icon-map.png" alt="Image" class="icon-footer" style="border-radius: 5px">
      </a>
    </div> -->
    
    <layoutheader :class="{ 'header-space': showHeaderSpace }" ref="header" />

    <headerHome></headerHome>

    <indexfiveservice></indexfiveservice>
    
    <footerHome></footerHome>

    <cursor />

    <FullScreenPopup :isVisible="showPopup" @close="showPopup = false">
      <Form
          class="login"
          @submit="handleSubmit"
          :validation-schema="schema"
          v-slot="{ errors }"
          ref="form"
      >
        <div class="row">
          <div class="col-lg-12 col-md-12" style="padding: 20px;">
            <div class="row">
              <div class="col-md-12" style="padding: 0 50px; border-bottom: 1px dotted #ccc; height: 100px; text-align: center">
                <img src="@/assets/img/phuongthanh/logo/logo.png" alt="Image" style="height: 100%;">
              </div>
              <div style="padding: 0 20px;">
                <div class="title-popup">
                  <p>
                    ĐẶT LỊCH HẸN
                  </p>
                  <span>
                    Vui lòng để lại thông tin đầy đủ theo mẫu bên dưới.
                    <strong>Phương Thành</strong> sẽ liên hệ với bạn trong thời gian sớm nhất.
                  </span>
                </div>
                <div class="col-md-12">
                  <div class="mb-3">
                    <Field
                        v-model="model.name"
                        placeholder="Vui lòng nhập họ và tên"
                        name="name"
                        type="text"
                        class="form-control"
                        :class="{ 'is-invalid': errors.name }"
                    />
                        <div class="invalid-feedback">{{ errors.name }}</div>
                  </div>
                </div>
                <div class="col-md-12">
                  <div class="mb-3">
                    <Field
                        v-model="model.phone"
                        placeholder="Vui lòng nhập số điện thoại"
                        name="phone"
                        type="text"
                        class="form-control"
                        :class="{ 'is-invalid': errors.phone }"
                    />
                        <div class="invalid-feedback">{{ errors.phone }}</div>
                  </div>
                </div>
                <div class="col-md-12">
                  <div class="mb-3">
                    <Field
                        v-model="model.email"
                        placeholder="Vui lòng nhập email"
                        name="email"
                        type="text"
                        class="form-control"
                        :class="{ 'is-invalid': errors.email }"
                    />
                        <div class="invalid-feedback">{{ errors.email }}</div>
                  </div>
                </div>
                <div class="col-md-12">
                  <div class="mb-3 content-tuvan">
                    <Field
                        as="textarea"
                        v-model="model.content"
                        placeholder="Bạn đang cần tư vấn và đặt lịch khám?"
                        name="content"
                        type="text"
                        class="form-control"
                        :class="{ 'is-invalid': errors.content }"
                    />
                        <div class="invalid-feedback">{{ errors.content }}</div>
                    <!-- <textarea
                      class="form-control"
                      placeholder="Bạn đang cần tư vấn và đặt lịch khám?"
                      style="height: 100px;"
                    ></textarea> -->
                  </div>
                </div>
                <div class="col-md-12" style="display: flex; justify-content: center;">
                  <div class="card-tools">
                    <b-button type="submit" variant="success"
                        style="
                          background-color: #5b303a;
                          border: none;
                          padding: 12px 20px;
                          font-family: 'Roboto Bold';"
                    >
                      ĐẶT LỊCH HẸN
                    </b-button>
                  </div>
                </div>
              </div>
            </div>
          </div>

        </div>
      </Form>
    </FullScreenPopup>

    <scrolltotop />

  </div>
</template>
<script>
import AOS from "aos";
import "aos/dist/aos.css";
import FullScreenPopup from '@/components/popup/FullScreenPopup.vue';
import VueMultiselect from 'vue-multiselect'
import 'vue-multiselect/dist/vue-multiselect.css';
import {notifyModel} from "@/models/notifyModel";
import { Form, Field } from "vee-validate";
import * as Yup from "yup";
import { Modal } from 'bootstrap';
export default {
  data() {
    return {
      showHeaderSpace: false,
      showPopup: false,
      theModal: null,




    };
  },
  components: {
    FullScreenPopup,
    VueMultiselect,
    Form,
    Field,
  },
  mounted() {
    this.$nextTick(() => {
      AOS.init();
    });
    const schemaData = {
      "@context": "http://schema.org",
      "@type": "Product",
      "name": "ShopNTH - Hệ Thống Shop Cầu Lông",
      "description": "ShopNTH - Hệ Thống Shop Cầu Lông",
    };

    // Tạo thẻ <script> cho JSON-LD
    const script = document.createElement('script');
    script.type = 'application/ld+json';
    script.text = JSON.stringify(schemaData);
    document.head.appendChild(script);

  },
  beforeUnmount() {
    if (this.showHeaderSpace) {
      window.removeEventListener("scroll", this.handleScroll);
    }
  },
  created() {
    // this.getData();
  },
  methods: {
    handleScroll() {
      const scroll =
          window.pageYOffset ||
          document.documentElement.scrollTop ||
          document.body.scrollTop;
      this.showHeaderSpace = scroll > 35;
    },

    // async getData() {
    //   await this.$store.dispatch("pageInfoStore/getInfo").then(res => {
    //     if (res != null && res.code ===0)
    //     {
    //       this.pageInfo = pageInfoModel.getJson(res.data);
    //     }

    //   });
    // },

    async handleSubmit() {

      await this.$store.dispatch("tuVanStore/create", this.model).then((res) => {
        if (res != null && res.code ===0) {
          this.showPopup = false
        }
        this.$store.dispatch("snackBarStore/addNotify", notifyModel.addMessage(res));
      });
    },
  },
};


</script>

<style>
.title-popup{
  text-align: center;
  padding-bottom: 20px;
}
.title-popup p{
  color: #5b303a;
  font-family: 'Roboto Bold';
  font-size: 32px;
  margin-top: 5px;
  margin-bottom: 10px;
}

.title-popup span{
  color: #000;
  font-family: 'Be Regular';
}
.title-popup strong{
  font-family: 'Roboto Bold';
}

@media (max-width: 768px) {
  .popup-image{
    display: none;
  }
  .title-popup{
    padding-bottom: 5px;
  }
  .title-popup p{
    margin-bottom: 5px;
  }
}
.content-tuvan textarea {
  height: 150px !important;
}
</style>


