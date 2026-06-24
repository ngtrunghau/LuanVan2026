<template>
  <div class="main-wrapper">
    <layoutheader :class="{ 'header-space': showHeaderSpace }" ref="header" />
    <div class="cart-view" style="padding-top: 150px;">
      <h3>Giỏ hàng</h3>
      <div class="custom-new-table">
        <div class="table-responsive">
          <table class="table table-hover table-center mb-0">
            <thead>
              <tr>
                <th style="text-align: center; width: 20%;">Ảnh</th>
                <th style="text-align: left; width: 20%;">Sản phẩm</th>
                <th style="text-align: center; width: 20%;">Đơn giá</th>
                <th style="text-align: center; width: 10%;">Số lượng</th>
                <th style="text-align: center; width: 20%;">Thành tiền</th>
                <th style="text-align: center; width: 10%;">Xử lý</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="(item, index) in cart" :key="index">
                <td style="text-align: center">
                  <img 
                    :src="item.imageUrl || defaultImage" 
                    width="100" 
                    alt="Product image" 
                    style="border: 1px solid #ccc;"
                    @error="handleImageError"
                  />
                  <!-- <div style="margin-left: 10px; text-align: left;">
                    <p>{{ item.name }}</p>
                    <b-button 
                      variant="link" 
                      size="sm" 
                      @click="removeFromCart(index)" 
                      style="color: #888; font-size: 12px;"
                    >
                      <i class="fa fa-solid fa-trash-can"></i> Bỏ sản phẩm
                    </b-button>
                  </div> -->
                </td>
                <td style="text-align: left">
                  <p>{{ item.name }}</p>
                </td>
                <td style="text-align: center">{{ formatCurrency(item.price) }}</td>
                <td style="text-align: center">
                  <div class="quantity-control">
                    <button 
                      class="btn-quantity" 
                      @click="updateQuantity(index, -1)" 
                      :disabled="item.quantity <= 1"
                    >
                      <i class="fa fa-solid fa-minus"></i>
                    </button>
                    <span class="quantity-value">{{ item.quantity }}</span>
                    <button class="btn-quantity" @click="updateQuantity(index, 1)">
                      <i class="fa fa-solid fa-plus"></i>
                    </button>
                  </div>
                </td>
                <td style="text-align: center">{{ formatCurrency(item.price * item.quantity) }}</td>
                <td style="text-align: center">
                  <b-button 
                      variant="link" 
                      size="sm" 
                      @click="removeFromCart(index)" 
                      style="color: #000; font-size: 20px;"
                    >
                      <i class="fa fa-solid fa-trash-can"></i>
                    </b-button>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>

      <div class="total mt-3">
        <strong>Tổng tiền: {{ formatCurrency(totalPrice) }}</strong>
      </div>

      <div class="d-flex justify-content-between mt-3">
        <router-link to="/san-pham" class="btn btn-outline-danger">
          <i class="fa fa-solid fa-basket-shopping"></i> Tiếp tục mua hàng
        </router-link>
        <router-link to="/thanh-toan" >
          <button class="btn btn-danger" @click="checkout">
            <i class="fa fa-solid fa-cart-shopping"></i> Tiến hành đặt hàng
          </button>
        </router-link>
        
      </div>
    </div>
  </div>
</template>

<script setup>
import { computed, getCurrentInstance, onBeforeUnmount, onMounted, reactive, toRefs } from "vue";
const {
  proxy
} = getCurrentInstance();
const state = reactive({
  cart: [],
  defaultImage: require('@/assets/img/caulong/logo/logoNTH_removeBackground.png'),
  isLoggedIn: false
});
const {
  cart,
  defaultImage,
  isLoggedIn
} = toRefs(state);
function formatCurrency(value) {
  return value ? value.toLocaleString("vi-VN") + "đ" : "0đ";
}
function removeFromCart(index) {
  const newCart = [...state.cart];
  newCart.splice(index, 1);
  updateCart(newCart);
}
function updateQuantity(index, change) {
  const newCart = [...state.cart];
  const newQuantity = newCart[index].quantity + change;
  if (newQuantity > 0) {
    newCart[index].quantity = newQuantity;
    updateCart(newCart);
  }
}
function updateCart(cartData) {
  // Cập nhật cả localStorage và auth-user nếu đã đăng nhập
  localStorage.setItem("cart", JSON.stringify(cartData));
  if (state.isLoggedIn) {
    const authUser = JSON.parse(localStorage.getItem('auth-user'));
    if (authUser) {
      authUser.cart = cartData;
      localStorage.setItem('auth-user', JSON.stringify(authUser));
    }
  }
  state.cart = cartData;
  window.dispatchEvent(new CustomEvent("cart-updated"));
}
function checkout() {
  proxy.$router.push('/checkout');
}
function loadCart() {
  // Kiểm tra người dùng đăng nhập
  const authUser = JSON.parse(localStorage.getItem('auth-user'));
  state.isLoggedIn = !!authUser;

  // Ưu tiên lấy giỏ hàng từ auth-user nếu có
  // Ưu tiên lấy giỏ hàng từ auth-user nếu có
  if (authUser?.cart) {
    state.cart = authUser.cart;
  } else {
    state.cart = JSON.parse(localStorage.getItem("cart")) || [];
  }
}
function handleImageError(e) {
  e.target.src = state.defaultImage;
}
const totalPrice = computed(() => {
  return state.cart.reduce((total, item) => total + item.price * item.quantity, 0);
});
onMounted(() => {
  loadCart();
  window.addEventListener('cart-updated', loadCart);
});
onBeforeUnmount(() => {
  window.removeEventListener('cart-updated', loadCart);
});
</script>

<style scoped>
.cart-view {
  max-width: 1200px;
  margin: 0 auto;
  padding: 20px;
}

.quantity-control {
  display: inline-flex;
  align-items: center;
  border: 1px solid #ddd;
  border-radius: 4px;
  padding: 2px;
}

.btn-quantity {
  width: 28px;
  height: 28px;
  border: none;
  background: #f8f9fa;
  cursor: pointer;
}

.btn-quantity:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.quantity-value {
  min-width: 30px;
  text-align: center;
}

.total {
  font-size: 1.2rem;
  text-align: right;
  padding: 10px;
  border-top: 1px solid #eee;
}

.btn-outline-danger {
  color: #DC1E35;
  border-color: #DC1E35;
}

.btn-outline-danger:hover {
  background-color: #DC1E35;
  color: white;
}

.btn-danger {
  background-color: #DC1E35;
  border-color: #DC1E35;
}

.btn-danger:hover {
  background-color: #c11a2e;
  border-color: #c11a2e;
}
</style>

