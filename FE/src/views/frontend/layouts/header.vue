<template>
  <!-- Header -->
  <header class="header header-fixed" v-if="isHomeOneRoute" :class="{ 'header-one': true, 'header-space': isScrolled }">
    <div class="">
      <nav class="navbar navbar-expand-lg header-nav">
        <div class="navbar-header header-menu">
          <div class="container navbar-header header-menu">
            <a id="mobile_btn" href="javascript:void(0);" @click="toggleSidebar">
              <span class="bar-icon">
                <span></span>
                <span></span>
                <span></span>
              </span>
            </a>
            <router-link to="/" class="logo">
              <img src="@/assets/img/caulong/logo/logoNTH_removeBackground.png" class="img-fluid" alt="Logo" />
            </router-link>

            <div style="display: flex">
              <div class="top-nav-search me-2">
                <form>
                  <input type="text" class="form-control" placeholder="Nhập thông tin tìm kiếm..." />
                  <b-button class="btn" type="submit"><i class="fa fa-search"></i></b-button>
                </form>
              </div>
              <div class="top-nav-button d-flex">
                <!-- Cart -->
                <li
                  class="nav-item dropdown noti-nav view-cart-header me-3 list-style-none d-flex align-items-center"
                  @mouseover="showDropdown = true"
                  @mouseleave="showDropdown = false"
                >
                  <router-link class="" :to="{ path: `/gio-hang` }">
                    <a href="javascript:;" class="nav-link p-0 position-relative">
                      <i class="fa-solid fa-cart-shopping"></i>
                      <small class="unread-msg1">{{ cartCount }}</small>
                    </a>
                  </router-link>
                  <div class="dropdown-menu notifications dropdown-menu-end" :class="{ show: showDropdown }">
                    <div class="shopping-cart">
                      <ul class="shopping-cart-items list-unstyled">
                        <li v-for="(item, index) in cartItems" :key="index" class="clearfix">
                          <div class="close-icon" @click="removeCartItem(index)">
                            <i class="fa-solid fa-circle-xmark"></i>
                          </div>
                          <router-link to="product-description">
                            <img class="avatar-img rounded" :src="item.imageUrl || defaultProductImage" alt="Product Image" />
                          </router-link>
                          <router-link to="product-description" class="item-name">{{ item.name }}</router-link>
                          <span class="item-price">{{ formatCurrency(item.price) }}</span>
                          <div class="quantity-control">
                            <button class="btn-quantity" @click="updateQuantity(index, -1)">
                              <i class="fa fa-solid fa-minus"></i>
                            </button>
                            <span class="quantity-value">{{ item.quantity }}</span>
                            <button class="btn-quantity" @click="updateQuantity(index, 1)">
                              <i class="fa fa-solid fa-plus"></i>
                            </button>
                          </div>
                        </li>
                      </ul>
                      <div class="booking-summary pt-3">
                        <div class="booking-item-wrap" v-if="cartItems.length > 0">
                          <ul class="booking-date">
                            <li>Tổng tiền <span>{{ cartTotal }}</span></li>
                          </ul>
                          <div class="booking-total">
                            <ul class="booking-total-list text-align">
                              <li>
                                <div class="clinic-booking pt-3">
                                  <router-link class="apt-btn" :to="{ path: `/gio-hang` }">
                                    Xem giỏ hàng
                                  </router-link>
                                </div>
                              </li>
                              <li>
                                <div class="clinic-booking pt-3">
                                  <a class="apt-btn" @click="handleCheckout">
                                    Thanh toán
                                  </a>
                                </div>
                              </li>
                            </ul>
                          </div>
                        </div>
                        <div v-else class="empty-cart-message">
                          <p>Giỏ hàng trống</p>
                        </div>
                      </div>
                    </div>
                  </div>
                </li>
                <!-- /Cart -->
                
                <!-- Auth Section -->
                <ul class="nav header-navbar-rht">
                  <li v-if="!currentUserAuth" class="register-btn">
                    <router-link :to="{ path: `/login` }" class="btn log-btn">
                      <i class="feather-lock"></i>
                      Đăng nhập
                    </router-link>
                  </li>
                  <li v-if="!currentUserAuth" class="register-btn">
                    <router-link :to="{ path: `/dang-ky` }" class="btn reg-btn">
                      <i class="feather-user"></i>
                      Đăng ký
                    </router-link>
                  </li>
                  
                  <li v-if="currentUserAuth" class="nav-item dropdown user-dropdown">
                    <a href="#" class="dropdown-toggle nav-link" data-bs-toggle="dropdown">
                      <span class="user-img">
                        <img :src="currentUserAuth.avatar || defaultAvatar" alt="User" style="width: 40px;">
                        <span class="status online"></span>
                      </span>
                    </a>
                    <div class="dropdown-menu dropdown-menu-end">
                      <div class="user-header">
                        <div class="avatar avatar-sm">
                          <img :src="currentUserAuth.avatar || defaultAvatar" alt="User" class="avatar-img rounded-circle" style="width: 40px;">
                        </div>
                        <div class="user-text">
                          <h6>{{ currentUserAuth.fullName || currentUserAuth.userName }}</h6>
                          <p class="text-muted mb-0">{{ currentUserAuth.role }}</p>
                        </div>
                      </div>
                      <router-link class="dropdown-item" to="/don-hang">
                        <i class="fa fa-solid fa-cart-shopping me-2"></i> Đơn hàng
                      </router-link>
                      <router-link class="dropdown-item" to="/thong-tin-ca-nhan">
                        <i class="fas fa-user me-2"></i> Thông tin cá nhân
                      </router-link>
                      <a class="dropdown-item" href="#" @click.prevent="logout">
                        <i class="fas fa-sign-out-alt me-2"></i> Đăng xuất
                      </a>
                    </div>
                  </li>
                </ul>
              </div>
            </div>
          </div>
        </div>

        <div class="container" style="width: 100%;">
          <div class="main-menu-wrapper">
            <div class="menu-header">
              <router-link to="/" class="menu-logo">
                <img src="@/assets/img/caulong/logo/logoNTH_removeBackground.png" class="img-fluid" alt="Logo" />
              </router-link>
              <a id="menu_close" class="menu-close" href="javascript:void(0);" @click="closeSidebar">
                <i class="fas fa-times"></i>
              </a>
            </div>
            <mainnav></mainnav>
          </div>
        </div>
      </nav>
    </div>
  </header>
  <!-- /Header -->
</template>

<script setup>
import { computed, getCurrentInstance, onBeforeUnmount, onMounted, reactive, toRefs } from "vue";
import { notifyModel } from "@/models/notifyModel";
const {
  proxy
} = getCurrentInstance();
const state = reactive({
  isScrolled: false,
  isSidebarOpen: false,
  isVisible: false,
  currentUserAuth: null,
  cartItems: [],
  cartTotal: 0,
  cartCount: 0,
  showDropdown: false,
  defaultAvatar: require('@/assets/img/caulong/logo/logoNTH_removeBackground.png'),
  defaultProductImage: require('@/assets/img/caulong/logo/logoNTH_removeBackground.png')
});
const {
  isScrolled,
  isSidebarOpen,
  isVisible,
  currentUserAuth,
  cartItems,
  cartTotal,
  cartCount,
  showDropdown,
  defaultAvatar,
  defaultProductImage
} = toRefs(state);
function loadAuthUser() {
  const authUser = localStorage.getItem("auth-user");
  state.currentUserAuth = authUser ? JSON.parse(authUser) : null;
}
function loadCartData() {
  // Ưu tiên lấy từ auth-user nếu đã đăng nhập
  let cartData = [];
  const authUser = JSON.parse(localStorage.getItem("auth-user"));
  if (authUser?.cart) {
    cartData = authUser.cart;
  } else {
    cartData = JSON.parse(localStorage.getItem("cart")) || [];
  }
  state.cartItems = cartData;
  state.cartTotal = formatCurrency(cartData.reduce((sum, item) => sum + item.price * item.quantity, 0));
  state.cartCount = cartData.reduce((total, item) => total + item.quantity, 0);
}
function handleStorageChange(event) {
  if (event.key === "auth-user" || event.key === "cart") {
    loadAuthUser();
    loadCartData();
  }
}
function removeCartItem(index) {
  const authUser = JSON.parse(localStorage.getItem("auth-user"));
  if (authUser?.cart) {
    authUser.cart.splice(index, 1);
    localStorage.setItem("auth-user", JSON.stringify(authUser));
  } else {
    const cart = JSON.parse(localStorage.getItem("cart")) || [];
    cart.splice(index, 1);
    localStorage.setItem("cart", JSON.stringify(cart));
  }
  loadCartData();
  window.dispatchEvent(new CustomEvent("cart-updated"));
}
function updateQuantity(index, amount) {
  const authUser = JSON.parse(localStorage.getItem("auth-user"));
  let cartItems = authUser?.cart || JSON.parse(localStorage.getItem("cart")) || [];
  if (cartItems[index]) {
    const newQuantity = cartItems[index].quantity + amount;
    if (newQuantity < 1) return;
    cartItems[index].quantity = newQuantity;
    if (authUser) {
      authUser.cart = cartItems;
      localStorage.setItem("auth-user", JSON.stringify(authUser));
    } else {
      localStorage.setItem("cart", JSON.stringify(cartItems));
    }
    loadCartData();
    window.dispatchEvent(new CustomEvent("cart-updated"));
  }
}
function formatCurrency(value) {
  if (!value) return "0 đ";
  return new Intl.NumberFormat("vi-VN", {
    style: "currency",
    currency: "VND"
  }).format(value).replace("₫", "đ");
}
function logout() {
  // 1. Xóa toàn bộ dữ liệu liên quan đến giỏ hàng
  localStorage.removeItem("cart");

  // 2. Xóa dữ liệu đăng nhập
  // 2. Xóa dữ liệu đăng nhập
  localStorage.removeItem("auth-user");
  localStorage.removeItem("token");
  localStorage.removeItem("user-token");
  if (window.axios) {
    delete window.axios.defaults.headers.common.Authorization;
  }

  // 3. Reset dữ liệu trong component
  // 3. Reset dữ liệu trong component
  state.currentUserAuth = null;
  state.cartItems = [];
  state.cartCount = 0;
  state.cartTotal = formatCurrency(0);

  // 4. Thông báo cho các component khác biết giỏ hàng đã thay đổi
  // 4. Thông báo cho các component khác biết giỏ hàng đã thay đổi
  window.dispatchEvent(new CustomEvent("cart-updated"));

  // 5. Chuyển hướng về trang chủ nếu đang ở trang khác
  // 5. Chuyển hướng về trang chủ nếu đang ở trang khác
  if (proxy.$route.path !== "/") {
    proxy.$router.push("/");
  }

  // 6. Có thể thêm thông báo cho người dùng
  // 6. Có thể thêm thông báo cho người dùng
  proxy.$store.dispatch("snackBarStore/addNotify", {
    message: "Đã đăng xuất thành công",
    variant: "success"
  });
}
function handleCheckout() {
  if (!state.currentUserAuth) {
    // Lưu redirect URL để quay lại sau khi đăng nhập
    localStorage.setItem("redirect-after-login", "/thanh-toan");

    // Hiển thị thông báo
    proxy.$store.dispatch("snackBarStore/addNotify", {
      message: "Vui lòng đăng nhập để thanh toán",
      variant: "warning"
    });

    // Chuyển đến trang đăng nhập
    proxy.$router.push("/login");
    return;
  }

  // Nếu đã đăng nhập, chuyển đến trang thanh toán
  // Nếu đã đăng nhập, chuyển đến trang thanh toán
  proxy.$router.push("/thanh-toan");
}
function handleScroll() {
  state.isScrolled = window.scrollY > 35;
}
function toggleSidebar() {
  state.isSidebarOpen = !state.isSidebarOpen;
  document.documentElement.classList.toggle("menu-opened");
}
function closeSidebar() {
  state.isSidebarOpen = false;
  document.documentElement.classList.remove("menu-opened");
}
const isHomeOneRoute = computed(() => {
  return proxy.$route.path === "/" || proxy.$route.path.includes("/san-pham-chi-tiet/") || proxy.$route.path === "/san-pham-vot" || proxy.$route.path.includes("/san-pham/") || proxy.$route.path === "/gio-hang" || proxy.$route.path === "/dang-ky" || proxy.$route.path === "/don-hang" || proxy.$route.path === "/thong-tin-ca-nhan" || proxy.$route.path.includes("/don-hang/chi-tiet/") || proxy.$route.path === "/login";
});
onMounted(() => {
  window.addEventListener("scroll", handleScroll);
  loadAuthUser();
  loadCartData();
  window.addEventListener("storage", handleStorageChange);
  window.addEventListener("cart-updated", loadCartData);
});
onBeforeUnmount(() => {
  window.removeEventListener("scroll", handleScroll);
  window.removeEventListener("storage", handleStorageChange);
  window.removeEventListener("cart-updated", loadCartData);
});
closeSidebar();
</script>

<style scoped>
/* Giữ nguyên các style hiện có */
.user-header {
  padding: 10px 15px;
}

.noti-nav .dropdown-menu {
  position: absolute;
  right: -10px;
  left: auto;
  top: 100%;
  z-index: 1050;
}

.quantity-control {
  display: flex;
  align-items: center;
  justify-content: flex-start;
  gap: 8px;
}

.btn-quantity {
  width: 20px;
  height: 20px;
  border: 1px solid #ccc;
  background: white;
  cursor: pointer;
  font-size: 8px;
  text-align: center;
  border-radius: 50%;
}

.btn-quantity:hover {
  background: #ddd;
}

.quantity-value {
  font-size: 14px;
  min-width: 20px;
  text-align: center;
}

@media screen and (max-width: 992px) {
  .header-one .logo.navbar-brand .img-fluid {
    /* height: 50px */
  }
  .btn-header {
    display: none
  }
}

.empty-cart-message {
  padding: 15px;
  text-align: center;
  color: red;
}
</style>
