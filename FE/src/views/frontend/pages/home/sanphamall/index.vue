<template>
  <div class="main-wrapper">
    <layoutheader :class="{ 'header-space': showHeaderSpace }" ref="header" />

    <div class="content" style="padding-top: 150px;">
      <div class="container">
        <div class="row">

          <SanPhamAll></SanPhamAll>
        </div>
      </div>
    </div>
    <footerHome></footerHome>
  </div>
</template>
<script>
export default {
  data() {
    return {
      title: "Pain Relief",
      text: "Home",
      text1: "Pain Relief",
    };
  },
};
</script>
