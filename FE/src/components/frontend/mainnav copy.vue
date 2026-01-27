<template>
  <ul class="main-nav">
    <template v-for="item in limitedListMenu" v-if="listMenu">
      <li class="has-submenu megamenu"  v-if="!hasItems(item)" :class="isCheck(item.link) ? 'active' : 'notactive'">
        <a @click="toggleVisibilityhomes(item)"
           >{{item.name}}
        </a>
      </li>
      <li class="has-submenu" v-else>
        <a href="javascript:void(0);" @click="toggleVisibilityhomes(item)"
        >
          {{item.name}}
          <i class="fas fa-angle-down" >
          </i>
        </a>
        <ul class="submenu" :style= "{display: 'block'}"  >
          <li :class="isCheck(value.link) ? 'active' : 'notactive'" v-for="(value) in item.children"
              class="has-submenu"
          >
            <a @click="toggleVisibilityhomes(value)"> {{value.name}}
              <i class="fa-solid fa-chevron-right" v-if="isChildren(value)">
              </i>
            </a>
            <ul
                class="submenu inner-submenu"
                :style="{ display:  'block' }"
                v-if="isChildren(value)"
            >
              <li
                  v-for="(itemChildren) in value.children"
              >
                <a
                    @click="toggleVisibilityhomes(itemChildren)"
                >
                  {{itemChildren.name}}
                </a>
              </li>
            </ul>
          </li>

        </ul>
      </li>
    </template>
    <div
      class="dots-menu"
    >
      <button @click="showPopup = true"
        style="
          border: none;
          background-color: inherit;"
      >
        <i class="fa fa-ellipsis-h" aria-hidden="true"></i>
      </button>
      <Popup :isVisible="showPopup" @close="showPopup = false" style="padding-top:25px;">
        <div class="row">
          <div class="col-md-3" v-for="item in listMenu" v-if="listMenu" style="padding-top: 10px; padding-bottom: 10px;">
            <div class="has-submenu megamenu list-menu"
              v-if="!hasItems(item)" :class="isCheck(item.link) ? 'active' : 'notactive'"
              style="text-transform: uppercase; font-weight: bold; color: #000;"
            >
              <a @click="toggleVisibilityhomes(item.link, item.id)">
                 {{item.name}}
              </a>
            </div>
            <div class="has-submenu list-menu" v-else>
              <a href="javascript:void(0);"
                 style="text-transform: uppercase; font-weight: bold; color: #000;"
              >   {{item.name}} <i class="fas fa-chevron-down"></i
              ></a>
              <ul class="submenu" :style= " {display: 'block'} "  >
                <li :class="isCheck(value.link) ? 'active' : 'notactive'" v-for="(value) in item.children" style="padding: 5px 0;">
                  <router-link :to="handleGetIdMenu(value.link, value.id)" style="font-weight: 400;"> {{value.name}}</router-link>
                </li>
              </ul>
            </div>
          </div>
        </div>
      </Popup>
    </div>
    <li class="login-link">
      <router-link to="/login">Đăng nhập</router-link>
    </li>
    <li class="login-link">
      <router-link to="/login">Đăng ký</router-link>
    </li>
  </ul>
</template>
<script>
import Popup from '@/components/popup/index.vue';
export default {
  data() {
    return {
      showPopup: false,
      showButton: false,
      start: "",
      end: "",
      interval: "",
      days: "",
      minutes: "",
      hours: "",
      seconds: "",
      starttime: "Nov 5, 2020 15:37:25",
      endtime: "Dec 31, 2021 16:37:25",
      showModal: false,
      showRegisterModal: false,
      showForgotModal: false,
      showNotify: false,
      email: "",
      password: "",
      submitted: false,
      authError: null,
      tryingToLogIn: false,
      isAuthError: false,
      capcha: null,
      modelRegister: {
        firstName: null,
        lastName: null,
        userName: null,
        soDienThoai: null,
        email: null,
        password: null,
        confirmPassword: null,
        phoneNumber: null,
        emailAddress: null
      },
      modelAuth: {
        isAuthError: false,
        message: null
      },
      model: {
        userName: "",
        password: ""
      },
      currentUserAuth: null,
      isShow: false,
   //   url : `${process.env.VUE_APP_API_URL}filesminio/view/`,
      showPDF: false,
      pdfID: '',
      toltip: null,
      file: {
        id: null,
        fileId: null,
        fileName: null,
        ext: ".pdf"
      },
      otpShow: false,
      verifyOpt: {
        sender: "du.tranphuoc@gmail.com",
        receiver: "0836980284",
        token: ""
      },
      sendSmsOtp: {
        sender: "du.tranphuoc@gmail.com",
        receiver: "0836980284",
        applicationTitle: "Test DGov"
      },
      accessToken: null,
      tempUser: null,
      showButtonSendOTP: true,
      event: null,
      listMenu: [],
      lienHe: [],
      suKien:[],
      idMenu : "",
      urlHeader: "",
      passwordFieldType: "password",
      isVisible: false,
      isVisiblehomes: false,
      isVisibledoctor: false,
      isVisibledoctorsblog: false,
      isVisiblepatients: false,
      isVisiblepatientsdoctor: false,
      isVisiblepatientssearch: false,
      isVisiblepatientsbooking: false,
      isVisiblepharmacy: false,
      isVisiblepages: false,
      isVisiblecall: false,
      isVisibleinvoice: false,
      isVisibleauthentication: false,
      isVisibleerror: false,
      isVisibleblog: false,
      isVisibleadmin: false,
      menuId : null,
      url : `${process.env.VUE_APP_URL}`,
    };
  },
  components: {
    Popup,
  },
  computed: {
    limitedListMenu() {
      return this.listMenu.slice(0, 11); // Lấy phần tử từ 0 đến 6
    },
    isHomeOneRoute() {
      return (
        this.$route.path === "/" ||
        this.$route.path === "/about-us" ||
        this.$route.path === "/blank-page" ||
        this.$route.path === "/blog-details" ||
        this.$route.path === "/blog-grid" ||
        this.$route.path === "/blog-list" ||
        this.$route.path === "/coming-soon" ||
        this.$route.path === "/components" ||
        this.$route.path === "/contact-us" ||
        this.$route.path === "/doctor-register" ||
        this.$route.path === "/doctor-search-grid" ||
        this.$route.path === "/doctor-signup" ||
        this.$route.path === "/error-404" ||
        this.$route.path === "/error-500" ||
        this.$route.path === "/faq" ||
        this.$route.path === "/forgot-password" ||
        this.$route.path === "/forgot-password2" ||
        this.$route.path === "/login-email-otp" ||
        this.$route.path === "/login-email" ||
        this.$route.path === "/login-phone-otp" ||
        this.$route.path === "/login" ||
        this.$route.path === "/maintenance" ||
        this.$route.path === "/map-grid" ||
        this.$route.path === "/map-list" ||
        this.$route.path === "/mobile-otp" ||
        this.$route.path === "/patient-signup" ||
        this.$route.path === "/pharmacy-register" ||
        this.$route.path === "/pricing" ||
        this.$route.path === "/privacy-policy" ||
        this.$route.path === "/register" ||
        this.$route.path === "/search-2" ||
        this.$route.path === "/search" ||
        this.$route.path === "/signup-success" ||
        this.$route.path === "/terms-condition" ||
        this.$route.path === "/email-otp" ||
        this.$route.path === "/login-phone"
      );
    },
    isHomeSeven2Route() {
      return (
        this.$route.path === "/index-7" ||
        this.$route.path === "/index-8" ||
        this.$route.path === "/index-9" ||
        this.$route.path === "/index-10" ||
        this.$route.path === "/accounts" ||
        this.$route.path === "/add-billing" ||
        this.$route.path === "/add-prescription" ||
        this.$route.path === "/appointments" ||
        this.$route.path === "/available-timings" ||
        this.$route.path === "/booking-2" ||
        this.$route.path === "/booking-success" ||
        this.$route.path === "/booking" ||
        this.$route.path === "/cart" ||
        this.$route.path === "/change-password" ||
        this.$route.path === "/chat-doctor" ||
        this.$route.path === "/chat" ||
        this.$route.path === "/checkout" ||
        this.$route.path === "/dependent" ||
        this.$route.path === "/doctor-add-blog" ||
        this.$route.path === "/doctor-blog" ||
        this.$route.path === "/doctor-change-password" ||
        this.$route.path === "/doctor-dashboard" ||
        this.$route.path === "/doctor-pending-blog" ||
        this.$route.path === "/doctor-profile-settings" ||
        this.$route.path === "/doctor-profile" ||
        this.$route.path === "/edit-billing" ||
        this.$route.path === "/edit-blog" ||
        this.$route.path === "/edit-prescription" ||
        this.$route.path === "/favourites" ||
        this.$route.path === "/invoice-view" ||
        this.$route.path === "/invoices" ||
        this.$route.path === "/medical-details" ||
        this.$route.path === "/medical-records" ||
        this.$route.path === "/my-patients" ||
        this.$route.path === "/orders-list" ||
        this.$route.path === "/patient-accounts" ||
        this.$route.path === "/patient-dashboard" ||
        this.$route.path === "/patient-profile" ||
        this.$route.path === "/payment-success" ||
        this.$route.path === "/pharmacy-details" ||
        this.$route.path === "/pharmacy-index" ||
        this.$route.path === "/product-all" ||
        this.$route.path === "/product-checkout" ||
        this.$route.path === "/product-description" ||
        this.$route.path === "/profile-settings" ||
        this.$route.path === "/reviews" ||
        this.$route.path === "/schedule-timings" ||
        this.$route.path === "/social-media" ||
        this.$route.path === "/video-call" ||
        this.$route.path === "/voice-call" ||
        this.$route.path === "/pharmacy-search"
      );
    },
    currentPath() {
      return this.$route.name;
    },
    homeMenu() {
      return (
        this.$route.name == "index" ||
        this.$route.name == "index-2" ||
        this.$route.name == "index-3" ||
        this.$route.name == "index-4" ||
        this.$route.name == "index-5" ||
        this.$route.name == "index-6" ||
        this.$route.name == "index-7" ||
        this.$route.name == "index-8" ||
        this.$route.name == "index-9" ||
        this.$route.name == "index-10" ||
        this.$route.name == "index-11" ||
        this.$route.name == "index-12" ||
        this.$route.name == "index-13"
      );
    },
    doctorMenu() {
      return (
        this.$route.name == "doctor-dashboard" ||
        this.$route.name == "appointments" ||
        this.$route.name == "schedule-timings" ||
        this.$route.name == "my-patients" ||
        this.$route.name == "patient-profile" ||
        this.$route.name == "edit-prescription" ||
        this.$route.name == "add-billing" ||
        this.$route.name == "chat-doctor" ||
        this.$route.name == "doctor-profile-settings" ||
        this.$route.name == "reviews" ||
        this.$route.name == "doctor-register" ||
        this.$route.name == "doctor-blog" ||
        this.$route.name == "edit-blog" ||
        this.$route.name == "blog-details" ||
        this.$route.name == "doctor-add-blog" ||
        this.$route.name == "doctor-pending-blog" ||
        this.$route.name == "add-prescription" ||
        this.$route.name == "edit-billing"
      );
    },
    patientsMenu() {
      return (
        this.$route.name == "map-grid" ||
        this.$route.name == "map-list" ||
        this.$route.name == "search" ||
        this.$route.name == "search-2" ||
        this.$route.name == "doctor-profile" ||
        this.$route.name == "booking" ||
        this.$route.name == "booking-2" ||
        this.$route.name == "checkout" ||
        this.$route.name == "booking-success" ||
        this.$route.name == "patient-dashboard" ||
        this.$route.name == "favourites" ||
        this.$route.name == "chat" ||
        this.$route.name == "profile-settings" ||
        this.$route.name == "change-password" ||
        this.$route.name == "dependent"
      );
    },
    pharmacyMenu() {
      return (
        this.$route.name == "pharmacy-index" ||
        this.$route.name == "pharmacy-details" ||
        this.$route.name == "pharmacy-search" ||
        this.$route.name == "product-all" ||
        this.$route.name == "product-description" ||
        this.$route.name == "cart" ||
        this.$route.name == "product-checkout" ||
        this.$route.name == "payment-success" ||
        this.$route.name == "pharmacy-register"
      );
    },
    pagesMenu() {
      return (
        this.$route.name == "about-us" ||
        this.$route.name == "contact-us" ||
        this.$route.name == "blank-page" ||
        this.$route.name == "pricing" ||
        this.$route.name == "faq" ||
        this.$route.name == "maintenance" ||
        this.$route.name == "coming-soon" ||
        this.$route.name == "terms-condition" ||
        this.$route.name == "privacy-policy" ||
        this.$route.name == "components" ||
        this.$route.name == "voice-call" ||
        this.$route.name == "video-call" ||
        this.$route.name == "invoices" ||
        this.$route.name == "invoice-view" ||
        this.$route.name == "login-email" ||
        this.$route.name == "login-phone" ||
        this.$route.name == "doctor-signup" ||
        this.$route.name == "patient-signup" ||
        this.$route.name == "forgot-password" ||
        this.$route.name == "forgot-password2" ||
        this.$route.name == "login-email-otp" ||
        this.$route.name == "login-phone-otp" ||
        this.$route.name == "error-404" ||
        this.$route.name == "error-500" ||
        this.$route.name == "invoices" ||
        this.$route.name === "mobile-otp"
      );

    },




    blogMenu() {
      return (
        this.$route.name == "blog-list" ||
        this.$route.name == "blog-grid" ||
        this.$route.name == "blog-details"
      );
    },
    adminMenu() {
      return this.$route.name == "quan-tri" || this.$route.name == "/quan-tri";
    },
  },
  created() {

    let authUser = localStorage.getItem("auth-user");
    if (authUser) {
      let jsonUserCurrent = JSON.parse(authUser);
      this.currentUserAuth = jsonUserCurrent;
 //    console.log("CURRENT USER AUTH  created : ", this.currentUserAuth)
    }
  },
  methods: {
    hasItems(item) {
   //   console.log("LOG hasItems ")
      return item.children !== undefined ? item.children?.length > 0 : false;
    },
    isChildren(item) {
     // console.log("LOG IS CHILDREN ", item !== undefined ? item.children?.length > 0 : false)
      return item !== undefined ? ( item.children !== undefined && item.children?.length > 0 ): false;
    },
    handleGetIdMenu(path, id) {
      console.log("LOG HANDLE GET ID MENU ", path, id)
      if (path != window.location.pathname &&  path !== "/" && path === '/ban-tin/id')
      {
       // console.log(`LOG NE IF  : ${id} : ` +  path)
     //   window.open(this.url + "/ban-tin/" +  id)
         return '/ban-tin/'+ id ;
      }
      if (path != window.location.pathname &&  path !== "/" && path === '/bao-tri/id')
      {
      //   console.log(`HANDLE BAO TRI : ${id} : ` +  path)
     //   window.open(this.url  + "/bao-tri/" +  id)
        return  this.$router.push("/bao-tri/" +  id);
      }
      if (path != window.location.pathname &&  path !== "/" && path === '/dich-vu/id')
      {
        //  console.log(`HANDLE GET ID MENU : ${id} : ` +  path)
       // window.open(this.url + "/dich-vu/" +  id)
        return  this.$router.push("/dich-vu/" +  id);
      }
      if (path != window.location.pathname)
     //  console.log(`LOG NE ELSE  : ${id} : ` +  path)
       // window.open(path)
         return path ;

    },

    isCheck(item) {
    //  console.log("LOG IS CHECK " , item)
      if (this.$route.name == "/")
      {
          return  `${this.$route.name}` == item;
      }
      if (item != null && this.$route.name != null)
        return  `/${this.$route.name}` == item;
      else
        return false ;
    },
    submitForm() {
      this.$router.push("/search");
    },
    toggleElement() {
      this.isVisible = !this.isVisible;
    },
    toggleVisibilityhomes(item) {
      document.documentElement.classList.remove("menu-opened");
      localStorage.setItem('menu', JSON.stringify(item));

      //console.log("LOG ITEM : ", item.link);

      if (item.link != window.location.pathname && item.link =='/ban-tin/id'){
      //    console.log("LOG ITEM 1 : ", item)
       return  this.$router.push("/ban-tin/" +  item.id);
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
      if (item.link != window.location.pathname)
    //      window.open(path)
   //     console.log("IF 02  " , path , window.location.pathname)
   //        console.log("LOG ITEM 3 : ", item)
        return this.$router.push(item.link);


    },

  //   onClickMenuParent(item)
  //   {
  // //    console.log("LOG ON CLICK MENU PARENT ")
  //     this.menuId = item.id;
  //     this.isVisibledoctor = !this.isVisibledoctor;
  //   },

    toggleVisibilitydoctorsblog() {
      this.isVisibledoctorsblog = !this.isVisibledoctorsblog;
    },
    toggleVisibilitypatients() {
      this.isVisiblepatients = !this.isVisiblepatients;
    },
    toggleVisibilitypatientsdoctors() {
      this.isVisiblepatientsdoctor = !this.isVisiblepatientsdoctor;
    },
    toggleVisibilitysearch() {
      this.isVisiblepatientssearch = !this.isVisiblepatientssearch;
    },
    toggleVisibilitybooking() {
      this.isVisiblepatientsbooking = !this.isVisiblepatientsbooking;
    },
    toggleVisibilitypharmacy() {
      this.isVisiblepharmacy = !this.isVisiblepharmacy;
    },
    toggleVisibilitypages() {
      this.isVisiblepages = !this.isVisiblepages;
    },
    toggleVisibilitycall() {
      this.isVisiblecall = !this.isVisiblecall;
    },
    toggleVisibilityinvoice() {
      this.isVisibleinvoice = !this.isVisibleinvoice;
    },
    toggleVisibilityauthentication() {
      this.isVisibleauthentication = !this.isVisibleauthentication;
    },
    toggleVisibilityerror() {
      this.isVisibleerror = !this.isVisibleerror;
    },
    toggleVisibilityblog() {
      this.isVisibleblog = !this.isVisibleblog;
    },
    toggleVisibilityadmin() {
      this.isVisibleadmin = !this.isVisibleadmin;
    },
    redirectReloadmapgrid() {
      this.$router.push({ path: "/map-grid" }).then(() => {
        this.$router.go();
      });
    },
    redirectReloadmaplist() {
      this.$router.push({ path: "/map-list" }).then(() => {
        this.$router.go();
      });
    },
  },
};
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
