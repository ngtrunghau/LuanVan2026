<template>
  <div class="main-Wrapper">
    <adminheader />
    <adminsidebar />

    <div class="page-wrapper">
      <div class="content container-fluid">
        <div class="card">
          <div class="card-header d-flex justify-content-between align-items-center">
            <div>
              <h3 class="card-title mb-1">Quản lý mã khuyến mãi</h3>
              <p class="text-muted mb-0">Tạo và kiểm soát các mã giảm giá của cửa hàng.</p>
            </div>
            <button class="btn btn-primary" type="button" @click="openCreate">
              <i class="fas fa-plus me-1"></i> Thêm mã
            </button>
          </div>

          <div class="card-body">
            <div class="row g-2 mb-3">
              <div class="col-md-6">
                <label class="form-label">Tìm kiếm</label>
                <input
                  v-model.trim="filters.search"
                  class="form-control"
                  placeholder="Mã hoặc mô tả khuyến mãi"
                  @keyup.enter="applyFilters"
                >
              </div>
              <div class="col-md-3">
                <label class="form-label">Trạng thái</label>
                <select v-model="filters.state" class="form-select" @change="applyFilters">
                  <option value="">Tất cả</option>
                  <option value="active">Đang hoạt động</option>
                  <option value="upcoming">Sắp diễn ra</option>
                  <option value="expired">Đã hết hạn</option>
                  <option value="disabled">Đã vô hiệu</option>
                </select>
              </div>
              <div class="col-md-3 d-flex align-items-end gap-2">
                <button class="btn btn-outline-primary flex-grow-1" @click="applyFilters">
                  Tìm kiếm
                </button>
                <button class="btn btn-outline-secondary" title="Làm mới" @click="resetFilters">
                  <i class="fas fa-retweet"></i>
                </button>
              </div>
            </div>

            <div v-if="loading" class="text-center py-5">
              <div class="spinner-border text-primary"></div>
            </div>

            <div v-else class="table-responsive">
              <table class="table table-hover align-middle">
                <thead>
                  <tr>
                    <th>STT</th>
                    <th>Mã</th>
                    <th>Giảm giá</th>
                    <th>Đơn tối thiểu</th>
                    <th>Thời gian</th>
                    <th>Lượt dùng</th>
                    <th>Trạng thái</th>
                    <th class="text-center">Thao tác</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="(item, index) in list" :key="item.id">
                    <td>{{ (currentPage - 1) * perPage + index + 1 }}</td>
                    <td>
                      <strong>{{ item.code }}</strong>
                      <div class="small text-muted">{{ item.descriptions }}</div>
                    </td>
                    <td>{{ formatDiscount(item) }}</td>
                    <td>{{ formatCurrency(item.minOrderValue || 0) }}</td>
                    <td class="small">
                      <div>Từ: {{ formatDate(item.startDate) }}</div>
                      <div>Đến: {{ formatDate(item.endDate) }}</div>
                    </td>
                    <td>
                      {{ item.usedCount || 0 }} /
                      {{ item.maxUsage || 'Không giới hạn' }}
                    </td>
                    <td>
                      <span class="badge" :class="stateClass(item.state)">
                        {{ stateLabel(item.state) }}
                      </span>
                    </td>
                    <td class="text-center text-nowrap">
                      <button
                        class="btn btn-outline-success btn-sm me-1"
                        title="Chỉnh sửa"
                        @click="openEdit(item)"
                      >
                        <i class="fas fa-pencil-alt"></i>
                      </button>
                      <button
                        class="btn btn-sm"
                        :class="item.isDeleted ? 'btn-outline-primary' : 'btn-outline-danger'"
                        :title="item.isDeleted ? 'Kích hoạt' : 'Vô hiệu'"
                        @click="toggleActive(item)"
                      >
                        <i :class="item.isDeleted ? 'fas fa-check' : 'fas fa-ban'"></i>
                      </button>
                    </td>
                  </tr>
                  <tr v-if="list.length === 0">
                    <td colspan="8" class="text-center text-muted py-4">
                      Không có mã khuyến mãi phù hợp.
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>

            <div class="row align-items-center mt-3">
              <div class="col-md-6">
                Hiển thị
                <select v-model.number="perPage" class="form-select form-select-sm d-inline-block mx-1 page-size">
                  <option :value="5">5</option>
                  <option :value="10">10</option>
                  <option :value="25">25</option>
                  <option :value="50">50</option>
                </select>
                trên tổng số {{ totalRows }} dòng
              </div>
              <div class="col-md-6 d-flex justify-content-end">
                <b-pagination
                  v-model="currentPage"
                  :total-rows="totalRows"
                  :per-page="perPage"
                  size="sm"
                  class="mb-0"
                />
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <div ref="modalElement" class="modal fade" tabindex="-1" data-bs-backdrop="static">
      <div class="modal-dialog modal-lg modal-dialog-centered">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">{{ form.id ? 'Cập nhật' : 'Thêm' }} mã khuyến mãi</h5>
            <button type="button" class="btn-close" @click="closeModal"></button>
          </div>
          <div class="modal-body">
            <Form :validation-schema="schema" v-slot="{ errors }" @submit="savePromotion">
              <div class="row g-3">
                <div class="col-md-6">
                  <label class="form-label">Mã khuyến mãi <span class="text-danger">*</span></label>
                  <Field
                    v-model.trim="form.code"
                    name="code"
                    class="form-control text-uppercase"
                    :class="{ 'is-invalid': errors.code }"
                    placeholder="VD: SALE10"
                  />
                  <div class="invalid-feedback">{{ errors.code }}</div>
                </div>
                <div class="col-md-6">
                  <label class="form-label">Loại giảm giá <span class="text-danger">*</span></label>
                  <Field
                    v-model="form.discountType"
                    name="discountType"
                    as="select"
                    class="form-select"
                    :class="{ 'is-invalid': errors.discountType }"
                  >
                    <option value="percent">Phần trăm (%)</option>
                    <option value="fixed">Số tiền cố định</option>
                  </Field>
                  <div class="invalid-feedback">{{ errors.discountType }}</div>
                </div>
                <div class="col-md-6">
                  <label class="form-label">
                    {{ form.discountType === 'percent' ? 'Phần trăm giảm' : 'Số tiền giảm' }}
                    <span class="text-danger">*</span>
                  </label>
                  <div v-if="form.discountType === 'percent'" class="input-group">
                    <Field
                      v-model.number="form.discountValue"
                      name="discountValue"
                      type="number"
                      min="1"
                      max="100"
                      placeholder="VD: 10"
                      class="form-control"
                      :class="{ 'is-invalid': errors.discountValue }"
                    />
                    <span class="input-group-text">%</span>
                  </div>
                  <Field v-else name="discountValue" v-slot="{ field }">
                    <CurrencyInput
                      v-model="form.discountValue"
                      placeholder="VD: 100.000"
                      class="form-control"
                      :class="{ 'is-invalid': errors.discountValue }"
                      @update:model-value="field.onChange"
                      @blur="field.onBlur"
                    />
                  </Field>
                  <div v-if="errors.discountValue" class="text-danger small mt-1">
                    {{ errors.discountValue }}
                  </div>
                  <div v-else-if="form.discountType === 'percent'" class="form-text">
                    Ví dụ: nhập 10 để giảm 10% giá trị đơn hàng.
                  </div>
                </div>
                <div class="col-md-6">
                  <label class="form-label">Giá trị đơn tối thiểu</label>
                  <Field name="minOrderValue" v-slot="{ field }">
                    <CurrencyInput
                      v-model="form.minOrderValue"
                      class="form-control"
                      :class="{ 'is-invalid': errors.minOrderValue }"
                      @update:model-value="field.onChange"
                      @blur="field.onBlur"
                    />
                  </Field>
                  <div class="invalid-feedback">{{ errors.minOrderValue }}</div>
                </div>
                <div class="col-md-6">
                  <label class="form-label">Ngày bắt đầu</label>
                  <Field v-model="form.startDate" name="startDate" type="datetime-local" class="form-control" />
                </div>
                <div class="col-md-6">
                  <label class="form-label">Ngày kết thúc</label>
                  <Field
                    v-model="form.endDate"
                    name="endDate"
                    type="datetime-local"
                    class="form-control"
                    :class="{ 'is-invalid': errors.endDate }"
                  />
                  <div class="invalid-feedback">{{ errors.endDate }}</div>
                </div>
                <div class="col-md-6">
                  <label class="form-label">Giới hạn lượt sử dụng</label>
                  <Field
                    v-model.number="form.maxUsage"
                    name="maxUsage"
                    type="number"
                    min="1"
                    class="form-control"
                    :class="{ 'is-invalid': errors.maxUsage }"
                    placeholder="Để trống nếu không giới hạn"
                  />
                  <div class="invalid-feedback">{{ errors.maxUsage }}</div>
                </div>
                <div class="col-md-6 d-flex align-items-end">
                  <div class="form-check form-switch mb-2">
                    <input id="promotionActive" v-model="form.isActive" class="form-check-input" type="checkbox">
                    <label class="form-check-label" for="promotionActive">Kích hoạt mã</label>
                  </div>
                </div>
                <div class="col-12">
                  <label class="form-label">Mô tả <span class="text-danger">*</span></label>
                  <Field
                    v-model.trim="form.descriptions"
                    name="descriptions"
                    as="textarea"
                    rows="3"
                    class="form-control"
                    :class="{ 'is-invalid': errors.descriptions }"
                  />
                  <div class="invalid-feedback">{{ errors.descriptions }}</div>
                </div>
              </div>
              <div class="text-end mt-4">
                <button type="button" class="btn btn-outline-secondary me-2" @click="closeModal">Đóng</button>
                <button type="submit" class="btn btn-primary" :disabled="saving">
                  {{ saving ? 'Đang lưu...' : 'Lưu' }}
                </button>
              </div>
            </Form>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import CurrencyInput from '@/components/common/CurrencyInput.vue';
import { getCurrentInstance, onMounted, reactive, ref, watch } from "vue";
import { Field, Form } from "vee-validate";
import * as Yup from "yup";
import { Modal } from "bootstrap";

const { proxy } = getCurrentInstance();
const modalElement = ref(null);
const modal = ref(null);
const loading = ref(false);
const saving = ref(false);
const list = ref([]);
const totalRows = ref(0);
const currentPage = ref(1);
const perPage = ref(10);
const filters = reactive({ search: "", state: "" });

function emptyForm() {
  return {
    id: 0,
    code: "",
    descriptions: "",
    discountType: "percent",
    discountValue: null,
    minOrderValue: 0,
    startDate: "",
    endDate: "",
    maxUsage: null,
    isActive: true
  };
}
const form = reactive(emptyForm());

const schema = Yup.object({
  code: Yup.string().trim().required("Vui lòng nhập mã khuyến mãi."),
  descriptions: Yup.string().trim().required("Vui lòng nhập mô tả."),
  discountType: Yup.string().oneOf(["percent", "fixed"]).required(),
  discountValue: Yup.number()
    .typeError("Mức giảm không hợp lệ.")
    .positive("Mức giảm phải lớn hơn 0.")
    .test("percent-limit", "Mức giảm không được vượt quá 100%.", function (value) {
      return this.parent.discountType !== "percent" || !value || value <= 100;
    })
    .required("Vui lòng nhập mức giảm."),
  minOrderValue: Yup.number().typeError("Giá trị không hợp lệ.").min(0),
  maxUsage: Yup.number()
    .nullable()
    .transform((value, originalValue) => originalValue === "" ? null : value)
    .positive("Giới hạn lượt dùng phải lớn hơn 0."),
  endDate: Yup.string().test("after-start", "Ngày kết thúc phải sau ngày bắt đầu.", function (value) {
    if (!value || !this.parent.startDate) return true;
    return new Date(value) >= new Date(this.parent.startDate);
  })
});

async function getData() {
  loading.value = true;
  try {
    const response = await proxy.$store.dispatch("adminPromotionStore/getPaging", {
      start: currentPage.value,
      limit: perPage.value,
      search: filters.search,
      state: filters.state
    });
    if (response?.code === 0) {
      list.value = response.data?.data || [];
      totalRows.value = response.data?.totalRows || 0;
    } else {
      notify(response?.message, "danger");
    }
  } finally {
    loading.value = false;
  }
}

function openCreate() {
  Object.assign(form, emptyForm());
  modal.value.show();
}

function openEdit(item) {
  Object.assign(form, {
    id: item.id,
    code: item.code || "",
    descriptions: item.descriptions || "",
    discountType: item.discountType || "percent",
    discountValue: item.discountValue,
    minOrderValue: item.minOrderValue || 0,
    startDate: toDateTimeLocal(item.startDate),
    endDate: toDateTimeLocal(item.endDate),
    maxUsage: item.maxUsage,
    isActive: item.isDeleted !== true
  });
  modal.value.show();
}

function closeModal() {
  modal.value.hide();
}

async function savePromotion() {
  saving.value = true;
  try {
    const payload = {
      id: form.id,
      code: form.code.toUpperCase(),
      descriptions: form.descriptions,
      discountType: form.discountType,
      discountValue: Number(form.discountValue),
      minOrderValue: Number(form.minOrderValue || 0),
      startDate: form.startDate || null,
      endDate: form.endDate || null,
      maxUsage: form.maxUsage ? Number(form.maxUsage) : null,
      isDeleted: !form.isActive
    };
    const action = form.id
      ? "adminPromotionStore/update"
      : "adminPromotionStore/create";
    const response = await proxy.$store.dispatch(action, payload);
    notify(response?.message, response?.code === 0 ? "success" : "danger");
    if (response?.code === 0) {
      closeModal();
      await getData();
    }
  } finally {
    saving.value = false;
  }
}

async function toggleActive(item) {
  const isActive = item.isDeleted === true;
  const response = await proxy.$store.dispatch("adminPromotionStore/setActive", {
    id: item.id,
    isActive
  });
  notify(response?.message, response?.code === 0 ? "success" : "danger");
  if (response?.code === 0) await getData();
}

function applyFilters() {
  currentPage.value = 1;
  getData();
}

function resetFilters() {
  filters.search = "";
  filters.state = "";
  currentPage.value = 1;
  getData();
}

function notify(message, variant) {
  proxy.$store.dispatch("snackBarStore/addNotify", {
    message: message || "Có lỗi xảy ra.",
    variant
  });
}

function formatCurrency(value) {
  return new Intl.NumberFormat("vi-VN", {
    style: "currency",
    currency: "VND"
  }).format(Number(value || 0));
}

function formatDiscount(item) {
  return item.discountType === "percent"
    ? `${item.discountValue}%`
    : formatCurrency(item.discountValue);
}

function formatDate(value) {
  return value ? new Date(value).toLocaleString("vi-VN") : "Không giới hạn";
}

function toDateTimeLocal(value) {
  if (!value) return "";
  const date = new Date(value);
  const local = new Date(date.getTime() - date.getTimezoneOffset() * 60000);
  return local.toISOString().slice(0, 16);
}

function stateLabel(state) {
  return {
    active: "Đang hoạt động",
    upcoming: "Sắp diễn ra",
    expired: "Đã hết hạn",
    disabled: "Đã vô hiệu"
  }[state] || state;
}

function stateClass(state) {
  return {
    active: "bg-success",
    upcoming: "bg-info text-dark",
    expired: "bg-secondary",
    disabled: "bg-danger"
  }[state] || "bg-secondary";
}

watch([currentPage, perPage], () => getData());
onMounted(() => {
  modal.value = new Modal(modalElement.value);
  getData();
});
</script>

<style scoped>
.page-size {
  width: 75px;
}
</style>
