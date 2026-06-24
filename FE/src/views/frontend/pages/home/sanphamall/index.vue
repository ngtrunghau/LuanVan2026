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
<script setup>
import { reactive, toRefs } from "vue";
const state = reactive({
  title: "Pain Relief",
  text: "Home",
  text1: "Pain Relief"
});
const {
  title,
  text,
  text1
} = toRefs(state);
</script>
