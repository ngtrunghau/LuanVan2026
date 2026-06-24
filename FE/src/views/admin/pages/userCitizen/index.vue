<template>
  <div class="main-Wrapper">
    <adminheader></adminheader>
    <adminsidebar></adminsidebar>
    <!-- Page Wrapper -->
    <div class="page-wrapper">
      <div class="content container-fluid">
        <adminbreadcrumb2 :title="title" />
        <div class="row">
          <div class="col-12">
            <div class="card">
              <div class="card-body">
                <div class="row">
                  <div class="mb-3 col-md-12">
                    <label>Nội dung</label>
                    <input
                        v-model="itemFilter.userName"
                        type="text"
                        name="untyped-input"
                        class="form-control"
                        placeholder="Tìm kiếm theo tên tài khoản"
                        style="height: 39px;"
                    />
                  </div>
                  <!-- <div class="mb-3 col-md-6 multi-role">
                    <label>Quyền</label>
                    <VueMultiselect
                        v-model="itemFilter.unitRole"
                        :options="listRole"
                        label="name"
                        placeholder="Nhấp vào để chọn"
                        selectLabel="Nhấn vào để chọn"
                        deselectLabel="Nhấn vào để xóa"
                      >
                    </VueMultiselect>
                  </div> -->
                </div>
                <div class="row">
                  <div class="col-12 text-center">
                    <b-button variant=""
                              class="w-10 btn-search"
                              style="margin-right: 10px ; height: 40px ; width: 130px; font-size: 14px; background-color: #F5E7B2; border: none; color: #000 !important; font-weight: bold;"
                              size="sm"
                              @click="handleSearch"
                    >
                      <i class="fa fa-search font-size-16 align-middle me-2" aria-hidden="true"></i>
                      Tìm kiếm
                    </b-button>
                    <b-button
                        class="w-10 btn-reset"
                        style="margin-right: 10px; height: 40px ; width: 130px; font-size: 14px; background-color: #F5E7B2; border: none; color: #000 !important; font-weight: bold;"
                        size="sm"
                        @click="handleClear"
                    >
                      <i class="fa fa-refresh font-size-16 align-middle me-2" aria-hidden="true"></i>
                      Làm mới
                    </b-button>
                  </div>
                </div>
              </div>
            </div>
            <div class="card">
              <div class="card-body">
                <div class="row">
                  <div class="col-12">
                    <div class="row mb-2">
                      <div class="col-sm-8 col-md-8 d-flex justify-content-left align-items-center">
                        <div
                            class="col-sm-12 d-flex justify-content-left align-items-center"
                        >
                        <div>
                          Hiển thị
                          <label class="d-inline-flex align-items-center" style="color: #F5E7B2;">
                            {{ this.listUser.length }}
                          </label>
                          trên tổng số <span style="color: red; font-weight: bold;">{{ totalRows }}</span> dòng
                        </div>
                        </div>
                      </div>
                      <div class="col-md-4" style="display: flex; justify-content: flex-end; align-items: center;">
                        <div class="card-tools">
                          <a
                              href="#info_user"
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
                    </div>
                    <div class="custom-new-table">
                      <div class="table-responsive">
                      <table class="table table-hover table-center mb-0">
                        <thead>
                        <th class="col150 cursor td-stt" style="text-align: center;">
                          STT
                        </th>
                        <th class="col150 cursor" style="text-align: center;">
                          Tài khoản
                        </th>
                        <th class="col100 cursor" style="text-align: center;">
                          Họ và tên
                        </th>
                        <th class="col150 cursor" style="text-align: center;">
                          CCCD
                        </th>
                        <th class="col150 cursor" style="text-align: center;">
                          Số điện thoại
                        </th>
                        <th class="col100 cursor td-xuly-user" style="text-align: center;">
                          Xử lý
                        </th>
                        </thead>
                        <tbody>
                        <tr v-for="(item, index) in listUser" :key="index">
                          <td style="text-align: center">
                            {{ index + ((currentPage-1)*perPage) + 1}}
                          </td>
                          <td style="text-align: left">
                            {{ item.userName }}
                          </td>
                          <td style="text-align: left">
                            {{ item.name }}
                          </td>
                          <td style="text-align: center">
                            {{ item.soCCCD}}
                          </td>
                          <td style="text-align: center">
                            {{ item.phoneNumber}}
                          </td>
                          <td style="text-align: center">
                            <a
                                href="#show_user"
                                data-bs-toggle="modal"
                                size="sm"
                                type="button"
                                class="btn btn-outline btn-sm"
                                v-on:click="handleGetInfo(item.id)"
                            >
                              <i class="fas fa-eye text-orange me-1"></i>
                            </a>
                            <a
                                href="#info_user"
                                data-bs-toggle="modal"
                                size="sm"
                                type="button"
                                class="btn btn-outline btn-sm"
                                v-on:click="handleGetInfo(item.id)"
                            >
                              <i class="fas fa-pencil-alt text-success me-1"></i>
                            </a>
                            <a
                                href="#delete_user"
                                data-bs-toggle="modal"
                                class="btn btn-outline btn-sm"
                                v-on:click="handleShowDeleteModal(item.id)"
                            >
                              <i class="fas fa-trash-alt text-danger me-1"></i>
                            </a>
                            <a
                                href="#reset_user"
                                data-bs-toggle="modal"
                                class="btn btn-outline btn-sm"
                                v-on:click="handleShowResetModal(item.id)"
                            >
                              <i class="fas fa-rotate text-danger me-1"></i>
                            </a>
                          </td>
                        </tr>
                        </tbody>
                      </table>
                    </div>
                    </div>

                    <div
                        class="modal fade"
                        id="info_user"
                        aria-hidden="true"
                        role="dialog"
                        style="display: none"
                        data-bs-backdrop="static"
                    >
                      <div class="modal-dialog modal-lg modal-dialog-centered" role="document">
                        <div class="modal-content">
                          <div class="modal-header">
                            <h5 class="modal-title">Thông tin tài khoản</h5>
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
                            >
                              <div class="row">
                                <div class="col-6">
                                  <div class="mb-3">
                                    <label class="text-left">Tên tài khoản</label>
                                    <span style="color: red">&nbsp;*</span>
                                    <Field
                                        v-model="model.userName"
                                        placeholder="Vui lòng nhập tên tài khoản"
                                        name="userName"
                                        type="text"
                                        class="form-control"
                                        :disabled = isView
                                        :class="{ 'is-invalid': errors.userName }"
                                    />
                                    <div class="invalid-feedback">{{ errors.userName }}</div>
                                  </div>
                                </div>
                                <div class="col-6">
                                  <div class="mb-3">
                                    <label class="text-left" >Mật khẩu</label>
                                    <Field
                                        name="password"
                                        v-model="model.password"
                                        type="password"
                                        class="form-control"
                                        placeholder="Nhập mật khẩu"
                                        :class="{ 'is-invalid': errors.password }"
                                    />
                                    <div class="invalid-feedback">{{ errors.password }}</div>
                                  </div>

                                </div>
                                <div class="col-6">
                                  <div class="mb-3">
                                    <label >Họ và Tên</label>
                                    <span style="color: red">&nbsp;*</span>
                                    <Field
                                        placeholder="Vui lòng nhập họ và tên"
                                        name="name"
                                        v-model="model.name"
                                        type="text"
                                        class="form-control"
                                        :class="{ 'is-invalid': errors.name }"
                                    />
                                    <div class="invalid-feedback">{{ errors.name }}</div>
                                  </div>
                                </div>
                                <div class="col-6">
                                  <div class="mb-3">
                                    <label class="text-left">Số điện thoại</label>
                                    <input type="hidden" v-model="model.id"/>
                                    <input
                                        id="phoneNumber"
                                        v-model="model.phoneNumber"
                                        type="text"
                                        class="form-control"
                                        placeholder="Nhập số điện thoại"
                                    />
                                  </div>
                                </div>
                                <div class="col-6">
                                  <div class="mb-3">
                                    <label class="text-left">Số CCCD</label>
                                    <input type="hidden" v-model="model.id"/>
                                    <input
                                        id="soCCCD"
                                        v-model="model.soCCCD"
                                        type="text"
                                        class="form-control"
                                        placeholder="Nhập CCCD"
                                    />
                                  </div>
                                </div>
                                <div class="col-6">
                                  <div class="mb-3">
                                    <label class="text-left">Email</label>
                                    <Field
                                        placeholder="Vui lòng nhập Email"
                                        name="email"
                                        type="text"
                                        class="form-control"
                                        :class="{ 'is-invalid': errors.email }"
                                    />
                                    <div class="invalid-feedback">{{ errors.email }}</div>
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
                        class="modal fade"
                        id="show_user"
                        aria-hidden="true"
                        role="dialog"
                        style="display: none"
                        data-bs-backdrop="static"
                    >
                      <div class="modal-dialog modal-lg modal-dialog-centered" role="document">
                        <div class="modal-content">
                          <div class="modal-header">
                            <h5 class="modal-title">Thông tin chi tiết tài khoản</h5>
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
                            >
                              <div class="row">
                                <div class="col-4">
                                  <div class="mb-3">
                                    <label class="text-left badge bg-danger">Tên tài khoản</label>
                                    <span style="color: red">&nbsp;*</span>
                                    <div>{{model.userName}}</div>
                                  </div>
                                </div>
                                <div class="col-4">
                                  <div class="mb-3">
                                    <label class="text-left badge bg-danger">Họ và Tên</label>
                                    <span style="color: red">&nbsp;*</span>
                                    <div>{{model.name}}</div>
                                  </div>
                                </div>
                                <div class="col-4">
                                  <div class="mb-3">
                                    <label class="text-left  badge bg-danger">Số điện thoại </label>
                                    <div style="font-size: 16px">{{model.phoneNumber}}</div>
                                  </div>
                                </div>
                                <div class="col-4">
                                  <div class="mb-3">
                                    <label class="text-left badge bg-danger">Số CCCD</label>
                                    <div>{{model.soCCCD}}</div>
                                  </div>
                                </div>
                                <div class="col-4">
                                  <div class="mb-3">
                                    <label class="text-left badge bg-danger">Email</label>
                                    <div>{{model.email}}</div>
                                  </div>
                                </div>
                                <div class="col-4">
                                  <div class="mb-3">
                                    <label class="text-left badge bg-danger">Địa chỉ</label>
                                    <div>{{model.address}}</div>
                                  </div>
                                </div>

                                <div class="col-12">
                                  <div class="mb-3">
                                    <label class="text-left badge bg-danger">Token FCM</label>
                                    <div>{{model.tokenFCM }}</div>
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
                              </div>
                            </Form>
                          </div>
                        </div>
                      </div>
                    </div>
                    <div
                        class="modal fade "
                        id="delete_user"
                        tabindex="-1"
                        role="dialog"
                        aria-hidden="true"
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
                    <div
                        class="modal fade "
                        id="reset_user"
                        tabindex="-1"
                        role="dialog"
                        aria-hidden="true"
                    >
                      <div class="modal-dialog modal-dialog-centered">
                        <div class="modal-content">
                            <div class="modal-header">
                            <h5 class="modal-title" id="acc_title">Bạn có chắc?</h5>
                            <b-button
                                type="button"
                                class="btn-close"
                                data-bs-dismiss="modal"
                                aria-label="Close"
                            ></b-button>
                            </div>
                            <div class="modal-body" style="font-weight: 500;">
                                <p id="acc_msg">Bạn chắc chắn muốn đặt mật khẩu lại mặc định: PhuongThanh#2o24</p>
                            </div>
                            <div class="modal-footer">
                            <b-button class="btn btn-delete w-md si_accept_cancel" v-on:click="handleReset" data-bs-dismiss="modal">
                                Khôi phục
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
import { computed, getCurrentInstance, onMounted, reactive, toRefs, watch } from "vue";
import VueDatePicker from '@vuepic/vue-datepicker';
import { userCitizenModel } from "@/models/userCitizenModel";
import Treeselect from 'vue3-treeselect';
import VueMultiselect from 'vue-multiselect';
import 'vue-multiselect/dist/vue-multiselect.css';
import { Form, Field } from "vee-validate";
import * as Yup from "yup";
import { Modal } from 'bootstrap';
import { notifyModel } from "@/models/notifyModel";
import EmailValidator from "vuelidate/lib/validators/email";
defineOptions({
  name: "admin/page"
});
const {
  proxy
} = getCurrentInstance();
const state = reactive({
  title: "DANH SÁCH",
  model: userCitizenModel.baseJson(),
  listUser: [],
  listRole: [],
  currentPage: 1,
  numberOfElement: 1,
  perPage: 10,
  pageOptions: [10, 25, 50, 100],
  totalRows: 1,
  sortBy: 'age',
  sortDesc: false,
  theModal: null,
  isView: false,
  itemFilter: {
    userName: null,
    unitRole: null
  }
});
const {
  title,
  model,
  listUser,
  listRole,
  currentPage,
  numberOfElement,
  perPage,
  pageOptions,
  totalRows,
  sortBy,
  sortDesc,
  theModal,
  isView,
  itemFilter
} = toRefs(state);
const schema = Yup.object().shape({
  userName: Yup.string().required("Tài khoản không được bỏ trống !"),
  name: Yup.string().required("Họ và tên không được bỏ trống !"),
  email: Yup.string().required("Email không được bỏ trống !").email("Email không đúng định dạng !"),
  password: Yup.string().required("Password không được bỏ trống !")
});
function handleClear() {
  state.itemFilter = {
    userName: null,
    unitRole: null
  };
}
function handleSearch() {
  getData();
}
async function getData() {
  let params = {
    start: state.currentPage,
    limit: state.perPage,
    sortBy: state.sortBy,
    userName: state.itemFilter.userName,
    unitRole: state.itemFilter.unitRole
  };
  await proxy.$store.dispatch("userCitizenStore/getPagingParams", params).then(res => {
    if (res != null && res.code === 0) {
      state.listUser = res.data.data;
      state.totalRows = res.data.totalRows;
      state.numberOfElement = res.data.data.length;
    }
  });
}
async function getListRole() {
  await proxy.$store.dispatch("unitRoleStore/getAll").then(res => {
    if (res != null && res.code === 0) {
      state.listRole = res.data || [];
    }
  });
}
async function handleGetInfo(id) {
  await proxy.$store.dispatch("userCitizenStore/getById", {
    id: id
  }).then(res => {
    if (res != null && res.code === 0) {
      state.model = userCitizenModel.getJson(res.data);
    }
  });
}
function handleShowDeleteModal(id) {
  state.model.id = id;
  proxy.showDeleteModal = true;
}
function handleShowResetModal(id) {
  state.model.id = id;
  proxy.showResetModal = true;
}
async function handleDelete() {
  if (state.model.id != 0 && state.model.id != null && state.model.id) {
    await proxy.$store.dispatch("userCitizenStore/delete", {
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
async function handleReset() {
  if (state.model.id != 0 && state.model.id != null && state.model.id) {
    await proxy.$store.dispatch("userCitizenStore/reset", {
      'id': state.model.id
    }).then(res => {
      if (res != null && res.code === 0) {
        proxy.showResetModal = false;
        getData();
      }
      proxy.$store.dispatch("snackBarStore/addNotify", notifyModel.addMessage(res));
    });
  }
}
async function handleSubmit() {
  if (state.model.id != 0 && state.model.id != null && state.model.id) {
    await proxy.$store.dispatch("userCitizenStore/update", state.model).then(res => {
      if (res != null && res.code === 0) {
        getData();
        state.model = userCitizenModel.baseJson();
        state.theModal.hide();
      }
      proxy.$store.dispatch("snackBarStore/addNotify", notifyModel.addMessage(res));
    });
  } else {
    await proxy.$store.dispatch("userCitizenStore/create", state.model).then(res => {
      if (res != null && res.code === 0) {
        getData();
        state.model = userCitizenModel.baseJson();
        state.theModal.hide();
      }
      proxy.$store.dispatch("snackBarStore/addNotify", notifyModel.addMessage(res));
    });
  }
}
const Email = computed(() => {
  return EmailValidator;
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
onMounted(() => {
  state.theModal = new Modal(document.getElementById('info_user'));
});
getData();
getListRole();
</script>

