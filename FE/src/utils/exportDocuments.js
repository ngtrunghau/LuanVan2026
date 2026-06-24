import { jsPDF } from "jspdf";
import autoTable from "jspdf-autotable";
import * as XLSX from "xlsx";
import regularFontUrl from "@/assets/fonts/SVN-Poppins-Regular.ttf";
import boldFontUrl from "@/assets/fonts/SVN-Poppins-Bold.ttf";

let fontCache = null;

function arrayBufferToBase64(buffer) {
  const bytes = new Uint8Array(buffer);
  const chunkSize = 0x8000;
  let binary = "";
  for (let i = 0; i < bytes.length; i += chunkSize) {
    binary += String.fromCharCode(...bytes.subarray(i, i + chunkSize));
  }
  return btoa(binary);
}

async function loadPdfFonts() {
  if (!fontCache) {
    fontCache = Promise.all([
      fetch(regularFontUrl).then(response => response.arrayBuffer()).then(arrayBufferToBase64),
      fetch(boldFontUrl).then(response => response.arrayBuffer()).then(arrayBufferToBase64)
    ]);
  }
  return fontCache;
}

async function createPdf(options = {}) {
  const doc = new jsPDF(options);
  const [regularFont, boldFont] = await loadPdfFonts();
  doc.addFileToVFS("SVN-Poppins-Regular.ttf", regularFont);
  doc.addFont("SVN-Poppins-Regular.ttf", "Poppins", "normal");
  doc.addFileToVFS("SVN-Poppins-Bold.ttf", boldFont);
  doc.addFont("SVN-Poppins-Bold.ttf", "Poppins", "bold");
  doc.setFont("Poppins", "normal");
  return doc;
}

function currency(value) {
  return `${Number(value || 0).toLocaleString("vi-VN")} đ`;
}

function dateTime(value) {
  if (!value) return "--";
  return new Intl.DateTimeFormat("vi-VN", {
    day: "2-digit",
    month: "2-digit",
    year: "numeric",
    hour: "2-digit",
    minute: "2-digit"
  }).format(new Date(value));
}

function safeFilePart(value) {
  return String(value || "khong-ma").replace(/[\\/:*?"<>|]/g, "-");
}

function orderStatus(status) {
  return {
    1: "Đã đặt",
    2: "Đang vận chuyển",
    3: "Đã nhận",
    4: "Đã hủy"
  }[Number(status)] || "Chưa xác định";
}

function fullAddress(address) {
  return [
    address?.address,
    address?.town?.name,
    address?.district?.name,
    address?.province?.name
  ].filter(Boolean).join(", ") || "--";
}

function setDocumentTitle(doc, title, subtitle) {
  const pageWidth = doc.internal.pageSize.getWidth();
  doc.setFont("Poppins", "bold");
  doc.setFontSize(18);
  doc.text(title, pageWidth / 2, 18, { align: "center" });
  doc.setFont("Poppins", "normal");
  doc.setFontSize(9);
  doc.setTextColor(90);
  doc.text(subtitle, pageWidth / 2, 25, { align: "center" });
  doc.setTextColor(0);
}

export async function exportOrderInvoicePdf(order) {
  if (!order?.id) throw new Error("Không có dữ liệu đơn hàng để xuất hóa đơn.");

  const doc = await createPdf();
  const payment = order.payments?.[0] || {};
  const items = order.items || [];
  const subTotal = items.reduce(
    (sum, item) => sum + Number(item.price || 0) * Number(item.quantity || 0),
    0
  );
  const discount = Number(order.discountAmount || Math.max(0, subTotal - Number(order.totalAmount || 0)));
  const total = Number(order.totalAmount ?? subTotal - discount);

  setDocumentTitle(doc, "HÓA ĐƠN BÁN HÀNG", `Ngày xuất: ${dateTime(new Date())}`);

  doc.setFontSize(10);
  doc.setFont("Poppins", "bold");
  doc.text("THÔNG TIN ĐƠN HÀNG", 14, 36);
  doc.setFont("Poppins", "normal");
  doc.text(`Mã đơn hàng: ${order.txnRef || `DH-${order.id}`}`, 14, 43);
  doc.text(`Ngày đặt: ${dateTime(order.orderDate)}`, 14, 49);
  doc.text(`Trạng thái: ${orderStatus(order.status)}`, 14, 55);
  doc.text(`Thanh toán: ${payment.paymentMethod || "Thanh toán khi nhận hàng"}`, 14, 61);

  doc.setFont("Poppins", "bold");
  doc.text("THÔNG TIN KHÁCH HÀNG", 108, 36);
  doc.setFont("Poppins", "normal");
  doc.text(`Người nhận: ${order.customer?.fullName || "--"}`, 108, 43);
  doc.text(`Số điện thoại: ${order.customer?.phone || "--"}`, 108, 49);
  const addressLines = doc.splitTextToSize(`Địa chỉ: ${fullAddress(order.address)}`, 88);
  doc.text(addressLines, 108, 55);

  autoTable(doc, {
    startY: Math.max(70, 55 + addressLines.length * 5),
    head: [["STT", "Sản phẩm", "Đơn giá", "SL", "Thành tiền"]],
    body: items.map((item, index) => [
      index + 1,
      item.products?.name || "--",
      currency(item.price),
      Number(item.quantity || 0).toLocaleString("vi-VN"),
      currency(Number(item.price || 0) * Number(item.quantity || 0))
    ]),
    styles: { font: "Poppins", fontSize: 8, cellPadding: 2.5 },
    headStyles: { font: "Poppins", fontStyle: "bold", fillColor: [31, 122, 140] },
    columnStyles: {
      0: { halign: "center", cellWidth: 12 },
      2: { halign: "right", cellWidth: 35 },
      3: { halign: "center", cellWidth: 14 },
      4: { halign: "right", cellWidth: 38 }
    },
    didParseCell(data) {
      data.cell.styles.font = "Poppins";
    }
  });

  let y = doc.lastAutoTable.finalY + 8;
  const labelX = 135;
  const valueX = 196;
  doc.setFontSize(9);
  doc.text("Tạm tính:", labelX, y);
  doc.text(currency(subTotal), valueX, y, { align: "right" });
  if (discount > 0) {
    y += 6;
    doc.text("Giảm giá:", labelX, y);
    doc.text(`-${currency(discount)}`, valueX, y, { align: "right" });
  }
  y += 7;
  doc.setFont("Poppins", "bold");
  doc.setFontSize(11);
  doc.text("TỔNG CỘNG:", labelX, y);
  doc.text(currency(total), valueX, y, { align: "right" });

  y += 18;
  doc.setFont("Poppins", "normal");
  doc.setFontSize(9);
  doc.text("Cảm ơn quý khách đã mua hàng!", 105, y, { align: "center" });
  doc.save(`hoa-don-${safeFilePart(order.txnRef || order.id)}.pdf`);
}

function createSheet(rows, widths = []) {
  const sheet = XLSX.utils.aoa_to_sheet(rows);
  sheet["!cols"] = widths.map(width => ({ wch: width }));
  return sheet;
}

export function exportDashboardExcel(report) {
  const workbook = XLSX.utils.book_new();
  const { filters, summaryCards, trend, categoryData, paymentData, inventory } = report;

  XLSX.utils.book_append_sheet(workbook, createSheet([
    ["BÁO CÁO TỔNG HỢP"],
    ["Thời gian", `${filters.fromDate} - ${filters.toDate}`],
    [],
    ["Chỉ số", "Giá trị", "So sánh/Ghi chú"],
    ...summaryCards.map(item => [item.title, item.value, item.changeText])
  ], [30, 25, 45]), "Tong quan");

  XLSX.utils.book_append_sheet(workbook, createSheet([
    ["Kỳ", "Doanh thu (VND)"],
    ...(trend.points || []).map(item => [item.label, Number(item.value || 0)])
  ], [25, 25]), "Doanh thu");

  XLSX.utils.book_append_sheet(workbook, createSheet([
    ["Loại sản phẩm", "Doanh thu (VND)"],
    ...(categoryData || []).map(item => [item.label, Number(item.value || 0)])
  ], [35, 25]), "Theo loai");

  XLSX.utils.book_append_sheet(workbook, createSheet([
    ["Phương thức", "Doanh thu (VND)"],
    ...(paymentData || []).map(item => [item.label, Number(item.value || 0)])
  ], [35, 25]), "Thanh toan");

  XLSX.utils.book_append_sheet(workbook, createSheet([
    ["Sản phẩm sắp hết", "Tồn hiện tại", "Bán/ngày", "Nhu cầu dự báo", "Lý do"],
    ...(inventory.lowStockProducts || []).map(item => [
      item.productName, item.currentStock, item.avgDailySales, item.forecastNeed, item.reason
    ]),
    [],
    ["Sản phẩm tồn dư", "Tồn hiện tại", "Ngày đủ hàng", "Tỷ lệ bán", "Lý do"],
    ...(inventory.overstockProducts || []).map(item => [
      item.productName, item.currentStock, item.stockCoverDays,
      `${Number(item.sellThroughRate || 0) * 100}%`, item.reason
    ])
  ], [35, 18, 18, 20, 50]), "Ton kho");

  XLSX.writeFile(workbook, `bao-cao-${filters.fromDate}-${filters.toDate}.xlsx`);
}

export async function exportDashboardPdf(report) {
  const doc = await createPdf({ orientation: "landscape" });
  const { filters, summaryCards, trend, categoryData, paymentData, inventory } = report;
  const period = `${filters.fromDate} - ${filters.toDate}`;
  setDocumentTitle(doc, "BÁO CÁO TỔNG HỢP DOANH THU VÀ TỒN KHO", `Kỳ báo cáo: ${period}`);

  autoTable(doc, {
    startY: 32,
    head: [["Chỉ số", "Giá trị", "So sánh/Ghi chú"]],
    body: summaryCards.map(item => [item.title, item.value, item.changeText]),
    styles: { font: "Poppins", fontSize: 7 },
    headStyles: { font: "Poppins", fontStyle: "bold", fillColor: [31, 122, 140] }
  });

  autoTable(doc, {
    startY: doc.lastAutoTable.finalY + 8,
    head: [["Kỳ", "Doanh thu", "Loại sản phẩm", "Doanh thu theo loại", "Thanh toán", "Doanh thu"]],
    body: Array.from({
      length: Math.max(trend.points?.length || 0, categoryData?.length || 0, paymentData?.length || 0)
    }, (_, index) => [
      trend.points?.[index]?.label || "",
      trend.points?.[index] ? currency(trend.points[index].value) : "",
      categoryData?.[index]?.label || "",
      categoryData?.[index] ? currency(categoryData[index].value) : "",
      paymentData?.[index]?.label || "",
      paymentData?.[index] ? currency(paymentData[index].value) : ""
    ]),
    styles: { font: "Poppins", fontSize: 6.5 },
    headStyles: { font: "Poppins", fontStyle: "bold", fillColor: [42, 157, 143] }
  });

  doc.addPage("a4", "landscape");
  setDocumentTitle(doc, "BÁO CÁO TỒN KHO", `Ngày xuất: ${dateTime(new Date())}`);
  autoTable(doc, {
    startY: 32,
    head: [["Sản phẩm sắp hết", "Tồn", "Bán/ngày", "Nhu cầu dự báo", "Lý do"]],
    body: (inventory.lowStockProducts || []).map(item => [
      item.productName, item.currentStock, item.avgDailySales, item.forecastNeed, item.reason
    ]),
    styles: { font: "Poppins", fontSize: 7 },
    headStyles: { font: "Poppins", fontStyle: "bold", fillColor: [244, 162, 97] }
  });
  autoTable(doc, {
    startY: doc.lastAutoTable.finalY + 8,
    head: [["Sản phẩm tồn dư", "Tồn", "Ngày đủ hàng", "Tỷ lệ bán", "Lý do"]],
    body: (inventory.overstockProducts || []).map(item => [
      item.productName, item.currentStock, item.stockCoverDays,
      `${(Number(item.sellThroughRate || 0) * 100).toFixed(2)}%`, item.reason
    ]),
    styles: { font: "Poppins", fontSize: 7 },
    headStyles: { font: "Poppins", fontStyle: "bold", fillColor: [231, 111, 81] }
  });

  doc.save(`bao-cao-${filters.fromDate}-${filters.toDate}.pdf`);
}
