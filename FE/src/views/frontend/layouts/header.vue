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
              <img src="@/assets/img/caulong/logo/logo_HBTShop-removebg.png" class="img-fluid" alt="Logo" />
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
                <img src="@/assets/img/caulong/logo/logo_HBTShop-removebg.png" class="img-fluid" alt="Logo" />
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

<script>
import { notifyModel } from "@/models/notifyModel";

export default {
  data() {
    return {
      isScrolled: false,
      isSidebarOpen: false,
      isVisible: false,
      currentUserAuth: null,
      cartItems: [],
      cartTotal: 0,
      cartCount: 0,
      showDropdown: false,
      defaultAvatar: require('@/assets/img/caulong/logo/logo_HBTShop-removebg.png'),
      defaultProductImage: require('@/assets/img/caulong/logo/logo_default.png')
    };
  },
  mounted() {
    window.addEventListener("scroll", this.handleScroll);
    this.loadAuthUser();
    this.loadCartData();
    window.addEventListener("storage", this.handleStorageChange);
    window.addEventListener("cart-updated", this.loadCartData);
  },
  beforeUnmount() {
    window.removeEventListener("scroll", this.handleScroll);
    window.removeEventListener("storage", this.handleStorageChange);
    window.removeEventListener("cart-updated", this.loadCartData);
  },
  created() {
    this.closeSidebar();
  },
  methods: {
    loadAuthUser() {
      const authUser = localStorage.getItem("auth-user");
      this.currentUserAuth = authUser ? JSON.parse(authUser) : null;
    },
    loadCartData() {
      // Ưu tiên lấy từ auth-user nếu đã đăng nhập
      let cartData = [];
      const authUser = JSON.parse(localStorage.getItem("auth-user"));
      
      if (authUser?.cart) {
        cartData = authUser.cart;
      } else {
        cartData = JSON.parse(localStorage.getItem("cart")) || [];
      }

      this.cartItems = cartData;
      this.cartTotal = this.formatCurrency(
        cartData.reduce((sum, item) => sum + item.price * item.quantity, 0)
      );
      this.cartCount = cartData.reduce((total, item) => total + item.quantity, 0);
    },
    handleStorageChange(event) {
      if (event.key === "auth-user" || event.key === "cart") {
        this.loadAuthUser();
        this.loadCartData();
      }
    },
    removeCartItem(index) {
      const authUser = JSON.parse(localStorage.getItem("auth-user"));
      
      if (authUser?.cart) {
        authUser.cart.splice(index, 1);
        localStorage.setItem("auth-user", JSON.stringify(authUser));
      } else {
        const cart = JSON.parse(localStorage.getItem("cart")) || [];
        cart.splice(index, 1);
        localStorage.setItem("cart", JSON.stringify(cart));
      }
      
      this.loadCartData();
      window.dispatchEvent(new CustomEvent("cart-updated"));
    },
    updateQuantity(index, amount) {
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
        
        this.loadCartData();
        window.dispatchEvent(new CustomEvent("cart-updated"));
      }
    },
    formatCurrency(value) {
      if (!value) return "0 đ";
      return new Intl.NumberFormat("vi-VN", { style: "currency", currency: "VND" })
        .format(value)
        .replace("₫", "đ");
    },
    logout() {
      // 1. Xóa toàn bộ dữ liệu liên quan đến giỏ hàng
      localStorage.removeItem("cart");
      
      // 2. Xóa dữ liệu đăng nhập
      localStorage.removeItem("auth-user");
      localStorage.removeItem("token");
      localStorage.removeItem("user-token");
      
      // 3. Reset dữ liệu trong component
      this.currentUserAuth = null;
      this.cartItems = [];
      this.cartCount = 0;
      this.cartTotal = this.formatCurrency(0);
      
      // 4. Thông báo cho các component khác biết giỏ hàng đã thay đổi
      window.dispatchEvent(new CustomEvent("cart-updated"));
      
      // 5. Chuyển hướng về trang chủ nếu đang ở trang khác
      if (this.$route.path !== "/") {
        this.$router.push("/");
      }
      
      // 6. Có thể thêm thông báo cho người dùng
      this.$store.dispatch("snackBarStore/addNotify", {
        message: "Đã đăng xuất thành công",
        variant: "success"
      });
    },
    handleCheckout() {
      if (!this.currentUserAuth) {
        // Lưu redirect URL để quay lại sau khi đăng nhập
        localStorage.setItem("redirect-after-login", "/thanh-toan");
        
        // Hiển thị thông báo
        this.$store.dispatch("snackBarStore/addNotify", {
          message: "Vui lòng đăng nhập để thanh toán",
          variant: "warning"
        });
        
        // Chuyển đến trang đăng nhập
        this.$router.push("/login");
        return;
      }
      
      // Nếu đã đăng nhập, chuyển đến trang thanh toán
      this.$router.push("/thanh-toan");
    },
    handleScroll() {
      this.isScrolled = window.scrollY > 35;
    },
    toggleSidebar() {
      this.isSidebarOpen = !this.isSidebarOpen;
      document.documentElement.classList.toggle("menu-opened");
    },
    closeSidebar() {
      this.isSidebarOpen = false;
      document.documentElement.classList.remove("menu-opened");
    }
  },
  computed: {
    isHomeOneRoute() {
      return (
        this.$route.path === "/" ||
        this.$route.path.includes("/san-pham-chi-tiet/") ||
        this.$route.path === "/san-pham-vot" ||
        this.$route.path.includes("/san-pham/") ||
        this.$route.path === "/gio-hang" ||
        this.$route.path === "/dang-ky" ||
        this.$route.path === "/don-hang" ||
        this.$route.path === "/thong-tin-ca-nhan" ||
        this.$route.path.includes("/don-hang/chi-tiet/") ||
        this.$route.path === "/login"
      );
    }
  }
};
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