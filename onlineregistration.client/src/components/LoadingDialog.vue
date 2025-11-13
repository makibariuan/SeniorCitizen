<template>
  <div v-if="visible" class="loading-overlay">
    <div class="loading-box">
      <div class="hourglass"></div>
      <p>Loading...</p>
    </div>
  </div>
</template>

<script setup>
  defineProps({
    visible: Boolean
  });
</script>

<style scoped>
  .loading-overlay {
    position: fixed;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    background: rgba(0,0,0,0.4);
    display: flex;
    justify-content: center;
    align-items: center;
    z-index: 99999;
  }

  .loading-box {
    background: #fff;
    padding: 20px 30px;
    border-radius: 12px;
    text-align: center;
    box-shadow: 0 4px 12px rgba(0,0,0,0.3);
  }

  .hourglass {
    width: 40px;
    height: 40px;
    margin: 0 auto 10px;
    border: 4px solid #ef9a9a;
    border-radius: 50%;
    border-top: 4px solid transparent;
    border-bottom: 4px solid transparent;
    animation: spin 1s linear infinite;
  }

  @keyframes spin {
    0% {
      transform: rotate(0deg);
    }

    100% {
      transform: rotate(360deg);
    }
  }

  .loading-box p {
    font-size: 16px;
    font-weight: 500;
    color: #333;
  }
</style>
