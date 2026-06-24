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
                    :validation-schema="schema" v-slot="{ errors }" @submit="handleSubmit" ref="form"
                  >
                  <div class="row">
                    <div class="tt-end mext-2 col-md-12" style="display: flex; justify-content: flex-end;">
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
  </template>
  <script setup>
import { getCurrentInstance, reactive, toRefs, watch } from "vue";
import VueMultiselect from 'vue-multiselect';
import Loading from "vue3-loading-overlay";
import Paginate from "vuejs-paginate-next";
import 'vue-multiselect/dist/vue-multiselect.css';
import SummernoteEditor from 'vue3-summernote-editor';
import Treeselect from 'vue3-treeselect';
import { sanPhamModel } from "@/models/sanPhamModel";
import '@vuepic/vue-datepicker/dist/main.css';
import ClassicEditor from "@/components/ckeditor5";
import { notifyModel } from "@/models/notifyModel";
import CKEditorCustom from "@/utils/view/CKEditorCustom.vue";
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
  treeView: [],
  listMenuMobi: [],
  listService: [],
  model: sanPhamModel.baseJson(),
  urlFile: `${process.env.VUE_APP_API_URL}files/view/`,
  // format : (date) => {
  //   const day = date.getDate();
  //   const month = date.getMonth() + 1;
  //   const year = date.getFullYear();
  //   return `${day}/${month}/${year}`;
  // },
  format: `dd/MM/yyyy`,
  locale: 'vi',
  editor: ClassicEditor,
  isFileOver: false,
  file: null,
  filePreview: null,
  keyId: null,
  listLoai: []
});
const {
  title,
  treeView,
  listMenuMobi,
  listService,
  model,
  urlFile,
  format,
  locale,
  editor,
  isFileOver,
  file,
  filePreview,
  keyId,
  listLoai
} = toRefs(state);
const schema = Yup.object().shape({
  name: Yup.string().required("Tên sản phẩm không được bỏ trống !")
});
function getAuthHeaders() {
  const token = localStorage.getItem("token");
  return token ? {
    Authorization: `Bearer ${token}`
  } : {};
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
  await proxy.$store.dispatch("sanPhamStore/update", state.model).then(res => {
    if (res != null && res.code === 0) {
      //  this.getData();
      proxy.$router.push('/quan-tri/quan-ly-san-pham');
    }
    proxy.$store.dispatch("snackBarStore/addNotify", notifyModel.addMessage(res));
  });
}
async function handleInfo() {
  const params = {
    id: proxy.$route.params.id
  };
  await proxy.$store.dispatch("sanPhamStore/getById", params).then(res => {
    //  console.log("ID: ", res);
    if (res.code === 0) {
      console.log(res);
      state.model = sanPhamModel.getJson(res.data);
      console.log("Danh sách loại sản phẩm:", state.listLoai);
      console.log("ID danh mục cần tìm:", res.data.categoriesId);
      state.model.categories = state.listLoai.find(cat => cat.id === res.data.categoriesId) || null;
      console.log("LIST SAN PHAM: ", state.model);
      // this.$refs.form.setFieldValue('fileImage', res.data.fileImage || null);
    } else {
      proxy.$store.dispatch("snackBarStore/addNotify", {
        message: res.message,
        code: res.code
      });
    }
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
async function upload() {
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
        console.log("LOG UPDATE : ", resultData.data);
      }
    });
  }
}
function addNodeToModel(node) {
  if (node != null && node.id) {
    // console.log("LOG ADD NODE TO MODEL : ", node);
    state.model.menu = {
      id: node.id,
      name: node.label
    };
  }
}
function normalizer(node) {
  if (node.children == null || node.children == 'null') {
    delete node.children;
  }
}
getListLoai();
handleInfo();
</script>

<style>

</style>

