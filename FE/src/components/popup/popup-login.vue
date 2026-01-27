<template>
    <div class="fullscreen-popup" v-if="isVisible">
      <div class="popup-content">
        <div class="close-button" @click="close"><i class="fa-solid fa-xmark"></i></div>
        <slot></slot>
      </div>
    </div>
  </template>
  
  <script>
  export default {
    name: 'FullScreenPopup',
    props: {
      isVisible: {
        type: Boolean,
        default: false,
      },
    },
    methods: {
      close() {
        this.$emit('close');
      },
    },
  };
  </script>
  
  <style scoped>
  @keyframes popupShow {
    0% {
      transform: scale(0.8) translateY(-50px);
      opacity: 0;
    }
    100% {
      transform: scale(1) translateY(0);
      opacity: 1;
    }
  }
  
  .fullscreen-popup {
    position: fixed;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    background-color: rgba(0, 0, 0, 0.8);
    display: flex;
    justify-content: center;
    align-items: center;
    z-index: 9999;
    opacity: 0;
    animation: fadeIn 0.5s forwards; /* Hiệu ứng mờ dần khi hiển thị */
  }
  
  .fullscreen-popup.show {
    opacity: 1;
  }
  
  .popup-content {
    background-color: #fff;
    border-radius: 10px;
    width: 50%;
    max-width: 1000px;
    max-height: 90%;
    overflow: auto;
    box-shadow: 0 0 10px rgba(0, 0, 0, 0.5);
    position: relative;
    animation: popupShow 1s forwards; /* Hiệu ứng xuất hiện popup */
    /* overflow: hidden; */
    overflow-x: hidden;
  }
  
  .close-button {
    color: #aaa;
    position: absolute;
    top: 10px;
    right: 20px;
    font-size: 28px;
    font-weight: bold;
    width: 30px;
    height: 30px;
    display: flex;
    justify-content: center;
    align-items: center;
    padding: 5px;
    color: red;
  }
  
  .close-button:hover,
  .close-button:focus {
    color: black;
    text-decoration: none;
    cursor: pointer;
  }
  
  @keyframes fadeIn {
    from {
      opacity: 0;
    }
    to {
      opacity: 1;
    }
  }
  </style>