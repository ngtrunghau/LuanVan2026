<template>
<div class="card">
    <div class="card-body pt-0">

        <!-- Tab Menu -->
        <h3 class="pt-4">Thông tin chi tiết</h3>
        <hr>
        <!-- /Tab Menu -->

        <!-- Tab Content -->
        <div class="tab-content pt-3">

            <!-- Overview Content -->
            <div role="tabpanel" id="doc_overview" class="tab-pane fade show active">
                <div class="row">
                        <!-- About Details -->
                        <div class="widget about-widget">
                            <h4 class="widget-title">{{ this.list.name }}</h4>
                            <span v-html="this.list.descriptions">
                            </span>
                        </div>
                        <!-- /About Details -->
                </div>
            </div>
            <!-- /Overview Content -->
        </div>
    </div>
</div>
</template>
<script>
import AOS from "aos";
import "aos/dist/aos.css";




export default {
  components: {

  },
  data() {
    return {

      list: [],
      url : `${process.env.VUE_APP_API_URL}files/view/`,
      urlFile:`${process.env.VUE_APP_API_URL}files/view`,
      
    };
  },

  props: {
    detail: { type : Object}
  },
  watch:{
    '$props':{
      handler: function (val) {
        this.list = val.detail;
        
      },
      deep: true
    }
  },
  mounted() {
    this.$nextTick(() => {
      AOS.init();
    });
  },
  created() {
  },
  methods: {
    nextSlide() {
      this.$refs.carousel.next();
    },
    prevSlide() {
      this.$refs.carousel.prev();
    },
  },
};
</script>