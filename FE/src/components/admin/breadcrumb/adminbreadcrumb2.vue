<template>
    <div class="row">
      <div class="col-sm-7 col-auto">
        <h3 class="page-title">{{ title }}</h3>
      </div>
  </div>
</template>

<script>
export default {
  computed: {
  },
  props: {
    title: {
      type: String,
      default: "",
    }
  },
};
</script>
