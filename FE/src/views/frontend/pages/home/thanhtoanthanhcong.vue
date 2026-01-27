<template>
    <div class="payment-result-container">
      <b-container class="text-center">
        <b-row class="justify-content-center">
          <b-col cols="12" md="8" lg="6">
            <b-card class="success-card shadow">
              <div class="icon-container mb-4">
                <b-icon-check-circle-fill variant="success" font-scale="4"></b-icon-check-circle-fill>
              </div>
              <h2 class="mb-3">Thanh toán thành công!</h2>
              <p class="text-muted mb-4">
                Cảm ơn bạn đã mua hàng. Đơn hàng của bạn đang được xử lý thành công.
              </p>
              
              <!-- <div v-if="orderInfo" class="order-details mb-4">
                <h5>Thông tin đơn hàng</h5>
                <p><strong>Mã đơn hàng:</strong> {{ orderInfo.orderId }}</p>
                <p><strong>Số tiền:</strong> {{ formatCurrency(orderInfo.amount) }}</p>
                <p><strong>Phương thức:</strong> {{ orderInfo.paymentMethod }}</p>
              </div> -->
              
              <div class="d-flex justify-content-center gap-3">
                <b-button 
                  variant="outline-primary" 
                  @click="goToOrderDetail"
                  v-if="orderInfo?.orderId"
                >
                  Xem chi tiết đơn hàng
                </b-button>
                <b-button variant="primary" @click="goToHome">
                  <b-icon-house-door-fill class="mr-2"></b-icon-house-door-fill>
                  Về trang chủ
                </b-button>
              </div>
            </b-card>
            
            <!-- <div class="mt-4">
              <p class="text-muted small">
                Bạn sẽ nhận được email xác nhận đơn hàng trong ít phút.
                Nếu cần hỗ trợ, vui lòng liên hệ <a href="mailto:support@yourstore.com">support@yourstore.com</a>
              </p>
            </div> -->
          </b-col>
        </b-row>
      </b-container>
    </div>
  </template>
  
  <script>
  export default {
    name: 'SuccessPayment',
    data() {
      return {
        orderInfo: null
      }
    },
    created() {
      // Lấy thông tin đơn hàng từ route query hoặc store
      this.orderInfo = {
        orderId: this.$route.query.orderId,
        amount: this.$route.query.amount,
        paymentMethod: 'VNPAY'
      }
      
      
      // Hoặc lấy từ Vuex store nếu bạn lưu ở đó
      // this.orderInfo = this.$store.state.payment.lastOrder
    },
    methods: {
      formatCurrency(value) {
        return new Intl.NumberFormat('vi-VN', { 
          style: 'currency', 
          currency: 'VND' 
        }).format(value || 0)
      },
      goToHome() {
        this.$router.push('/')
      },
      goToOrderDetail() {
        if (this.orderInfo?.orderId) {
          this.$router.push(`/don-hang/${this.orderInfo.orderId}`)
        }
      }
    }
  }
  </script>
  
  <style scoped>
  .payment-result-container {
    padding: 3rem 0;
    min-height: 70vh;
  }
  
  .success-card {
    border: none;
    border-top: 5px solid var(--success);
  }
  
  .icon-container {
    color: var(--success);
  }
  
  .order-details {
    background-color: #f8f9fa;
    padding: 1.5rem;
    border-radius: 0.5rem;
    text-align: left;
  }
  </style>