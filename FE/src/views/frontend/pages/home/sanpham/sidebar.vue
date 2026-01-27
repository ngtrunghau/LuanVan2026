<template>
  <div>
    <!-- Product Card -->
    <div class="card search-filter">
      <div class="card-body">
        <div class="clini-infos mt-0">
          <h2>{{ formatCurrency(list.price) }}</h2>
        </div>
        <div v-if="list.stockQuantity == 0">
          <span class="badge badge-warning">Hết hàng</span>
        </div>
        <div v-else>
          <span class="badge badge-primary">Còn hàng</span>
          <span class="mx-2">(Số lượng: {{ list.stockQuantity }})</span>
          <div class="clinic-details mt-4 d-flex justify-content-center">
            <b-button class="apt-btn add-cart" @click="addToCart">
              <i class="fa fa-solid fa-basket-shopping"></i>
              Thêm giỏ hàng
            </b-button>
          </div>
        </div>

        
      </div>
    </div>
    
    <!-- Benefits Section -->
    <div class="card search-filter">
      <div class="card-body">
        <div class="card flex-fill mt-0 mb-0">
          <ul class="list-group list-group-flush benifits-col">
            <li class="list-group-item d-flex align-items-center">
              <div><i class="fas fa-shipping-fast"></i></div>
              <div>Free Shipping<br /><span class="text-sm">Sản phẩm từ 500K</span></div>
            </li>
            <li class="list-group-item d-flex align-items-center">
              <div><i class="far fa-question-circle"></i></div>
              <div>Hỗ trợ 24/7<br /><span class="text-sm">Gọi mọi lúc</span></div>
            </li>
            <li class="list-group-item d-flex align-items-center">
              <div><i class="fas fa-hands"></i></div>
              <div>An toàn 100%<br /><span class="text-sm">Thanh toán an toàn</span></div>
            </li>
            <li class="list-group-item d-flex align-items-center">
              <div><i class="fas fa-tag"></i></div>
              <div>Ưu đãi hấp dẫn<br /><span class="text-sm">Giảm giá lên tới 90%</span></div>
            </li>
          </ul>
        </div>
      </div>
    </div>

    <!-- Cart Modal -->
    <b-modal v-model="showCartModal" title="Giỏ hàng của bạn" hide-footer centered size="xl" style="z-index: 9999;">
      <div v-if="cart.length">
        <div class="table-responsive">
          <table class="table table-hover table-center mb-0">
            <thead>
              <tr>
                <th style="text-align: left; width: 40%;">Sản phẩm</th>
                <th style="text-align: center; width: 20%;">Đơn giá</th>
                <th style="text-align: center; width: 20%;">Số lượng</th>
                <th style="text-align: center; width: 20%;">Thành tiền</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="(item, index) in cart" :key="index">
                <td style="display: flex; align-items: center;">
                  <img :src="item.imageUrl || defaultImage" width="100" alt="Product image" style="border: 1px solid #ccc;"/>
                  <div style="margin-left: 10px; text-align: left;">
                    <p>{{ item.name }}</p>
                    <b-button variant="link" size="sm" @click="removeFromCart(index)" style="color: #888; font-size: 12px;">
                      <i class="fa fa-solid fa-trash-can"></i> Bỏ sản phẩm
                    </b-button>
                  </div>
                </td>
                <td style="text-align: center">{{ formatCurrency(item.price) }}</td>
                <td style="text-align: center">
                  <div class="quantity-control">
                    <button class="btn-quantity" @click="updateQuantity(index, -1)" :disabled="item.quantity <= 1">
                      <i class="fa fa-solid fa-minus"></i>
                    </button>
                    <span class="quantity-value">{{ item.quantity }}</span>
                    <button class="btn-quantity" @click="updateQuantity(index, 1)">
                      <i class="fa fa-solid fa-plus"></i>
                    </button>
                  </div>
                </td>
                <td style="text-align: center">{{ formatCurrency(item.price * item.quantity) }}</td>
              </tr>
            </tbody>
          </table>
        </div>
        <div class="total mt-3">
          <strong>Tổng tiền: {{ formatCurrency(totalPrice) }}</strong>
        </div>
      </div>
      <div v-else class="text-center">
        <i class="fas fa-shopping-cart fa-4x text-muted mb-3"></i>
        <p>Không có sản phẩm nào trong giỏ hàng của bạn!</p>
      </div>

      <div v-if="cart.length" class="d-flex justify-content-between mt-3">
        <b-button variant="outline-danger" @click="showCartModal = false">
          <i class="fa fa-solid fa-basket-shopping"></i> Tiếp tục mua hàng
        </b-button>
        <b-button variant="danger" @click="proceedToCheckout">
          <i class="fa fa-solid fa-cart-shopping"></i> Tiến hành đặt hàng
        </b-button>
      </div>
    </b-modal>
  </div>
</template>

<script>
export default {
  props: {
    sidebar: { 
      type: Object,
      default: () => ({})
    }
  },
  
  data() {
    return {
      quantityValue: 1,
      list: {},
      cart: [],
      showCartModal: false,
      defaultImage: require('@/assets/img/caulong/logo/logo_HBTShop-removebg.png')
    };
  },

  computed: {
    totalPrice() {
      return this.cart.reduce((total, item) => total + item.price * item.quantity, 0);
    },
    isLoggedIn() {
      return !!localStorage.getItem("auth-user");
    }
  },

  methods: {
    formatCurrency(value) {
      return value ? value.toLocaleString("vi-VN") + "đ" : "0đ";
    },

    addToCart() {
      if (!this.list.id) {
        this.showMessage('Sản phẩm không hợp lệ!', 'error');
        return;
      }

      const cartData = this.getCartData();
      const existingProduct = cartData.find(item => item.id === this.list.id);

      if (existingProduct) {
        existingProduct.quantity += this.quantityValue;
      } else {
        cartData.push({
          id: this.list.id,
          name: this.list.name,
          price: this.list.price,
          quantity: this.quantityValue,
          imageUrl: this.list.imageUrl || this.defaultImage
        });
      }

      this.updateCart(cartData);
      this.showCartModal = true;
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

    getCartData() {
      if (this.isLoggedIn) {
        const authUser = JSON.parse(localStorage.getItem("auth-user"));
        return authUser?.cart || [];
      }
      return JSON.parse(localStorage.getItem("cart")) || [];
      
    },

    updateCart(cartData) {
      if (this.isLoggedIn) {
        const authUser = JSON.parse(localStorage.getItem("auth-user"));
        authUser.cart = cartData;
        localStorage.setItem("auth-user", JSON.stringify(authUser));
      }
      
      localStorage.setItem("cart", JSON.stringify(cartData));
      this.cart = cartData;
      window.dispatchEvent(new CustomEvent("cart-updated"));
    },

    proceedToCheckout() {
      // Kiểm tra đăng nhập
      const authUser = JSON.parse(localStorage.getItem("auth-user"));
      
      if (!authUser) {
        // Chưa đăng nhập - chuyển hướng đến trang đăng nhập
        // Lưu trữ URL hiện tại để redirect lại sau khi đăng nhập
        localStorage.setItem("redirect-after-login", "/thanh-toan");
        this.$router.push("/login");
        
        // Có thể thêm thông báo
        this.showMessage("Vui lòng đăng nhập để thanh toán", "warning");
        return;
      }
      
      // Đã đăng nhập - chuyển đến trang thanh toán
      this.$router.push("/thanh-toan");
    },

    loadCart() {
      this.cart = this.getCartData();
    },

    loadProduct() {
      if (Object.keys(this.sidebar).length > 0) {
        this.list = { ...this.sidebar };
      } else {
        const storedProduct = JSON.parse(localStorage.getItem("currentProduct"));
        if (storedProduct) this.list = { ...storedProduct };
      }
    },

    syncCartOnLogin() {
      const guestCart = JSON.parse(localStorage.getItem("cart")) || [];
      if (guestCart.length > 0 && this.isLoggedIn) {
        const authUser = JSON.parse(localStorage.getItem("auth-user"));
        const mergedCart = this.mergeCarts(authUser?.cart || [], guestCart);
        
        authUser.cart = mergedCart;
        localStorage.setItem("auth-user", JSON.stringify(authUser));
        localStorage.removeItem("cart");
        
        this.cart = mergedCart;
        this.showMessage('Đã đồng bộ giỏ hàng vào tài khoản', 'success');
      }
    },

    mergeCarts(userCart, guestCart) {
      const merged = [...userCart];
      
      guestCart.forEach(guestItem => {
        const existingItem = merged.find(item => item.id === guestItem.id);
        if (existingItem) {
          existingItem.quantity += guestItem.quantity;
        } else {
          merged.push(guestItem);
        }
      });
      
      return merged;
    },

    checkAuthAndSync() {
      if (this.isLoggedIn) {
        const authUser = JSON.parse(localStorage.getItem("auth-user"));
        if (!authUser.cart && JSON.parse(localStorage.getItem("cart"))) {
          this.syncCartOnLogin();
        }
      }
    },

    showMessage(message, type = 'success') {
      // Sử dụng hệ thống thông báo của bạn
      console[type === 'success' ? 'log' : 'error'](message);
      
      // Hoặc nếu dùng Vuex
      if (this.$store && this.$store.dispatch) {
        this.$store.dispatch("snackBarStore/addNotify", {
          message: message,
          variant: type
        });
      }
    }
  },

  watch: {
    sidebar: {
      handler(newVal) {
        if (newVal && Object.keys(newVal).length > 0) {
          this.list = { ...newVal };
          localStorage.setItem("currentProduct", JSON.stringify(newVal));
        }
      },
      immediate: true,
      deep: true
    }
  },

  mounted() {
    this.loadProduct();
    this.loadCart();
    window.addEventListener('storage', this.loadCart);
    window.$cartComponent = this;
    this.checkAuthAndSync();
  },

  beforeUnmount() {
    window.removeEventListener('storage', this.loadCart);
    delete window.$cartComponent;
  }
};
</script>


<style scoped>
.add-cart {
  background-color: #fff;
  border: 1px solid #DC1E35;
  color: #DC1E35;
  transition: all 0.3s;
}

.add-cart:hover {
  background-color: #DC1E35;
  color: white;
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
</style>