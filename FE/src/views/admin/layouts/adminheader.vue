<template>
  <!-- Header -->
  <div class="header">
    <!-- Logo -->
    <div class="header-left">
      <router-link to="/quan-tri/tai-khoan" class="logo">
        <img src="@/assets/img/caulong/logo/logo_HBTShop-removebg.png" alt="Logo" />
      </router-link>

      <router-link to="/quan-tri/tai-khoan" class="logo logo-small">
        <img
          src="@/assets/img/caulong/logo/logo_HBTShop-removebg.png"
          alt="Logo"
          width="100"
          height="100"
        />
      </router-link>
    </div>
    <!-- /Logo -->

    <a href="javascript:void(0);" id="toggle_btn" @click="toggleSidebar">
      <i class="fe fe-text-align-left"></i>
    </a>

    <!-- <div class="top-nav-search">
      <form>
        <input type="text" class="form-control" placeholder="Search here" />
        <b-button class="btn" type="submit"><i class="fa fa-search"></i></b-button>
      </form>
    </div> -->

    <!-- Mobile Menu Toggle -->
    <a class="mobile_btn" id="mobile_btn" @click="toggleSidebar1">
      <i class="fa fa-bars"></i>
    </a>
    <!-- /Mobile Menu Toggle -->

    <!-- Header Right Menu -->
    <ul class="nav user-menu">
      <!-- Notifications -->
      
      <!-- /Notifications -->

      <!-- User Menu -->
      <li class="nav-item dropdown has-arrow">
        <a
          href="javascript:void(0)"
          class="dropdown-toggle nav-link"
          data-bs-toggle="dropdown"
        >
          <span class="user-img"
            ><img
              class="rounded-circle"
              src="@/assets/pharmacy/img/profiles/avatar-01.jpg"
              width="31"
              alt="Ryan Taylor"
          /></span>
        </a>
        <div class="dropdown-menu">
          <div class="user-header">
            <div class="avatar avatar-sm">
              <img
                src="@/assets/pharmacy/img/profiles/avatar-01.jpg"
                alt="User Image"
                class="avatar-img rounded-circle"
              />
            </div>
            <div class="user-text" v-if="this.model">
              <h6>{{ this.model.userName}}</h6>
              <p class="text-muted mb-0">{{ this.model.role}}</p>
            </div>
          </div>
          <!-- <router-link class="dropdown-item" to="/quan-tri/profile"
                       v-if="this.model"
            >Thông tin tài khoản</router-link
          > -->
          <div class="dropdown-item" @click.prevent="logout">Đăng xuất</div>
        </div>
      </li>
      <!-- /User Menu -->
    </ul>
    <!-- /Header Right Menu -->
  </div>
  <!-- /Header -->
</template>

<script>
export default {
  data() {
    return {
      model : null
    };
  },
  mounted() {

    const auth = localStorage.getItem('auth-user')
    let authParse = JSON.parse(auth)
    if (authParse) {

      this.model = authParse ;

    }
    // Add click event listener
    this.$nextTick(() => {
      document.addEventListener("click", this.handleToggleClick);
    });

    // Add mouseover event listener
    document.addEventListener("mouseover", (event) => {
      event.stopPropagation();

      var body = document.body;
      var toggleBtn = document.getElementById("toggle_btn");
      var sidebar = document.getElementsByClassName("sidebar")[0];
      var subdropUL = document.getElementsByClassName("subdrop");

      if (body.classList.contains("mini-sidebar") && toggleBtn.style.display !== "none") {
        var target = event.target.closest(".sidebar");

        if (target) {
          body.classList.add("expand-menu");
          for (var i = 0; i < subdropUL.length; i++) {
            var ul = subdropUL[i].nextElementSibling;
            if (ul) {
              ul.style.display = "block";
            }
          }
        } else {
          body.classList.remove("expand-menu");
          for (var i = 0; i < subdropUL.length; i++) {
            var ul = subdropUL[i].nextElementSibling;
            if (ul) {
              ul.style.display = "none";
            }
          }
        }

        event.preventDefault();
      }
    });
  },
  beforeUnmount() {
    document.removeEventListener("click", this.handleToggleClick);
  },

  methods: {
    logout() {
      
      // 2. Xóa dữ liệu đăng nhập
      localStorage.removeItem("auth-user");
      localStorage.removeItem("token");
      localStorage.removeItem("user-token");
      
      
      // 5. Chuyển hướng về trang chủ nếu đang ở trang khác
      window.location.href = "/dang-nhap";
      
      // 6. Có thể thêm thông báo cho người dùng
      this.$store.dispatch("snackBarStore/addNotify", {
        message: "Đã đăng xuất thành công",
        variant: "success"
      });
    },
    reloadPage() {
      window.location.href = "/";
    },
    toggleSidebar1() {
      const body = document.body;
      body.classList.toggle("slide-nav");
    },
    toggleSidebar() {
      const body = document.body;
      body.classList.toggle("mini-sidebar");
    },
  },
};
</script>
