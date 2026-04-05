import {createRouter, createWebHistory} from 'vue-router';

import IndexFive from '@/views/frontend/pages/home/indexFive.vue'


/****************** THUOC ******************/



/********* WEB ***********/
import Login from "@/views/frontend/pages/home/login.vue";
import LoginQuanTri from "@/views/frontend/pages/home/loginquantri.vue";
import SanPham from "@/views/frontend/pages/home/sanpham/index.vue";
import SanPhamVot from "@/views/frontend/pages/home/sanphamvot/index.vue";
import SanPhamByCategory from "@/views/frontend/pages/home/sanphamall/index.vue";
import GioHang from "@/views/frontend/pages/home/giohang.vue";
import ThanhToan from "@/views/frontend/pages/home/thanhtoan.vue";
import DangKy from "@/views/frontend/pages/home/dangky.vue";
import ThanhToanThanhCong from "@/views/frontend/pages/home/thanhtoanthanhcong.vue";
import DonHang from "@/views/frontend/pages/home/donhang.vue";
import DonHangChiTiet from "@/views/frontend/pages/home/donhangchitiet.vue";
import ThongTinCaNhan from "@/views/frontend/pages/home/thongtincanhan.vue";
import PatientSignup from "@/views/frontend/pages/home/patientSignup.vue";

/**************** ADMIN  *************/

import Dashboard from '@/views/admin/pages/dashboard/index.vue'
import QuanLySanPham from '@/views/admin/pages/sanPham/index.vue'
import QuanLySanPhamChiTiet from '@/views/admin/pages/sanPham/updateSanPham.vue'
import QuanLySanPhamAdd from '@/views/admin/pages/sanPham/addSanPham.vue'
import QuanLyLoai from '@/views/admin/pages/loai/index.vue'
import QuanLyVaiTro from '@/views/admin/pages/unitRole/index.vue'
import QuanLyTaiKhoan from '@/views/admin/pages/user/index.vue'
import QuanLyBoDieuKhien from '@/views/admin/pages/bodieukhien/index.vue'
import QuanLyChucNang from '@/views/admin/pages/chucnang/index.vue'
import QuanLyKhachHang from '@/views/admin/pages/khachHang/index.vue'
import QuanLyDiaChi from '@/views/admin/pages/diaChi/index.vue'
import QuanLyKho from '@/views/admin/pages/kho/index.vue'
import QuanLyDatHang from '@/views/admin/pages/datHang/index.vue'
import ChiTietVanChuyen from '@/views/admin/pages/vanchuyen/index.vue'

import AdminLogin from '@/views/admin/pages/authentication/login.vue'
import AdminRegister from '@/views/admin/pages/authentication/register.vue'
import AdminForgotPassword from '@/views/admin/pages/authentication/forgotPassword.vue'
import AdminLockScreen from '@/views/admin/pages/authentication/lockScreen.vue'
import AdminError404 from '@/views/admin/pages/404/error404.vue'


import User from '@/views/admin/pages/user/index.vue'
import UserCitizen from '@/views/admin/pages/userCitizen/index.vue'




const routes = [

/**************** NEW  *************/



/****************************************/

/**************** Frontend  *************/

    {
        path: '/',
        name: '/',
        component: IndexFive,
        meta: {
            headerClass: 'header-ten',
            title: 'ShopNTH - Hệ Thống Shop Cầu Lông!',
            description: 'ShopNTH - Hệ Thống Shop Cầu Lông!'
       }
    },
    /**************** WEB  *************/
    {
        path: '/login',
        name: 'login',
        component: Login
    },
    {
        path: '/dang-nhap',
        name: 'dang-nhap',
        component: LoginQuanTri
    },
    {
        path: '/san-pham-vot',
        name: 'san-pham-vot',
        component: SanPhamVot
    },
    {
        path: '/san-pham-chi-tiet/:id?',
        name: 'san-pham-chi-tiet/:id?',
        component: SanPham
    },
    {
        path: '/san-pham/:id?',
        name: 'san-pham/:id?',
        component: SanPhamByCategory,
    },
    {
        path: '/gio-hang',
        name: 'gio-hang',
        component: GioHang,
    },
    {
        path: '/thanh-toan',
        name: 'thanh-toan',
        component: ThanhToan,
    },
    {
        path: '/dang-ky',
        name: 'dang-ky',
        component: DangKy,
    },
    {
        path: '/patient-signup',
        name: 'patient-signup',
        component: PatientSignup,
    },
    {
        path: '/thanh-toan-thanh-cong',
        name: 'thanh-toan-thanh-cong',
        component: ThanhToanThanhCong,
    },
    {
        path: '/don-hang',
        name: 'don-hang',
        component: DonHang,
    },
    {
        path: '/don-hang/chi-tiet/:id?',
        name: 'don-hang/chi-tiet/:id?',
        component: DonHangChiTiet
    },
    {
        path: '/thong-tin-ca-nhan',
        name: 'thong-tin-ca-nhan',
        component: ThongTinCaNhan
    },
    {
        path: '/error-404',
        name: 'error-404',
        component: AdminError404
    },

    /**************** Admin  *************/
    {
        path: '/quan-tri/dashboard',
        name: 'quan-tri/dashboard',
        component: Dashboard
    },
    {
        path: '/quan-tri/login',
        name: 'admin/login',
        alias: '/admin/login',
        component: AdminLogin
    },
    {
        path: '/quan-tri/register',
        name: 'admin/register',
        alias: '/admin/register',
        component: AdminRegister
    },
    {
        path: '/quan-tri/forgot-password',
        name: 'admin/forgot-password',
        alias: '/admin/forgot-password',
        component: AdminForgotPassword
    },
    {
        path: '/quan-tri/lock-screen',
        name: 'admin/lock-screen',
        alias: '/admin/lock-screen',
        component: AdminLockScreen
    },
    {
        path: '/quan-tri/quan-ly-san-pham',
        name: 'quan-tri/quan-ly-san-pham',
        component: QuanLySanPham
    },
    {
        path: '/quan-tri/quan-ly-san-pham/them-san-pham',
        name: 'quan-tri/quan-ly-san-pham/them-san-pham',
        component: QuanLySanPhamAdd
    },
    {
        path: '/quan-tri/quan-ly-san-pham/chi-tiet/:id?',
        name: 'quan-tri/quan-ly-san-pham/chi-tiet/:id?',
        component: QuanLySanPhamChiTiet
    },
    {
        path: '/quan-tri/quan-ly-loai',
        name: 'quan-tri/quan-ly-loai',
        component: QuanLyLoai
    },
    {
        path: '/quan-tri/quan-ly-vai-tro',
        name: 'quan-tri/quan-ly-vai-tro',
        component: QuanLyVaiTro
    },
    {
        path: '/quan-tri/quan-ly-tai-khoan',
        name: 'quan-tri/quan-ly-tai-khoan',
        component: QuanLyTaiKhoan
    },

    {
        path: '/quan-tri/bo-dieu-khien',
        name: '/quan-tri/bo-dieu-khien',
        component: QuanLyBoDieuKhien
    },
    {
        path: '/quan-tri/chuc-nang',
        name: '/quan-tri/chuc-nang',
        component: QuanLyChucNang
    },
    {
        path: '/quan-tri/khach-hang',
        name: '/quan-tri/khach-hang',
        component: QuanLyKhachHang
    },
    {
        path: '/quan-tri/dia-chi',
        name: '/quan-tri/dia-chi',
        component: QuanLyDiaChi
    },
    {
        path: '/quan-tri/kho',
        name: '/quan-tri/kho',
        component: QuanLyKho
    },
    {
        path: '/quan-tri/dat-hang',
        name: '/quan-tri/dat-hang',
        component: QuanLyDatHang
    },
    {
        path: '/quan-tri/chi-tiet-van-chuyen',
        name: 'quan-tri/chi-tiet-van-chuyen',
        component: ChiTietVanChuyen
    },
    {
        path: '/quan-tri/error-404',
        name: 'admin/error-404',
        alias: '/admin/error-404',
        component: AdminError404
    },
    {
        path: '/quan-tri/tai-khoan',
        name: 'quan-tri/tai-khoan',
        component: User
    },

    {
        path: '/quan-tri/tai-khoan-cong-dan',
        name: 'quan-tri/tai-khoan-cong-dan',
        component: UserCitizen
    },





    /**************** Admin  *************/


]


export  const router = createRouter({
    history: createWebHistory(),
    routes,
});


router.beforeEach((to, from, next) => {
// Scroll to the top of the page
window.scrollTo({ top: 0, behavior: 'smooth' });

// Continue with the navigation
next();
});

