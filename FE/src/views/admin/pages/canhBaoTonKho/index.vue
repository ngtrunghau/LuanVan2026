<template>
  <div class="main-Wrapper">
    <adminheader />
    <adminsidebar />
    <div class="page-wrapper">
      <div class="content container-fluid">
        <div class="card">
          <div class="card-header d-flex justify-content-between align-items-center">
            <div>
              <h3 class="card-title mb-1">Cảnh báo tồn kho</h3>
              <p class="text-muted mb-0">
                Theo dõi sản phẩm sắp hết và cấu hình ngưỡng cảnh báo.
              </p>
            </div>
            <button class="btn btn-outline-secondary" :disabled="loading" @click="loadData">
              <i class="fas fa-retweet me-1"></i> Làm mới
            </button>
          </div>
          <div class="card-body">
            <div class="row g-3 mb-4">
              <div class="col-md-4">
                <div class="alert alert-warning mb-0">
                  <strong>{{ alertCount }}</strong> sản phẩm đang dưới ngưỡng.
                </div>
              </div>
              <div class="col-md-4">
                <div class="alert alert-danger mb-0">
                  <strong>{{ outOfStockCount }}</strong> sản phẩm đã hết hàng.
                </div>
              </div>
              <div class="col-md-4">
                <input
                  v-model.trim="search"
                  class="form-control"
                  placeholder="Tìm tên sản phẩm"
                >
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
                    <th class="text-center">Tồn hiện tại</th>
                    <th style="width: 190px">Ngưỡng cảnh báo</th>
                    <th>Trạng thái</th>
                    <th class="text-center">Xử lý</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="item in filteredItems" :key="item.productId">
                    <td>
                      <div class="d-flex align-items-center gap-2">
                        <img
                          v-if="item.imageUrl"
                          :src="item.imageUrl"
                          class="product-image"
                          alt=""
                        >
                        <strong>{{ item.productName }}</strong>
                      </div>
                    </td>
                    <td class="text-center">
                      <strong :class="item.currentStock === 0 ? 'text-danger' : ''">
                        {{ item.currentStock }}
                      </strong>
                    </td>
                    <td>
                      <div class="input-group input-group-sm">
                        <input
                          v-model.number="item.alertThreshold"
                          type="number"
                          min="0"
                          class="form-control"
                        >
                        <button
                          class="btn btn-outline-primary"
                          :disabled="savingProductId === item.productId"
                          @click="saveThreshold(item)"
                        >
                          Lưu
                        </button>
                      </div>
                    </td>
                    <td>
                      <span class="badge" :class="statusClass(item)">
                        {{ statusLabel(item) }}
                      </span>
                    </td>
                    <td class="text-center text-nowrap">
                      <router-link
                        to="/quan-tri/kho"
                        class="btn btn-sm btn-outline-success me-1"
                      >
                        Nhập hàng
                      </router-link>
                      <router-link
                        :to="`/quan-tri/quan-ly-san-pham/chi-tiet/${item.productId}`"
                        class="btn btn-sm btn-outline-secondary"
                      >
                        Sản phẩm
                      </router-link>
                    </td>
                  </tr>
                  <tr v-if="filteredItems.length === 0">
                    <td colspan="5" class="text-center text-muted py-4">
                      Không tìm thấy sản phẩm.
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { computed, getCurrentInstance, onMounted, ref } from "vue";

const { proxy } = getCurrentInstance();
const loading = ref(false);
const items = ref([]);
const alertCount = ref(0);
const outOfStockCount = ref(0);
const savingProductId = ref(null);
const search = ref("");

const filteredItems = computed(() => {
  const keyword = search.value.toLowerCase();
  return items.value.filter(item =>
    !keyword || item.productName.toLowerCase().includes(keyword)
  );
});

async function loadData() {
  loading.value = true;
  try {
    const response = await proxy.$store.dispatch("stockAlertStore/getAlerts", {
      includeAll: true,
      defaultThreshold: 5
    });
    if (response?.code === 0) {
      items.value = response.data?.items || [];
      alertCount.value = response.data?.alertCount || 0;
      outOfStockCount.value = response.data?.outOfStockCount || 0;
    }
  } finally {
    loading.value = false;
  }
}

async function saveThreshold(item) {
  if (item.alertThreshold == null || item.alertThreshold < 0) {
    notify("Ngưỡng cảnh báo không hợp lệ.", "danger");
    return;
  }
  savingProductId.value = item.productId;
  try {
    const response = await proxy.$store.dispatch("stockAlertStore/setThreshold", {
      productId: item.productId,
      alertThreshold: Number(item.alertThreshold)
    });
    notify(response?.message, response?.code === 0 ? "success" : "danger");
    if (response?.code === 0) await loadData();
  } finally {
    savingProductId.value = null;
  }
}

function statusLabel(item) {
  if (item.currentStock === 0) return "Hết hàng";
  if (item.isAlert) return "Sắp hết";
  return "Bình thường";
}

function statusClass(item) {
  if (item.currentStock === 0) return "bg-danger";
  if (item.isAlert) return "bg-warning text-dark";
  return "bg-success";
}

function notify(message, variant) {
  proxy.$store.dispatch("snackBarStore/addNotify", {
    message: message || "Có lỗi xảy ra.",
    variant
  });
}

onMounted(loadData);
</script>

<style scoped>
.product-image {
  width: 42px;
  height: 42px;
  object-fit: cover;
  border-radius: 6px;
}
</style>
