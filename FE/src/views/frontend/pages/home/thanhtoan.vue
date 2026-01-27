<template>
  <div class="checkout-container">
    <div class="row">
      <div class="col-md-8">
        <div class="card mb-4">
          <div class="card-header text-white" style="background-color: #F5E7B2;">
            <h5 style="margin-bottom: 0;">Thông tin mua hàng</h5>
          </div>
          <div class="card-body">
            <b-form @submit.prevent="submitOrder">
              <b-form-group label="Họ và tên" label-for="fullName">
                <b-form-input
                  id="fullName"
                  v-model="form.fullName"
                  required
                ></b-form-input>
              </b-form-group>
              
              <b-form-group label="Email" label-for="email">
                <b-form-input
                  id="email"
                  v-model="form.email"
                  type="email"
                  required
                ></b-form-input>
              </b-form-group>

              <b-form-group label="Số điện thoại" label-for="phone">
                <b-form-input
                  id="phone"
                  v-model="form.phone"
                  type="tel"
                  required
                ></b-form-input>
              </b-form-group>

              <!-- Phần địa chỉ mới -->
              <b-form-group label="Địa chỉ giao hàng">
                <div v-if="userAddresses.length > 0" class="mb-3">
                  <vue-multiselect
                    v-model="selectedAddressOption"
                    :options="addressOptions"
                    label="text"
                    track-by="value"
                    placeholder="Chọn địa chỉ"
                    @select="onAddressSelect"
                  ></vue-multiselect>
                </div>
                
                <b-button 
                  variant="outline-primary" 
                  size="sm"
                  @click="showAddressModal = true"
                  class="mb-3"
                >
                  <i class="fas fa-plus"></i> Thêm địa chỉ mới
                </b-button>
                
              </b-form-group>

              <b-form-group label="Ghi chú" label-for="note">
                <b-form-textarea
                  id="note"
                  v-model="form.note"
                  placeholder="Ghi chú về đơn hàng..."
                ></b-form-textarea>
              </b-form-group>

              <div class="payment-method mt-4">
                <h6>Phương thức thanh toán</h6>
                <div class="bank-transfer">
                  <i class="fas fa-university"></i> 
                  <strong class="ms-2">Chuyển khoản ngân hàng</strong>
                  <!-- <div class="bank-info mt-2">
                    <p>Ngân hàng: <strong>Vietcombank</strong></p>
                    <p>Số tài khoản: <strong>123456789</strong></p>
                    <p>Chủ tài khoản: <strong>Công ty TNHH HBT Shop</strong></p>
                    <p>Nội dung chuyển khoản: <strong>MH{{ new Date().getTime() }}</strong></p>
                  </div> -->
                </div>
              </div>
            </b-form>
          </div>
        </div>
      </div>

      <div class="col-md-4">
        <!-- Tóm tắt đơn hàng -->
        <div class="card sticky-top" style="top: 20px;">
          <div class="card-header text-white" style="background-color: #F5E7B2;">
            <h5 style="margin-bottom: 0;">Đơn hàng ({{ cartItems.length }} sản phẩm)</h5>
          </div>
          <div class="card-body">
            <div v-for="(item, index) in cartItems" :key="index" class="mb-3">
              <div class="d-flex">
                <img 
                  :src="item.imageUrl || defaultImage" 
                  alt="Ảnh sản phẩm"
                  class="product-thumbnail mr-3"
                  @error="handleImageError"
                >
                <div style="padding-left: 10px;">
                  <div>
                    <strong>{{ item.name }}</strong>
                  </div>
                  <span>{{ formatCurrency(item.price * item.quantity) }}</span>
                  <div class="text-muted small">{{ item.variant }}</div>
                  <div class="text-muted small">Số lượng: {{ item.quantity }}</div>
                </div>
              </div>
            </div>

            <hr>

            <div class="d-flex justify-content-between">
              <span>Tạm tính:</span>
              <span>{{ formatCurrency(subTotal) }}</span>
            </div>
            <div class="d-flex justify-content-between">
              <span>Phí vận chuyển:</span>
              <span>{{ formatCurrency(shippingFee) }}</span>
            </div>
            <hr>
            <div class="d-flex justify-content-between font-weight-bold">
              <span>Tổng cộng:</span>
              <span>{{ formatCurrency(totalAmount) }}</span>
            </div>

            <hr>

            <div class="d-flex justify-content-between">
              <router-link to="/gio-hang" class="btn btn-outline-secondary">
                <i class="fas fa-arrow-left"></i> Quay về giỏ hàng
              </router-link>
              <b-button @click="submitOrder" variant="primary">
                <i class="fas fa-shopping-bag"></i> ĐẶT HÀNG
              </b-button>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Modal thêm địa chỉ -->
    <b-modal v-model="showAddressModal" title="Thêm địa chỉ mới" hide-footer>
      <Form @submit="addNewAddress" :validation-schema="addressSchema">
        <div class="mb-3">
          <label class="text-left">Địa chỉ</label>
          <span style="color: red">&nbsp;*</span>
          <Field
            v-model="newAddress.address"
            name="address"
            type="text"
            class="form-control"
            placeholder="Nhập địa chỉ cụ thể"
          />
          <ErrorMessage name="address" class="text-danger small" />
        </div>
        
        <div class="mb-3">
          <label class="text-left">Tỉnh/Thành phố</label>
          <span style="color: red">&nbsp;*</span>
          <Field name="province" v-slot="{ field }">
            <VueMultiselect
              v-bind="field"
              v-model="newAddress.province"
              :options="listTinh"
              label="name"
              placeholder="Chọn tỉnh/thành"
              track-by="id"
            />
          </Field>
          <ErrorMessage name="province" class="text-danger small" />
        </div>
        
        <div class="mb-3">
          <label class="text-left">Quận/Huyện</label>
          <span style="color: red">&nbsp;*</span>
          <Field name="district" v-slot="{ field }">
            <VueMultiselect
              v-bind="field"
              v-model="newAddress.district"
              :options="listTP"
              :disabled="!newAddress.province"
              label="name"
              placeholder="Chọn quận/huyện"
              track-by="id"
            />
          </Field>
          <ErrorMessage name="district" class="text-danger small" />
        </div>
        
        <div class="mb-3">
          <label class="text-left">Phường/Xã</label>
          <Field name="town" v-slot="{ field }">
            <VueMultiselect
              v-bind="field"
              v-model="newAddress.town"
              :options="listPhuong"
              :disabled="!newAddress.district"
              label="name"
              placeholder="Chọn phường/xã"
              track-by="id"
            />
          </Field>
        </div>
        
        <!-- <b-form-checkbox v-model="newAddress.isDefault">
          Đặt làm địa chỉ mặc định
        </b-form-checkbox> -->
        
        <div class="text-end mt-3">
          <b-button type="submit" variant="primary">Lưu địa chỉ</b-button>
        </div>
      </Form>
    </b-modal>
  </div>
</template>

<script>
import { Form, Field, ErrorMessage } from "vee-validate";
import * as Yup from "yup";
import VueMultiselect from 'vue-multiselect';
import 'vue-multiselect/dist/vue-multiselect.css';
import {notifyModel} from "@/models/notifyModel";

export default {
  components: {
    Form,
    Field,
    ErrorMessage,
    VueMultiselect
  },
  data() {
    return {
      cartItems: [],
      form: {
        email: '',
        fullName: '',
        phone: '',
        province: null,
        district: null,
        ward: null,
        address: '',
        note: '',
        paymentMethod: 'bank'
      },
      showAddressModal: false,
      listTinh: [],
      listTP: [],
      listPhuong: [],
      itemsDiaChi: {
        province: null,
        district: null,
        town: null
      },
      newAddress: {
        address: '',
        province: null,
        district: null,
        town: null,
        isDefault: true
      },
      userAddresses: [],
      shippingFee: 30000,
      selectedAddress: null,
      selectedAddressOption: null, // Lưu object đầy đủ (dùng cho VueMultiselect)
      defaultImage: require('@/assets/img/caulong/logo/logo_HBTShop-removebg.png')
    }
  },
  setup() {
    const addressSchema = Yup.object().shape({
      address: Yup.string().required('Vui lòng nhập địa chỉ'),
      province: Yup.object().required('Vui lòng chọn tỉnh/thành phố'),
      district: Yup.object().required('Vui lòng chọn quận/huyện')
    });
    
    return {
      addressSchema
    };
  },
  computed: {
    subTotal() {
      return this.cartItems.reduce((total, item) => total + (item.price * item.quantity), 0)
    },
    totalAmount() {
      return this.subTotal + this.shippingFee
    },
    addressOptions() {
      return [
        ...this.userAddresses.map(addr => ({
          value: addr.id,
          text: `${addr.address}, ${addr.town}, ${addr.district}, ${addr.province}`,
          data: addr
        }))
      ];
    }
  },
  created() {
    this.loadUserData();
    this.loadCartItems();
    this.getListTinh();
    this.loadUserAddresses();
  },
  methods: {
    loadUserData() {
      const authUser = JSON.parse(localStorage.getItem('auth-user'));
      if (authUser) {
        this.form = {
          ...this.form,
          email: authUser.email || '',
          fullName: authUser.fullName || '',
          phone: authUser.phone || ''
        };
      }
    },
    
    loadCartItems() {
      const authUser = JSON.parse(localStorage.getItem('auth-user'));
      let cartData = [];
      
      if (authUser?.cart) {
        cartData = authUser.cart;
      } else {
        cartData = JSON.parse(localStorage.getItem('cart')) || [];
      }
      
      this.cartItems = cartData;
      console.log("DATA: ", this.cartItems);
      
    },
    
    async loadUserAddresses() {
      const authUser = JSON.parse(localStorage.getItem('auth-user'));
      if (authUser?.id) {
        try {
          const res = await this.$store.dispatch("diaChiStore/getAddressCustomer", { id: authUser.id });
          if (res?.code === 0) {
            this.userAddresses = res.data || [];
          }
        } catch (error) {
          console.error('Lỗi khi lấy địa chỉ:', error);
        }
      }
    },
    
    async getListTinh() {
      await this.$store.dispatch("tinhStore/getAll").then((res) => {
        if (res?.code === 0) {
          this.listTinh = res.data || [];
        }
      });
    },

    async getListTP(id) {
      await this.$store.dispatch("huyenStore/getAll", {id: id}).then((res) => {
        this.listTP = res.data || [];
      });
    },

    async getListPhuong(id) {
      await this.$store.dispatch("phuongStore/getAll", {id: id}).then((res) => {
        this.listPhuong = res.data || [];
      });
    },
    
    onAddressSelect(selectedOption) {
      // Chỉ lấy ID từ data của option đã chọn
      this.selectedAddress = selectedOption.data.id; 
      
      // Điền thông tin vào form (nếu cần)
      if (selectedOption.data) {
        this.fillAddressForm(selectedOption.data);
      }
    },
    
    fillAddressForm(address) {
      this.form.address = address.address;
      this.itemsDiaChi.province = address.province;
      this.itemsDiaChi.district = address.district;
      this.itemsDiaChi.town = address.town;
    },
    
    async addNewAddress() {
      const authUser = JSON.parse(localStorage.getItem('auth-user'));
      if (!authUser?.id) return;

      try {
        const addressData = {
          customerId: authUser.id,
          address: this.newAddress.address,
          provinceId: this.newAddress.province?.id,
          districtId: this.newAddress.district?.id,
          townId: this.newAddress.town?.id,
          isDefault: this.newAddress.isDefault
        };

        const res = await this.$store.dispatch("diaChiStore/createCustomer", addressData);
        
        if (res?.code === 0) {
          this.showAddressModal = false;
          this.loadUserAddresses();
          this.$store.dispatch("snackBarStore/addNotify", notifyModel.addMessage(res));
          
          // Tự động chọn địa chỉ vừa thêm
          if (res.data) {
            this.fillAddressForm({
              address: res.data.address,
              province: this.newAddress.province,
              district: this.newAddress.district,
              town: this.newAddress.town
            });
          }
          
          // Reset form
          this.newAddress = {
            address: '',
            province: null,
            district: null,
            town: null,
            isDefault: true
          };
        }
      } catch (error) {
        console.error('Lỗi khi thêm địa chỉ:', error);
        this.$store.dispatch("snackBarStore/addNotify", {
          message: 'Thêm địa chỉ thất bại',
          variant: 'danger'
        });
      }
    },
    
    formatCurrency(value) {
      return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(value)
    },

    async submitOrder() {
      try {
        const authUser = JSON.parse(localStorage.getItem('auth-user'));
        
        // Kiểm tra xem đã chọn địa chỉ chưa
        if (!this.selectedAddress) {
          this.$store.dispatch("snackBarStore/addNotify", {
            message: 'Vui lòng chọn địa chỉ giao hàng',
            variant: 'danger'
          });
          return;
        }

        const orderData = {
          customerId: authUser.id,
          totalAmount: this.totalAmount,
          addressId: this.selectedAddress, // Chỉ gửi id của địa chỉ
          listOrderItems: this.cartItems.map(item => ({
            productsId: item.id,
            name: item.name,
            price: item.price,
            quantity: item.quantity,
            imageUrl: item.imageUrl
          })),
          paymentMethod: this.form.paymentMethod
        };

        // Gọi API tạo đơn hàng
        const res = await this.$store.dispatch("odersStore/createCustomer", orderData);
        
        if (res?.code === 0) {
          // Kiểm tra nếu có URL thanh toán (VNPay)
          if (res.data?.url) {
            // Điều hướng sang URL thanh toán
            window.location.href = res.data.url; 
            // Hoặc mở tab mới (nếu cần):
            // window.open(res.data.url, '_blank');
          } else {
            // Nếu không có URL (thanh toán thường), chuyển đến trang cảm ơn
            this.$router.push({
              path: '/cam-on',
              query: { orderId: res.data?.orderNumber || '' }
            });
          }

          // Xóa giỏ hàng
          if (authUser) {
            authUser.cart = [];
            localStorage.setItem('auth-user', JSON.stringify(authUser));
          } else {
            localStorage.removeItem('cart');
          }
          
          window.dispatchEvent(new CustomEvent('cart-updated'));
          
        }
      } catch (error) {
        console.error('Lỗi khi đặt hàng:', error);
        this.$store.dispatch("snackBarStore/addNotify", {
          message: error.message || 'Đặt hàng thất bại',
          variant: 'danger'
        });
      }
    },
    
    
  },
  watch: {
    'itemsDiaChi.province': {
      handler(val) {
        if (val) {
          this.getListTP(val.id);
          this.itemsDiaChi.district = null;
          this.itemsDiaChi.town = null;
        }
      },
      deep: true
    },
    'itemsDiaChi.district': {
      handler(val) {
        if (val) {
          this.getListPhuong(val.id);
          this.itemsDiaChi.town = null;
        }
      },
      deep: true
    },
    'newAddress.province': {
      handler(val) {
        if (val) {
          this.getListTP(val.id);
          this.newAddress.district = null;
          this.newAddress.town = null;
        }
      },
      deep: true
    },
    'newAddress.district': {
      handler(val) {
        if (val) {
          this.getListPhuong(val.id);
          this.newAddress.town = null;
        }
      },
      deep: true
    }
  }
}
</script>

<style scoped>
.checkout-container {
  max-width: 1200px;
  margin: 20px auto;
  padding: 0 15px;
}

.card-header {
  font-weight: 600;
}

.product-thumbnail {
  width: 60px;
  height: 60px;
  object-fit: cover;
  border: 1px solid #eee;
}

.bank-transfer {
  background-color: #f8f9fa;
  padding: 15px;
  border-radius: 5px;
  border: 1px solid #ddd;
}

.address-form {
  background-color: #f8f9fa;
  padding: 15px;
  border-radius: 5px;
  margin-top: 10px;
}
</style>