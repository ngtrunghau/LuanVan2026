import { createApp } from 'vue'
import { router } from './router';
import App from './App.vue'
import {BootstrapVue3, BToastPlugin} from 'bootstrap-vue-3'
import Antd from 'ant-design-vue';
import 'ant-design-vue/dist/reset.css';
import VueSelect from 'vue3-select2-component'
import VCalendar from 'v-calendar';
import VueFeather from 'vue-feather';
import DatePicker from 'vue3-datepicker';
import 'vue3-datepicker/dist/vue3-datepicker.css';
import Vue3Autocounter from 'vue3-autocounter';
import VueTelInput from 'vue3-tel-input';
import Treeselect from 'vue3-treeselect'
import { TreeView } from "vue-tree-view";
import VueMultiselect from 'vue-multiselect'

import { FileUpload } from 'primevue/fileupload';
import { RootTree } from "vue3-jstree-component"


// plugins
import 'bootstrap/dist/css/bootstrap.css';
import 'bootstrap-vue-3/dist/bootstrap-vue-3.css';
import '@fortawesome/fontawesome-free/css/fontawesome.min.css';
import '@fortawesome/fontawesome-free/css/all.min.css';
import 'vue3-tel-input/dist/vue3-tel-input.css';


/***********************************************************************/
                     /* Frontend */
/***********************************************************************/

//Breadcrumb
import Breadcrumb from '@/components/frontend/breadcrumb/aboutus.vue'

import MobiBreadcrumb from '@/components/frontend/breadcrumb/mobi.vue'


import Breadcrumb1 from '@/components/frontend/breadcrumb/breadcrumb1.vue'
import Mainmenu from '@/components/frontend/mainmenu.vue'
import Mainnav from '@/components/frontend/mainnav.vue'
import Profilewidget from '@/components/frontend/profilewidget.vue'


//pages
import Header from '@/views/frontend/layouts/header.vue'
import Cursor from '@/views/frontend/layouts/cursor.vue'
import Scrolltotop from '@/views/frontend/layouts/scrolltotop.vue'


import HeaderHome from '@/views/frontend/pages/home/headerHome'
import IndexFiveService from '@/views/frontend/pages/home/indexfiveservice.vue'
import FooterHome from '@/views/frontend/pages/home/footerHome'


/***********************************************************************/
                      /*  CẦU LÔNG */

import SanPhamDetail from '@/views/frontend/pages/home/sanpham/details.vue'
import SanPhamSidebar from '@/views/frontend/pages/home/sanpham/sidebar.vue'
import SanPhamVot from '@/views/frontend/pages/home/sanphamvot/content.vue'
import SanPhamAll from '@/views/frontend/pages/home/sanphamall/content.vue'
/***********************************************************************/

//pharmacy breadcrumb
import PharmacyBreadCrumb from '@/components/admin/breadcrumb/adminbreadcrumb.vue'
import PharmacyBreadCrumb1 from '@/components/admin/breadcrumb/adminbreadcrumb1.vue'
import PharmacyBreadCrumb2 from '@/components/admin/breadcrumb/adminbreadcrumb2.vue'
//pharmacy model
import pharmacymodel from '@/components/admin/model/adminmodel.vue'
import PharmacyDelete from '@/components/admin/admindelete.vue'

//pharmacy components
import PharmacyHeader from '@/views/admin/layouts/adminheader.vue'
import PharmacySidebar from '@/views/admin/layouts/adminsidebar.vue'




// IMPORT THƯ VIỆN
import store from '@/state/store'
import axios from "axios";
window.axios  = axios;


// Ckeditor
import CKEditor from '@ckeditor/ckeditor5-vue';
import VueDatePicker from '@vuepic/vue-datepicker';
import '@vuepic/vue-datepicker/dist/main.css'



// VUE - META




const app = createApp(App);

// const metaManager = createMetaManager();
// app.use(metaManager);

/***********************************************************************/
                      /*  CẦU LÔNG */
app.component('SanPhamDetail',SanPhamDetail)
app.component('SanPhamSidebar',SanPhamSidebar)
app.component('SanPhamVot',SanPhamVot)
app.component('SanPhamAll',SanPhamAll)


/***********************************************************************/

// Breadcrumb
app.component('breadcrumb',Breadcrumb)
app.component('mobibreadcrumb',MobiBreadcrumb)

app.component('breadcrumb1',Breadcrumb1)



app.component('mainmenu',Mainmenu)
app.component('mainnav',Mainnav)
app.component('profilewidget',Profilewidget)


//pages
app.component('layoutheader', Header)
app.component('cursor',Cursor)
app.component('scrolltotop',Scrolltotop)
app.component('headerHome',HeaderHome)
app.component('indexfiveservice',IndexFiveService)
app.component('footerHome',FooterHome)


 /*************************** Admin ****************************/
// Admin Breadcrumb


 /*************************** Pharmacy ****************************/
 // Pharmacy Breadcrumb
app.component('pharmacybreadcrumb',PharmacyBreadCrumb)
app.component('pharmacybreadcrumb1',PharmacyBreadCrumb1)
app.component('pharmacybreadcrumb2',PharmacyBreadCrumb2)

//Model
app.component('pharmacymodel',pharmacymodel)

//pharmacy
app.component('pharmacyheader',PharmacyHeader)
app.component('pharmacysidebar',PharmacySidebar)

app.component('pharmacydelete', PharmacyDelete)
app.component('Treeselect', Treeselect)
app.component('VueMultiselect ', VueMultiselect )
app.component('TreeView ', TreeView )
app.component('RootTree ', RootTree )
app.component('FileUpload ', FileUpload )
//app.component('VJstree', VJstree)
app.component('vue3-autocounter', Vue3Autocounter)
app.component('vue-select', VueSelect)
app.component(VueFeather.name, VueFeather)
app.component('datepicker', DatePicker)

.use(VueTelInput)
    .use(BootstrapVue3)
    .use(BToastPlugin)
.use(Antd)
app.use(VCalendar, {})
app.use(store)

//ckedittor
app.use(VueDatePicker)
app.use(CKEditor)
app.use(BootstrapVue3);



// app.use(createMetaManager())

app.use(router).mount('#app');


