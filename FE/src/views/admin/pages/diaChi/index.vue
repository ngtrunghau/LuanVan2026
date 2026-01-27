<template>
  <div class="main-Wrapper">
    <pharmacyheader></pharmacyheader>
    <pharmacysidebar></pharmacysidebar>
    <!-- Page Wrapper -->
    <div class="page-wrapper">
      <div class="content container-fluid">
        <div class="row">
          <div class="col-12">
            <div class="card">
              <div class="card-header">
                <h3 class="card-title">Danh sách dịa chỉ</h3>
                <div class="top-nav-search">
                  <form>
                    <input type="text" class="form-control" placeholder="Nhập nội dung..." />
                    <b-button class="btn" type="submit"><i class="fa fa-search"></i></b-button>
                  </form>
                </div>
                <div class="card-tools">
                  <a
                      href="#info_modal"
                      data-bs-toggle="modal"
                      size="sm"
                      type="button"
                      class="btn btn-tool card-add"
                  >
                    <i class="fas fa-plus"></i>
                  </a>
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
                              {{ this.list.length }}
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
                          Tên khách hàng
                        </th>
                        <th class="col100 cursor" style="text-align: left;">
                          Địa chỉ
                        </th>
                        <th class="col100 cursor" style="text-align: left;">
                          Phường, xã
                        </th>
                        <th class="col100 cursor" style="text-align: left;">
                          Thành phố, Quận
                        </th>
                        <th class="col100 cursor" style="text-align: left;">
                          Tỉnh
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
                            {{ item.customer?.fullName }}
                          </td>
                          <td style="text-align: left">
                            {{ item.address }}
                          </td>
                          <td style="text-align: left">
                            {{ item.town?.name }}
                          </td>
                          <td style="text-align: left">
                            {{ item.district?.name }}
                          </td>
                          <td style="text-align: left">
                            {{ item.province?.name }}
                          </td>
                          <td style="text-align: center">
                            <a
                                href="#info_modal"
                                data-bs-toggle="modal"
                                size="sm"
                                type="button"
                                class="btn btn-outline btn-sm"
                                v-on:click="handleGetInfo(item.id)"
                            >
                              <i class="fas fa-pencil-alt text-success me-1"></i>
                            </a>
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
                        class="modal fade"
                        id="info_modal"
                        aria-hidden="true"
                        role="dialog"
                        data-bs-backdrop="static"
                        ref="ref_info_modal"
                    >
                      <div class="modal-dialog modal-lg modal-dialog-centered" role="document">
                        <div class="modal-content">
                          <div class="modal-header">
                            <h5 class="modal-title">Thông tin khách hàng</h5>
                            <b-button
                                type="button"
                                class="btn-close"
                                data-bs-dismiss="modal"
                                aria-label="Close"
                            ></b-button>
                          </div>
                          <div class="modal-body">
                            <Form
                                class="login"
                                @submit="handleSubmit"
                                :validation-schema="schema"
                                v-slot="{ errors }"
                                ref="form"
                            >
                              <div class="row">
                                <div class="col-12">
                                  <div class="mb-3">
                                    <label class="text-left">Khách hàng</label>
                                    <span style="color: red">&nbsp;*</span>
                                    <Field
                                        name="customer"
                                        v-slot="{ field}"
                                    >
                                      <VueMultiselect
                                          v-bind="field"
                                          v-model="itemsDiaChi.customer"
                                          :options="listKH"
                                          label="fullName"
                                          placeholder="Nhấp vào để chọn"
                                          selectLabel="Nhấn vào để chọn"
                                          deselectLabel="Nhấn vào để xóa"
                                          track-by="id"
                                          :class="{ 'is-invalid': errors.customer }"
                                      >
                                      </VueMultiselect>
                                      <div class="invalid-feedback">{{ errors.customer}}</div>
                                    </Field>
                                  </div>
                                </div>
                                <div class="col-12">
                                  <div class="mb-3">
                                    <label class="text-left">Địa chỉ</label>
                                    <span style="color: red">&nbsp;*</span>
                                    <Field
                                        v-model="model.address"
                                        placeholder="Vui lòng nhập địa chỉ"
                                        name="address"
                                        type="text"
                                        class="form-control"
                                        :class="{ 'is-invalid': errors.address }"
                                    />
                                    <div class="invalid-feedback">{{ errors.address }}</div>
                                  </div>
                                </div>
                                <div class="col-12">
                                  <div class="mb-3">
                                    <label class="text-left">Tỉnh</label>
                                    <span style="color: red">&nbsp;*</span>
                                    <Field
                                        name="province"
                                        v-slot="{ field}"
                                    >
                                      <VueMultiselect
                                          v-bind="field"
                                          v-model="itemsDiaChi.province"
                                          :options="listTinh"
                                          label="name"
                                          placeholder="Nhấp vào để chọn"
                                          selectLabel="Nhấn vào để chọn"
                                          deselectLabel="Nhấn vào để xóa"
                                          track-by="id"
                                          :class="{ 'is-invalid': errors.province }"
                                      >
                                      </VueMultiselect>
                                      <div class="invalid-feedback">{{ errors.province}}</div>
                                    </Field>
                                  </div>
                                </div>
                                <div class="col-12">
                                  <div class="mb-3">
                                    <label class="text-left">Thành phố/Quận/Huyện</label>
                                    <span style="color: red">&nbsp;*</span>
                                    <Field
                                        name="district"
                                        v-slot="{ field}"
                                    >
                                      <VueMultiselect
                                          v-bind="field"
                                          v-model="itemsDiaChi.district"
                                          :options="listTP"
                                          :disabled="!itemsDiaChi.province"
                                          label="name"
                                          placeholder="Nhấp vào để chọn"
                                          selectLabel="Nhấn vào để chọn"
                                          deselectLabel="Nhấn vào để xóa"
                                          track-by="id"
                                          :class="{ 'is-invalid': errors.district }"
                                      >
                                      </VueMultiselect>
                                      <div class="invalid-feedback">{{ errors.district}}</div>
                                    </Field>
                                  </div>
                                </div>
                                <div class="col-12">
                                  <div class="mb-3">
                                    <label class="text-left">Phường</label>
                                    <span style="color: red">&nbsp;*</span>
                                    <Field
                                        name="town"
                                        v-slot="{ field}"
                                    >
                                      <VueMultiselect
                                          v-bind="field"
                                          v-model="itemsDiaChi.town"
                                          :options="listPhuong"
                                          :disabled="!itemsDiaChi.district"
                                          label="name"
                                          placeholder="Nhấp vào để chọn"
                                          selectLabel="Nhấn vào để chọn"
                                          deselectLabel="Nhấn vào để xóa"
                                          track-by="id"
                                          :class="{ 'is-invalid': errors.town }"
                                      >
                                      </VueMultiselect>
                                      <div class="invalid-feedback">{{ errors.town}}</div>
                                    </Field>
                                  </div>
                                </div>



                              </div>
                              <div class="text-end pt-2 mt-3">
                                <b-button
                                    type="button"
                                    class="btn si_accept_cancel btn-submit w-md btn-out"
                                    data-bs-dismiss="modal"
                                >
                                  Đóng
                                </b-button>
                                <b-button  type="submit" variant="success" class="btn-submit w-md ms-1 cs-btn-primary">
                                  Lưu
                                </b-button>
                              </div>
                            </Form>
                          </div>
                        </div>
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
  <pharmacymodel />
  <pharmacydelete />
</template>
<script >

import VueDatePicker from '@vuepic/vue-datepicker';
import { diaChiModel } from "@/models/diaChiModel";
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
      model: diaChiModel.baseJson(),
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
      listTinh: [],
      listTP: [],
      listPhuong: [],
      listKH: [],
      itemsDiaChi:{
        customer: null,
        province: null,
        district: null,
        town: null,
      },
    };
  },
  name: "pharmacy/user",

  created() {
    this.getListTinh();

    this.getListKH();
    this.getData();
  },
  mounted() {
    this.theModal = new Modal(document.getElementById('info_modal'));



    this.$refs.ref_info_modal.addEventListener('hidden.bs.modal', event => {
      this.model = diaChiModel.baseJson()
    });
    this.$refs.ref_delete.addEventListener('hidden.bs.modal', event => {
      this.model = diaChiModel.baseJson()
    });
  },
  setup() {
    const schema = Yup.object().shape({
      
    });
    return {
      schema
    };
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
    },
    'itemsDiaChi.province': {
        handler(val) {
            if (val) {
              this.itemsDiaChi.district = null; // Xóa huyện khi đổi tỉnh
              this.itemsDiaChi.town = null; // Xóa phường khi đổi tỉnh
                this.getListTP(val.id);
            } else {
                this.listTP = [];
            }
        },
        deep: true
    },
    'itemsDiaChi.district': {
        handler(val) {
            if (val) {
                this.itemsDiaChi.town = null; // Xóa phường khi đổi huyện
                this.getListPhuong(val.id);
            } else {
                this.listPhuong = [];
            }
        },
        deep: true
    },
  },

  methods: {
    async getListTinh(){
      await  this.$store.dispatch("tinhStore/getAll").then((res) =>{
            if (res != null && res.code ===0) {
              this.listTinh = res.data || [];
            }
      })
    },
    async getListTP(id){
        await  this.$store.dispatch("huyenStore/getAll", {id: id}).then((res) =>{
            this.listTP = res.data || [];
        })
    },
    async getListPhuong(id){
        await  this.$store.dispatch("phuongStore/getAll", {id: id}).then((res) =>{
            this.listPhuong = res.data || [];
        })
    },
    async getListKH(){
        await  this.$store.dispatch("khachHangStore/getAll").then((res) =>{
            this.listKH = res.data || [];
        })
    },
    async getData() {
      let params = {
        start: this.currentPage,
        limit: this.perPage,
        sortBy: this.sortBy,
      };

      await this.$store.dispatch("diaChiStore/getPagingParams", params).then(async (res) => {
        if (res != null && res.code === 0) {
          this.list = res.data.data;
          this.totalRows = res.data.totalRows;
          this.numberOfElement = res.data.data.length;

          for (let user of this.list) {
            // Lấy danh sách huyện theo tỉnh
            await this.getListTP(user.provinceId);
            // Gán huyện
            user.district = this.listTP.find(huyen => huyen.id === user.districtId) || null;

            // Lấy danh sách phường theo huyện
            await this.getListPhuong(user.districtId);
            // Gán phường
            user.town = this.listPhuong.find(phuong => phuong.id === user.townId) || null;

            // Gán tỉnh
            user.province = this.listTinh.find(tinh => tinh.id === user.provinceId) || null;
            // Gán khách hàng
            user.customer = this.listKH.find(kh => kh.id === user.customerId) || null;
          }

          console.log("LIST KHÁCH HÀNG: ", this.list);
        }
        this.$store.dispatch("snackBarStore/addNotify", notifyModel.addMessage(res));
      });
    },
    async handleGetInfo(id) {
      const res = await this.$store.dispatch("diaChiStore/getById", { id: id });

      if (res != null && res.code === 0) {
        this.model = diaChiModel.getJson(res.data);

        // Gán tỉnh
        this.itemsDiaChi.province = this.listTinh.find(tinh => tinh.id === res.data.provinceId) || null;
        console.log("PROVINCE:", this.itemsDiaChi.province);

        if (this.itemsDiaChi.province) {
          await this.getListTP(this.itemsDiaChi.province.id); // Load danh sách huyện
          this.itemsDiaChi.district = this.listTP.find(tp => tp.id === res.data.districtId) || null;
          console.log("DISTRICT:", this.itemsDiaChi.district);
        }

        if (this.itemsDiaChi.district) {
          await this.getListPhuong(this.itemsDiaChi.district.id); // Load danh sách phường
          this.itemsDiaChi.town = this.listPhuong.find(p => p.id === res.data.townId) || null;
          console.log("TOWN:", this.itemsDiaChi.town);
        }

        // Gán khách hàng
        this.itemsDiaChi.customer = this.listKH.find(kh => kh.id === res.data.customerId) || null;
        console.log("CUSTOMER:", this.itemsDiaChi.customer);
      }
    },

    handleShowDeleteModal(id) {
      this.model.id = id;
      this.showDeleteModal = true;
    },
    async handleDelete() {
      if (this.model.id != 0 && this.model.id != null && this.model.id) {
        await this.$store.dispatch("diaChiStore/delete", { 'id': this.model.id }).then((res) => {
          if (res != null && res.code ===0) {
            this.showDeleteModal = false;
            this.getData();
          }
          this.$store.dispatch("snackBarStore/addNotify", notifyModel.addMessage(res));
        });
      }
    },
    async handleSubmit() {
      this.model.provinceId = this.itemsDiaChi.province.id
      this.model.districtId = this.itemsDiaChi.district.id
      this.model.townId = this.itemsDiaChi.town.id
      this.model.customerId = this.itemsDiaChi.customer.id

      if (
          this.model.id != 0 &&
          this.model.id != null &&
          this.model.id
      ) {
        await this.$store.dispatch("diaChiStore/update", this.model).then((res) => {
          if (res != null && res.code ===0) {
            this.getData();
            this.model= diaChiModel.baseJson()
            this.theModal.hide();
          }
          this.$store.dispatch("snackBarStore/addNotify", notifyModel.addMessage(res));
        });
      } else {
        await this.$store.dispatch("diaChiStore/create", this.model).then((res) => {
          if (res != null && res.code ===0) {
            this.getData();
            this.model= diaChiModel.baseJson()
            this.theModal.hide();
          }
          this.$store.dispatch("snackBarStore/addNotify", notifyModel.addMessage(res));
        });

      }

    },

  }
};
</script>

