<template>
  <div class="main-Wrapper">
    <adminheader></adminheader>
    <adminsidebar></adminsidebar>
    <div class="page-wrapper">
      <div class="content container-fluid">
        <adminbreadcrumb2 :title="title" />
        <div class="row">
          <div class="col-12">
            <div class="card">
              <div class="card-body">
                <!-- Thêm tabs vào đây -->
                <b-tabs content-class="mt-3" v-model="activeTab">
                  <b-tab title="Đã đặt" @click="loadTabData(1)"></b-tab>
                  <b-tab title="Đang vận chuyển" @click="loadTabData(2)"></b-tab>
                  <b-tab title="Đã nhận" @click="loadTabData(3)"></b-tab>
                </b-tabs>
                
                <div class="row">
                  <div class="col-12">
                    <div class="row mb-2">
                      <div class="col-sm-8 col-md-8 d-flex justify-content-left align-items-center">
                        <div class="col-sm-12 d-flex justify-content-left align-items-center">
                          <div>
                            Hiển thị
                            <label class="d-inline-flex align-items-center" style="color: #F5E7B2;">
                              {{ currentTabData.list.length }}
                            </label>
                            trên tổng số <span style="color: red; font-weight: bold;">{{ currentTabData.totalRows }}</span> dòng
                          </div>
                        </div>
                      </div>
                      <div class="col-md-4" style="display: flex; justify-content: flex-end; align-items: center;">
                        <div class="card-tools">
                          <button type="button" class="btn btn-tool" @click="loadTabData(activeTab + 1)">
                            <i class="fas fa-retweet"></i>
                          </button>
                        </div>
                      </div>
                    </div>
                    <div class="custom-new-table">
                      <div class="table-responsive">
                        <table class="table table-hover table-center mb-0">
                          <thead>
                            <th class="col150 cursor td-stt" style="text-align: center;">STT</th>
                            <th class="col150 cursor" style="text-align: center;">Tên khách hàng</th>
                            <th class="col100 cursor" style="text-align: center;">SĐT</th>
                            <th class="col100 cursor" style="text-align: center;">Số lượng</th>
                            <th class="col100 cursor td-xuly-user" style="text-align: center;">Xử lý</th>
                          </thead>
                          <tbody>
                            <tr v-for="(item, index) in currentTabData.list" :key="index">
                              <td style="text-align: center">{{ index + ((currentPage-1)*perPage) + 1}}</td>
                              <td style="text-align: left">{{ item.customer.fullName }}</td>
                              <td style="text-align: left">{{ item.customer.phone }}</td>
                              <td style="text-align: center">
                                <b-button v-if="item.items.length > 0" variant="outline-danger btn-sm" class="cs-btn-primary" size="sm">
                                  {{ item.items.length }}
                                </b-button>
                              </td>
                              <td style="text-align: center">
                                <a href="#info_modal" data-bs-toggle="modal" size="sm" type="button" 
                                   class="btn btn-outline btn-sm" v-on:click="handleGetInfo(item.id)">
                                  <i class="fas fa-eye text-success me-1"></i>
                                </a>
                                <a href="#delete_modal" data-bs-toggle="modal" class="btn btn-outline btn-sm" 
                                   v-on:click="handleShowDeleteModal(item.id)">
                                  <i class="fas fa-trash-alt text-danger me-1"></i>
                                </a>
                              </td>
                            </tr>
                          </tbody>
                        </table>
                      </div>
                    </div>

                    <!-- Các modal giữ nguyên như cũ -->
                    <div class="modal fade" id="info_modal" aria-hidden="true" role="dialog" data-bs-backdrop="static" ref="ref_info_modal">
                      <!-- Nội dung modal giữ nguyên -->
                      <div class="modal-dialog modal-xl modal-dialog-centered" role="document">
                        <div class="modal-content">
                          <div class="modal-header">
                            <h5 class="modal-title">Thông tin đơn hàng</h5>
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
                                ref="form"
                            >
                              <div class="row mb-3">
                                <div class="col-md-6 mt-2 mb-3">
                                  <h5>Thông tin đơn hàng</h5>
                                  <ul class="list-group">
                                    <li class="list-group-item">
                                      <strong>Mã đơn hàng:</strong> {{ model.txnRef }}
                                    </li>
                                    <li class="list-group-item">
                                      <strong>Ngày đặt:</strong> {{ formatDate(model.orderDate) }}
                                    </li>
                                    <li class="list-group-item">
                                      <strong>Tổng tiền:</strong> {{ formatCurrency(model.totalAmount) }}
                                    </li>
                                    <li class="list-group-item">
                                      <strong>Phương thức thanh toán:</strong> Chuyển khoản
                                    </li>
                                  </ul>
                                </div>
                                <div class="col-md-6 mt-2">
                                  <h5>Thông tin giao hàng</h5>
                                  <ul class="list-group">
                                    <li class="list-group-item">
                                      <strong>Người nhận:</strong> {{ model.customer?.fullName }}
                                    </li>
                                    <li class="list-group-item">
                                      <strong>Địa chỉ:</strong> 
                                      {{ model.address?.address }}, {{ model.address?.town.name }}, {{ model.address?.district.name }}, {{ model.address?.province.name }}
                                    </li>
                                    <li class="list-group-item">
                                      <strong>SĐT:</strong> {{ model.customer?.phone }}
                                    </li>
                                    <li class="list-group-item">
                                      <strong>Trạng thái:</strong> 
                                      <span v-if="model.status == 1" class="badge bg-warning ms-2">
                                        Chưa thanh toán
                                      </span>
                                      <span v-else class="badge bg-success ms-2">
                                        Đã thanh toán
                                      </span>
                                    </li>
                                  </ul>
                                </div>
                              </div>
                              <div class="table-responsive">
                                <table class="table table-bordered">
                                  <thead>
                                    <tr>
                                      <th>Hình ảnh</th>
                                      <th>Sản phẩm</th>
                                      <th>Đơn giá</th>
                                      <th>Số lượng</th>
                                      <th>Thành tiền</th>
                                    </tr>
                                  </thead>
                                  <tbody>
                                    <tr v-for="(product, index) in model.items" :key="index">
                                      <td style="text-align: center">
                                        <img 
                                          :src="product.products.imageUrl" 
                                          width="100" 
                                          alt="Product image" 
                                          style="border: 1px solid #ccc;"
                                        />
                                      </td>
                                      <td>{{ product.products.name }}</td>
                                      <td>{{ formatCurrency(product.price) }}</td>
                                      <td>{{ product.quantity }}</td>
                                      <td>{{ formatCurrency(product.price * product.quantity) }}</td>
                                    </tr>
                                  </tbody>
                                  <tfoot>
                                    <tr>
                                      <th colspan="4" class="text-end">Tổng cộng:</th>
                                      <th>{{ formatCurrency(calculateSubTotal(model.items)) }}</th>
                                    </tr>
                                  </tfoot>
                                </table>
                              </div>

                              <div class="mt-4">
                                <h5 class="mb-3">Lịch sử đơn hàng</h5>
                                <OrderLogTimeline :logs="model.orderLogs || []" />
                              </div>

                              <div v-if="[1, 2].includes(model.status)" class="mt-4">
                                <label class="form-label">Lý do hủy đơn</label>
                                <textarea
                                  v-model.trim="cancellationReason"
                                  class="form-control"
                                  rows="2"
                                  placeholder="Nhập lý do hủy đơn hàng"
                                />
                              </div>
                            
                              <div class="text-end pt-2 mt-3">
                                <b-button
                                  type="button"
                                  variant="outline-danger"
                                  class="me-1"
                                  :disabled="exportingInvoice"
                                  @click="handleExportInvoice"
                                >
                                  <i class="fas fa-file-pdf me-1"></i>
                                  {{ exportingInvoice ? "Đang xuất..." : "Xuất hóa đơn PDF" }}
                                </b-button>
                                <b-button
                                    type="button"
                                    class="btn si_accept_cancel btn-submit w-md btn-out"
                                    data-bs-dismiss="modal"
                                >
                                  Đóng
                                </b-button>
                                <b-button
                                  v-if="[1, 2].includes(model.status)"
                                  type="button"
                                  variant="danger"
                                  class="ms-1"
                                  :disabled="!cancellationReason"
                                  @click="handleCancelOrder"
                                >
                                  Hủy đơn
                                </b-button>
                                <b-button  
                                  v-if="activeTab === 0"
                                  type="submit" variant="success" class="btn-submit w-md ms-1 cs-btn-primary"
                                >
                                  Vận chuyển
                                </b-button>
                              </div>
                            </Form>
                          </div>
                        </div>
                      </div>
                    </div>
                    
                    <div class="modal fade" id="delete_modal" tabindex="-1" role="dialog" aria-hidden="true" ref="ref_delete">
                      <!-- Nội dung modal giữ nguyên -->
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
                    
                    <div class="row">
                      <div class="col-md-6 col-sm-6 mt-2">
                        <div>
                          Hiển thị
                          <label class="d-inline-flex align-items-center">
                            <b-form-select class="form-select form-select-sm" v-model="perPage" size="sm" 
                                          :options="pageOptions" style="width: 70px; text-align: center;"></b-form-select>&nbsp;
                          </label>
                          trên tổng số {{ currentTabData.totalRows }} dòng
                        </div>
                      </div>
                      <div class="col-md-6 col-sm-6 mt-2" style="display: flex; justify-content: flex-end;">
                        <b-pagination v-model="currentPage" :total-rows="currentTabData.totalRows" 
                                     :per-page="perPage" align="right" size="sm" class="my-0" />
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
    <!-- Page Wrapper -->

  </div>
</template>
<script setup>
import { computed, getCurrentInstance, onMounted, reactive, toRefs, watch } from "vue";
import VueDatePicker from '@vuepic/vue-datepicker';
import { odersModel } from "@/models/odersModel";
import Treeselect from 'vue3-treeselect';
import VueMultiselect from 'vue-multiselect';
import 'vue-multiselect/dist/vue-multiselect.css';
import { Form, Field } from "vee-validate";
import * as Yup from "yup";
import { Modal } from 'bootstrap';
import { notifyModel } from "@/models/notifyModel";
import OrderLogTimeline from "@/components/orders/OrderLogTimeline.vue";
import { exportOrderInvoicePdf } from "@/utils/exportDocuments";
defineOptions({
  name: "admin/page"
});
const {
  proxy
} = getCurrentInstance();
const state = reactive({
  title: "DANH SÁCH ĐƠN HÀNG",
  model: odersModel.baseJson(),
  activeTab: 0,
  // Tab hiện tại (0, 1, 2 tương ứng với 3 tab)
  tabData: [{
    // Tab 1: Tất cả đơn hàng
    list: [],
    totalRows: 0,
    numberOfElement: 0
  }, {
    // Tab 2: Đơn chưa xử lý
    list: [],
    totalRows: 0,
    numberOfElement: 0
  }, {
    // Tab 3: Đơn đã xử lý
    list: [],
    totalRows: 0,
    numberOfElement: 0
  }],
  listSP: [],
  currentPage: 1,
  numberOfElement: 1,
  perPage: 5,
  pageOptions: [5, 10, 25, 50, 100],
  sortBy: 'age',
  sortDesc: false,
  theModal: null,
  isView: false,
  itemFilter: {
    userName: null,
    unitRole: null
  },
  cancellationReason: "",
  exportingInvoice: false
});
const {
  title,
  model,
  activeTab,
  tabData,
  listSP,
  currentPage,
  numberOfElement,
  perPage,
  pageOptions,
  sortBy,
  sortDesc,
  theModal,
  isView,
  itemFilter,
  cancellationReason,
  exportingInvoice
} = toRefs(state);
const schema = Yup.object().shape({
  // userName: Yup.string().required("Tài khoản không được bỏ trống !"),
  // name : Yup.string().required("Họ và tên không được bỏ trống !"),
  // password : Yup.string().required("Mật khẩu không được bỏ trống !"),
  // unitRole : Yup.mixed().required("Vai trò không được bỏ trống !"),
});
function formatDate(isoString) {
  if (!isoString) return "N/A";
  try {
    const date = new Date(isoString);
    if (isNaN(date.getTime())) return "N/A";
    return new Intl.DateTimeFormat('vi-VN', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
      hour12: false
    }).format(date);
  } catch (e) {
    return "N/A";
  }
}
function formatCurrency(value) {
  return new Intl.NumberFormat('vi-VN', {
    style: 'currency',
    currency: 'VND'
  }).format(value);
}
function calculateSubTotal(items) {
  if (!items) return 0;
  return items.reduce((total, item) => total + item.price * item.quantity, 0);
}
async function handleExportInvoice() {
  state.exportingInvoice = true;
  try {
    await exportOrderInvoicePdf(state.model);
  } catch (error) {
    proxy.$store.dispatch("snackBarStore/addNotify", {
      code: -1,
      message: error.message || "Không thể xuất hóa đơn PDF."
    });
  } finally {
    state.exportingInvoice = false;
  }
}
function handleClear() {
  state.itemFilter = {
    userName: null,
    unitRole: null
  };
}
function handleSearch() {
  loadTabData(state.activeTab + 1);
}
async function loadTabData(tabNumber) {
  let params = {
    start: state.currentPage,
    limit: state.perPage,
    sortBy: state.sortBy,
    userName: state.itemFilter.userName,
    unitRole: state.itemFilter.unitRole
  };
  let actionName;
  switch (tabNumber) {
    case 1:
      actionName = "odersStore/getPagingParams"; // Tất cả đơn hàng
      break;
    case 2:
      actionName = "odersStore/getPagingParamsStatus2"; // Đơn chưa xử lý
      break;
    case 3:
      actionName = "odersStore/getPagingParamsStatus3"; // Đơn đã xử lý
      break;
    default:
      actionName = "odersStore/getPagingParams";
  }
  await proxy.$store.dispatch(actionName, params).then(res => {
    if (res != null && res.code === 0) {
      const targetTab = tabNumber - 1; // Vì mảng bắt đầu từ 0
      const orders = res.data?.data || [];
      state.tabData[targetTab].list = orders;
      state.tabData[targetTab].totalRows = res.data?.totalRows || 0;
      state.tabData[targetTab].numberOfElement = orders.length;
    }
    proxy.$store.dispatch("snackBarStore/addNotify", notifyModel.addMessage(res));
  });
}
async function getListSanPham() {
  await proxy.$store.dispatch("sanPhamStore/getAll").then(res => {
    if (res != null && res.code === 0) {
      state.listSP = res.data || [];
    }
  });
}
async function handleGetInfo(id) {
  await proxy.$store.dispatch("odersStore/getById", {
    id: id
  }).then(res => {
    if (res != null && res.code === 0) {
      state.model = res.data;
      console.log("MODEL: ", state.model);
    }
  });
}
function handleShowDeleteModal(id) {
  state.model.id = id;
  proxy.showDeleteModal = true;
}
async function handleDelete() {
  if (state.model.id != 0 && state.model.id != null && state.model.id) {
    await proxy.$store.dispatch("odersStore/delete", {
      'id': state.model.id
    }).then(res => {
      if (res != null && res.code === 0) {
        proxy.showDeleteModal = false;
        loadTabData(state.activeTab + 1); // Reload dữ liệu tab hiện tại sau khi xóa
      }
      proxy.$store.dispatch("snackBarStore/addNotify", notifyModel.addMessage(res));
    });
  }
}
async function handleReset() {
  if (state.model.id != 0 && state.model.id != null && state.model.id) {
    await proxy.$store.dispatch("odersStore/reset", {
      'id': state.model.id
    }).then(res => {
      if (res != null && res.code === 0) {
        proxy.showResetModal = false;
        loadTabData(state.activeTab + 1); // Reload dữ liệu tab hiện tại
      }
      proxy.$store.dispatch("snackBarStore/addNotify", notifyModel.addMessage(res));
    });
  }
}
async function handleSubmit() {
  let params = {
    id: state.model.shippingDetail[0].id,
    ordersId: state.model.id,
    status: 2
  };
  await proxy.$store.dispatch("shippingStore/update", params).then(res => {
    if (res != null && res.code === 0) {
      loadTabData(state.activeTab + 1); // Reload dữ liệu tab hiện tại
      state.theModal.hide();
    }
    proxy.$store.dispatch("snackBarStore/addNotify", notifyModel.addMessage(res));
  });
}
async function handleCancelOrder() {
  const latestShipping = state.model.shippingDetail?.[0];
  if (!latestShipping || !state.cancellationReason) return;

  const res = await proxy.$store.dispatch("shippingStore/update", {
    id: latestShipping.id,
    ordersId: state.model.id,
    status: 4,
    note: state.cancellationReason
  });
  if (res?.code === 0) {
    state.cancellationReason = "";
    state.theModal.hide();
    loadTabData(state.activeTab + 1);
  }
  proxy.$store.dispatch("snackBarStore/addNotify", notifyModel.addMessage(res));
}
const currentTabData = computed(() => {
  return state.tabData[state.activeTab];
});
watch(() => state.perPage, val => {
  loadTabData(state.activeTab + 1);
}, {
  deep: true
});
watch(() => state.currentPage, val => {
  loadTabData(state.activeTab + 1);
}, {
  deep: true
});
onMounted(() => {
  state.theModal = new Modal(document.getElementById('info_modal'));
  proxy.$refs.ref_info_modal.addEventListener('hidden.bs.modal', event => {
    state.model = odersModel.baseJson();
    state.cancellationReason = "";
  });
  proxy.$refs.ref_delete.addEventListener('hidden.bs.modal', event => {
    state.model = odersModel.baseJson();
  });
});
getListSanPham();
loadTabData(1); // Load dữ liệu tab đầu tiên
</script>

