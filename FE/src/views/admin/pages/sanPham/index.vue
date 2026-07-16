<template>
  <div class="main-Wrapper">
    <adminheader></adminheader>
    <adminsidebar></adminsidebar>
    <!-- Page Wrapper -->
    <div class="page-wrapper">
      <div class="content container-fluid">
        <div class="row">
          <div class="col-12">
            <div class="card">
              <div class="card-header">
                <h3 class="card-title">Danh sách sản phẩm</h3>
                <div class="top-nav-search">
                  <form @submit.prevent="handleSearch">
                    <input v-model.trim="searchText" type="text" class="form-control" placeholder="Tìm theo tên sản phẩm..." />
                    <b-button class="btn" type="submit"><i class="fa fa-search"></i></b-button>
                  </form>
                </div>
                <div class="card-tools">
                  <router-link
                      :to="{
                          path: `/quan-tri/quan-ly-san-pham/them-san-pham`,
                        }">
                    <button
                        type="button"
                        size="sm"
                        class="btn btn-tool card-add"
                    >
                    <i class="fas fa-plus"></i>
                    </button>
                  </router-link>
                  
                  <button type="button" class="btn btn-tool" @click="getData">
                    <i class="fas fa-retweet"></i>
                  </button>
                </div>
              </div>
              <div class="card-body">
                <div class="row">
                  <div class="col-12">
                    <div class="row mb-2">
                      <div class="col-sm-12 col-md-12">
                        <div
                            class="col-sm-12 d-flex justify-content-left align-items-center"
                        >
                          <div>
                            Hiển thị
                            <label class="d-inline-flex align-items-center" style="color: #F5E7B2;">
                              {{ list.length }}
                            </label>
                            trên tổng số <span style="color: red; font-weight: bold;">{{ totalRows }}</span> dòng
                          </div>
                        </div>
                      </div>
                    </div>
                    <div class="custom-new-table">
                      <div class="table-responsive">
                      <table class="table table-hover table-center mb-0">
                        <thead>
                        <th class="col150 cursor" style="text-align: center;">
                          STT
                        </th>
                        <th class="col150 cursor" style="text-align: left;">
                          Tên sản phẩm
                        </th>
                        <th class="col150 cursor" style="text-align: left;">
                          Hình ảnh
                        </th>
                        <th class="col100 cursor" style="text-align: left;">
                          Màu sắc
                        </th>
                        <th class="col100 cursor" style="text-align: left;">
                          Số lượng kho
                        </th>
                        <th class="col100 cursor" style="text-align: left;">
                          Giá
                        </th>
                        <th class="col100 cursor" style="text-align: center;">
                          Xử lý
                        </th>
                        </thead>
                        <tbody>
                        <tr v-for="(item, index) in list" :key="index">
                          <td style="text-align: center">
                            {{ index + ((currentPage-1)*perPage) + 1}}
                          </td>
                          <td style="text-align: left">
                            {{ item.name }}
                          </td>
                          <td style="text-align: left">
                            <div class="img-table">
                              <img :src="item.imageUrl" alt="">
                            </div>
                          </td>
                          <td style="text-align: left">
                            {{ item.color }}
                          </td>
                          <td style="text-align: left">
                            {{ item.stockQuantity }}
                          </td>
                          <td style="text-align: left">
                            {{ formatVnd(item.price) }}
                          </td>
                          <td style="text-align: center">
                            <router-link
                                :to="{
                                    path: `/quan-tri/quan-ly-san-pham/chi-tiet/${item.id}`,
                                  }">
                              <button
                                  type="button"
                                  size="sm"
                                  class="btn btn-outline btn-sm"
                              >
                                <i class="fas fa-pencil-alt text-success me-1"></i>
                              </button>
                            </router-link>
                            
                            <a
                                href="#delete_modal"
                                data-bs-toggle="modal"
                                class="btn btn-outline btn-sm"
                                v-on:click="handleShowDeleteModal(item.id)"
                            >
                              <i class="fas fa-trash-alt text-danger me-1"></i>
                            </a>
                          </td>
                        </tr>
                        </tbody>
                      </table>
                    </div>
                    </div>

                    
                    <div
                        class="modal fade "
                        id="delete_modal"
                        tabindex="-1"
                        role="dialog"
                        aria-hidden="true"
                        data-bs-backdrop="static"
                        ref="ref_delete"
                    >
                      <div class="modal-dialog modal-dialog-centered">
                        <div class="modal-content">
                          <div class="modal-header">
                            <h5 class="modal-title" id="acc_title">Xóa</h5>
                            <b-button
                                type="button"
                                class="btn-close"
                                data-bs-dismiss="modal"
                                aria-label="Close"
                            ></b-button>
                          </div>
                          <div class="modal-body" style="font-weight: 500;">
                            <p id="acc_msg">Dữ liệu được chọn sẽ được xóa vĩnh viễn. Bạn có chắc muốn xóa dữ liệu này?</p>
                          </div>
                          <div class="modal-footer">
                            <b-button class="btn btn-delete w-md si_accept_cancel" v-on:click="handleDelete" data-bs-dismiss="modal">
                              Xóa
                            </b-button>
                            <b-button
                                type="button"
                                class="btn si_accept_cancel btn-submit w-md btn-out"
                                data-bs-dismiss="modal"
                            >
                              Đóng
                            </b-button>
                          </div>
                        </div>
                      </div>
                    </div>
                    <div class="row" >
                      <div class="col-md-6 col-sm-6 mt-2">
                        <div>
                          Hiển thị
                          <label class="d-inline-flex align-items-center">
                            <b-form-select
                                class="form-select form-select-sm"
                                v-model="perPage"
                                size="sm"
                                :options="pageOptions"
                                style="width: 70px; text-align: center;"
                            ></b-form-select
                            >&nbsp;
                          </label>
                          trên tổng số {{ totalRows }} dòng
                        </div>
                      </div>
                      <div class="col-md-6 col-sm-6 mt-2" style="display: flex; justify-content: flex-end;">
                        <b-pagination
                            v-model="currentPage"
                            :total-rows="totalRows"
                            :per-page="perPage"
                            align="right"
                            size="sm"
                            class="my-0"
                        />
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>

      </div>
    </div>
  </div>
</template>
<script setup>
import { formatVnd } from '@/utils/currency';
import { getCurrentInstance, onMounted, reactive, toRefs, watch } from "vue";
import VueDatePicker from '@vuepic/vue-datepicker';
import { sanPhamModel } from "@/models/sanPhamModel";
import Treeselect from 'vue3-treeselect';
import VueMultiselect from 'vue-multiselect';
import 'vue-multiselect/dist/vue-multiselect.css';
import { Form, Field } from "vee-validate";
import * as Yup from "yup";
import { Modal } from 'bootstrap';
import { notifyModel } from "@/models/notifyModel";
defineOptions({
  name: "admin/page"
});
const {
  proxy
} = getCurrentInstance();
const state = reactive({
  title: "DANH SÁCH",
  model: sanPhamModel.baseJson(),
  currentPage: 1,
  numberOfElement: 1,
  perPage: 5,
  pageOptions: [5, 10, 25, 50, 100],
  totalRows: 1,
  sortBy: 'age',
  sortDesc: false,
  theModal: null,
  isView: false,
  list: [],
  listLoai: []
  ,searchText: ""
});
const {
  title,
  model,
  currentPage,
  numberOfElement,
  perPage,
  pageOptions,
  totalRows,
  sortBy,
  sortDesc,
  theModal,
  isView,
  list,
  listLoai
  ,searchText
} = toRefs(state);
async function getListLoai() {
  await proxy.$store.dispatch("loaiStore/getAll").then(res => {
    if (res != null && res.code === 0) {
      state.listLoai = res.data || [];
    }
  });
}
async function getData() {
  let params = {
    start: state.currentPage,
    limit: state.perPage,
    sortBy: state.sortBy,
    content: state.searchText || null
  };
  await proxy.$store.dispatch("sanPhamStore/getPagingParams", params).then(res => {
    if (res != null && res.code === 0) {
      const products = res.data?.data || [];
      state.list = products;
      state.totalRows = res.data?.totalRows || 0;
      state.numberOfElement = products.length;
      state.list = state.list.map(user => {
        return {
          ...user,
          categories: state.listLoai.find(role => role.id === user.categoriesId) || null
        };
      });
      console.log("LIST SAN PHAM: ", state.list);
    }
    proxy.$store.dispatch("snackBarStore/addNotify", notifyModel.addMessage(res));
  });
}
function handleSearch() {
  state.currentPage = 1;
  getData();
}
function handleShowDeleteModal(id) {
  state.model.id = id;
  proxy.showDeleteModal = true;
}
async function handleDelete() {
  if (state.model.id != 0 && state.model.id != null && state.model.id) {
    await proxy.$store.dispatch("sanPhamStore/delete", {
      'id': state.model.id
    }).then(res => {
      if (res != null && res.code === 0) {
        proxy.showDeleteModal = false;
        getData();
      }
      proxy.$store.dispatch("snackBarStore/addNotify", notifyModel.addMessage(res));
    });
  }
}
watch(() => state.perPage, val => {
  getData();
}, {
  deep: true
});
watch(() => state.currentPage, val => {
  getData();
}, {
  deep: true
});
onMounted(() => {});
getListLoai();
getData();
</script>

