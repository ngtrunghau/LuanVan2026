<template>
  <ul class="main-nav">
      <li class="has-submenu megamenu">
        <router-link to="/">
          Trang chủ
        </router-link>
      </li>
      <!-- <li class="has-submenu megamenu">
        <router-link to="/">
          Giới thiệu
        </router-link>
      </li> -->
      <!-- Menu động từ listLoai -->
      <li 
        class="has-submenu megamenu" 
        v-for="loai in listLoai" 
        :key="loai.id"
      >
        <router-link :to="`/san-pham/${loai.id}`">
          {{ loai.name }}
        </router-link>
      </li>
      <!-- <li class="has-submenu megamenu">
        <router-link to="/san-pham-all">
          Giày cầu lông
        </router-link>
      </li>
      <li class="has-submenu megamenu">
        <router-link to="/san-pham-all">
          Áo cầu lông
        </router-link>
      </li>
      <li class="has-submenu megamenu">
        <router-link to="/san-pham-all">
          Quần cầu lông
        </router-link>
      </li> -->
      <!-- <li class="has-submenu">
        <a href="javascript:void(0);" @click="toggleVisibilityhomes(item)"
        >
          Trang chủ 1
          <i class="fas fa-angle-down" >
          </i>
        </a>
        <ul class="submenu" :style= "{display: 'block'}"  >
          <li 
              class="has-submenu"
          >
            <a @click="toggleVisibilityhomes(value)"> 
              <i class="fa-solid fa-chevron-right">
              </i>
            </a>
            <ul
                class="submenu inner-submenu"
                :style="{ display:  'block' }"
            >
              <li>
                <a
                    @click="toggleVisibilityhomes(itemChildren)"
                >
                  Trang chủ 2
                </a>
              </li>
            </ul>
          </li>

        </ul>
      </li> -->

    <li class="login-link">
      <router-link to="/login">Đăng nhập</router-link>
    </li>
    <li class="login-link">
      <router-link to="/dang-ky">Đăng ký</router-link>
    </li>
  </ul>
</template>
<script setup>
import { computed, getCurrentInstance, reactive, toRefs } from "vue";
const {
  proxy
} = getCurrentInstance();
const state = reactive({
  listLoai: [] // Danh sách loại sản phẩm
});
const {
  listLoai
} = toRefs(state);
async function getListLoai() {
  try {
    await proxy.$store.dispatch("loaiStore/getAllCustomer").then(res => {
      if (res != null && res.code === 0) {
        state.listLoai = res.data || [];
      }
    });
  } catch (error) {
    console.error("Lỗi khi lấy danh sách loại:", error);
  }
}
function hasItems(item) {
  //   console.log("LOG hasItems ")
  return item.children !== undefined ? item.children?.length > 0 : false;
}
function isChildren(item) {
  // console.log("LOG IS CHILDREN ", item !== undefined ? item.children?.length > 0 : false)
  return item !== undefined ? item.children !== undefined && item.children?.length > 0 : false;
}
function handleGetIdMenu(path, id) {
  console.log("LOG HANDLE GET ID MENU ", path, id);
  if (path != window.location.pathname && path !== "/" && path === '/ban-tin/id') {
    // console.log(`LOG NE IF  : ${id} : ` +  path)
    //   window.open(this.url + "/ban-tin/" +  id)
    return '/ban-tin/' + id;
  }
  if (path != window.location.pathname && path !== "/" && path === '/bao-tri/id') {
    //   console.log(`HANDLE BAO TRI : ${id} : ` +  path)
    //   window.open(this.url  + "/bao-tri/" +  id)
    return proxy.$router.push("/bao-tri/" + id);
  }
  if (path != window.location.pathname && path !== "/" && path === '/dich-vu/id') {
    //  console.log(`HANDLE GET ID MENU : ${id} : ` +  path)
    // window.open(this.url + "/dich-vu/" +  id)
    return proxy.$router.push("/dich-vu/" + id);
  }
  if (path != window.location.pathname)
    //  console.log(`LOG NE ELSE  : ${id} : ` +  path)
    // window.open(path)
    return path;
}
function isCheck(item) {
  //  console.log("LOG IS CHECK " , item)
  if (proxy.$route.name == "/") {
    return `${proxy.$route.name}` == item;
  }
  if (item != null && proxy.$route.name != null) return `/${proxy.$route.name}` == item;else return false;
}
function toggleElement() {
  proxy.isVisible = !proxy.isVisible;
}
function toggleVisibilityhomes(item) {
  document.documentElement.classList.remove("menu-opened");
  localStorage.setItem('menu', JSON.stringify(item));

  //console.log("LOG ITEM : ", item.link);

  //console.log("LOG ITEM : ", item.link);

  if (item.link != window.location.pathname && item.link == '/ban-tin/id') {
    //    console.log("LOG ITEM 1 : ", item)
    return proxy.$router.push("/ban-tin/" + item.id);
  }

  // if (item.link != window.location.pathname &&  item.link !== "/" && item.links === '/bao-tri/id')
  // {
  //   console.log("LOG ITEM 2 : ", item)
  //   localStorage.setItem('menu', JSON.stringify(item));
  //
  //   return  this.$router.push("/bao-tri/" +  item.id);
  // }
  //
  // if (item.path != window.location.pathname &&  item.link !== "/" && item.link === '/dich-vu/id')
  // {
  //   console.log("LOG ITEM 3 : ", item)
  //  // window.open(url + "/dich-vu/" +  id)
  //   //  console.log(`HANDLE GET ID MENU : ${id} : ` +  path)
  //   return  this.$router.push("/dich-vu/" +  item.id);
  // }
  // if (item.link != window.location.pathname &&  item.link !== "/" && item.links === '/bao-tri/id')
  // {
  //   console.log("LOG ITEM 2 : ", item)
  //   localStorage.setItem('menu', JSON.stringify(item));
  //
  //   return  this.$router.push("/bao-tri/" +  item.id);
  // }
  //
  // if (item.path != window.location.pathname &&  item.link !== "/" && item.link === '/dich-vu/id')
  // {
  //   console.log("LOG ITEM 3 : ", item)
  //  // window.open(url + "/dich-vu/" +  id)
  //   //  console.log(`HANDLE GET ID MENU : ${id} : ` +  path)
  //   return  this.$router.push("/dich-vu/" +  item.id);
  // }
  if (item.link != window.location.pathname)
    //      window.open(path)
    //     console.log("IF 02  " , path , window.location.pathname)
    //        console.log("LOG ITEM 3 : ", item)
    return proxy.$router.push(item.link);
}
const limitedListMenu = computed(() => {
  return proxy.listMenu.slice(0, 11); // Lấy phần tử từ 0 đến 6
});
const currentPath = computed(() => {
  return proxy.$route.name;
});
const adminMenu = computed(() => {
  return proxy.$route.name == "quan-tri" || proxy.$route.name == "/quan-tri";
});
getListLoai();
let authUser = localStorage.getItem("auth-user");
if (authUser) {
  let jsonUserCurrent = JSON.parse(authUser);
  proxy.currentUserAuth = jsonUserCurrent;
  //    console.log("CURRENT USER AUTH  created : ", this.currentUserAuth)
}
</script>

<style>

.list-menu::before{
  content: '';
  display: inline-block;
  width: 4px;
  height: 12px;
  margin-right: 10px;
  -webkit-transform: skew(-20deg);
  -khtml-transform: skew(-20deg);
  -moz-transform: skew(-20deg);
  -o-transform: skew(-20deg);
  transform: skew(-20deg);
  background-color: #F5E7B2;
}

.dots-menu{
  display: flex;
  justify-content: center;
  align-items: center;
  font-size: 15px;
  color: #000;
}

@media (max-width: 992px) {
  .dots-menu{
    display: none
  }
}

</style>
