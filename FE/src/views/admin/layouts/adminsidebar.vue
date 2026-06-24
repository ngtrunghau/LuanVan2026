<template>
  <!-- Sidebar -->
  <div class="sidebar" id="sidebar">
    <div class="sidebar-inner slimscroll">
      <perfect-scrollbar
          class="scroll-area"
          :settings="settings"
          @ps-scroll-y="scrollHanle"
      >
      <!-- <div id="sidebar-menu" class="sidebar-menu">
        <ul>
          <template v-for="item in listMenu">
            <li class="menu-title " v-if="item.level === 0">
              <i :class="`${item.icon}`" style="font-size: 16px;"></i>

              <span class="banner-content aos aos-init aos-animate" >{{item.name}}</span>
              <i :class="`${item.icon}`" style="font-size: 16px;"></i>
            </li>
            <template v-if="hasItems(item)">
              <li v-for="(value) in item.children" :key="value.id" :class="isCheck(value.link) ? 'active' : 'notactive'" >
                <a :menuId="value.id" :pathname="value.link" style="cursor: pointer"
                    class="side-nav-link-ref"
                    @click="handleGetIdMenu(value.link)"
                >
                  <i :class="`${value.icon}`" style="font-size: 16px; display: flex; justify-content: center;"></i>
                  <span >{{ value.name}}</span>
                </a>
              </li>
            </template>
          </template>

        </ul>
      </div> -->
      <div id="sidebar-menu" class="sidebar-menu">
        <ul>
          <li>
            <router-link to="/quan-tri/dashboard" class="btn consult-btn" :class="{ 'active': isActive('/quan-tri/dashboard') }">
              <i class="fa fa-star" style="font-size: 16px; display: flex; justify-content: center;"></i>
              <span>Dashboard</span>
            </router-link>
          </li>
          <li>
            <router-link to="" class="btn consult-btn" :class="{ 'active': isActive('/') }">
              <i class="fa fa-star" style="font-size: 16px; display: flex; justify-content: center;"></i>
              <span>Quản lý sản phẩm</span>
            </router-link>
            <li>
              <router-link to="/quan-tri/quan-ly-san-pham/them-san-pham"
                class="btn consult-btn"
                :class="{ 'active': isActive('/quan-tri/quan-ly-san-pham/them-san-pham') }"
                style="display: flex; align-items: center; padding-top: 8px; padding-bottom: 8px;">
                <i class="fa fa-plus" style="font-size: 16px; display: flex; justify-content: center;"></i>
                <span>Thêm sản phẩm</span>
              </router-link>
            </li>
            <li>
              <router-link to="/quan-tri/quan-ly-san-pham"
                class="btn consult-btn"
                :class="{ 'active': isActive('/quan-tri/quan-ly-san-pham') }"
                style="display: flex; align-items: center; padding-top: 8px; padding-bottom: 8px;">
                <i class="fa fa-bars" style="font-size: 16px; display: flex; justify-content: center;"></i>
                <span>Danh sách sản phẩm</span>
              </router-link>
            </li>
          </li>
          <li>
            <router-link to="/quan-tri/quan-ly-loai" class="btn consult-btn" :class="{ 'active': isActive('/quan-tri/quan-ly-loai') }">
              <i class="fa fa-star" style="font-size: 16px; display: flex; justify-content: center;"></i>
              <span>Quản lý loại</span>
            </router-link>
          </li>
          <li>
            <router-link to="/quan-tri/quan-ly-vai-tro" class="btn consult-btn" :class="{ 'active': isActive('/quan-tri/quan-ly-vai-tro') }">
              <i class="fa fa-star" style="font-size: 16px; display: flex; justify-content: center;"></i>
              <span>Quản lý vai trò</span>
            </router-link>
          </li>
          <li>
            <router-link to="/quan-tri/quan-ly-tai-khoan" class="btn consult-btn" :class="{ 'active': isActive('/quan-tri/quan-ly-tai-khoan') }">
              <i class="fa fa-star" style="font-size: 16px; display: flex; justify-content: center;"></i>
              <span>Quản lý tài khoản</span>
            </router-link>
          </li>
          <li v-if="showAdminConfigMenus">
            <router-link to="/quan-tri/bo-dieu-khien" class="btn consult-btn" :class="{ 'active': isActive('/quan-tri/bo-dieu-khien') }">
              <i class="fa fa-star" style="font-size: 16px; display: flex; justify-content: center;"></i>
              <span>Quản lý bộ điều khiển</span>
            </router-link>
          </li>
          <li v-if="showAdminConfigMenus">
            <router-link to="/quan-tri/chuc-nang" class="btn consult-btn" :class="{ 'active': isActive('/quan-tri/chuc-nang') }">
              <i class="fa fa-star" style="font-size: 16px; display: flex; justify-content: center;"></i>
              <span>Quản lý chức năng</span>
            </router-link>
          </li>
          <li>
            <router-link to="/quan-tri/khach-hang" class="btn consult-btn" :class="{ 'active': isActive('/quan-tri/khach-hang') }">
              <i class="fa fa-star" style="font-size: 16px; display: flex; justify-content: center;"></i>
              <span>Quản lý khách hàng</span>
            </router-link>
          </li>
          <li>
            <router-link to="/quan-tri/dia-chi" class="btn consult-btn" :class="{ 'active': isActive('/quan-tri/dia-chi') }">
              <i class="fa fa-star" style="font-size: 16px; display: flex; justify-content: center;"></i>
              <span>Quản lý địa chỉ</span>
            </router-link>
          </li>
          <li>
            <router-link to="/quan-tri/kho" class="btn consult-btn" :class="{ 'active': isActive('/quan-tri/kho') }">
              <i class="fa fa-star" style="font-size: 16px; display: flex; justify-content: center;"></i>
              <span>Quản lý kho</span>
            </router-link>
          </li>
          <li>
            <router-link to="/quan-tri/dat-hang" class="btn consult-btn" :class="{ 'active': isActive('/quan-tri/dat-hang') }">
              <i class="fa fa-star" style="font-size: 16px; display: flex; justify-content: center;"></i>
              <span>Quản lý đơn hàng</span>
            </router-link>
          </li>
          <li>
            <router-link to="/quan-tri/canh-bao-ton-kho" class="btn consult-btn" :class="{ 'active': isActive('/quan-tri/canh-bao-ton-kho') }">
              <i class="fas fa-exclamation-triangle" style="font-size: 16px; display: flex; justify-content: center;"></i>
              <span>Cảnh báo tồn kho</span>
            </router-link>
          </li>
          <li>
            <router-link to="/quan-tri/khuyen-mai" class="btn consult-btn" :class="{ 'active': isActive('/quan-tri/khuyen-mai') }">
              <i class="fas fa-tags" style="font-size: 16px; display: flex; justify-content: center;"></i>
              <span>Quản lý khuyến mãi</span>
            </router-link>
          </li>
          <li>
            <router-link to="/quan-tri/danh-gia" class="btn consult-btn" :class="{ 'active': isActive('/quan-tri/danh-gia') }">
              <i class="fas fa-comment-dots" style="font-size: 16px; display: flex; justify-content: center;"></i>
              <span>Kiểm duyệt đánh giá</span>
            </router-link>
          </li>
          <!-- <li>
            <router-link to="/quan-tri/chi-tiet-van-chuyen" class="btn consult-btn" :class="{ 'active': isActive('/quan-tri/chi-tiet-van-chuyen') }">
              <i class="fa fa-star" style="font-size: 16px; display: flex; justify-content: center;"></i>
              <span>Quản lý vận chuyển</span>
            </router-link>
          </li> -->
        </ul>
      </div>

      </perfect-scrollbar>
    </div>
  </div>
  <!-- /Sidebar -->
</template>

<script setup>
import { computed, getCurrentInstance, reactive, toRefs } from "vue";
import { PerfectScrollbar } from "vue3-perfect-scrollbar";
import "vue3-perfect-scrollbar/dist/vue3-perfect-scrollbar.css";
const {
  proxy
} = getCurrentInstance();
const state = reactive({
  settings: {
    suppressScrollX: true
  },
  reportsMenuData: false,
  authenticationMenuData: false,
  errorMenuData: false,
  formMenuData: false,
  tablesMenuData: false,
  multilevelMenuData: false,
  activeClass: "active",
  showAdminConfigMenus: false,
  listMenu: []
});
const {
  settings,
  reportsMenuData,
  authenticationMenuData,
  errorMenuData,
  formMenuData,
  tablesMenuData,
  multilevelMenuData,
  activeClass,
  showAdminConfigMenus,
  listMenu
} = toRefs(state);
function isActive(path) {
  return proxy.$route.path === path; // Kiểm tra nếu route hiện tại khớp với path
}
function scrollHanle(evt) {}
function togglereportsMenu() {
  state.reportsMenuData = !state.reportsMenuData;
}
function toggleauthenticationMenu() {
  state.authenticationMenuData = !state.authenticationMenuData;
}
function toggleerrorMenu() {
  state.errorMenuData = !state.errorMenuData;
}
function toggleformsMenu() {
  proxy.formsMenuData = !proxy.formsMenuData;
}
function toggletablesMenu() {
  state.tablesMenuData = !state.tablesMenuData;
}
function togglemultilevelMenu() {
  state.multilevelMenuData = !state.multilevelMenuData;
}
function isCustomDropdown() {
  //Search bar
  var searchOptions = document.getElementById("search-close-options");
  var dropdown = document.getElementById("search-dropdown");
  var searchInput = document.getElementById("search-options");
  searchInput.addEventListener("focus", () => {
    var inputLength = searchInput.value.length;
    if (inputLength > 0) {
      dropdown.classList.add("show");
      searchOptions.classList.remove("d-none");
    } else {
      dropdown.classList.remove("show");
      searchOptions.classList.add("d-none");
    }
  });
  searchInput.addEventListener("keyup", () => {
    var inputLength = searchInput.value.length;
    if (inputLength > 0) {
      dropdown.classList.add("show");
      searchOptions.classList.remove("d-none");
    } else {
      dropdown.classList.remove("show");
      searchOptions.classList.add("d-none");
    }
  });
  searchOptions.addEventListener("click", () => {
    searchInput.value = "";
    dropdown.classList.remove("show");
    searchOptions.classList.add("d-none");
  });
  document.body.addEventListener("click", e => {
    if (e.target.getAttribute("id") !== "search-options") {
      dropdown.classList.remove("show");
      searchOptions.classList.add("d-none");
    }
  });
}
function initActiveMenu(ele) {
  setTimeout(() => {
    if (document.querySelector("#sidebar")) {
      let a = document.querySelector("#sidebar").querySelector('[href="' + ele + '"]');
      if (a) {
        a.classList.add("active");
        let parentCollapseDiv = a.closest(".collapse.menu-dropdown");
        if (parentCollapseDiv) {
          parentCollapseDiv.classList.add("show");
          parentCollapseDiv.parentElement.children[0].classList.add("active");
          parentCollapseDiv.parentElement.children[0].setAttribute("aria-expanded", "true");
          if (parentCollapseDiv.parentElement.closest(".collapse.menu-dropdown")) {
            parentCollapseDiv.parentElement.closest(".collapse").classList.add("show");
            if (parentCollapseDiv.parentElement.closest(".collapse").previousElementSibling) parentCollapseDiv.parentElement.closest(".collapse").previousElementSibling.classList.add("active");
          }
        }
      }
    }
  }, 1000);
}
function hasItems(item) {
  //   console.log("LOG HAS ITEM :  ",item.children !== undefined ? item.children?.length > 0 : false)
  return item.children !== undefined ? item.children?.length > 0 : false;
}
function isCheck(item) {
  if (item != null && proxy.$route.name != null) return `/${proxy.$route.name}` == item;else return false;
}
function handleGetIdMenu(path) {
  proxy.$router.push(path);
}
const currentPath = computed(() => {
  //  console.log("LOG :  ", this.$route.name)
  return proxy.$route.name;
});
const reportsMenu = computed(() => {
  return proxy.$route.name == "admin/invoice-report" || proxy.$route.name == "admin/invoice";
});
const authenticationMenu = computed(() => {
  return proxy.$route.name == "admin/forgot-password" || proxy.$route.name == "admin/lock-screen" || proxy.$route.name == "admin/login" || proxy.$route.name == "admin/register";
});
const errorMenu = computed(() => {
  return proxy.$route.name == "admin/error-404" || proxy.$route.name == "admin/error-500";
});
const tablesMenu = computed(() => {
  return proxy.$route.name == "admin/data-tables" || proxy.$route.name == "admin/tables-basic";
});
const formsMenu = computed(() => {
  return proxy.$route.name == "admin/form-basic-inputs" || proxy.$route.name == "admin/form-input-groups" || proxy.$route.name == "admin/form-horizontal" || proxy.$route.name == "admin/form-mask" || proxy.$route.name == "admin/form-validation" || proxy.$route.name == "admin/form-vertical";
});
const multilevelMenu = computed(() => {});
var currentUser = localStorage.getItem("auth-user");
if (currentUser) {
  let data = JSON.parse(currentUser);
  if (data && data.menu) state.listMenu = data.menu;
}
</script>
<style scoped>
.btn.consult-btn.active {
  background-color: #bf8b43; /* Màu xanh khi active */
  color: white; /* Chữ màu trắng */
  font-weight: bold;
}
</style>
