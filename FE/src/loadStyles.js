// CẤU HÌNH STYLES THEO TỪNG ĐƯỜNG DAN
export function loadStyles() {
    if (window.location.href.includes("/quan-tri")) {
        // Load pharmacy template styles
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
