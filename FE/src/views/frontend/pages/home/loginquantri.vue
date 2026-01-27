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
                    <img src="@/assets/img/caulong/logo/logo_HBTShop-removebg.png" alt="shape-image" style="width: 100px"/>
                    <h4>ShopHBT - Hệ thống shop Cầu lông</h4>
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
<script>
import {notifyModel} from "@/models/notifyModel";
import * as Yup from "yup";
import {Field, Form} from "vee-validate";

export default {
  components: {
    Form,
    Field,
  },
  data() {
    return {
      model:{
        username: null,
        password: null
      },
    };
  },
  setup() {
    const schema = Yup.object().shape({
      username: Yup.string().required("Tài khoản không được bỏ trống !"),
      password: Yup.string().required("Mật khẩu không được bỏ trống !"),
    });
    return {
      schema,
    };
  },
  methods: {
      async submitForm() {
        await this.$store.dispatch("authStore/login", this.model).then((res) => {
          if (res && res.code ==0 ) {
            this.listMenu = res.data.menu;
            console.log("LOG SUCCCESS ", res.data.accessToken)
            localStorage.setItem('auth-user', JSON.stringify(res.data));
            localStorage.setItem('token', res.data.accessToken);
            // window.location.href="/admin"
            // this.$router.push("/quan-tri/profile");
            window.location.href = "/quan-tri/dashboard";
          }
          this.$store.dispatch("snackBarStore/addNotify", notifyModel.addMessage(res))
        })
      },
  },
};
</script>
