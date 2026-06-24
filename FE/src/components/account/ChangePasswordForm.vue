<template>
  <div class="card">
    <div class="card-header">
      <h5 class="mb-1">Đổi mật khẩu</h5>
      <p class="text-muted mb-0">Sau khi đổi mật khẩu, bạn cần đăng nhập lại.</p>
    </div>
    <div class="card-body">
      <Form :validation-schema="schema" v-slot="{ errors }" @submit="submitForm">
        <div class="mb-3">
          <label class="form-label">Mật khẩu hiện tại</label>
          <Field
            v-model="form.currentPassword"
            name="currentPassword"
            type="password"
            class="form-control"
            :class="{ 'is-invalid': errors.currentPassword }"
            autocomplete="current-password"
          />
          <div class="invalid-feedback">{{ errors.currentPassword }}</div>
        </div>
        <div class="mb-3">
          <label class="form-label">Mật khẩu mới</label>
          <Field
            v-model="form.newPassword"
            name="newPassword"
            type="password"
            class="form-control"
            :class="{ 'is-invalid': errors.newPassword }"
            autocomplete="new-password"
          />
          <div class="form-text">Tối thiểu 8 ký tự, gồm chữ và số.</div>
          <div class="invalid-feedback">{{ errors.newPassword }}</div>
        </div>
        <div class="mb-3">
          <label class="form-label">Xác nhận mật khẩu mới</label>
          <Field
            v-model="form.confirmPassword"
            name="confirmPassword"
            type="password"
            class="form-control"
            :class="{ 'is-invalid': errors.confirmPassword }"
            autocomplete="new-password"
          />
          <div class="invalid-feedback">{{ errors.confirmPassword }}</div>
        </div>
        <button class="btn btn-primary" type="submit" :disabled="submitting">
          {{ submitting ? 'Đang xử lý...' : 'Đổi mật khẩu' }}
        </button>
      </Form>
    </div>
  </div>
</template>

<script setup>
import { getCurrentInstance, reactive, ref } from "vue";
import { Field, Form } from "vee-validate";
import * as Yup from "yup";

const props = defineProps({
  loginPath: {
    type: String,
    required: true
  }
});
const { proxy } = getCurrentInstance();
const submitting = ref(false);
const form = reactive({
  currentPassword: "",
  newPassword: "",
  confirmPassword: ""
});
const schema = Yup.object({
  currentPassword: Yup.string().required("Vui lòng nhập mật khẩu hiện tại."),
  newPassword: Yup.string()
    .min(8, "Mật khẩu mới phải có ít nhất 8 ký tự.")
    .matches(/[A-Za-zÀ-ỹ]/, "Mật khẩu mới phải có ít nhất một chữ.")
    .matches(/\d/, "Mật khẩu mới phải có ít nhất một số.")
    .required("Vui lòng nhập mật khẩu mới."),
  confirmPassword: Yup.string()
    .oneOf([Yup.ref("newPassword")], "Xác nhận mật khẩu không khớp.")
    .required("Vui lòng xác nhận mật khẩu mới.")
});

async function submitForm() {
  submitting.value = true;
  try {
    const response = await proxy.$store.dispatch(
      "accountStore/changePassword",
      form
    );
    proxy.$store.dispatch("snackBarStore/addNotify", {
      message: response?.message || "Không thể đổi mật khẩu.",
      variant: response?.code === 0 ? "success" : "danger"
    });
    if (response?.code !== 0) return;

    localStorage.removeItem("token");
    localStorage.removeItem("auth-user");
    localStorage.removeItem("user-token");
    if (window.axios) {
      delete window.axios.defaults.headers.common.Authorization;
    }
    await proxy.$router.replace(props.loginPath);
  } finally {
    submitting.value = false;
  }
}
</script>
