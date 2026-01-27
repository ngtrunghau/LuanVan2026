<template>
  <div class="main-Wrapper">
    <pharmacyheader></pharmacyheader>
    <pharmacysidebar></pharmacysidebar>
    <!-- Page Wrapper -->
    <div class="page-wrapper">
      <div class="content container-fluid">
        <pharmacybreadcrumb2 :title="title" />
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
                        <span class="font-size-13">THÔNG TIN BÀI VIẾT</span>
                      </div>
                    </div>
                    <div class="tt-end mext-2 col-md-6" style="display: flex; justify-content: flex-end;">
                      <b-button
                          type="submit"
                          style="background-color: #e9ab2e; border: none;"
                      >
                        Đăng bài viết
                      </b-button>
                    </div>
                    <div class="col-12">
                        <div class="mb-3">
                        <label class="text-left">Tên sản phẩm</label>
                        <span style="color: red">&nbsp;*</span>
                        <Field
                            v-model="model.name"
                            placeholder="Vui lòng nhập tên sản phẩm"
                            name="name"
                            type="text"
                            class="form-control"
                            :class="{ 'is-invalid': errors.name }"
                        />
                        <div class="invalid-feedback">{{ errors.name }}</div>
                        </div>
                    </div>
                    <div class="col-12">
                        <div class="mb-2">
                            <label for="formFileSm" class="text-left mb-0">Hình ảnh</label>
                            <span style="color: red">&nbsp;*</span>
                            <Field 
                                id="formFileSm" name="fileImage"
                                ref="fileInput" type="file" class="form-control"
                                @change="upload($event)"
                                :class="{ 'is-invalid': errors.imageUrl }" 
                            />
                            <template v-if="model.imageUrl">
                            <div class="img-model">
                                <img :src="model.imageUrl" alt="">
                            </div>
                            </template>
                            <div class="invalid-feedback">{{ errors.imageUrl }}
                            </div>
                        </div>
                    </div>
                    <div class="col-12">
                        <div class="mb-3">
                        <label class="text-left">Loại</label>
                        <span style="color: red">&nbsp;*</span>
                        <Field
                            name="unitRole"
                            v-slot="{ field}"
                        >
                            <VueMultiselect
                                v-bind="field"
                                v-model="model.categories"
                                :options="listLoai"
                                label="name"
                                placeholder="Nhấp vào để chọn"
                                selectLabel="Nhấn vào để chọn"
                                deselectLabel="Nhấn vào để xóa"
                                track-by="id"
                                :class="{ 'is-invalid': errors.categories }"
                            >
                            </VueMultiselect>
                            <div class="invalid-feedback">{{ errors.categories}}</div>
                        </Field>


                        </div>
                    </div>
                    <div class="col-12">
                        <div class="mb-3">
                        <label class="text-left">Màu sắc</label>
                        <span style="color: red">&nbsp;*</span>
                        <Field
                            v-model="model.color"
                            placeholder="Vui lòng nhập màu sắc"
                            name="color"
                            type="text"
                            class="form-control"
                            :class="{ 'is-invalid': errors.color }"
                        />
                        <div class="invalid-feedback">{{ errors.color }}</div>
                        </div>
                    </div>
                    <div class="col-12">
                        <div class="mb-3">
                        <label class="text-left">Giá</label>
                        <span style="color: red">&nbsp;*</span>
                        <Field
                            v-model="model.price"
                            placeholder="Vui lòng nhập giá"
                            name="price"
                            type="text"
                            class="form-control"
                            :class="{ 'is-invalid': errors.price }"
                        />
                        <div class="invalid-feedback">{{ errors.price }}</div>
                        </div>
                    </div>
                    <div class="col-md-12">
                        <div class="mb-3">
                        <label class="text-left">Nội dung bài viết</label>
                        <span style="color: red" >&nbsp;*</span>
                        <Field name="descriptions" v-slot="{ field }">
                            <CKEditorCustom
                            v-bind="field"
                            v-model="model.descriptions"
                            :class="{ 'is-invalid': errors.descriptions }"
                            >
                            </CKEditorCustom>
                        </Field>
                        <div class="invalid-feedback">{{ errors.descriptions }}</div>
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
import {sanPhamModel} from "@/models/sanPhamModel";
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
      model: sanPhamModel.baseJson(),
      urlFile:`${process.env.VUE_APP_API_URL}files/view`,
      url:`${process.env.VUE_APP_API_URL}files/view/`,
      format : `dd/MM/yyyy`,
      locale: 'vi',
      listLoai: [],
    };
  },
  name: "pharmacy/user",

  created() {
    this.getListLoai();

  },

  watch: {

  },

  setup() {
      const schema = Yup.object().shape({
          name: Yup.string().required("Tên không được bỏ trống !"),
          
      });
      return {
          schema,
      };
  },

  methods: {
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
    async getListLoai(){
        await  this.$store.dispatch("loaiStore/getAll").then((res) =>{
                if (res != null && res.code ===0) {
                this.listLoai = res.data || [];
                }
        })
    },


    async handleSubmit() {
        this.model.categoriesId = this.model.categories.id
      console.log("SUBMIT : ", );
      await this.$store.dispatch("sanPhamStore/create", this.model).then((res) => {
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
            axios.post(`${process.env.VUE_APP_API_URL}file/delete/${this.model.icon.fileId}`).then((response) => {
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
        axios.post(`${process.env.VUE_APP_API_URL}File/upload`,formData).then((response) => {
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
