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

<script>
export default {
  data() {
    return {
      cart: [],
      defaultImage: require('@/assets/img/caulong/logo/logo_HBTShop-removebg.png'),
      isLoggedIn: false
    };
  },

  computed: {
    totalPrice() {
      return this.cart.reduce((total, item) => total + item.price * item.quantity, 0);
    }
  },

  methods: {
    formatCurrency(value) {
      return value ? value.toLocaleString("vi-VN") + "đ" : "0đ";
    },

    removeFromCart(index) {
      const newCart = [...this.cart];
      newCart.splice(index, 1);
      this.updateCart(newCart);
    },

    updateQuantity(index, change) {
      const newCart = [...this.cart];
      const newQuantity = newCart[index].quantity + change;
      
      if (newQuantity > 0) {
        newCart[index].quantity = newQuantity;
        this.updateCart(newCart);
      }
    },

    updateCart(cartData) {
      // Cập nhật cả localStorage và auth-user nếu đã đăng nhập
      localStorage.setItem("cart", JSON.stringify(cartData));
      
      if (this.isLoggedIn) {
        const authUser = JSON.parse(localStorage.getItem('auth-user'));
        if (authUser) {
          authUser.cart = cartData;
          localStorage.setItem('auth-user', JSON.stringify(authUser));
        }
      }
      
      this.cart = cartData;
      window.dispatchEvent(new CustomEvent("cart-updated"));
    },

    checkout() {
      this.$router.push('/checkout');
    },

    loadCart() {
      // Kiểm tra người dùng đăng nhập
      const authUser = JSON.parse(localStorage.getItem('auth-user'));
      this.isLoggedIn = !!authUser;
      
      // Ưu tiên lấy giỏ hàng từ auth-user nếu có
      if (authUser?.cart) {
        this.cart = authUser.cart;
      } else {
        this.cart = JSON.parse(localStorage.getItem("cart")) || [];
      }
    },

    handleImageError(e) {
      e.target.src = this.defaultImage;
    }
  },

  mounted() {
    this.loadCart();
    window.addEventListener('cart-updated', this.loadCart);
  },

  beforeUnmount() {
    window.removeEventListener('cart-updated', this.loadCart);
  }
};
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
