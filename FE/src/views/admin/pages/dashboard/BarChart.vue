<template>
    <div class="chart-container">
      <Bar v-if="!loading" :data="processedChartData" :options="chartOptions" />
    </div>
  </template>
  
  <script>
  import { Bar } from 'vue-chartjs'
  import { Chart as ChartJS, Title, Tooltip, Legend, BarElement, CategoryScale, LinearScale } from 'chart.js'
  
  ChartJS.register(Title, Tooltip, Legend, BarElement, CategoryScale, LinearScale)
  
  export default {
    name: 'BarChart',
    components: { Bar },
    props: {
      chartData: {
        type: Object,
        required: true
      },
      loading: {
        type: Boolean,
        default: false
      }
    },
    data() {
      return {
        // Bảng màu cố định theo tháng (có thể tùy chỉnh)
        monthColors: {
          'Tháng 1': '#4e73df',  // Xanh dương
          'Tháng 2': '#1cc88a',  // Xanh lá
          'Tháng 3': '#36b9cc',  // Xanh ngọc
          'Tháng 4': '#f6c23e',  // Vàng
          'Tháng 5': '#e74a3b',  // Đỏ
          'Tháng 6': '#5a5c69',  // Xám
          'Tháng 7': '#fd7e14',  // Cam
          'Tháng 8': '#6610f2',  // Tím
          'Tháng 9': '#6f42c1',  // Tím nhạt
          'Tháng 10': '#d63384', // Hồng
          'Tháng 11': '#20c997', // Xanh lục
          'Tháng 12': '#0dcaf0'  // Xanh biển
        },
        chartOptions: {
          responsive: true,
          plugins: {
            legend: {
              display: false // Ẩn chú thích (nếu muốn hiện, đổi thành true)
            },
            tooltip: {
              callbacks: {
                label: (context) => {
                  return `${context.parsed.y.toLocaleString('vi-VN')} VND`;
                }
              }
            }
          },
          scales: {
            y: {
              ticks: {
                callback: (value) => value.toLocaleString('vi-VN')
              }
            }
          }
        }
      }
    },
    computed: {
      processedChartData() {
        return {
          labels: this.chartData.labels,
          datasets: [{
            label: 'Doanh số',
            data: this.chartData.datasets[0].data,
            backgroundColor: this.chartData.labels.map(
              label => this.monthColors[label] || '#858796' // Màu mặc định nếu không khớp
            ),
            borderColor: '#ffffff',
            borderWidth: 1
          }]
        }
      }
    }
  }
  </script>