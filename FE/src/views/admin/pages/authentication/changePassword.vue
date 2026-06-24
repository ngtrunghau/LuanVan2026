<template>
  <div class="main-Wrapper">
    <adminheader />
    <adminsidebar />
    <div class="page-wrapper">
      <div class="content container-fluid">
        <div class="row justify-content-center">
          <div class="col-lg-7 col-xl-5">
            <ChangePasswordForm login-path="/quan-tri/login" />
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import ChangePasswordForm from "@/components/account/ChangePasswordForm.vue";
</script>
