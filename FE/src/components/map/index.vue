<template>
  <div id="lienhe" style="height: 580px; width: 100%; position: relative;">
    <!-- Popup để chọn loại bản đồ -->
    <div class="map-control">
      <div class="title-map-lien-he">
        Lớp bản đồ
      </div>
      <label>
        <input type="radio" value="street" v-model="mapType" @change="updateMapType" /> Đường phố
      </label>
      <label>
        <input type="radio" value="satellite" v-model="mapType" @change="updateMapType" /> Vệ tinh
      </label>
    </div>
  </div>
</template>

<script>
import { nextTick } from 'vue';
import L from 'leaflet';
import 'leaflet/dist/leaflet.css';
import customIconUrl from '@/assets/img/phuongthanh/lienhe/address-red.png';

export default {
  name: 'MapView',
  data() {
    return {
      map: null,
      googleStreetLayer: null,
      googleHybridLayer: null,
      mapType: 'street', // Loại bản đồ mặc định là bản đồ thường
      marker: null, // Biến lưu marker
    };
  },
  mounted() {
    console.log("LOG MAP NE : ")
    nextTick(() => {
      // Tọa độ góc Tây Nam và Đông Bắc của Việt Nam để giới hạn bản đồ
      const vietnamBounds = [
        [8.179, 102.144],  // Tọa độ góc Tây Nam
        [23.393, 109.469], // Tọa độ góc Đông Bắc
      ];

      // Khởi tạo bản đồ với các thiết lập giới hạn
      this.map = L.map('lienhe', {
        center: [10.2975748, 105.7632659], // Tọa độ trung tâm bản đồ
        zoom: 14,                    // Mức độ zoom ban đầu
        minZoom: 8,                  // Mức độ zoom tối thiểu
        maxZoom: 18,                 // Mức độ zoom tối đa
        maxBounds: vietnamBounds,    // Giới hạn phạm vi bản đồ trong Việt Nam
        maxBoundsViscosity: 1.0      // Độ mượt khi kéo ra ngoài giới hạn
      });

      // Khởi tạo lớp bản đồ Google Street và Hybrid (vệ tinh)
      this.googleStreetLayer = L.tileLayer(
        'http://{s}.google.com/vt/lyrs=m&x={x}&y={y}&z={z}',
        {
          maxZoom: 18,
          subdomains: ['mt0', 'mt1', 'mt2', 'mt3']
        }
      ).addTo(this.map); // Mặc định hiển thị bản đồ thường

      this.googleHybridLayer = L.tileLayer(
        'http://{s}.google.com/vt/lyrs=y&x={x}&y={y}&z={z}',
        {
          maxZoom: 18,
          subdomains: ['mt0', 'mt1', 'mt2', 'mt3']
        }
      );

      const customTitle = L.divIcon({
        html: `
          <div style="
            position: relative; 
            background: none; 
            text-align: center;
            width: 90px">
            <div style="
              position: relative;
              font-size: 14px; 
              font-weight: bold; 
              color: red; 
              margin-bottom: -10px;
              position: absolute;
              left: -54px;
              top: -30px;">
              Trung tâm nha khoa Phương Thành
            </div>
          </div>
        `,
        iconSize: [32, 42], // Kích thước tổng thể của icon (gồm cả text và marker)
        iconAnchor: [16, 42] // Điểm neo của marker, căn chỉnh với bản đồ
      });

      // Tạo icon tùy chỉnh
      const customIcon = L.icon({
        iconUrl: customIconUrl,
        iconSize: [32, 32],
        iconAnchor: [16, 32],
        popupAnchor: [0, -32]
      });

      // Đánh dấu vị trí cụ thể (luôn hiển thị)
      this.marker = L.marker([10.2975748, 105.7632659], { icon: customTitle, interactive: false }).addTo(this.map);
      this.marker = L.marker([10.2975748, 105.7632659], { icon: customIcon, interactive: false }).addTo(this.map);
      // this.marker.bindPopup('<b>Trung tâm nha khoa Phương Thành</b>').openPopup();
    });
  },
  methods: {
    updateMapType() {
      // Thay đổi chế độ bản đồ dựa trên giá trị của mapType
      if (this.mapType === 'street') {
        this.map.removeLayer(this.googleHybridLayer); // Xóa lớp vệ tinh
        this.map.addLayer(this.googleStreetLayer); // Thêm lớp bản đồ thường
      } else {
        this.map.removeLayer(this.googleStreetLayer); // Xóa lớp bản đồ thường
        this.map.addLayer(this.googleHybridLayer); // Thêm lớp bản đồ vệ tinh
      }

      // Đảm bảo marker vẫn hiển thị khi thay đổi lớp bản đồ
      // if (this.marker) {
      //   this.marker.addTo(this.map); // Đảm bảo marker được thêm vào lại nếu cần
      //   this.marker.bindPopup('<b>Khu nông nghiệp ứng dụng công nghệ cao Hậu Giang</b>').openPopup();
      // }
    }
  }
};
</script>

<style scoped>
#lienhe {
  height: 100%;
  width: 100%;
}
.map-control {
  position: absolute;
  width: 150px;
  bottom: 0px;
  left: 0px;
  background: white;
  border-radius: 5px;
  box-shadow: 0px 2px 5px rgba(0, 0, 0, 0.3);
  z-index: 1000;
}

.map-control .title-map-lien-he{
  background-color: #0f4327;
  font-size: 16px;
  font-family: 'SVN Poppins Medium';
  color: #fff;
  padding: 10px;
  margin-bottom: 10px;
  border-radius: 5px 5px 0 0;
}
.map-control label {
  display: block;
  font-family: 'SVN Poppins Medium';
  font-size: 16px;
  margin-bottom: 5px;
  padding: 0 10px;
  cursor: pointer;
  text-align: left;
}


</style>