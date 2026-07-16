<template>
  <div class="col-md-12 col-lg-12 col-xl-12">
    <div class="row align-items-center pb-3">
      <div class="col-md-4 col-12 d-md-block d-none custom-short-by">
        <h3 class="title product-title">Sản phẩm</h3>
      </div>
      <div class="col-md-8 col-12">
        <div class="d-flex flex-wrap gap-2 justify-content-md-end">
          <CurrencyInput
            v-model="filterMinPrice"
            placeholder="Giá từ"
            class="form-control form-control-sm"
            style="max-width: 140px;"
          />
          <CurrencyInput
            v-model="filterMaxPrice"
            placeholder="Giá đến"
            class="form-control form-control-sm"
            style="max-width: 140px;"
          />
          <b-button size="sm" variant="primary" @click="applyPriceFilter">
            Lọc giá
          </b-button>
          <b-button size="sm" variant="outline-secondary" @click="resetPriceFilter">
            Xóa lọc
          </b-button>
        </div>
      </div>
      <!-- <div class="col-md-8 col-12 d-md-block d-none custom-short-by">
        <div class="sort-by pb-3">
          <span class="sort-title">Sort by</span>
          <span class="sortby-fliter">
            <vue-select :options="Select" placeholder="Select" />
          </span>
        </div>
      </div> -->
    </div>

    <div class="row">
      <div
        class="col-md-4 col-lg-3 col-xl-3 product-custom"
        v-for="item in list"
        :key="item.id"
      >
        <div class="profile-widget">
          <div class="doc-img">
            <router-link :to="{path: `/san-pham-chi-tiet/${item.id}`}" tabindex="-1">
              <img
                v-if="item.imageUrl"
                class="img-fluid"
                alt="Product image"
                :src="item.imageUrl"
              />
              <img
                v-else
                class="img-fluid"
                alt="Product image"
                src="@/assets/img/caulong/logo/logoNTH_removeBackground.png"
              />
            </router-link>
            <!-- <a href="javascript:void(0)" class="fav-btn" tabindex="-1">
              <i class="far fa-bookmark"></i>
            </a> -->
          </div>
          <div class="pro-content">
            <h3 class="title pb-2">
              <router-link :to="{path: `/san-pham-chi-tiet/${item.id}`}" tabindex="-1">
                {{ item.name }}
              </router-link>
            </h3>
            <div class="row align-items-center">
              <div class="col-lg-6">
                <span class="price">{{ formatCurrency(item.price) }}</span>
              </div>
              <div class="col-lg-6 text-end">
                <router-link to="" class="cart-icon"
                  ><i class="fas fa-shopping-cart"></i
                ></router-link>
              </div>
            </div>
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
</template>
<script setup>
import CurrencyInput from '@/components/common/CurrencyInput.vue';
import { computed, getCurrentInstance, reactive, toRefs, watch } from "vue";
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
  filterMinPrice: null,
  filterMaxPrice: null
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
  filterMinPrice,
  filterMaxPrice
} = toRefs(state);
function formatCurrency(value) {
  return value ? value.toLocaleString("vi-VN") + "đ" : "0đ";
}
async function getData() {
  let params = {
    start: state.currentPage,
    limit: state.perPage,
    sortBy: state.sortBy,
    idDonViCha: categoryId.value,
    minPrice: state.filterMinPrice || null,
    maxPrice: state.filterMaxPrice || null
  };
  await proxy.$store.dispatch("sanPhamStore/getPagingParamsById", params).then(res => {
    if (res != null && res.code === 0) {
      const items = res.data?.data || [];
      state.list = items;
      state.totalRows = res.data?.totalRows || 0;
      state.numberOfElement = items.length;
      // this.list = this.list.map(user => {
      //   return {
      //     ...user,
      //     categories: this.list.find(role => role.id === user.categoriesId) || null
      //   };
      // });
      console.log("LIST SAN PHAM: ", state.list);
    }
  });
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
function applyPriceFilter() {
  state.currentPage = 1;
  getData();
}
function resetPriceFilter() {
  state.filterMinPrice = null;
  state.filterMaxPrice = null;
  state.currentPage = 1;
  getData();
}
const categoryId = computed(() => {
  return proxy.$route.params.id || null;
});
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
watch(() => proxy.$route.params.id, newId => {
  state.currentPage = 1; // Reset về trang đầu tiên
  // Reset về trang đầu tiên
  getData();
}, {
  immediate: true
});
// this.getData();
</script>

