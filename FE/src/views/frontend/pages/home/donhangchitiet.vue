<template>
  <layoutheader :class="{ 'header-space': showHeaderSpace }" ref="header" />
  <div class="container" style="padding-top: 150px;">
    <h1 class="mb-4">Theo dõi đơn hàng</h1>
    
    <div class="card shadow" v-for="(item, index) in this.list" :key="index">
      <div class="card-body">
        <!-- Thông tin cơ bản (luôn hiển thị) -->
        <div class="row mb-4">
          <div class="col-md-12 d-flex justify-content-end">
            <a
                href="#info_modal"
                data-bs-toggle="modal"
                size="sm"
                type="button"
                class="btn btn-outline btn-sm"
                v-on:click="handleGetInfo(item.id)"
            >
                Xem chi tiết<i class="fa fa-solid fa-angles-right ms-2"></i>
            </a>
            <!-- <router-link :to="{path: `/don-hang/chi-tiet/${item.id}`}" tabindex="-1">
             
                Xem chi tiết<i class="fa fa-solid fa-angles-right ms-2"></i>
            </router-link> -->
          </div>
          <div class="col-md-6 mt-2">
            <h5>Thông tin đơn hàng</h5>
            <ul class="list-group">
              <li class="list-group-item">
                <strong>Mã đơn hàng:</strong> {{ item.txnRef }}
              </li>
              <li class="list-group-item">
                <strong>Ngày đặt:</strong> {{ formatDate(item.orderDate) }}
              </li>
              <li class="list-group-item">
                <strong>Tổng tiền:</strong> {{ formatCurrency(item.totalAmount) }}
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
                <strong>Người nhận:</strong> {{ item.customer.fullName }}
              </li>
              <li class="list-group-item">
                <strong>Địa chỉ:</strong> 
                {{ item.address.address }}, {{ item.address.town.name }}, {{ item.address.district.name }}, {{ item.address.province.name }}
              </li>
              <li class="list-group-item">
                <strong>SĐT:</strong> {{ item.customer.phone }}
              </li>
              <li class="list-group-item">
                <strong>Trạng thái:</strong> 
                <span v-if="item.status == 1" class="badge bg-warning ms-2">
                  Chưa thanh toán
                </span>
                <span v-else class="badge bg-success ms-2">
                  Đã thanh toán
                </span>
              </li>
            </ul>
          </div>
        </div>

        <!-- Nút xem thêm chi tiết -->
        <div class="text-center">
          <button 
            class="btn " 
            @click="toggleOrderDetails(item.id)"
          >
            <i class="bi" :class="showDetails ? 'fa fa-solid fa-angles-up' : 'fa fa-solid fa-angles-down'"></i>
          </button>
        </div>

        <!-- Chi tiết đơn hàng (ẩn/hiện) -->
        <div v-if="isOrderExpanded(item.id)">
          <h5 class="mt-4">Chi tiết đơn hàng</h5>
          <div class="table-responsive">
            <table class="table table-bordered">
              <thead>
                <tr>
                  <th>Sản phẩm</th>
                  <th>Đơn giá</th>
                  <th>Số lượng</th>
                  <th>Thành tiền</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="(product, index) in item.orderItems" :key="index">
                  <td>{{ product.products.name }}</td>
                  <td>{{ formatCurrency(product.price) }}</td>
                  <td>{{ product.quantity }}</td>
                  <td>{{ formatCurrency(product.price * product.quantity) }}</td>
                </tr>
              </tbody>
              <tfoot>
                <tr>
                  <th colspan="3" class="text-end">Tổng cộng:</th>
                  <th>{{ formatCurrency(calculateSubTotal(item.orderItems)) }}</th>
                </tr>
              </tfoot>
            </table>
          </div>

          <!-- <h5 class="mt-4">Lịch sử trạng thái</h5>
          <div class="timeline">
            <div class="timeline-item">
              <div class="timeline-indicator"></div>
              <div class="timeline-content">
                <div class="d-flex justify-content-between">
                  <strong>Đơn hàng đã đặt</strong>
                  <small class="text-muted">10:30 15/05/2023</small>
                </div>
                <p class="mb-0">Khách hàng đã đặt hàng</p>
              </div>
            </div>
            <div class="timeline-item">
              <div class="timeline-indicator"></div>
              <div class="timeline-content">
                <div class="d-flex justify-content-between">
                  <strong>Đã xác nhận</strong>
                  <small class="text-muted">11:15 15/05/2023</small>
                </div>
                <p class="mb-0">Nhân viên đã xác nhận đơn hàng</p>
              </div>
            </div>
            <div class="timeline-item">
              <div class="timeline-indicator"></div>
              <div class="timeline-content">
                <div class="d-flex justify-content-between">
                  <strong>Đang đóng gói</strong>
                  <small class="text-muted">09:00 16/05/2023</small>
                </div>
              </div>
            </div>
            <div class="timeline-item">
              <div class="timeline-indicator"></div>
              <div class="timeline-content">
                <div class="d-flex justify-content-between">
                  <strong>Đang giao hàng</strong>
                  <small class="text-muted">14:30 16/05/2023</small>
                </div>
                <p class="mb-0">Đơn hàng đang được vận chuyển</p>
              </div>
            </div>
          </div> -->
        </div>
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
            <h5 class="modal-title">Thông tin đơn hàng</h5>
            <b-button
                type="button"
                class="btn-close"
                data-bs-dismiss="modal"
                aria-label="Close"
            ></b-button>
          </div>
          <div class="modal-body">
            <div class="row">
              <div class="col-12">
                <div class="mb-3">
                  <label class="text-left">Ngày đặt</label>
                  <span style="color: red">&nbsp;*</span>
                  <!-- {{ formatDate(this.model.dateShip) }} -->
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
          </div>
        </div>
      </div>
    </div>
  </div>
  <footerHome></footerHome>
</template>

<script setup>
import { getCurrentInstance, reactive, toRefs } from "vue";
defineOptions({
  name: 'OrderTracking'
});
const {
  proxy
} = getCurrentInstance();
const state = reactive({
  showDetails: false,
  list: [],
  model: [],
  expandedOrders: {} // Lưu trạng thái expand/collapse của từng đơn hàng (key: orderId, value: boolean)
});
const {
  showDetails,
  list,
  model,
  expandedOrders
} = toRefs(state);
async function getData() {
  const authUser = JSON.parse(localStorage.getItem('auth-user'));
  let params = {
    id: authUser.id
  };
  await proxy.$store.dispatch("shippingStore/getByIdCustomer", params).then(res => {
    if (res != null && res.code === 0) {
      state.list = res.data;
      console.log("LIST: ", state.list);
    }
  });
}
async function handleGetInfo(id) {
  await proxy.$store.dispatch("shippingStore/getByIdOrder", {
    id: id
  }).then(res => {
    if (res != null && res.code === 0) {
      state.model = res.data[0];
      console.log("MODEL: ", state.model);
    }
  });
}
function toggleDetails(index) {
  proxy.orders[index].showDetails = !proxy.orders[index].showDetails;
}
function formatCurrency(value) {
  return new Intl.NumberFormat('vi-VN', {
    style: 'currency',
    currency: 'VND'
  }).format(value);
}
function calculateSubTotal(orderItems) {
  if (!orderItems) return 0;
  return orderItems.reduce((total, item) => total + item.price * item.quantity, 0);
}
function toggleOrderDetails(orderId) {
  // Đảo trạng thái (nếu không có key thì mặc định là true)
  state.expandedOrders = {
    ...state.expandedOrders,
    [orderId]: !state.expandedOrders[orderId]
  };
}
function isOrderExpanded(orderId) {
  // Kiểm tra trạng thái (mặc định false nếu không có key)
  return !!state.expandedOrders[orderId];
}
function formatDate(isoString) {
  const date = new Date(isoString);
  return new Intl.DateTimeFormat('vi-VN', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric',
    hour: '2-digit',
    minute: '2-digit',
    hour12: false // 24h format
  }).format(date);
}
getData();
</script>

<style scoped>
.timeline {
  position: relative;
  padding-left: 20px;
}

.timeline-item {
  position: relative;
  padding-bottom: 20px;
}

.timeline-item:last-child {
  padding-bottom: 0;
}

.timeline-indicator {
  position: absolute;
  left: -20px;
  top: 0;
  width: 12px;
  height: 12px;
  border-radius: 50%;
  background-color: #0d6efd;
}

.timeline-content {
  padding: 10px 15px;
  background-color: #f8f9fa;
  border-radius: 5px;
}

.timeline-item::before {
  content: '';
  position: absolute;
  left: -16px;
  top: 12px;
  height: 100%;
  width: 2px;
  background-color: #dee2e6;
}

.timeline-item:last-child::before {
  display: none;
}

.table th, .table td {
  vertical-align: middle;
}

.btn-outline-primary {
  transition: all 0.2s ease;
}

.bi {
  transition: transform 0.2s ease;
}
</style>