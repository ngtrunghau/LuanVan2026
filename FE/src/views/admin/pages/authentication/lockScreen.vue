<template>
  <!-- Main Wrapper -->
  <div class="main-wrapper login-body">
    <div class="login-wrapper">
      <div class="container">
        <div class="loginbox">
          <div class="login-left">
            <img class="img-fluid" src="@/assets/admin/img/logo.png" alt="Logo" />
          </div>
          <div class="login-right">
            <div class="login-right-wrap">
              <div class="lock-user">
                <img
                  class="rounded-circle"
                  src="@/assets/admin/img/profiles/avatar-02.jpg"
                  alt="User Image"
                />
                <h4>ShopNTH Admin</h4>
              </div>

              <!-- Form -->
              <form @submit.prevent="submitForm">
                <div class="form-group">
                  <input class="form-control" type="password" placeholder="Mật khẩu" />
                </div>
                <div class="form-group mb-0">
                  <b-button class="btn btn-primary w-100" type="submit">Enter</b-button>
                </div>
              </form>
              <!-- /Form -->

              <div class="text-center dont-have">
                Đăng nhập bằng tài khoản khác?
                <router-link to="/quan-tri/login">Đăng nhập</router-link>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
  <!-- /Main Wrapper -->
</template>

<script setup>
import { getCurrentInstance, reactive, toRefs } from "vue";
defineOptions({
  name: "/admin/lock-screen"
});
const {
  proxy
} = getCurrentInstance();
const state = reactive({});
function submitForm() {
  proxy.$router.push("/quan-tri/dashboard");
}
</script>
