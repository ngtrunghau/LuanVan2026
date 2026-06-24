<template>
  <div class="login-body">
    <div class="main-wrapper">
      <layoutheader :class="{ 'header-space': showHeaderSpace }" ref="header" />

      <!-- Page Content -->
      <div class="login-content-info">
        <div class="container">
          <!-- Login Email -->
          <div class="row justify-content-center">
            <div class="col-lg-4 col-md-6">
              <div class="account-content">
                <div class="login-shapes">
                  <div class="shape-img-left">
                    <img src="@/assets/img/shape-01.png" alt="shape-image" />
                  </div>
                  <div class="shape-img-right">
                    <img src="@/assets/img/shape-02.png" alt="shape-image" />
                  </div>
                </div>
                <div class="account-info">
                  <!-- <div class="login-back">
                    <router-link to="/"
                      ><i class="fas fa-arrow-left-long"></i> Trở về</router-link
                    >
                  </div> -->
                  <div class="login-title text-center">
                    <img src="@/assets/img/caulong/logo/logoNTH_removeBackground.png" alt="shape-image" style="width: 100px"/>
                    <h4>ShopNTH - Hệ thống shop Cầu lông</h4>
                  </div>
                  <Form :validation-schema="schema" v-slot="{ errors }"   @submit="submitForm">
                    <div class="mb-3">
                      <label class="mb-2">Tài khoản</label>
                      <Field
                          v-model="model.username"
                          placeholder="Vui lòng nhập tên tiêu đề"
                          name="username"
                          type="text"
                          class="form-control"
                          :class="{ 'is-invalid': errors.username }"
                      />
                      <div class="invalid-feedback">{{ errors.username }}</div>
                      <!-- <input
                        type="text"
                        class="form-control"
                        placeholder="example@email.com"
                      /> -->
                    </div>
                    <div class="mb-3">
                      <div class="form-group-flex">
                        <label class="mb-2">Mật khẩu</label>
                        <!-- <router-link to="/forgot-password" class="forgot-link"
                          >Forgot password?</router-link
                        > -->
                      </div>
                      <div class="pass-group">
                        <Field
                            v-if="showPassword"
                            v-model="model.password"
                            placeholder="Vui lòng nhập tên tiêu đề"
                            name="password"
                            type="password"
                            class="form-control"
                            :class="{ 'is-invalid': errors.password }"
                        />
                        <Field
                        v-else
                            v-model="model.password"
                            placeholder="Vui lòng nhập tên tiêu đề"
                            name="password"
                            type="password"
                            class="form-control"
                            :class="{ 'is-invalid': errors.password }"
                        />
                        <div class="invalid-feedback">{{ errors.password }}</div>
                        <!-- <input
                          v-if="showPassword"
                          type="text"
                          class="form-control pass-input"
                          v-model="password"
                          placeholder="**********"
                        />
                        <input
                          v-else
                          type="password"
                          class="form-control pass-input"
                          placeholder="**********"
                          v-model="password"
                        /> -->
                        <!-- <span
                          class="toggle-password"
                          @click="toggleShow"
                          :class="{
                            'feather-eye': showPassword,
                            'feather-eye-off': !showPassword,
                          }"
                        ></span> -->
                      </div>
                    </div>
                    <div class="mb-3">
                      <b-button class="btn w-100" type="submit">Đăng nhập</b-button>
                    </div>
                    
                  </Form>
                  <div class="text-center mt-3">
                    <p class="text-muted">
                      Chưa có tài khoản? 
                      <router-link to="/dang-ky" class="text-primary">Đăng ký ngay</router-link>
                    </p>
                  </div>
                </div>
              </div>
            </div>
          </div>
          <!-- /Login Email -->
        </div>
      </div>
      <!-- /Page Content -->

      <cursor></cursor>
    </div>
  </div>
</template>
<script setup>
import { computed, getCurrentInstance, onBeforeUnmount, onMounted, reactive, toRefs } from "vue";
import { notifyModel } from "@/models/notifyModel";
import * as Yup from "yup";
import { Field, Form } from "vee-validate";
const {
  proxy
} = getCurrentInstance();
const state = reactive({
  showPassword: false,
  password: null,
  model: {
    username: null,
    password: null
  },
  loginRetry: null
});
const {
  showPassword,
  password,
  model,
  loginRetry
} = toRefs(state);
const schema = Yup.object().shape({
  username: Yup.string().required("Tài khoản không được bỏ trống !"),
  password: Yup.string().required("Mật khẩu không được bỏ trống !")
});
function toggleShow() {
  state.showPassword = !state.showPassword;
}
async function submitForm() {
  try {
    const res = await proxy.$store.dispatch("khachHangStore/login", state.model);
    if (res && res.code === 0) {
      // Lưu thông tin đăng nhập
      localStorage.setItem('auth-user', JSON.stringify(res.data));
      localStorage.setItem('token', res.data.accessToken);
      if (window.axios) {
        window.axios.defaults.headers.common.Authorization = `Bearer ${res.data.accessToken}`;
      }

      // Đồng bộ giỏ hàng ngay lập tức
      await syncCartAfterLogin();

      // Kiểm tra redirect sau khi đăng nhập
      const redirectPath = localStorage.getItem("redirect-after-login") || "/";
      localStorage.removeItem("redirect-after-login");
      proxy.$router.push(redirectPath);
    } else {
      showNotification(res?.message || 'Đăng nhập thất bại', 'error');
    }
  } catch (error) {
    console.error("Lỗi đăng nhập:", error);
    showNotification('Đăng nhập thất bại, vui lòng thử lại', 'error');
  }
}
async function syncCartAfterLogin() {
  // Đợi 300ms để đảm bảo component giỏ hàng đã load
  await new Promise(resolve => setTimeout(resolve, 300));

  // Gọi syncCartOnLogin từ component giỏ hàng
  // Gọi syncCartOnLogin từ component giỏ hàng
  if (window.$cartComponent?.syncCartOnLogin) {
    await window.$cartComponent.syncCartOnLogin();
  } else {
    console.warn('Không tìm thấy component giỏ hàng');

    // Fallback: Tự xử lý đồng bộ nếu không có component
    const guestCart = JSON.parse(localStorage.getItem("cart")) || [];
    if (guestCart.length > 0) {
      const authUser = JSON.parse(localStorage.getItem("auth-user"));
      authUser.cart = [...(authUser.cart || []), ...guestCart];
      localStorage.setItem("auth-user", JSON.stringify(authUser));
      localStorage.removeItem("cart");
    }
  }
}
function showNotification(message, type = 'success') {
  // Sử dụng hệ thống thông báo của bạn
  console[type === 'success' ? 'log' : 'error'](message);

  // Hoặc nếu dùng Vuex
  // Hoặc nếu dùng Vuex
  if (proxy.$store && proxy.$store.dispatch) {
    proxy.$store.dispatch("snackBarStore/addNotify", {
      message: message,
      variant: type
    });
  }
}
const buttonLabel = computed(() => {
  return state.showPassword ? "Hide" : "Show";
});
onMounted(() => {
  state.loginRetry = null;
});
onBeforeUnmount(() => {
  if (state.loginRetry) {
    clearTimeout(state.loginRetry);
  }
});
</script>

<style scoped>
.text-muted {
  color: #6c757d;
}
.text-primary {
  color: #007bff;
  cursor: pointer;
  text-decoration: none;
}
.text-primary:hover {
  text-decoration: underline;
}
</style>


