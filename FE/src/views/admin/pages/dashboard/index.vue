<template>
  <div class="main-Wrapper">
    <adminheader></adminheader>
    <adminsidebar></adminsidebar>

    <div class="page-wrapper">
      <div class="content container-fluid">
        <div class="dashboard-page">
          <div class="card header-card mb-3">
            <div class="card-body d-flex justify-content-between align-items-center flex-wrap gap-2">
              <div>
                <h3 class="mb-1">Báo cáo tổng hợp doanh thu và tồn kho</h3>
                <p class="text-muted mb-0">Doanh thu được ghi nhận theo ngày giao hàng thành công.</p>
              </div>
              <div class="d-flex gap-2">
                <button class="btn btn-outline-success" @click="handleExportExcel" :disabled="loading">
                  <i class="fas fa-file-excel"></i>
                  Xuất Excel
                </button>
                <button class="btn btn-outline-danger" @click="handleExportPdf" :disabled="loading">
                  <i class="fas fa-file-pdf"></i>
                  Xuất PDF
                </button>
                <button class="btn btn-outline-secondary" @click="reloadDashboard" :disabled="loading">
                  <i class="fas fa-retweet"></i>
                  Làm mới
                </button>
              </div>
            </div>
          </div>

          <div class="card filter-card mb-3">
            <div class="card-body">
              <div class="row g-2 align-items-end">
                <div class="col-md-2 col-sm-6">
                  <label class="form-label">Từ ngày</label>
                  <input v-model="filters.fromDate" type="date" class="form-control" />
                </div>
                <div class="col-md-2 col-sm-6">
                  <label class="form-label">Đến ngày</label>
                  <input v-model="filters.toDate" type="date" class="form-control" />
                </div>
                <div class="col-md-2 col-sm-6">
                  <label class="form-label">So sánh từ ngày</label>
                  <input v-model="filters.compareFromDate" type="date" class="form-control" />
                </div>
                <div class="col-md-2 col-sm-6">
                  <label class="form-label">So sánh đến ngày</label>
                  <input v-model="filters.compareToDate" type="date" class="form-control" />
                </div>
                <div class="col-md-2 col-sm-6">
                  <label class="form-label">Nhóm doanh thu</label>
                  <select v-model="filters.groupBy" class="form-select">
                    <option value="day">Theo ngày</option>
                    <option value="month">Theo tháng</option>
                    <option value="year">Theo năm</option>
                  </select>
                </div>
                <div class="col-md-1 col-sm-6">
                  <label class="form-label">Kỳ kho (ngày)</label>
                  <input v-model.number="filters.lookbackDays" type="number" min="7" max="365" class="form-control" />
                </div>
                <div class="col-md-1 col-sm-6">
                  <label class="form-label">Dự báo (ngày)</label>
                  <input v-model.number="filters.forecastDays" type="number" min="3" max="90" class="form-control" />
                </div>
              </div>
              <div class="mt-3 d-flex justify-content-end">
                <button class="btn btn-primary" @click="reloadDashboard" :disabled="loading">Áp dụng bộ lọc</button>
              </div>
            </div>
          </div>

          <div v-if="error" class="alert alert-danger mb-3">{{ error }}</div>

          <div class="row g-3 mb-3">
            <div class="col-xl-3 col-md-4 col-sm-6" v-for="card in summaryCards" :key="card.title">
              <div class="card metric-card h-100">
                <div class="card-body">
                  <small class="text-muted d-block">{{ card.title }}</small>
                  <h5 class="mb-1">{{ card.value }}</h5>
                  <small :class="card.changeClass">{{ card.changeText }}</small>
                </div>
              </div>
            </div>
          </div>

          <div class="row g-3 mb-3">
            <div class="col-xl-6">
              <div class="card h-100">
                <div class="card-header"><strong>Xu hướng doanh thu</strong></div>
                <div class="card-body chart-body">
                  <Line v-if="trendLineData.labels.length" :data="trendLineData" :options="lineOptions" />
                  <div v-else class="empty-state">Không có dữ liệu doanh thu</div>
                </div>
              </div>
            </div>

            <div class="col-xl-6">
              <div class="card h-100">
                <div class="card-header"><strong>Doanh thu theo kỳ</strong></div>
                <div class="card-body chart-body">
                  <Bar v-if="trendBarData.labels.length" :data="trendBarData" :options="barOptions" />
                  <div v-else class="empty-state">Không có dữ liệu doanh thu</div>
                </div>
              </div>
            </div>

            <div class="col-xl-6">
              <div class="card h-100">
                <div class="card-header"><strong>Cơ cấu doanh thu theo loại</strong></div>
                <div class="card-body chart-body">
                  <Pie v-if="categoryPieData.labels.length" :data="categoryPieData" :options="pieOptions" />
                  <div v-else class="empty-state">Không có dữ liệu doanh thu theo loại</div>
                </div>
              </div>
            </div>

            <div class="col-xl-6">
              <div class="card h-100">
                <div class="card-header"><strong>Trạng thái tồn kho</strong></div>
                <div class="card-body chart-body">
                  <Pie v-if="inventoryStatusPieData.labels.length" :data="inventoryStatusPieData" :options="quantityPieOptions" />
                  <div v-else class="empty-state">Không có dữ liệu tồn kho</div>
                </div>
              </div>
            </div>

            <div class="col-xl-6">
              <div class="card h-100">
                <div class="card-header"><strong>Nhu cầu sản phẩm cao nhất</strong></div>
                <div class="card-body chart-body">
                  <Bar v-if="topDemandBarData.labels.length" :data="topDemandBarData" :options="quantityBarOptions" />
                  <div v-else class="empty-state">Không có dữ liệu nhu cầu</div>
                </div>
              </div>
            </div>

            <div class="col-xl-6">
              <div class="card h-100">
                <div class="card-header"><strong>Doanh thu theo phương thức thanh toán</strong></div>
                <div class="card-body chart-body">
                  <Pie v-if="paymentPieData.labels.length" :data="paymentPieData" :options="pieOptions" />
                  <div v-else class="empty-state">Không có dữ liệu thanh toán</div>
                </div>
              </div>
            </div>
          </div>

          <div class="row g-3">
            <div class="col-xl-6">
              <div class="card h-100">
                <div class="card-header d-flex justify-content-between align-items-center">
                  <strong>Dự báo sắp hết hàng</strong>
                  <div class="d-flex align-items-center gap-2">
                    <span class="badge bg-warning text-dark">{{ inventory.lowStockCount || 0 }}</span>
                    <router-link to="/quan-tri/canh-bao-ton-kho" class="btn btn-sm btn-outline-warning">
                      Xử lý
                    </router-link>
                  </div>
                </div>
                <div class="card-body table-responsive p-0">
                  <table class="table table-sm mb-0">
                    <thead>
                      <tr>
                        <th>Sản phẩm</th>
                          <th>Tồn</th>
                          <th>Bán/ngày</th>
                          <th>Dự báo cần</th>
                          <th></th>
                      </tr>
                    </thead>
                    <tbody>
                      <tr v-for="item in inventory.lowStockProducts || []" :key="`low-${item.productId}`">
                        <td>
                          <strong>{{ item.productName }}</strong>
                          <div class="small text-muted">{{ item.reason }}</div>
                        </td>
                        <td>{{ item.currentStock }}</td>
                        <td>{{ item.avgDailySales }}</td>
                        <td>{{ item.forecastNeed }}</td>
                        <td>
                          <router-link to="/quan-tri/kho" class="btn btn-sm btn-outline-success">
                            Nhập hàng
                          </router-link>
                        </td>
                      </tr>
                      <tr v-if="!(inventory.lowStockProducts || []).length">
                          <td colspan="5" class="text-center text-muted">Không có mặt hàng sắp hết</td>
                      </tr>
                    </tbody>
                  </table>
                </div>
              </div>
            </div>

            <div class="col-xl-6">
              <div class="card h-100">
                <div class="card-header d-flex justify-content-between align-items-center">
                  <strong>Cảnh báo tồn dư</strong>
                  <span class="badge bg-danger">{{ inventory.overstockCount || 0 }}</span>
                </div>
                <div class="card-body table-responsive p-0">
                  <table class="table table-sm mb-0">
                    <thead>
                      <tr>
                        <th>Sản phẩm</th>
                        <th>Tồn</th>
                        <th>Ngày đủ hàng</th>
                        <th>Tỷ lệ bán</th>
                      </tr>
                    </thead>
                    <tbody>
                      <tr v-for="item in inventory.overstockProducts || []" :key="`over-${item.productId}`">
                        <td>
                          <strong>{{ item.productName }}</strong>
                          <div class="small text-muted">{{ item.reason }}</div>
                        </td>
                        <td>{{ item.currentStock }}</td>
                        <td>{{ item.stockCoverDays }}</td>
                        <td>{{ toPercent(item.sellThroughRate) }}</td>
                      </tr>
                      <tr v-if="!(inventory.overstockProducts || []).length">
                        <td colspan="4" class="text-center text-muted">Không có mặt hàng tồn dư cao</td>
                      </tr>
                    </tbody>
                  </table>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { computed, getCurrentInstance, reactive, toRefs } from "vue";
import { Line, Bar, Pie } from "vue-chartjs";
import { Chart as ChartJS, CategoryScale, LinearScale, PointElement, LineElement, BarElement, ArcElement, Title, Tooltip, Legend } from "chart.js";
import { exportDashboardExcel, exportDashboardPdf } from "@/utils/exportDocuments";
ChartJS.register(CategoryScale, LinearScale, PointElement, LineElement, BarElement, ArcElement, Title, Tooltip, Legend);
defineOptions({
  name: "AdminDashboardAdvanced"
});
const {
  proxy
} = getCurrentInstance();
const state = reactive({
  loading: false,
  error: null,
  filters: {
    fromDate: "",
    toDate: "",
    compareFromDate: "",
    compareToDate: "",
    groupBy: "day",
    lookbackDays: 30,
    forecastDays: 14
  },
  overview: {},
  trend: {
    points: [],
    total: 0
  },
  categoryData: [],
  paymentData: [],
  inventory: {
    lowStockProducts: [],
    overstockProducts: [],
    stockStatusChart: [],
    topDemandChart: []
  },
  colorPalette: ["#1f7a8c", "#2a9d8f", "#e9c46a", "#f4a261", "#e76f51", "#5f0f40", "#9a031e", "#0f4c5c", "#3a86ff", "#8338ec"],
  lineOptions: {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      legend: {
        display: true
      },
      tooltip: {
        callbacks: {
          label: ctx => formatCurrency(ctx.parsed.y)
        }
      }
    },
    scales: {
      y: {
        ticks: {
          callback: value => shortNumber(value)
        }
      }
    }
  },
  barOptions: {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      legend: {
        display: true
      },
      tooltip: {
        callbacks: {
          label: ctx => formatCurrency(ctx.parsed.y)
        }
      }
    },
    scales: {
      y: {
        ticks: {
          callback: value => shortNumber(value)
        }
      }
    }
  },
  quantityBarOptions: {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      legend: {
        display: true
      },
      tooltip: {
        callbacks: {
          label: ctx => `${ctx.dataset.label}: ${Number(ctx.parsed.y || 0).toLocaleString("vi-VN")}`
        }
      }
    },
    scales: {
      y: {
        beginAtZero: true,
        ticks: {
          precision: 0,
          callback: value => Number(value).toLocaleString("vi-VN")
        }
      }
    }
  },
  pieOptions: {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      legend: {
        position: "bottom"
      },
      tooltip: {
        callbacks: {
          label: ctx => `${ctx.label}: ${formatCurrency(ctx.parsed)}`
        }
      }
    }
  },
  quantityPieOptions: {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      legend: {
        position: "bottom"
      },
      tooltip: {
        callbacks: {
          label: ctx => `${ctx.label}: ${Number(ctx.parsed || 0).toLocaleString("vi-VN")} sản phẩm`
        }
      }
    }
  }
});
const {
  loading,
  error,
  filters,
  overview,
  trend,
  categoryData,
  paymentData,
  inventory,
  colorPalette,
  lineOptions,
  barOptions,
  quantityBarOptions,
  pieOptions,
  quantityPieOptions
} = toRefs(state);
function initializeDefaultDateRange() {
  const now = new Date();
  const toDate = toDateInput(now);
  const fromDateObj = new Date(now);
  fromDateObj.setDate(fromDateObj.getDate() - 29);
  const fromDate = toDateInput(fromDateObj);
  const compareToObj = new Date(fromDateObj);
  compareToObj.setDate(compareToObj.getDate() - 1);
  const compareFromObj = new Date(compareToObj);
  compareFromObj.setDate(compareFromObj.getDate() - 29);
  state.filters.fromDate = fromDate;
  state.filters.toDate = toDate;
  state.filters.compareFromDate = toDateInput(compareFromObj);
  state.filters.compareToDate = toDateInput(compareToObj);
}
function toDateInput(date) {
  const y = date.getFullYear();
  const m = `${date.getMonth() + 1}`.padStart(2, "0");
  const d = `${date.getDate()}`.padStart(2, "0");
  return `${y}-${m}-${d}`;
}
async function reloadDashboard() {
  state.loading = true;
  state.error = null;
  try {
    if (!validateFilters()) {
      return;
    }
    const [overviewRes, trendRes, categoryRes, paymentRes, inventoryRes] = await Promise.all([proxy.$store.dispatch("dashboardStore/getRevenueOverview", {
      fromDate: state.filters.fromDate,
      toDate: state.filters.toDate,
      compareFromDate: state.filters.compareFromDate,
      compareToDate: state.filters.compareToDate
    }), proxy.$store.dispatch("dashboardStore/getRevenueTrend", {
      groupBy: state.filters.groupBy,
      fromDate: state.filters.fromDate,
      toDate: state.filters.toDate
    }), proxy.$store.dispatch("dashboardStore/getRevenueByCategory", {
      fromDate: state.filters.fromDate,
      toDate: state.filters.toDate
    }), proxy.$store.dispatch("dashboardStore/getRevenueByPaymentMethod", {
      fromDate: state.filters.fromDate,
      toDate: state.filters.toDate
    }), proxy.$store.dispatch("dashboardStore/getInventoryInsights", {
      lookbackDays: state.filters.lookbackDays,
      forecastDays: state.filters.forecastDays
    })]);
    const responses = [["Tổng quan", overviewRes], ["Xu hướng", trendRes], ["Danh mục", categoryRes], ["Thanh toán", paymentRes], ["Tồn kho", inventoryRes]];
    const failures = responses.filter(([, response]) => response?.code !== 0);
    if (failures.length) {
      state.error = failures.map(([name, response]) => `${name}: ${response?.message || "không tải được dữ liệu"}`).join(" | ");
    }
    state.overview = overviewRes?.code === 0 ? overviewRes.data || {} : {};
    state.trend = trendRes?.code === 0 ? trendRes.data || {
      points: []
    } : {
      points: []
    };
    state.categoryData = categoryRes?.code === 0 ? categoryRes.data || [] : [];
    state.paymentData = paymentRes?.code === 0 ? paymentRes.data || [] : [];
    state.inventory = inventoryRes?.code === 0 ? inventoryRes.data || {} : {
      lowStockProducts: [],
      overstockProducts: [],
      stockStatusChart: [],
      topDemandChart: []
    };
  } catch (e) {
    state.error = `Loi he thong: ${e.message}`;
  } finally {
    state.loading = false;
  }
}
function validateFilters() {
  if (!state.filters.fromDate || !state.filters.toDate) {
    state.error = "Vui lòng chọn đầy đủ khoảng thời gian báo cáo.";
    return false;
  }
  if (state.filters.fromDate > state.filters.toDate) {
    state.error = "Từ ngày không được lớn hơn đến ngày.";
    return false;
  }
  if (state.filters.compareFromDate && state.filters.compareToDate && state.filters.compareFromDate > state.filters.compareToDate) {
    state.error = "Khoảng thời gian so sánh không hợp lệ.";
    return false;
  }
  return true;
}
function getExportData() {
  return {
    filters: { ...state.filters },
    summaryCards: summaryCards.value,
    trend: state.trend,
    categoryData: state.categoryData,
    paymentData: state.paymentData,
    inventory: state.inventory
  };
}
function handleExportExcel() {
  exportDashboardExcel(getExportData());
}
async function handleExportPdf() {
  state.loading = true;
  try {
    await exportDashboardPdf(getExportData());
  } catch (e) {
    state.error = `Không thể xuất PDF: ${e.message}`;
  } finally {
    state.loading = false;
  }
}
function formatCurrency(value) {
  return new Intl.NumberFormat("vi-VN", {
    style: "currency",
    currency: "VND"
  }).format(Number(value || 0));
}
function formatDate(value) {
  if (!value) return "--";
  return new Intl.DateTimeFormat("vi-VN").format(new Date(value));
}
function shortNumber(value) {
  const n = Number(value || 0);
  if (Math.abs(n) >= 1000000000) {
    return `${(n / 1000000000).toFixed(1)}B`;
  }
  if (Math.abs(n) >= 1000000) {
    return `${(n / 1000000).toFixed(1)}M`;
  }
  if (Math.abs(n) >= 1000) {
    return `${(n / 1000).toFixed(1)}K`;
  }
  return `${n}`;
}
function toPercent(value) {
  const n = Number(value || 0) * 100;
  return `${n.toFixed(2)}%`;
}
const summaryCards = computed(() => {
  const rangePercent = Number(state.overview.rangeChangePercent || 0);
  const momPercent = Number(state.overview.monthOverMonthPercent || 0);
  const rangeComparisonText = Number(state.overview.compareRangeRevenue || 0) === 0 ? Number(state.overview.rangeRevenue || 0) > 0 ? "Phát sinh mới so với kỳ trước" : "Hai kỳ chưa phát sinh doanh thu" : `${rangePercent >= 0 ? "+" : ""}${rangePercent.toFixed(2)}% so với kỳ so sánh`;
  const momComparisonText = Number(state.overview.previousMonthRevenue || 0) === 0 ? Number(state.overview.currentMonthRevenue || 0) > 0 ? "Phát sinh mới so với tháng trước" : "Chưa phát sinh doanh thu" : `${momPercent >= 0 ? "+" : ""}${momPercent.toFixed(2)}% so với cùng kỳ`;
  return [{
    title: "Doanh thu hôm nay",
    value: formatCurrency(state.overview.todayRevenue || 0),
    changeText: "Theo đơn giao thành công",
    changeClass: "text-muted"
  }, {
    title: "Doanh thu tháng này",
    value: formatCurrency(state.overview.monthRevenue || 0),
    changeText: "Từ ngày 01 đến hiện tại",
    changeClass: "text-muted"
  }, {
    title: "Doanh thu năm nay",
    value: formatCurrency(state.overview.yearRevenue || 0),
    changeText: "Tổng doanh thu trong năm",
    changeClass: "text-muted"
  }, {
    title: "Kỳ hiện tại",
    value: formatCurrency(state.overview.rangeRevenue || 0),
    changeText: rangeComparisonText,
    changeClass: rangePercent >= 0 ? "text-success" : "text-danger"
  }, {
    title: "Kỳ so sánh",
    value: formatCurrency(state.overview.compareRangeRevenue || 0),
    changeText: `${formatDate(state.overview.compareFromDate)} - ${formatDate(state.overview.compareToDate)}`,
    changeClass: "text-muted"
  }, {
    title: "Cùng kỳ tháng trước",
    value: formatCurrency(state.overview.previousMonthRevenue || 0),
    changeText: "Cùng số ngày của tháng hiện tại",
    changeClass: "text-muted"
  }, {
    title: "MoM",
    value: formatCurrency(state.overview.monthOverMonthAmount || 0),
    changeText: momComparisonText,
    changeClass: momPercent >= 0 ? "text-success" : "text-danger"
  }, {
    title: "Đơn hoàn tất trong kỳ",
    value: Number(state.overview.completedOrderCount || 0).toLocaleString("vi-VN"),
    changeText: "Đơn đã giao thành công",
    changeClass: "text-muted"
  }, {
    title: "Sản phẩm đã bán",
    value: Number(state.overview.unitsSold || 0).toLocaleString("vi-VN"),
    changeText: "Tổng số lượng trong kỳ",
    changeClass: "text-muted"
  }, {
    title: "Giá trị đơn trung bình",
    value: formatCurrency(state.overview.averageOrderValue || 0),
    changeText: "Doanh thu kỳ / đơn hoàn tất",
    changeClass: "text-muted"
  }];
});
const trendLineData = computed(() => {
  const labels = (state.trend.points || []).map(x => x.label);
  const values = (state.trend.points || []).map(x => Number(x.value || 0));
  return {
    labels,
    datasets: [{
      label: "Doanh thu",
      data: values,
      borderColor: "#1f7a8c",
      backgroundColor: "rgba(31,122,140,0.2)",
      fill: true,
      tension: 0.35
    }]
  };
});
const trendBarData = computed(() => {
  const labels = (state.trend.points || []).map(x => x.label);
  const values = (state.trend.points || []).map(x => Number(x.value || 0));
  return {
    labels,
    datasets: [{
      label: "Doanh thu",
      data: values,
      backgroundColor: labels.map((_, idx) => state.colorPalette[idx % state.colorPalette.length]),
      borderWidth: 1
    }]
  };
});
const categoryPieData = computed(() => {
  const labels = (state.categoryData || []).map(x => x.label);
  const values = (state.categoryData || []).map(x => Number(x.value || 0));
  return {
    labels,
    datasets: [{
      label: "Doanh thu theo loai",
      data: values,
      backgroundColor: labels.map((_, idx) => state.colorPalette[idx % state.colorPalette.length])
    }]
  };
});
const inventoryStatusPieData = computed(() => {
  const labels = (state.inventory.stockStatusChart || []).map(x => x.label);
  const values = (state.inventory.stockStatusChart || []).map(x => Number(x.value || 0));
  return {
    labels,
    datasets: [{
      label: "Trang thai ton kho",
      data: values,
      backgroundColor: ["#e9c46a", "#e76f51", "#2a9d8f"]
    }]
  };
});
const paymentPieData = computed(() => {
  const labels = (state.paymentData || []).map(x => x.label);
  const values = (state.paymentData || []).map(x => Number(x.value || 0));
  return {
    labels,
    datasets: [{
      label: "Doanh thu theo thanh toán",
      data: values,
      backgroundColor: labels.map((_, idx) => state.colorPalette[idx % state.colorPalette.length])
    }]
  };
});
const topDemandBarData = computed(() => {
  const labels = (state.inventory.topDemandChart || []).map(x => x.label);
  const values = (state.inventory.topDemandChart || []).map(x => Number(x.value || 0));
  return {
    labels,
    datasets: [{
      label: "So luong ban",
      data: values,
      backgroundColor: labels.map((_, idx) => state.colorPalette[idx % state.colorPalette.length])
    }]
  };
});
initializeDefaultDateRange();
reloadDashboard();
</script>

<style scoped>
.dashboard-page .header-card {
  border: 1px solid #d6e0e8;
}

.dashboard-page .filter-card {
  border: 1px solid #d9e3eb;
}

.metric-card {
  border: 1px solid #e1e7ee;
}

.metric-card h5 {
  font-weight: 700;
}

.chart-body {
  min-height: 340px;
  position: relative;
}

.empty-state {
  min-height: 280px;
  display: flex;
  align-items: center;
  justify-content: center;
  color: #6c757d;
}

.table thead th {
  font-size: 13px;
  background: #f7fafc;
}

.table tbody td {
  font-size: 13px;
  vertical-align: top;
}

@media (max-width: 768px) {
  .chart-body {
    min-height: 280px;
  }
}
</style>
