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
</template>
<script setup>
import { getCurrentInstance, onMounted, reactive, toRefs, watch } from "vue";
import VueDatePicker from '@vuepic/vue-datepicker';
import { diaChiModel } from "@/models/diaChiModel";
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
  itemsDiaChi: {
    customer: null,
    province: null,
    district: null,
    town: null
  }
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
  listTinh,
  listTP,
  listPhuong,
  listKH,
  itemsDiaChi
} = toRefs(state);
const schema = Yup.object().shape({});
async function getListTinh() {
  await proxy.$store.dispatch("tinhStore/getAll").then(res => {
    if (res != null && res.code === 0) {
      state.listTinh = res.data || [];
    }
  });
}
async function getListTP(id) {
  await proxy.$store.dispatch("huyenStore/getAll", {
    id: id
  }).then(res => {
    state.listTP = res.data || [];
  });
}
async function getListPhuong(id) {
  await proxy.$store.dispatch("phuongStore/getAll", {
    id: id
  }).then(res => {
    state.listPhuong = res.data || [];
  });
}
async function getListKH() {
  await proxy.$store.dispatch("khachHangStore/getAll").then(res => {
    state.listKH = res.data || [];
  });
}
async function getData() {
  let params = {
    start: state.currentPage,
    limit: state.perPage,
    sortBy: state.sortBy
  };
  await proxy.$store.dispatch("diaChiStore/getPagingParams", params).then(async res => {
    if (res != null && res.code === 0) {
      state.list = res.data.data;
      state.totalRows = res.data.totalRows;
      state.numberOfElement = res.data.data.length;
      for (let user of state.list) {
        // Lấy danh sách huyện theo tỉnh
        await getListTP(user.provinceId);
        // Gán huyện
        user.district = state.listTP.find(huyen => huyen.id === user.districtId) || null;

        // Lấy danh sách phường theo huyện
        await getListPhuong(user.districtId);
        // Gán phường
        user.town = state.listPhuong.find(phuong => phuong.id === user.townId) || null;

        // Gán tỉnh
        user.province = state.listTinh.find(tinh => tinh.id === user.provinceId) || null;
        // Gán khách hàng
        user.customer = state.listKH.find(kh => kh.id === user.customerId) || null;
      }
      console.log("LIST KHÁCH HÀNG: ", state.list);
    }
    proxy.$store.dispatch("snackBarStore/addNotify", notifyModel.addMessage(res));
  });
}
async function handleGetInfo(id) {
  const res = await proxy.$store.dispatch("diaChiStore/getById", {
    id: id
  });
  if (res != null && res.code === 0) {
    state.model = diaChiModel.getJson(res.data);

    // Gán tỉnh
    state.itemsDiaChi.province = state.listTinh.find(tinh => tinh.id === res.data.provinceId) || null;
    console.log("PROVINCE:", state.itemsDiaChi.province);
    if (state.itemsDiaChi.province) {
      await getListTP(state.itemsDiaChi.province.id); // Load danh sách huyện
      state.itemsDiaChi.district = state.listTP.find(tp => tp.id === res.data.districtId) || null;
      console.log("DISTRICT:", state.itemsDiaChi.district);
    }
    if (state.itemsDiaChi.district) {
      await getListPhuong(state.itemsDiaChi.district.id); // Load danh sách phường
      state.itemsDiaChi.town = state.listPhuong.find(p => p.id === res.data.townId) || null;
      console.log("TOWN:", state.itemsDiaChi.town);
    }

    // Gán khách hàng
    state.itemsDiaChi.customer = state.listKH.find(kh => kh.id === res.data.customerId) || null;
    console.log("CUSTOMER:", state.itemsDiaChi.customer);
  }
}
function handleShowDeleteModal(id) {
  state.model.id = id;
  proxy.showDeleteModal = true;
}
async function handleDelete() {
  if (state.model.id != 0 && state.model.id != null && state.model.id) {
    await proxy.$store.dispatch("diaChiStore/delete", {
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
async function handleSubmit() {
  state.model.provinceId = state.itemsDiaChi.province.id;
  state.model.districtId = state.itemsDiaChi.district.id;
  state.model.townId = state.itemsDiaChi.town.id;
  state.model.customerId = state.itemsDiaChi.customer.id;
  if (state.model.id != 0 && state.model.id != null && state.model.id) {
    await proxy.$store.dispatch("diaChiStore/update", state.model).then(res => {
      if (res != null && res.code === 0) {
        getData();
        state.model = diaChiModel.baseJson();
        state.theModal.hide();
      }
      proxy.$store.dispatch("snackBarStore/addNotify", notifyModel.addMessage(res));
    });
  } else {
    await proxy.$store.dispatch("diaChiStore/create", state.model).then(res => {
      if (res != null && res.code === 0) {
        getData();
        state.model = diaChiModel.baseJson();
        state.theModal.hide();
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
watch(() => state.itemsDiaChi.province, val => {
  if (val) {
    state.itemsDiaChi.district = null; // Xóa huyện khi đổi tỉnh
    state.itemsDiaChi.town = null; // Xóa phường khi đổi tỉnh
    getListTP(val.id);
  } else {
    state.listTP = [];
  }
}, {
  deep: true
});
watch(() => state.itemsDiaChi.district, val => {
  if (val) {
    state.itemsDiaChi.town = null; // Xóa phường khi đổi huyện
    getListPhuong(val.id);
  } else {
    state.listPhuong = [];
  }
}, {
  deep: true
});
onMounted(() => {
  state.theModal = new Modal(document.getElementById('info_modal'));
  proxy.$refs.ref_info_modal.addEventListener('hidden.bs.modal', event => {
    state.model = diaChiModel.baseJson();
  });
  proxy.$refs.ref_delete.addEventListener('hidden.bs.modal', event => {
    state.model = diaChiModel.baseJson();
  });
});
getListTinh();
getListKH();
getData();
</script>

