<template>
  <div class="main-Wrapper">
    <layoutheader :class="{ 'header-space': showHeaderSpace }" ref="header" />
    <!-- Page Wrapper -->
    <div class="page-wrapper" style="padding-top: 60px;">
      <div class="content container-fluid container">
        <div class="row">
          <div class="col-md-12">
            <div class="card">
              <div class="card-header">
                <Form
                  :validation-schema="schema" v-slot="{ errors }" @submit="handleSubmit"
                >
                  <div class="row">
                    <div class="col-md-6">
                      <div class="cs-title-box">
                        <span class="font-size-13">THÔNG TIN CÁ NHÂN</span>
                      </div>
                    </div>
                    <div class="tt-end mext-2 col-md-6" style="display: flex; justify-content: flex-end;">
                      <b-button
                          type="submit"
                          style="background-color: #e9ab2e; border: none;"
                      >
                        Lưu
                      </b-button>
                    </div>
                    <div class="col-12">
                        <div class="mb-3">
                        <label class="text-left">Họ và tên</label>
                        <span style="color: red">&nbsp;*</span>
                        <Field
                            v-model="model.fullName"
                            placeholder="Vui lòng nhập họ và tên"
                            name="fullName"
                            type="text"
                            class="form-control"
                            :class="{ 'is-invalid': errors.fullName }"
                        />
                        <div class="invalid-feedback">{{ errors.fullName }}</div>
                        </div>
                    </div>
                    <div class="col-12">
                        <div class="mb-3">
                        <label class="text-left">Email</label>
                        <span style="color: red">&nbsp;*</span>
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
                    <div class="col-12">
                        <div class="mb-3">
                        <label class="text-left">Số điện thoại</label>
                        <span style="color: red">&nbsp;*</span>
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
                    <div class="col-12">
                        <div class="mb-3">
                        <label class="text-left">Tài khoản</label>
                        <span style="color: red">&nbsp;*</span>
                        <Field
                            v-model="model.userName"
                            placeholder="Vui lòng nhập tài khoản"
                            name="userName"
                            type="text"
                            class="form-control"
                            :class="{ 'is-invalid': errors.userName }"
                        />
                        <div class="invalid-feedback">{{ errors.userName }}</div>
                        </div>
                    </div>
                    <div class="col-12">
                        <div class="mb-3">
                        <label class="text-left">Mật khẩu</label>
                        <span style="color: red">&nbsp;*</span>
                        <Field
                            v-model="model.password"
                            placeholder="Vui lòng nhập mật khẩu"
                            name="password"
                            type="text"
                            class="form-control"
                            :class="{ 'is-invalid': errors.password }"
                        />
                        <div class="invalid-feedback">{{ errors.password }}</div>
                        </div>
                    </div>
                  </div>
                </Form>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
  <pharmacymodel />
  <pharmacydelete />
</template>
<script >
import axios from 'axios';
import VueMultiselect from 'vue-multiselect'
import Loading from "vue3-loading-overlay";
import Paginate from "vuejs-paginate-next";
import 'vue-multiselect/dist/vue-multiselect.css';
import Treeselect from 'vue3-treeselect'
import {khachHangModel} from "@/models/khachHangModel";
import VueDatePicker from '@vuepic/vue-datepicker';
import '@vuepic/vue-datepicker/dist/main.css'
import {notifyModel} from "@/models/notifyModel";
import CKEditorCustom from "@/utils/view/CKEditorCustom.vue";
import {defineComponent ,ref } from '@vue/runtime-core';
import { Form, Field } from "vee-validate";
import * as Yup from "yup";
export default defineComponent ( {
  components: {
    Treeselect,
    loading: Loading,
    paginate: Paginate,
    VueMultiselect,
    VueDatePicker,
    CKEditorCustom,
    Form,
    Field,
  },
  data() {

    return {
      title: "TẠO SẢN PHẨM",
      treeView: [],
      listMenuMobi: [],
      listService: [],
      model: khachHangModel.baseJson(),
      urlFile:`${process.env.VUE_APP_API_URL}files/view`,
      url:`${process.env.VUE_APP_API_URL}files/view/`,
      format : `dd/MM/yyyy`,
      locale: 'vi',
    };
  },
  name: "pharmacy/user",

  created() {
    this.handleInfo();
  },

  watch: {

  },

  setup() {
      const schema = Yup.object().shape({
          
      });
      return {
          schema,
      };
  },

  methods: {
    getAuthHeaders() {
      const token = localStorage.getItem("token");
      return token ? { Authorization: `Bearer ${token}` } : {};
    },

    async handleInfo() {
        const authUser = JSON.parse(localStorage.getItem('auth-user'));
        const params = {
            id: authUser.id
        }
        await this.$store.dispatch("khachHangStore/getById", params).then((res) => {
        //  console.log("ID: ", res);
            if (res.code===0) {
                console.log(res)
                this.model = khachHangModel.getJson(res.data);
            } else {
            this.$store.dispatch("snackBarStore/addNotify", {
                message: res.message,
                code: res.code,
            });
            }
        });
    },
    addCoQuanToModel(node, instanceId ){
      if(node.id){
        this.model.menu = {id : node.id , name : node.name } ;
       }
    },
    normalizer(node){
        if(node.children == null || node.children == 'null'){
            delete node.children;
        }
    },


    async handleSubmit() {
        this.model.categoriesId = this.model.categories.id
      console.log("SUBMIT : ", );
      await this.$store.dispatch("khachHangStore/create", this.model).then((res) => {
        if (res != null && res.code ===0) {
            this.model= {}
            this.$router.push('/quan-tri/quan-ly-san-pham')
        }
        this.$store.dispatch("snackBarStore/addNotify", notifyModel.addMessage(res));
      });
    },

    getColorWithExtFile(ext) {
        if (ext == '.png' || ext == '.jpg'|| ext == '.jpeg' )
            return 'text-danger';

        },
    getIconWithExtFile(ext) {
        if (ext == '.png' || ext == '.jpg'|| ext == '.jpeg')
            return 'mdi mdi-file-image-outline';
    },

    deleteImage() {
        if (this.model != null && this.model.icon != null) {
            //console.log("LOG this.model : ", this.model)
            axios.post(`${process.env.VUE_APP_API_URL}file/delete/${this.model.icon.fileId}`, null, {
              headers: this.getAuthHeaders()
            }).then((response) => {
                this.model.icon = null;
                // console.log('log model file remove', this.model.icon);
            }).catch((error) => {
                // Handle error here
                //  console.error('Error deleting file:', error);
            });
        }
    },
    async upload() {
        if ( event.target &&  event.target.files.length > 0 ) {
        const formData = new FormData()
        // formData.append('code', "ICON")
        formData.append('files', event.target.files[0])
        axios.post(`${process.env.VUE_APP_API_URL}File/upload`, formData, {
          headers: this.getAuthHeaders()
        }).then((response) => {
            let resultData = response.data
            if (response.data.code == 0){
            this.model.imageUrl = resultData.data
            console.log("LOG UPDATE : ", resultData.data);
            }
        })
        }
    },

  }
});
</script>
<style>


</style>
