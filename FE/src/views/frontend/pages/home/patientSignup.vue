<template>
  <div class="login-body">
    <div class="main-wrapper">
      <layoutheader :class="{ 'header-space': showHeaderSpace }" ref="header" />

      <!-- Page Content -->
      <div class="login-content-info">
        <div class="container">
          <!-- Patient Signup -->
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
                  <div class="login-back">
                    <router-link to="/signup"
                      ><i class="fa-solid fa-arrow-left-long"></i> Trở về</router-link
                    >
                  </div>
                  <div class="login-title">
                    <h3>Đăng ký</h3>
                    <p class="mb-0">Xin chào bạn! Hãy điền đầy đủ thông tin</p>
                  </div>
                  <form @submit.prevent="submitForm">
                    <div class="mb-3">
                      <label class="mb-2">Số điện thoại</label>
                      <vue-tel-input
                        mode="international"
                        class="form-control form-control-lg group_formcontrol form-control-phone"
                        id="phone"
                        name="phone"
                        value="0123 456 789"
                      ></vue-tel-input>
                    </div>
                    <div class="mb-3">
                      <label class="mb-2">Họ và tên</label>
                      <input
                        type="text"
                        class="form-control"
                        placeholder="Nhập họ và tên"
                      />
                    </div>
                    <!-- <div class="mb-3">
                      <label class="mb-2">Last Name</label>
                      <input
                        type="text"
                        class="form-control"
                        placeholder="Enter Your Last Name"
                      />
                    </div> -->
                    <!-- <div class="mb-3 form-check-box terms-check-box">
                      <div class="form-group-flex">
                        <label class="custom_check">
                          I have read and agree to Doccure’s
                          <a href="javascript:void(0);">Terms of Service</a> and
                          <a href="javascript:void(0);">Privacy Policy.</a>
                          <input type="checkbox" name="Terms" />
                          <span class="checkmark"></span>
                        </label>
                      </div>
                    </div> -->
                    <div class="mb-3">
                      <b-button class="btn w-100" type="submit">Đăng ký</b-button>
                    </div>
                    <!-- <div class="mb-3 back-btn-light">
                      <b-button class="btn btn-light w-100" type="submit">
                        Create an Account
                      </b-button>
                    </div> -->
                    <div class="account-signup">
                      <p>
                        Đã có tài khoản?
                        <router-link to="/login">Đăng nhập</router-link>
                      </p>
                    </div>
                  </form>
                </div>
              </div>
            </div>
          </div>
          <!-- /Patient Signup -->
        </div>
      </div>
      <!-- /Page Content -->
    </div>
  </div>
</template>
<script>
export default {
  methods: {
    submitForm() {
      this.$router.push("/signup-success");
    },
  },
};
</script>

<style scoped>
.vti__dropdown {
  padding: 0 !important;
}
</style>
