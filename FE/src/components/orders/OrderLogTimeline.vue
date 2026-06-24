<template>
  <div class="order-log">
    <div v-if="normalizedLogs.length === 0" class="text-muted">
      Chưa có lịch sử cập nhật đơn hàng.
    </div>
    <div
      v-for="log in normalizedLogs"
      :key="log.id"
      class="order-log__item"
    >
      <span
        class="order-log__indicator"
        :class="`order-log__indicator--${log.status}`"
      />
      <div class="order-log__content">
        <div class="d-flex flex-wrap justify-content-between gap-2">
          <strong>{{ getStatusTitle(log.status) }}</strong>
          <small class="text-muted">{{ formatDate(log.dateShip) }}</small>
        </div>
        <div class="text-muted mt-1">
          {{ log.note || getStatusDescription(log.status) }}
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { computed } from "vue";

const props = defineProps({
  logs: {
    type: Array,
    default: () => [],
  },
});

const normalizedLogs = computed(() =>
  [...props.logs]
    .filter((log) => log && log.isDeleted !== true)
    .sort((first, second) => {
      const firstDate = new Date(first.dateShip || 0).getTime();
      const secondDate = new Date(second.dateShip || 0).getTime();
      return secondDate - firstDate || (second.id || 0) - (first.id || 0);
    }),
);

function getStatusTitle(status) {
  return {
    1: "Đã đặt hàng",
    2: "Đang vận chuyển",
    3: "Giao hàng thành công",
    4: "Đã hủy",
  }[status] || "Đơn hàng được cập nhật";
}

function getStatusDescription(status) {
  return {
    1: "Đơn hàng đã được tạo và đang chờ xử lý.",
    2: "Đơn hàng đã được xác nhận và chuyển sang đơn vị vận chuyển.",
    3: "Khách hàng đã xác nhận nhận được đơn hàng.",
    4: "Đơn hàng đã được hủy và ngừng xử lý.",
  }[status] || "Thông tin đơn hàng đã thay đổi.";
}

function formatDate(value) {
  if (!value) return "Chưa xác định";

  const date = new Date(value);
  if (Number.isNaN(date.getTime())) return "Chưa xác định";

  return new Intl.DateTimeFormat("vi-VN", {
    day: "2-digit",
    month: "2-digit",
    year: "numeric",
    hour: "2-digit",
    minute: "2-digit",
    second: "2-digit",
    hour12: false,
  }).format(date);
}
</script>

<style scoped>
.order-log {
  position: relative;
  padding-left: 1.5rem;
}

.order-log__item {
  position: relative;
  padding: 0 0 1rem 1rem;
}

.order-log__item:not(:last-child)::before {
  position: absolute;
  top: 0.75rem;
  bottom: -0.25rem;
  left: -0.05rem;
  width: 2px;
  content: "";
  background: #dee2e6;
}

.order-log__indicator {
  position: absolute;
  top: 0.25rem;
  left: -0.4rem;
  z-index: 1;
  width: 0.75rem;
  height: 0.75rem;
  border: 2px solid #fff;
  border-radius: 50%;
  background: #6c757d;
  box-shadow: 0 0 0 2px #6c757d;
}

.order-log__indicator--1 {
  background: #0dcaf0;
  box-shadow: 0 0 0 2px #0dcaf0;
}

.order-log__indicator--2 {
  background: #ffc107;
  box-shadow: 0 0 0 2px #ffc107;
}

.order-log__indicator--3 {
  background: #198754;
  box-shadow: 0 0 0 2px #198754;
}

.order-log__indicator--4 {
  background: #dc3545;
  box-shadow: 0 0 0 2px #dc3545;
}

.order-log__content {
  padding: 0.75rem 1rem;
  border: 1px solid #e9ecef;
  border-radius: 0.5rem;
  background: #f8f9fa;
}
</style>
