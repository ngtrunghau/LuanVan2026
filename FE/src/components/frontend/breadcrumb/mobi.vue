<template>
    <div class="container">
      <div class="row align-items-center" style="margin-top:30px">
        <div class="col-md-12 col-12 text-center">
          <h2 class="breadcrumb-title" style="font-size: 18px">{{ title }}</h2>
        </div>
    </div>
  </div>
</template>

<script>
export default {
  props: {
    title: {
      type: String,
      default: "",
    },
    text: {
      type: String,
      default: "",
    },
    text1: {
      type: String,
      default: "",
    },
  },
};
</script>