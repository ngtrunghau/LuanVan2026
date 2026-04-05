<template>
    <div class="login-body">
      <div class="main-wrapper">
        <layoutheader :class="{ 'header-space': showHeaderSpace }" ref="header" />
        
        <!-- Page Content -->
        <div class="login-content-info">
          <div class="container">
            <div class="row justify-content-center">
              <div class="col-lg-8 col-md-6">
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
                      <router-link to="/">
                        <i class="fas fa-arrow-left-long"></i> Trở về
                      </router-link>
                    </div>
                    <div class="login-title text-center">
                      <img src="@/assets/img/caulong/logo/logoNTH_removeBackground.png" alt="logo" style="width: 100px"/>
                      <h4>Đăng ký tài khoản</h4>
                    </div>
                    
                    <Form
                      class="login"
                      @submit="handleSubmit"
                      :validation-schema="schema"
                      v-slot="{ errors }"
                      ref="form"
                    >
                      <div class="mb-3">
                        <label class="mb-2">Tên khách hàng <span class="text-danger">*</span></label>
                        <Field
                          v-model="model.fullName"
                          placeholder="Vui lòng nhập tên khách hàng"
                          name="fullName"
                          type="text"
                          class="form-control"
                          :class="{ 'is-invalid': errors.fullName }"
                        />
                        <div class="invalid-feedback">{{ errors.fullName }}</div>
                      </div>
  
                      <div class="mb-3">
                        <label class="mb-2">Số điện thoại <span class="text-danger">*</span></label>
                        <Field
                          v-model="model.phone"
                          placeholder="Vui lòng nhập số điện thoại"
                          name="phone"
                          type="text"
                          class="form-control"
                          :class="{ 'is-invalid': errors.phone }"
                        />
                        <div class="invalid-feedback">{{ errors.phone }}</div>
                      </div>
  
                      <div class="mb-3">
                        <label class="mb-2">Email <span class="text-danger">*</span></label>
                        <Field
                          v-model="model.email"
                          placeholder="Vui lòng nhập email"
                          name="email"
                          type="email"
                          class="form-control"
                          :class="{ 'is-invalid': errors.email }"
                        />
                        <div class="invalid-feedback">{{ errors.email }}</div>
                      </div>
  
                      <div class="mb-3">
                        <label class="mb-2">Tài khoản <span class="text-danger">*</span></label>
                        <Field
                          v-model="model.userName"
                          placeholder="Vui lòng nhập tài khoản"
                          name="userName"
                          type="text"
                          class="form-control"
                          :class="{ 'is-invalid': errors.userName }"
                        />
                        <div class="invalid-feedback">{{ errors.userName }}</div>
                      </div>
  
                      <div class="mb-3">
                        <label class="mb-2">Mật khẩu <span class="text-danger">*</span></label>
                        <div class="pass-group">
                          <Field
                            v-model="model.password"
                            placeholder="Vui lòng nhập mật khẩu"
                            name="password"
                            :type="showPassword ? 'text' : 'password'"
                            class="form-control"
                            :class="{ 'is-invalid': errors.password }"
                          />
                          <span
                            class="toggle-password"
                            @click="toggleShow"
                            :class="{
                              'fa fa-eye': showPassword,
                              'fa fa-eye-slash': !showPassword,
                            }"
                          ></span>
                          <div class="invalid-feedback">{{ errors.password }}</div>
                        </div>
                      </div>
  
                      <div class="mb-3">
                        <b-button type="submit" variant="primary" class="btn w-100">
                          Đăng ký
                        </b-button>
                      </div>
                    </Form>
  
                    <div class="text-center mt-3">
                      <p class="text-muted">
                        Đã có tài khoản? 
                        <router-link to="/login" class="text-primary">Đăng nhập ngay</router-link>
                      </p>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </template>
  
  <script>
  import { Form, Field } from "vee-validate";
  import * as Yup from "yup";
  import { khachHangModel } from "@/models/khachHangModel";
  import { notifyModel } from "@/models/notifyModel";
  
  export default {
    components: {
      Form,
      Field,
    },
    data() {
      return {
        model: khachHangModel.baseJson(),
        showPassword: false,
        showHeaderSpace: false
      };
    },
    setup() {
      const schema = Yup.object().shape({
        fullName: Yup.string().required("Tên khách hàng không được bỏ trống !"),
        phone: Yup.string().required("Số điện thoại không được bỏ trống !"),
        email: Yup.string().email("Email không hợp lệ").required("Email không được bỏ trống !"),
        userName: Yup.string().required("Tài khoản không được bỏ trống !"),
        password: Yup.string().required("Mật khẩu không được bỏ trống !"),
      });
      return { schema };
    },
    methods: {
      toggleShow() {
        this.showPassword = !this.showPassword;
      },
      async handleSubmit() {
        try {
          const res = await this.$store.dispatch("khachHangStore/create", this.model);
          
          this.$store.dispatch("snackBarStore/addNotify", notifyModel.addMessage(res));
          
          if (res && res.code === 0) {
            // Đăng ký thành công
            this.$router.push("/login");
          }
        } catch (error) {
          console.error("Lỗi đăng ký:", error);
        }
      }
    }
  };
  </script>
  
  <style scoped>
  /* Thêm style tương tự trang đăng nhập */
  .login-body {
    background-color: #f8f9fa;
  }
  .account-content {
    background: #fff;
    border-radius: 10px;
    box-shadow: 0 0 10px rgba(0,0,0,0.1);
    padding: 20px;
  }
  .pass-group {
    position: relative;
  }
  .toggle-password {
    position: absolute;
    right: 10px;
    top: 50%;
    transform: translateY(-50%);
    cursor: pointer;
  }
  .text-danger {
    color: #dc3545;
  }
  .text-primary {
    color: #0d6efd;
    text-decoration: none;
  }
  .text-primary:hover {
    text-decoration: underline;
  }
  </style>
