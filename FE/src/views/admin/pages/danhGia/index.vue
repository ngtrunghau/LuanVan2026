<template>
  <div class="main-Wrapper">
    <adminheader />
    <adminsidebar />
    <div class="page-wrapper">
      <div class="content container-fluid">
        <div class="card">
          <div class="card-header d-flex justify-content-between align-items-center flex-wrap gap-2">
            <div>
              <h3 class="card-title mb-1">Kiểm duyệt đánh giá</h3>
              <p class="text-muted mb-0">Duyệt đánh giá hợp lệ hoặc ẩn nội dung vi phạm.</p>
            </div>
            <button class="btn btn-outline-secondary" :disabled="loading" @click="loadData">
              <i class="fas fa-retweet me-1"></i>Làm mới
            </button>
          </div>

          <div class="card-body">
            <div class="row g-2 mb-3">
              <div class="col-md-3">
                <select v-model="status" class="form-select" @change="changeFilter">
                  <option value="">Tất cả trạng thái</option>
                  <option :value="0">Chờ duyệt</option>
                  <option :value="1">Đã duyệt</option>
                  <option :value="2">Đã ẩn</option>
                </select>
              </div>
              <div class="col-md-6">
                <input
                  v-model.trim="keyword"
                  class="form-control"
                  placeholder="Tìm theo sản phẩm hoặc nội dung đánh giá"
                  @keyup.enter="changeFilter"
                >
              </div>
              <div class="col-md-3">
                <button class="btn btn-primary w-100" @click="changeFilter">
                  <i class="fas fa-search me-1"></i>Tìm kiếm
                </button>
              </div>
            </div>

            <div v-if="loading" class="text-center py-5">
              <div class="spinner-border text-primary"></div>
            </div>
            <div v-else class="table-responsive">
              <table class="table table-hover align-middle">
                <thead>
                  <tr>
                    <th>Sản phẩm</th>
                    <th>Khách hàng</th>
                    <th>Đánh giá</th>
                    <th>Trạng thái</th>
                    <th>Lý do ẩn</th>
                    <th class="text-center">Xử lý</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="item in items" :key="item.id">
                    <td>
                      <div class="d-flex align-items-center gap-2">
                        <img v-if="item.productImage" :src="item.productImage" class="product-image" alt="">
                        <div>
                          <strong>{{ item.productName || "--" }}</strong>
                          <div class="small text-muted">Đơn #{{ item.orderId || "--" }}</div>
                        </div>
                      </div>
                    </td>
                    <td>
                      {{ item.customerName || `Khách hàng #${item.customerId}` }}
                      <div class="small text-muted">{{ formatDate(item.date) }}</div>
                    </td>
                    <td class="review-content">
                      <div class="text-warning mb-1">
                        <i v-for="star in 5" :key="star" class="fas fa-star" :class="{ 'text-muted': star > item.totalStar }"></i>
                      </div>
                      {{ item.comment || "(Không có nội dung)" }}
                    </td>
                    <td>
                      <span class="badge" :class="statusClass(item.moderationStatus)">
                        {{ statusLabel(item.moderationStatus) }}
                      </span>
                      <div v-if="item.moderatedBy" class="small text-muted mt-1">
                        {{ item.moderatedBy }} · {{ formatDate(item.moderatedAt) }}
                      </div>
                    </td>
                    <td>{{ item.moderationReason || "--" }}</td>
                    <td class="text-center text-nowrap">
                      <button
                        v-if="item.moderationStatus !== 1"
                        class="btn btn-sm btn-outline-success me-1"
                        :disabled="processingId === item.id"
                        @click="approve(item)"
                      >
                        Duyệt
                      </button>
                      <button
                        v-if="item.moderationStatus !== 2"
                        class="btn btn-sm btn-outline-danger"
                        :disabled="processingId === item.id"
                        @click="openHide(item)"
                      >
                        Ẩn
                      </button>
                    </td>
                  </tr>
                  <tr v-if="items.length === 0">
                    <td colspan="6" class="text-center text-muted py-4">Không có đánh giá phù hợp.</td>
                  </tr>
                </tbody>
              </table>
            </div>

            <div class="d-flex justify-content-between align-items-center mt-3">
              <span class="text-muted">Tổng số: {{ totalRows }}</span>
              <b-pagination v-model="page" :total-rows="totalRows" :per-page="limit" @update:model-value="loadData" />
            </div>
          </div>
        </div>
      </div>
    </div>

    <div class="modal fade" id="hide_review_modal" tabindex="-1" ref="hideModalElement">
      <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">Ẩn đánh giá vi phạm</h5>
            <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
          </div>
          <div class="modal-body">
            <label class="form-label">Lý do ẩn <span class="text-danger">*</span></label>
            <textarea
              v-model.trim="hideReason"
              rows="4"
              maxlength="500"
              class="form-control"
              :class="{ 'is-invalid': hideReasonError }"
              placeholder="Ví dụ: ngôn từ xúc phạm, quảng cáo, nội dung không liên quan..."
            ></textarea>
            <div v-if="hideReasonError" class="invalid-feedback">{{ hideReasonError }}</div>
          </div>
          <div class="modal-footer">
            <button class="btn btn-outline-secondary" data-bs-dismiss="modal">Đóng</button>
            <button class="btn btn-danger" :disabled="processingId" @click="confirmHide">
              Xác nhận ẩn
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { getCurrentInstance, onBeforeUnmount, onMounted, ref } from "vue";
import { Modal } from "bootstrap";

const { proxy } = getCurrentInstance();
const loading = ref(false);
const processingId = ref(null);
const items = ref([]);
const totalRows = ref(0);
const page = ref(1);
const limit = ref(10);
const status = ref(0);
const keyword = ref("");
const selectedReview = ref(null);
const hideReason = ref("");
const hideReasonError = ref("");
const hideModalElement = ref(null);
let hideModal = null;

async function loadData() {
  loading.value = true;
  try {
    const response = await proxy.$store.dispatch("danhGiaStore/getModerationList", {
      start: page.value,
      limit: limit.value,
      status: status.value === "" ? null : Number(status.value),
      keyword: keyword.value || null
    });
    if (response?.code === 0) {
      items.value = response.data?.data || [];
      totalRows.value = response.data?.totalRows || 0;
    } else {
      notify(response?.message || "Không thể tải danh sách đánh giá.", false);
    }
  } finally {
    loading.value = false;
  }
}

function changeFilter() {
  page.value = 1;
  loadData();
}

async function approve(item) {
  await moderate(item.id, 1, null);
}

function openHide(item) {
  selectedReview.value = item;
  hideReason.value = item.moderationReason || "";
  hideReasonError.value = "";
  hideModal?.show();
}

async function confirmHide() {
  if (!selectedReview.value) return;
  if (!hideReason.value) {
    hideReasonError.value = "Lý do ẩn không được bỏ trống.";
    return;
  }
  hideReasonError.value = "";
  const success = await moderate(selectedReview.value.id, 2, hideReason.value);
  if (success) hideModal?.hide();
}

async function moderate(id, moderationStatus, reason) {
  processingId.value = id;
  try {
    const response = await proxy.$store.dispatch("danhGiaStore/moderate", {
      id,
      status: moderationStatus,
      reason
    });
    notify(response?.message, response?.code === 0);
    if (response?.code === 0) {
      await loadData();
      return true;
    }
    return false;
  } finally {
    processingId.value = null;
  }
}

function notify(message, success) {
  proxy.$store.dispatch("snackBarStore/addNotify", {
    message: message || (success ? "Thao tác thành công." : "Có lỗi xảy ra."),
    code: success ? 0 : -1
  });
}

function statusLabel(value) {
  return { 0: "Chờ duyệt", 1: "Đã duyệt", 2: "Đã ẩn" }[value] || "Không xác định";
}

function statusClass(value) {
  return { 0: "bg-warning text-dark", 1: "bg-success", 2: "bg-danger" }[value] || "bg-secondary";
}

function formatDate(value) {
  return value ? new Intl.DateTimeFormat("vi-VN", {
    day: "2-digit", month: "2-digit", year: "numeric", hour: "2-digit", minute: "2-digit"
  }).format(new Date(value)) : "--";
}

onMounted(() => {
  hideModal = new Modal(hideModalElement.value);
  loadData();
});

onBeforeUnmount(() => hideModal?.dispose());
</script>

<style scoped>
.product-image {
  width: 52px;
  height: 52px;
  object-fit: cover;
  border-radius: 6px;
  border: 1px solid #e3e6e9;
}

.review-content {
  min-width: 240px;
  max-width: 420px;
  white-space: normal;
}
</style>
