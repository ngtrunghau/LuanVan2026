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
                <div>
                  <label class="d-flex align-items-center">
                    <input
                      type="radio"
                      class="me-2"
                      value="cod"
                      checked
                      disabled
                    />
                    <i class="fas fa-truck"></i>
                    <strong class="ms-2">Thanh toán khi nhận hàng</strong>
                  </label>
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
            <div class="input-group my-3">
              <input
                v-model.trim="promotionCode"
                class="form-control"
                placeholder="Nhập mã khuyến mãi"
                :disabled="isApplyingPromotion"
                @keyup.enter="applyPromotion"
              >
              <button
                class="btn btn-outline-primary"
                type="button"
                :disabled="isApplyingPromotion || !promotionCode"
                @click="applyPromotion"
              >
                {{ isApplyingPromotion ? 'Đang kiểm tra...' : 'Áp dụng' }}
              </button>
            </div>
            <div
              v-if="promotionMessage"
              class="small mb-2"
              :class="discountAmount > 0 ? 'text-success' : 'text-danger'"
            >
              {{ promotionMessage }}
            </div>
            <div
              v-if="discountAmount > 0"
              class="d-flex justify-content-between text-success"
            >
              <span>Khuyến mãi:</span>
              <span>-{{ formatCurrency(discountAmount) }}</span>
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
              <b-button
                @click="submitOrder"
                variant="primary"
                :disabled="cartItems.length === 0"
              >
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

<script setup>
import { computed, getCurrentInstance, reactive, toRefs, watch } from "vue";
import { Form, Field, ErrorMessage } from "vee-validate";
import * as Yup from "yup";
import VueMultiselect from 'vue-multiselect';
import 'vue-multiselect/dist/vue-multiselect.css';
import { notifyModel } from "@/models/notifyModel";
const {
  proxy
} = getCurrentInstance();
const state = reactive({
  cartItems: [],
  form: {
    email: '',
    fullName: '',
    phone: '',
    province: null,
    district: null,
    ward: null,
    address: '',
    note: ''
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
  selectedAddressOption: null,
  promotionCode: '',
  promotionMessage: '',
  discountAmount: 0,
  isApplyingPromotion: false,
  // Lưu object đầy đủ (dùng cho VueMultiselect)
  defaultImage: require('@/assets/img/caulong/logo/logoNTH_removeBackground.png')
});
const {
  cartItems,
  form,
  showAddressModal,
  listTinh,
  listTP,
  listPhuong,
  itemsDiaChi,
  newAddress,
  userAddresses,
  shippingFee,
  selectedAddress,
  selectedAddressOption,
  promotionCode,
  promotionMessage,
  discountAmount,
  isApplyingPromotion,
  defaultImage
} = toRefs(state);
const addressSchema = Yup.object().shape({
  address: Yup.string().required('Vui lòng nhập địa chỉ'),
  province: Yup.object().required('Vui lòng chọn tỉnh/thành phố'),
  district: Yup.object().required('Vui lòng chọn quận/huyện')
});
function loadUserData() {
  const authUser = JSON.parse(localStorage.getItem('auth-user'));
  if (authUser) {
    state.form = {
      ...state.form,
      email: authUser.email || '',
      fullName: authUser.fullName || '',
      phone: authUser.phone || ''
    };
  }
}
function loadCartItems() {
  const authUser = JSON.parse(localStorage.getItem('auth-user'));
  let cartData = [];
  if (authUser?.cart) {
    cartData = authUser.cart;
  } else {
    cartData = JSON.parse(localStorage.getItem('cart')) || [];
  }
  state.cartItems = cartData;
  console.log("DATA: ", state.cartItems);

  if (state.cartItems.length === 0) {
    proxy.$store.dispatch("snackBarStore/addNotify", {
      message: 'Giỏ hàng đang trống. Vui lòng thêm sản phẩm trước khi đặt hàng.',
      variant: 'danger'
    });
    proxy.$router.replace('/gio-hang');
  }
}
async function loadUserAddresses() {
  const authUser = JSON.parse(localStorage.getItem('auth-user'));
  if (authUser?.id) {
    try {
      const res = await proxy.$store.dispatch("diaChiStore/getAddressCustomer", {
        id: authUser.id
      });
      if (res?.code === 0) {
        state.userAddresses = res.data || [];
      }
    } catch (error) {
      console.error('Lỗi khi lấy địa chỉ:', error);
    }
  }
}
async function getListTinh() {
  await proxy.$store.dispatch("tinhStore/getAll").then(res => {
    if (res?.code === 0) {
      state.listTinh = res.data || [];
    }
  });
}
async function getListTP(id) {
  await proxy.$store.dispatch("huyenStore/getAll", {
    id: id
  }).then(res => {
    state.listTP = res.data || [];
  });
}
async function getListPhuong(id) {
  await proxy.$store.dispatch("phuongStore/getAll", {
    id: id
  }).then(res => {
    state.listPhuong = res.data || [];
  });
}
function onAddressSelect(selectedOption) {
  // Chỉ lấy ID từ data của option đã chọn
  state.selectedAddress = selectedOption.data.id;

  // Điền thông tin vào form (nếu cần)
  // Điền thông tin vào form (nếu cần)
  if (selectedOption.data) {
    fillAddressForm(selectedOption.data);
  }
}
function fillAddressForm(address) {
  state.form.address = address.address;
  state.itemsDiaChi.province = address.province;
  state.itemsDiaChi.district = address.district;
  state.itemsDiaChi.town = address.town;
}
async function addNewAddress() {
  const authUser = JSON.parse(localStorage.getItem('auth-user'));
  if (!authUser?.id) return;
  try {
    const addressData = {
      customerId: authUser.id,
      address: state.newAddress.address,
      provinceId: state.newAddress.province?.id,
      districtId: state.newAddress.district?.id,
      townId: state.newAddress.town?.id,
      isDefault: state.newAddress.isDefault
    };
    const res = await proxy.$store.dispatch("diaChiStore/createCustomer", addressData);
    if (res?.code === 0) {
      state.showAddressModal = false;
      loadUserAddresses();
      proxy.$store.dispatch("snackBarStore/addNotify", notifyModel.addMessage(res));

      // Tự động chọn địa chỉ vừa thêm
      if (res.data) {
        fillAddressForm({
          address: res.data.address,
          province: state.newAddress.province,
          district: state.newAddress.district,
          town: state.newAddress.town
        });
      }

      // Reset form
      state.newAddress = {
        address: '',
        province: null,
        district: null,
        town: null,
        isDefault: true
      };
    }
  } catch (error) {
    console.error('Lỗi khi thêm địa chỉ:', error);
    proxy.$store.dispatch("snackBarStore/addNotify", {
      message: 'Thêm địa chỉ thất bại',
      variant: 'danger'
    });
  }
}
function formatCurrency(value) {
  return new Intl.NumberFormat('vi-VN', {
    style: 'currency',
    currency: 'VND'
  }).format(value);
}
async function applyPromotion() {
  if (!state.promotionCode) return;

  state.isApplyingPromotion = true;
  state.promotionMessage = '';
  state.discountAmount = 0;
  try {
    const res = await proxy.$store.dispatch("promotionStore/validateCustomer", {
      code: state.promotionCode,
      orderValue: subTotal.value
    });
    if (res?.code === 0) {
      state.promotionCode = res.data?.code || state.promotionCode.toUpperCase();
      state.discountAmount = Number(res.data?.discountAmount || 0);
      state.promotionMessage = res.message || 'Áp dụng mã khuyến mãi thành công.';
      return;
    }

    state.promotionMessage = res?.message || 'Mã khuyến mãi không hợp lệ.';
  } catch (error) {
    state.promotionMessage = error?.message || 'Không thể kiểm tra mã khuyến mãi.';
  } finally {
    state.isApplyingPromotion = false;
  }
}
async function submitOrder() {
  try {
    const authUser = JSON.parse(localStorage.getItem('auth-user'));

    if (state.cartItems.length === 0) {
      proxy.$store.dispatch("snackBarStore/addNotify", {
        message: 'Giỏ hàng đang trống. Vui lòng thêm sản phẩm trước khi đặt hàng.',
        variant: 'danger'
      });
      proxy.$router.replace('/gio-hang');
      return;
    }

    // Kiểm tra xem đã chọn địa chỉ chưa
    if (!state.selectedAddress) {
      proxy.$store.dispatch("snackBarStore/addNotify", {
        message: 'Vui lòng chọn địa chỉ giao hàng',
        variant: 'danger'
      });
      return;
    }
    const orderData = {
      customerId: authUser.id,
      totalAmount: totalAmount.value,
      addressId: state.selectedAddress,
      // Chỉ gửi id của địa chỉ
      listOrderItems: state.cartItems.map(item => ({
        productsId: item.id,
        name: item.name,
        price: item.price,
        quantity: item.quantity,
        imageUrl: item.imageUrl
      })),
      paymentMethod: 'cod',
      promotionCode: state.discountAmount > 0 ? state.promotionCode : null
    };

    // Gọi API tạo đơn hàng
    const res = await proxy.$store.dispatch("odersStore/createCustomer", orderData);
    if (res?.code === 0) {
      proxy.$router.push({
        path: '/thanh-toan-thanh-cong',
        query: {
          orderId: res.data?.orderId || '',
          paymentMethod: 'cod'
        }
      });

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
    proxy.$store.dispatch("snackBarStore/addNotify", {
      message: error.message || 'Đặt hàng thất bại',
      variant: 'danger'
    });
  }
}
const subTotal = computed(() => {
  return state.cartItems.reduce((total, item) => total + item.price * item.quantity, 0);
});
const totalAmount = computed(() => {
  if (state.cartItems.length === 0) return 0;

  return Math.max(0, subTotal.value - state.discountAmount) + state.shippingFee;
});
const addressOptions = computed(() => {
  return [...state.userAddresses.map(addr => ({
    value: addr.id,
    text: `${addr.address}, ${addr.town}, ${addr.district}, ${addr.province}`,
    data: addr
  }))];
});
watch(() => state.itemsDiaChi.province, val => {
  if (val) {
    getListTP(val.id);
    state.itemsDiaChi.district = null;
    state.itemsDiaChi.town = null;
  }
}, {
  deep: true
});
watch(() => state.itemsDiaChi.district, val => {
  if (val) {
    getListPhuong(val.id);
    state.itemsDiaChi.town = null;
  }
}, {
  deep: true
});
watch(() => state.newAddress.province, val => {
  if (val) {
    getListTP(val.id);
    state.newAddress.district = null;
    state.newAddress.town = null;
  }
}, {
  deep: true
});
watch(() => state.newAddress.district, val => {
  if (val) {
    getListPhuong(val.id);
    state.newAddress.town = null;
  }
}, {
  deep: true
});
watch(subTotal, () => {
  state.discountAmount = 0;
  state.promotionMessage = state.promotionCode
    ? 'Giỏ hàng đã thay đổi, vui lòng áp dụng lại mã khuyến mãi.'
    : '';
});
loadUserData();
loadCartItems();
getListTinh();
loadUserAddresses();
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

