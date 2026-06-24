<template>
    <layoutheader :class="{ 'header-space': showHeaderSpace }" ref="header" />
    <div class="container" style="padding-top: 150px;">
      <!-- Thêm tabs navigation -->
      <ul class="nav nav-tabs mb-4" id="orderTabs" role="tablist">
        <li class="nav-item" role="presentation">
          <button 
            class="nav-link active" 
            id="orders-tab" 
            data-bs-toggle="tab" 
            data-bs-target="#orders" 
            type="button"
            @click="activeTab = 'orders'"
          >
            Theo dõi đơn hàng
          </button>
        </li>
        <li class="nav-item" role="presentation" v-if="showReviewsTab">
          <button 
            class="nav-link" 
            id="reviews-tab" 
            data-bs-toggle="tab" 
            data-bs-target="#reviews" 
            type="button"
            @click="activeTab = 'reviews'"
          >
            Đánh giá sản phẩm
          </button>
        </li>
      </ul>
      <div class="tab-content">
        <!-- Tab Theo dõi đơn hàng -->
        <div class="tab-pane fade show active" id="orders" role="tabpanel">
          <!-- Giữ nguyên phần hiển thị đơn hàng hiện tại của bạn -->
          <div class="card shadow" v-for="(item, index) in filteredOrders" :key="index">
            <!-- ... (giữ nguyên nội dung hiện tại) ... -->
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
                      <span v-if="item.status == 1" class="badge bg-info ms-2">
                        Đã đặt hàng
                      </span>
                      <span v-else-if="item.status == 2" class="badge bg-warning ms-2">
                        Đang vận chuyển
                      </span>
                      <span v-else-if="item.status == 3" class="badge bg-success ms-2">
                        Giao hàng thành công
                      </span>
                      <span v-else class="badge bg-danger ms-2">Đã hủy</span>
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

                <h5 class="mt-4 mb-3">Lịch sử đơn hàng</h5>
                <OrderLogTimeline :logs="item.shippingDetails || []" />
              </div>
            </div>
          </div>
        </div>
        
        <!-- Tab Đánh giá sản phẩm -->
        <div class="tab-pane fade" id="reviews" role="tabpanel" v-if="showReviewsTab">
          <!-- <div v-if="reviewsLoading" class="text-center py-4">
            <div class="spinner-border text-primary" role="status">
              <span class="visually-hidden">Loading...</span>
            </div>
          </div>
          
          <div v-else-if="this.list.length === 0" class="alert alert-info">
            Bạn chưa có sản phẩm nào cần đánh giá
          </div> -->
          
          <div>
            <div class="card shadow mb-3" v-for="(item, index) in list" :key="index">
              <div class="card-body" v-for="(review, index) in item.items" :key="index">
                <div class="row">
                  <div class="col-md-2">
                    <img 
                      :src="review.products.imageUrl || 'https://via.placeholder.com/100'" 
                      class="img-thumbnail" 
                      alt="Product image"
                    >
                  </div>
                  <div class="col-md-10">
                    <div class="d-flex">
                      <h5>{{ review.products.name }}</h5>
                      <span class="text-muted ms-2"> Số lượng: {{ review.quantity }}</span>
                    </div>
                    <p class="text-muted">Đã mua ngày: {{ formatDate(item.orderDate) }}</p>
                    
                    <div v-if="!review.submitted" class="mb-3">
                      <label class="form-label">Đánh giá của bạn:</label>
                      <div class="rating">
                        <i 
                          v-for="star in 5" 
                          :key="star" 
                          class="fas fa-star"
                          :class="{ 'text-warning': star <= review.rating }"
                          @click="updateRating(review, star)"
                        ></i>
                      </div>
                    </div>
                    
                    <div v-if="!review.submitted" class="mb-3">
                      <label class="form-label">Nhận xét:</label>
                      <textarea 
                        class="form-control" 
                        rows="3" 
                        v-model="review.comment"
                        placeholder="Hãy chia sẻ cảm nhận về sản phẩm..."
                      ></textarea>
                    </div>
                    
                    <div v-if="!review.submitted" class="d-flex justify-content-end">
                      <button 
                        class="btn btn-primary"
                        @click="submitReview(review)"
                        :disabled="review.submitting"
                      >
                        <span v-if="review.submitting">
                          <span class="spinner-border spinner-border-sm" role="status"></span>
                          Đang gửi...
                        </span>
                        <span v-else>
                          Gửi đánh giá
                        </span>
                      </button>
                    </div>
                    <div v-else class="alert alert-success mb-0">
                      Cảm ơn bạn đã đánh giá sản phẩm. Nội dung đang chờ quản trị viên duyệt.
                    </div>
                  </div>
                </div>
              </div>
            </div>
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
              <Form
                  class="login"
                  @submit="handleSubmit"
                  ref="form"
              >
                <div class="row">
                  <div class="col-12">
                    <div class="mb-3">
                      <strong class="text-left">Ngày đặt:</strong>
                      {{ model.dateShip ? formatDate(model.dateShip) : 'Đang cập nhật' }}
                    </div>
                  </div>
                  <div class="col-12">
                    <div class="mb-3">
                      <strong class="text-left">Trạng thái:</strong>
                      <span v-if="model.status == 1" class="badge bg-info ms-2">
                        Đã đặt hàng
                      </span>
                      <span v-else-if="model.status == 2" class="badge bg-warning ms-2">
                        Đang vận chuyển
                      </span>
                      <span v-else-if="model.status == 3" class="badge bg-success ms-2">
                        Giao hàng thành công
                      </span>
                      <span v-else class="badge bg-danger ms-2">Đã hủy</span>
                    </div>
                  </div>
                </div>
                <div class="mt-3">
                  <h5 class="mb-3">Lịch sử đơn hàng</h5>
                  <OrderLogTimeline :logs="orderLogs" />
                </div>
                <div v-if="model.status == 1" class="mt-3">
                  <label class="form-label">Lý do hủy đơn</label>
                  <textarea
                    v-model.trim="cancellationReason"
                    class="form-control"
                    rows="2"
                    placeholder="Nhập lý do bạn muốn hủy đơn"
                  />
                </div>
                <div class="text-end pt-2 mt-3">
                  <b-button
                      type="button"
                      class="btn si_accept_cancel btn-submit w-md btn-out"
                      data-bs-dismiss="modal"
                  >
                    Đóng
                  </b-button>
                  <b-button  
                    v-if="model.status == 2"
                    type="submit" variant="success" class="btn-submit w-md ms-1 cs-btn-primary"
                  >
                    Đã nhận hàng
                  </b-button>
                  <b-button
                    v-if="model.status == 1"
                    type="button"
                    variant="danger"
                    class="ms-1"
                    :disabled="!cancellationReason"
                    @click="handleCancelOrder"
                  >
                    Hủy đơn
                  </b-button>
                </div>
              </Form>
            </div>
          </div>
        </div>
      </div>
    </div>
    <footerHome></footerHome>
  </template>
  
  <script setup>
import { computed, getCurrentInstance, onMounted, reactive, toRefs, watch } from "vue";
import { Form, Field } from "vee-validate";
import { notifyModel } from "@/models/notifyModel";
import { Modal } from 'bootstrap';
import OrderLogTimeline from "@/components/orders/OrderLogTimeline.vue";
defineOptions({
  name: 'OrderTracking'
});
const {
  proxy
} = getCurrentInstance();
const state = reactive({
  activeTab: 'orders',
  showReviewsTab: true,
  showDetails: false,
  list: [],
  currentPage: 1,
  numberOfElement: 1,
  perPage: 10,
  pageOptions: [5, 10, 25, 50, 100],
  totalRows: 1,
  model: [],
  orderLogs: [],
  cancellationReason: "",
  theModal: null,
  expandedOrders: {},
  // Lưu trạng thái expand/collapse của từng đơn hàng (key: orderId, value: boolean)

  // Data for reviews
  reviews: [],
  reviewsLoading: false
});
const {
  activeTab,
  showReviewsTab,
  showDetails,
  list,
  currentPage,
  numberOfElement,
  perPage,
  pageOptions,
  totalRows,
  model,
  orderLogs,
  cancellationReason,
  theModal,
  expandedOrders,
  reviews,
  reviewsLoading
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
      state.orderLogs = res.data || [];
      state.model = state.orderLogs[0] || {};
      console.log("MODEL: ", state.model);
    }
  });
}
async function handleSubmit() {
  const authUser = JSON.parse(localStorage.getItem('auth-user') || 'null');
  if (!authUser?.id) {
    return;
  }
  let params = {
    id: state.model.id,
    ordersId: state.model.ordersId,
    customerId: authUser.id,
    status: 3
  };
  await proxy.$store.dispatch("shippingStore/updateCustomer", params).then(res => {
    if (res != null && res.code === 0) {
      getData();
      state.theModal.hide();
    }
    proxy.$store.dispatch("snackBarStore/addNotify", notifyModel.addMessage(res));
  });
}
async function handleCancelOrder() {
  const authUser = JSON.parse(localStorage.getItem('auth-user') || 'null');
  if (!authUser?.id || !state.model?.id || !state.cancellationReason) return;

  const res = await proxy.$store.dispatch("shippingStore/updateCustomer", {
    id: state.model.id,
    ordersId: state.model.ordersId,
    customerId: authUser.id,
    status: 4,
    note: state.cancellationReason
  });
  if (res?.code === 0) {
    state.cancellationReason = "";
    state.theModal.hide();
    getData();
  }
  proxy.$store.dispatch("snackBarStore/addNotify", notifyModel.addMessage(res));
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
  if (!isoString) return "N/A"; // Xử lý giá trị rỗng
  // Xử lý giá trị rỗng
  try {
    const date = new Date(isoString);
    if (isNaN(date.getTime())) return "N/A"; // Kiểm tra Date hợp lệ
    return new Intl.DateTimeFormat('vi-VN', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
      hour12: false
    }).format(date);
  } catch (e) {
    return "N/A"; // Phòng lỗi bất ngờ
  }
}
async function loadReviews() {
  state.reviewsLoading = true;
  const authUser = JSON.parse(localStorage.getItem('auth-user'));
  let params = {
    start: state.currentPage,
    limit: state.perPage,
    idDonViCha: authUser.id
  };
  await proxy.$store.dispatch("odersStore/getPagingParamsStatus3Customer", params).then(response => {
    if (response != null && response.code === 0) {
      state.list = (response.data.data || []).map(order => ({
        ...order,
        items: (order.items || []).map(item => ({
          ...item,
          productId: item.products?.id,
          orderId: order.id,
          rating: 0,
          comment: '',
          submitting: false,
          submitted: false
        }))
      }));
      state.totalRows = response.data.totalRows;
      state.numberOfElement = response.data.data.length;

      // this.reviews = this.list.map(item => ({
      //   ...item,
      //   rating: item.rating || 0,
      //   comment: item.comment || '',
      //   submitting: false
      // }));
      console.log("LIST ĐÁNH GIÁ: ", state.list);
    }
    proxy.$store.dispatch("snackBarStore/addNotify", notifyModel.addMessage(response));
    state.reviewsLoading = false;
  });
  // try {
  //   this.reviewsLoading = true;
  //   const authUser = JSON.parse(localStorage.getItem('auth-user'));
  //   let params = {
  //     start: this.currentPage,
  //     limit: this.perPage,
  //     idDonViCha: authUser.id
  //   }
  //   const response = await this.$store.dispatch("odersStore/getPagingParamsStatus3Customer", params);

  //   if (response?.code === 0) {
  //     this.reviews = response.data.map(item => ({
  //       ...item,
  //       rating: item.rating || 0,
  //       comment: item.comment || '',
  //       submitting: false
  //     }));
  //   }
  // } catch (error) {
  //   console.error("Error loading reviews:", error);
  //   this.$store.dispatch("snackBarStore/addNotify",
  //     notifyModel.addMessage({ code: -1, message: "Lỗi khi tải danh sách đánh giá" })
  //   );
  // } finally {
  //   this.reviewsLoading = false;
  // }
}
function updateRating(review, rating) {
  review.rating = rating;
}
async function submitReview(review) {
  try {
    review.submitting = true;
    const authUser = JSON.parse(localStorage.getItem('auth-user'));
    const payload = {
      productId: review.productId,
      orderId: review.orderId,
      totalStar: review.rating,
      comment: review.comment
    };
    const response = await proxy.$store.dispatch("danhGiaStore/create", payload);
    if (response?.code === 0) {
      proxy.$store.dispatch("snackBarStore/addNotify", notifyModel.addMessage({
        code: 0,
        message: "Đánh giá đã được gửi thành công!"
      }));
      // Xóa khỏi danh sách sau khi gửi thành công
      review.submitted = true;
    }
  } catch (error) {
    console.error("Error submitting review:", error);
    proxy.$store.dispatch("snackBarStore/addNotify", notifyModel.addMessage({
      code: -1,
      message: "Lỗi khi gửi đánh giá"
    }));
  } finally {
    review.submitting = false;
  }
}
const filteredOrders = computed(() => {
  // Có thể thêm filter nếu cần
  return state.list;
});
watch(() => state.activeTab, newVal => {
  if (newVal === 'reviews') {
    loadReviews();
  }
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
  state.theModal = new Modal(document.getElementById('info_modal'));
});
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
