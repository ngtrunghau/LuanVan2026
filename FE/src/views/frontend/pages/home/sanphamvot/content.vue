<template>
  <div class="col-md-7 col-lg-9 col-xl-9">
    <div class="row align-items-center pb-3">
      <div class="col-md-4 col-12 d-md-block d-none custom-short-by">
        <h3 class="title pharmacy-title">Sản phẩm</h3>
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
        class="col-md-12 col-lg-4 col-xl-4 product-custom"
        v-for="item in this.list.filter(item => item.categoriesId == 10)"
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
            <h3 class="title pb-4">
              <router-link :to="{path: `/san-pham-chi-tiet/${item.id}`}" tabindex="-1">
                {{ item.name }}
              </router-link>
            </h3>
            <div class="row align-items-center">
              <div class="col-lg-6">
                <span class="price">{{ item.price }}</span>
              </div>
              <div class="col-lg-6 text-end">
                <router-link to="cart" class="cart-icon"
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
<script >
import productall from "@/assets/json/pharmacy/productall.json";
import VueDatePicker from '@vuepic/vue-datepicker';
import { sanPhamModel } from "@/models/sanPhamModel";
import Treeselect from 'vue3-treeselect';
import VueMultiselect from 'vue-multiselect'
import 'vue-multiselect/dist/vue-multiselect.css';
import { Form, Field } from "vee-validate";
import * as Yup from "yup";
import { Modal } from 'bootstrap';
import {notifyModel} from "@/models/notifyModel";

export default {
  components: {
    VueDatePicker,
    Treeselect,
    VueMultiselect,
    Form,
    Field,
  },
  data() {
    return {
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
      listLoai: [],
      productall: productall,
    };
  },
  name: "pharmacy/user",

  created() {
    this.getListLoai();
    this.getData();
  },
  mounted() {
    
  },
  
  watch: {
    perPage: {
      deep: true,
      handler(val){
        this.getData();
      }
    },
    currentPage: {
      deep: true,
      handler(val){
        this.getData();
      }
    }
  },

  methods: {
    async getListLoai(){
      await  this.$store.dispatch("loaiStore/getAllCustomer").then((res) =>{
            if (res != null && res.code ===0) {
              this.listLoai = res.data || [];
            }
      })
    },
    async getData() {
      let params = {
        start: this.currentPage,
        limit: this.perPage,
        sortBy: this.sortBy,
      }
      await this.$store.dispatch("sanPhamStore/getPagingParams", params ).then(res => {
            if (res != null && res.code ===0) {
              this.list = res.data.data
              this.totalRows = res.data.totalRows
              this.numberOfElement = res.data.data.length
              this.list = this.list.map(user => {
                return {
                  ...user,
                  categories: this.list.find(role => role.id === user.categoriesId) || null
                };
              });
              console.log("LIST SAN PHAM: ", this.list);
            }
      });
    },
    
    handleShowDeleteModal(id) {
      this.model.id = id;
      this.showDeleteModal = true;
    },
    async handleDelete() {
      if (this.model.id != 0 && this.model.id != null && this.model.id) {
        await this.$store.dispatch("sanPhamStore/delete", { 'id': this.model.id }).then((res) => {
          if (res != null && res.code ===0) {
            this.showDeleteModal = false;
            this.getData();
          }
          this.$store.dispatch("snackBarStore/addNotify", notifyModel.addMessage(res));
        });
      }
    },
    

  }
};
</script>


