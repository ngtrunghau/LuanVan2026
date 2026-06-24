<template>
  <!-- ScrollToTop -->
  <div
    class="progress-wrap"
    :class="{ 'active-progress': isScrolled }"
    @click="scrollToTop"
  >
    <svg
      class="progress-circle svg-content"
      width="100%"
      height="100%"
      viewBox="-1 -1 102 102"
    >
      <path
        ref="progressPath"
        d="M50,1 a49,49 0 0,1 0,98 a49,49 0 0,1 0,-98"
        :style="`stroke-dasharray: ${pathLength}px, ${pathLength}px; stroke-dashoffset: ${pathOffset}px;`"
      ></path>
    </svg>
  </div>
  <!-- /ScrollToTop -->
</template>

<script setup>
import { getCurrentInstance, onBeforeUnmount, onMounted, reactive, toRefs } from "vue";
const {
  proxy
} = getCurrentInstance();
const state = reactive({
  pathLength: 0,
  pathOffset: 0,
  isScrolled: false
});
const {
  pathLength,
  pathOffset,
  isScrolled
} = toRefs(state);
function updateProgress() {
  const scrollTop = window.pageYOffset || document.documentElement.scrollTop || document.body.scrollTop;
  const windowHeight = window.innerHeight || document.documentElement.clientHeight || document.body.clientHeight;
  const documentHeight = Math.max(document.body.scrollHeight, document.documentElement.scrollHeight, document.body.offsetHeight, document.documentElement.offsetHeight, document.body.clientHeight, document.documentElement.clientHeight);
  const scrollPercentage = scrollTop / (documentHeight - windowHeight) * 100;
  state.pathOffset = state.pathLength - scrollPercentage * state.pathLength / 100;
  state.isScrolled = scrollTop > 50;
}
function scrollToTop() {
  window.scrollTo({
    top: 0,
    behavior: "smooth"
  });
}
onMounted(() => {
  proxy.$nextTick(() => {
    const progressPath = proxy.$refs.progressPath;
    state.pathLength = progressPath.getTotalLength();
    progressPath.style.transition = progressPath.style.WebkitTransition = "none";
    progressPath.style.strokeDasharray = `${state.pathLength}px ${state.pathLength}px`;
    progressPath.style.strokeDashoffset = state.pathLength;
    progressPath.getBoundingClientRect();
    progressPath.style.transition = progressPath.style.WebkitTransition = "stroke-dashoffset 10ms linear";
    updateProgress();
  });
  window.addEventListener("scroll", updateProgress);
});
onBeforeUnmount(() => {
  window.removeEventListener("scroll", updateProgress);
});
</script>
