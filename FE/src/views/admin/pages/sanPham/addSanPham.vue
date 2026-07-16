<template>
  <div class="main-Wrapper">
    <adminheader></adminheader>
    <adminsidebar></adminsidebar>
    <!-- Page Wrapper -->
    <div class="page-wrapper">
      <div class="content container-fluid">
        <adminbreadcrumb2 :title="title" />
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
                        <span class="font-size-13">THÔNG TIN SẢN PHẨM</span>
                      </div>
                    </div>
                    <div class="tt-end mext-2 col-md-6" style="display: flex; justify-content: flex-end;">
                      <b-button
                          type="submit"
                          style="background-color: #e9ab2e; border: none;"
                      >
                        Đăng sản phẩm
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
                            <Field name="imageUrl" v-slot="{ field }">
                              <input
                                id="formFileSm" ref="fileInput" type="file" class="form-control"
                                accept="image/png,image/jpeg,image/jpg"
                                @change="upload($event, field)"
                                :class="{ 'is-invalid': errors.imageUrl }" 
                              />
                            </Field>
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
                            name="categories"
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
                        <Field name="price" v-slot="{ field }">
                          <CurrencyInput
                              v-model="model.price"
                              placeholder="Vui lòng nhập giá"
                              class="form-control"
                              :class="{ 'is-invalid': errors.price }"
                              @update:model-value="field.onChange"
                              @blur="field.onBlur"
                          />
                        </Field>
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
</template>
<script setup>
import { getCurrentInstance, reactive, toRefs, watch } from "vue";
import axios from 'axios';
import VueMultiselect from 'vue-multiselect';
import Loading from "vue3-loading-overlay";
import Paginate from "vuejs-paginate-next";
import 'vue-multiselect/dist/vue-multiselect.css';
import Treeselect from 'vue3-treeselect';
import { sanPhamModel } from "@/models/sanPhamModel";
import VueDatePicker from '@vuepic/vue-datepicker';
import '@vuepic/vue-datepicker/dist/main.css';
import { notifyModel } from "@/models/notifyModel";
import CKEditorCustom from "@/utils/view/CKEditorCustom.vue";
import { defineComponent, ref } from '@vue/runtime-core';
import { Form, Field } from "vee-validate";
import * as Yup from "yup";
import CurrencyInput from "@/components/common/CurrencyInput.vue";
defineOptions({
  name: "admin/page"
});
const {
  proxy
} = getCurrentInstance();
const state = reactive({
  title: "TẠO SẢN PHẨM",
  treeView: [],
  listMenuMobi: [],
  listService: [],
  model: sanPhamModel.baseJson(),
  urlFile: `${process.env.VUE_APP_API_URL}files/view`,
  url: `${process.env.VUE_APP_API_URL}files/view/`,
  format: `dd/MM/yyyy`,
  locale: 'vi',
  listLoai: []
});
const {
  title,
  treeView,
  listMenuMobi,
  listService,
  model,
  urlFile,
  url,
  format,
  locale,
  listLoai
} = toRefs(state);
const schema = Yup.object().shape({
  name: Yup.string().trim().required("Tên sản phẩm không được bỏ trống !"),
  imageUrl: Yup.string().required("Hình ảnh không được bỏ trống !"),
  categories: Yup.object().nullable().required("Loại sản phẩm không được bỏ trống !"),
  color: Yup.string().trim().required("Màu sắc không được bỏ trống !"),
  price: Yup.number()
    .typeError("Giá sản phẩm không hợp lệ !")
    .positive("Giá sản phẩm phải lớn hơn 0 !")
    .required("Giá sản phẩm không được bỏ trống !"),
  descriptions: Yup.string().test(
    "required-html",
    "Nội dung bài viết không được bỏ trống !",
    value => Boolean(value?.replace(/<[^>]*>/g, '').replace(/&nbsp;/g, ' ').trim())
  )
});
function getAuthHeaders() {
  const token = localStorage.getItem("token");
  return token ? {
    Authorization: `Bearer ${token}`
  } : {};
}
function addCoQuanToModel(node, instanceId) {
  if (node.id) {
    state.model.menu = {
      id: node.id,
      name: node.name
    };
  }
}
function normalizer(node) {
  if (node.children == null || node.children == 'null') {
    delete node.children;
  }
}
async function getListLoai() {
  await proxy.$store.dispatch("loaiStore/getAll").then(res => {
    if (res != null && res.code === 0) {
      state.listLoai = res.data || [];
    }
  });
}
async function handleSubmit() {
  state.model.categoriesId = state.model.categories.id;
  console.log("SUBMIT : ");
  await proxy.$store.dispatch("sanPhamStore/create", state.model).then(res => {
    if (res != null && res.code === 0) {
      state.model = {};
      proxy.$router.push('/quan-tri/quan-ly-san-pham');
    }
    proxy.$store.dispatch("snackBarStore/addNotify", notifyModel.addMessage(res));
  });
}
function getColorWithExtFile(ext) {
  if (ext == '.png' || ext == '.jpg' || ext == '.jpeg') return 'text-danger';
}
function getIconWithExtFile(ext) {
  if (ext == '.png' || ext == '.jpg' || ext == '.jpeg') return 'mdi mdi-file-image-outline';
}
function deleteImage() {
  if (state.model != null && state.model.icon != null) {
    //console.log("LOG this.model : ", this.model)
    axios.post(`${process.env.VUE_APP_API_URL}file/delete/${state.model.icon.fileId}`, null, {
      headers: getAuthHeaders()
    }).then(response => {
      state.model.icon = null;
      // console.log('log model file remove', this.model.icon);
    }).catch(error => {
      // Handle error here
      //  console.error('Error deleting file:', error);
    });
  }
}
async function upload(event, field) {
  if (event.target && event.target.files.length > 0) {
    const formData = new FormData();
    // formData.append('code', "ICON")
    formData.append('files', event.target.files[0]);
    axios.post(`${process.env.VUE_APP_API_URL}File/upload`, formData, {
      headers: getAuthHeaders()
    }).then(response => {
      let resultData = response.data;
      if (response.data.code == 0) {
        state.model.imageUrl = resultData.data;
        field.onChange(resultData.data);
        console.log("LOG UPDATE : ", resultData.data);
      }
    });
  }
}
getListLoai();
</script>
<style>


</style>
