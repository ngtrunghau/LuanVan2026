// CẤU HÌNH STYLES THEO TỪNG ĐƯỜNG DAN
export function loadStyles() {
    const path = window.location.pathname.replace(/\/$/, '') || '/';
    const isAdminLogin = path === '/quan-tri/login' || path === '/admin/login';

    // Trang đăng nhập quản trị đang dùng giao diện frontend (loginquantri.vue),
    // nên phải nạp bộ CSS frontend thay vì CSS của dashboard quản trị.
    if (isAdminLogin) {
        import("@/assets/css/feather.css");
        import("@/assets/css/custom.css");
        import("@/assets/css/style.css");
    } else if (path.startsWith("/quan-tri")) {
        // Load admin styles
        import("@/assets/admin/css/feathericon.min.css");
        import("@/assets/admin/css/custom.css");
    } else if (window.location.href.includes("/template/admin/")) {
        // Load admin template styles
        import("@/assets/admin/css/feathericon.min.css");
        import("@/assets/admin/css/custom.css");
    } else {
        // Load default styles
        import("@/assets/css/feather.css");
        import("@/assets/css/custom.css");
        import("@/assets/css/style.css");
    }
}
