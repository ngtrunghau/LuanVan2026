<template>
  <div class="main-Wrapper">
    <pharmacyheader></pharmacyheader>
    <pharmacysidebar></pharmacysidebar>
    <div class="page-wrapper">
      <div class="content container-fluid">
        <div class="dashboard">
          <div class="card">
            <div class="card-header">
                <h3 class="card-title">Báo cáo doanh số</h3>
                <div class="card-tools">
                  <button type="button" class="btn btn-tool" @click="getData">
                    <i class="fas fa-retweet"></i>
                  </button>
                </div>
              </div>
          </div>
          <!-- <h1>Báo cáo doanh số</h1>
          <button @click="getData">Làm mới dữ liệu</button> -->
          <div class="card-body">
            <div v-if="error" class="error-message">
              {{ error }}
            </div>
            
            <BarChart 
              v-if="!loading && chartData.datasets[0].data.length > 0"
              :chart-data="chartData"
            />
            <div v-else-if="loading" class="loading">Đang tải dữ liệu...</div>
            <div v-else class="no-data">Không có dữ liệu để hiển thị</div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import BarChart from '@/views/admin/pages/dashboard/BarChart.vue'
import {notifyModel} from "@/models/notifyModel";
export default {
  components: { BarChart },
  data() {
    return {
      loading: false,
      error: null,
      list: [], // Khởi tạo list rỗng
      chartData: {
        labels: [],
        datasets: [{
          label: 'Doanh số',
          backgroundColor: '#4e73df',
          data: []
        }]
      }
    }
  },
  methods: {
    async getData() {
      try {
        this.loading = true
        this.error = null
        
        const res = await this.$store.dispatch("dashboardStore/getAll")
        console.log("API Response:", res) // Debug log
        
        if (res?.code === 0) {
          // Gán dữ liệu vào list
          this.list = res.data || [] // Sửa ở đây: res.data thay vì res.data.data
          this.prepareChartData()
        } else {
          this.error = res?.message || 'Có lỗi khi tải dữ liệu'
        }
        
        this.$store.dispatch("snackBarStore/addNotify", notifyModel.addMessage(res));
      } catch (err) {
        this.error = 'Lỗi hệ thống: ' + err.message
        console.error("API error:", err)
      } finally {
        this.loading = false
      }
    },
    prepareChartData() {
      // Đảm bảo xử lý cả trường hợp undefined
      this.chartData.labels = this.list.map(item => item?.label || '')
      this.chartData.datasets[0].data = this.list.map(item => item?.y || 0)
      console.log("Chart data prepared:", this.chartData) // Debug
    }
  },
  mounted() {
    this.getData()
  }
}
</script>