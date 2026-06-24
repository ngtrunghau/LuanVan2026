<template>
  <!-- Header -->
  <div class="header">
    <!-- Logo -->
    <div class="header-left">
      <router-link to="/quan-tri/tai-khoan" class="logo">
        <img src="@/assets/img/caulong/logo/logoNTH_removeBackground.png" alt="Logo" />
      </router-link>

      <router-link to="/quan-tri/tai-khoan" class="logo logo-small">
        <img
          src="@/assets/img/caulong/logo/logoNTH_removeBackground.png"
          alt="Logo"
          width="100"
          height="100"
        />
      </router-link>
    </div>
    <!-- /Logo -->

    <a href="javascript:void(0);" id="toggle_btn" @click="toggleSidebar">
      <i class="fe fe-text-align-left"></i>
    </a>

    <!-- <div class="top-nav-search">
      <form>
        <input type="text" class="form-control" placeholder="Search here" />
        <b-button class="btn" type="submit"><i class="fa fa-search"></i></b-button>
      </form>
    </div> -->

    <!-- Mobile Menu Toggle -->
    <a class="mobile_btn" id="mobile_btn" @click="toggleSidebar1">
      <i class="fa fa-bars"></i>
    </a>
    <!-- /Mobile Menu Toggle -->

    <!-- Header Right Menu -->
    <ul class="nav user-menu">
      <!-- Notifications -->
      <li class="nav-item dropdown">
        <a
          href="javascript:void(0)"
          class="dropdown-toggle nav-link"
          data-bs-toggle="dropdown"
          title="Cảnh báo tồn kho"
        >
          <i class="far fa-bell"></i>
          <span v-if="stockAlertCount > 0" class="badge rounded-pill notification-badge">
            {{ stockAlertCount > 99 ? '99+' : stockAlertCount }}
          </span>
        </a>
        <div class="dropdown-menu dropdown-menu-end stock-alert-dropdown">
          <div class="dropdown-header d-flex justify-content-between align-items-center">
            <strong>Cảnh báo tồn kho</strong>
            <button class="btn btn-sm btn-link p-0" @click.stop="loadStockAlerts">Làm mới</button>
          </div>
          <div v-if="stockAlertsLoading" class="text-center py-3">
            <div class="spinner-border spinner-border-sm text-primary"></div>
          </div>
          <template v-else>
            <router-link
              v-for="item in stockAlerts"
              :key="item.productId"
              to="/quan-tri/canh-bao-ton-kho"
              class="dropdown-item stock-alert-item"
            >
              <span class="alert-icon" :class="item.currentStock === 0 ? 'critical' : 'warning'">
                <i class="fas fa-box"></i>
              </span>
              <span>
                <strong class="d-block">{{ item.productName }}</strong>
                <small>
                  Còn {{ item.currentStock }}, ngưỡng {{ item.alertThreshold }}
                </small>
              </span>
            </router-link>
            <div v-if="stockAlerts.length === 0" class="text-center text-muted py-3">
              Không có sản phẩm sắp hết hàng.
            </div>
          </template>
          <router-link
            to="/quan-tri/canh-bao-ton-kho"
            class="dropdown-item text-center border-top"
          >
            Xem tất cả cảnh báo
          </router-link>
        </div>
      </li>
      <!-- /Notifications -->

      <!-- User Menu -->
      <li class="nav-item dropdown has-arrow">
        <a
          href="javascript:void(0)"
          class="dropdown-toggle nav-link"
          data-bs-toggle="dropdown"
        >
          <span class="user-img"
            ><img
              class="rounded-circle"
              src="@/assets/admin/img/profiles/avatar-01.jpg"
              width="31"
              alt="Ryan Taylor"
          /></span>
        </a>
        <div class="dropdown-menu">
          <div class="user-header">
            <div class="avatar avatar-sm">
              <img
                src="@/assets/admin/img/profiles/avatar-01.jpg"
                alt="User Image"
                class="avatar-img rounded-circle"
              />
            </div>
            <div class="user-text" v-if="this.model">
              <h6>{{ this.model.userName}}</h6>
              <p class="text-muted mb-0">{{ this.model.role}}</p>
            </div>
          </div>
          <!-- <router-link class="dropdown-item" to="/quan-tri/profile"
                       v-if="this.model"
            >Thông tin tài khoản</router-link
          > -->
          <router-link class="dropdown-item" to="/quan-tri/doi-mat-khau">
            Đổi mật khẩu
          </router-link>
          <div class="dropdown-item" @click.prevent="logout">Đăng xuất</div>
        </div>
      </li>
      <!-- /User Menu -->
    </ul>
    <!-- /Header Right Menu -->
  </div>
  <!-- /Header -->
</template>

<script setup>
import { getCurrentInstance, onBeforeUnmount, onMounted, reactive, toRefs } from "vue";
const {
  proxy
} = getCurrentInstance();
const state = reactive({
  model: null,
  stockAlerts: [],
  stockAlertCount: 0,
  stockAlertsLoading: false,
  stockAlertTimer: null
});
const {
  model,
  stockAlerts,
  stockAlertCount,
  stockAlertsLoading
} = toRefs(state);
async function loadStockAlerts() {
  state.stockAlertsLoading = true;
  try {
    const response = await proxy.$store.dispatch("stockAlertStore/getAlerts", {
      includeAll: false,
      defaultThreshold: 5
    });
    if (response?.code === 0) {
      state.stockAlerts = (response.data?.items || []).slice(0, 8);
      state.stockAlertCount = response.data?.alertCount || 0;

      const notificationKey = `stock-alert-count-${state.stockAlertCount}`;
      if (state.stockAlertCount > 0 && !sessionStorage.getItem(notificationKey)) {
        proxy.$store.dispatch("snackBarStore/addNotify", {
          message: `Có ${state.stockAlertCount} sản phẩm sắp hết hàng.`,
          variant: "warning"
        });
        sessionStorage.setItem(notificationKey, "1");
      }
    }
  } finally {
    state.stockAlertsLoading = false;
  }
}
function logout() {
  // 2. Xóa dữ liệu đăng nhập
  localStorage.removeItem("auth-user");
  localStorage.removeItem("token");
  localStorage.removeItem("user-token");
  if (window.axios) {
    delete window.axios.defaults.headers.common.Authorization;
  }

  // 5. Chuyển hướng về trang chủ nếu đang ở trang khác
  // 5. Chuyển hướng về trang chủ nếu đang ở trang khác
  window.location.href = "/dang-nhap";

  // 6. Có thể thêm thông báo cho người dùng
  // 6. Có thể thêm thông báo cho người dùng
  proxy.$store.dispatch("snackBarStore/addNotify", {
    message: "Đã đăng xuất thành công",
    variant: "success"
  });
}
function reloadPage() {
  window.location.href = "/";
}
function toggleSidebar1() {
  const body = document.body;
  body.classList.toggle("slide-nav");
}
function toggleSidebar() {
  const body = document.body;
  body.classList.toggle("mini-sidebar");
}
onMounted(() => {
  const auth = localStorage.getItem('auth-user');
  let authParse = JSON.parse(auth);
  if (authParse) {
    state.model = authParse;
  }
  loadStockAlerts();
  state.stockAlertTimer = window.setInterval(loadStockAlerts, 5 * 60 * 1000);
  // Add click event listener
  // Add click event listener
  proxy.$nextTick(() => {
    document.addEventListener("click", proxy.handleToggleClick);
  });

  // Add mouseover event listener
  // Add mouseover event listener
  document.addEventListener("mouseover", event => {
    event.stopPropagation();
    var body = document.body;
    var toggleBtn = document.getElementById("toggle_btn");
    var sidebar = document.getElementsByClassName("sidebar")[0];
    var subdropUL = document.getElementsByClassName("subdrop");
    if (body.classList.contains("mini-sidebar") && toggleBtn.style.display !== "none") {
      var target = event.target.closest(".sidebar");
      if (target) {
        body.classList.add("expand-menu");
        for (var i = 0; i < subdropUL.length; i++) {
          var ul = subdropUL[i].nextElementSibling;
          if (ul) {
            ul.style.display = "block";
          }
        }
      } else {
        body.classList.remove("expand-menu");
        for (var i = 0; i < subdropUL.length; i++) {
          var ul = subdropUL[i].nextElementSibling;
          if (ul) {
            ul.style.display = "none";
          }
        }
      }
      event.preventDefault();
    }
  });
});
onBeforeUnmount(() => {
  document.removeEventListener("click", proxy.handleToggleClick);
  if (state.stockAlertTimer) {
    window.clearInterval(state.stockAlertTimer);
  }
});
</script>

<style scoped>
.notification-badge {
  position: absolute;
  top: 4px;
  right: 1px;
  background: #dc3545;
  color: #fff;
  font-size: 10px;
}

.stock-alert-dropdown {
  width: 340px;
  max-height: 480px;
  overflow-y: auto;
}

.stock-alert-item {
  display: flex;
  gap: 10px;
  white-space: normal;
  align-items: center;
}

.alert-icon {
  width: 32px;
  height: 32px;
  border-radius: 50%;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  flex: 0 0 auto;
}

.alert-icon.warning {
  color: #856404;
  background: #fff3cd;
}

.alert-icon.critical {
  color: #fff;
  background: #dc3545;
}
</style>

