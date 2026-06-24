<template>
  <div class="main-wrapper">
    <layoutheader :class="{ 'header-space': showHeaderSpace }" ref="header" />

    <div class="content" style="padding-top: 150px;">
      <div class="container">
        <div class="row">
          <div class="col-md-5 col-lg-3 col-xl-3 theiaStickySidebar">
            <div class="stickysidebar">
              <!-- Search Filter -->
              <div class="card search-filter">
                <div class="card-header">
                  <h4 class="card-title mb-0">Tìm kiếm</h4>
                </div>
                <div class="card-body">
                  <!-- <div class="filter-widget">
									<div class="cal-icon">
										<input type="text" class="form-control datetimepicker" placeholder="Select Date">
									</div>			
								</div> -->
                  <div class="filter-widget">
                    <h4>Thương hiệu</h4>
                    <div>
                      <label class="custom_check">
                        <input type="checkbox" name="gender_type" checked />
                        <span class="checkmark"></span> SVN
                      </label>
                    </div>
                    <div>
                      <label class="custom_check">
                        <input type="checkbox" name="gender_type" />
                        <span class="checkmark"></span> SVN-1
                      </label>
                    </div>
                    <div>
                      <label class="custom_check">
                        <input type="checkbox" name="gender_type" />
                        <span class="checkmark"></span> SVN-2
                      </label>
                    </div>
                    <div>
                      <label class="custom_check">
                        <input type="checkbox" name="gender_type" />
                        <span class="checkmark"></span> SVN-3
                      </label>
                    </div>
                    <div>
                      <label class="custom_check">
                        <input type="checkbox" name="gender_type" />
                        <span class="checkmark"></span> SVN-4
                      </label>
                    </div>
                    <div>
                      <label class="custom_check">
                        <input type="checkbox" name="gender_type" />
                        <span class="checkmark"></span> SVN-5
                      </label>
                    </div>
                    <div>
                      <label class="custom_check">
                        <input type="checkbox" name="gender_type" />
                        <span class="checkmark"></span> SVN-6
                      </label>
                    </div>
                  </div>
                  <div class="btn-search">
                    <b-button type="button" class="btn w-100">Lọc</b-button>
                  </div>
                </div>
              </div>
              <!-- /Search Filter -->
            </div>
          </div>

          <SanPhamVot></SanPhamVot>
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
