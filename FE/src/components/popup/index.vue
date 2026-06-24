<template>
    <div class="popup" v-if="isVisible" :style="popupStyle">
        <div class="popup-content">
        <span class="close-button" @click="close">&times;</span>
        <slot></slot>
        </div>
    </div>
  </template>
  
  <script setup>
import { computed, getCurrentInstance } from "vue";
defineOptions({
  name: 'Popup'
});
const props = defineProps({
  isVisible: {
    type: Boolean,
    default: false
  },
  position: {
    type: Object,
    default: () => ({
      top: 0,
      left: 0
    })
  }
});
const {
  proxy
} = getCurrentInstance();
function close() {
  proxy.$emit('close');
}
const popupStyle = computed(() => {
  return {
    position: 'absolute',
    top: `${props.position.top}px`,
    left: `${props.position.left}px`
  };
});
</script>
  
  <style scoped>
.popup {
  z-index: 1;
  display: flex;
  justify-content: center;
  align-items: center;
  width: 100%;
}

.popup-content {
  background-color: #fefefe;
  padding: 20px;
  border: 1px solid #888;
  width: 100%;
  border-radius: 10px;
  margin-top: 85px;
}

.close-button {
  color: #aaa;
  float: right;
  font-size: 28px;
  font-weight: bold;
}

.close-button:hover,
.close-button:focus {
  color: black;
  text-decoration: none;
  cursor: pointer;
}
</style>