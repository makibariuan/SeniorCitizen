<template>
  <teleport to="body">
    <div v-if="show" class="dialog-overlay">
      <div class="dialog-box">
        <h3>{{ title }}</h3>
        <p>{{ message }}</p>
        <!-- slot for custom content -->
        <slot></slot>
        <button @click="$emit('close')" class="dialog-btn">OK</button>
      </div>
    </div>
  </teleport>
</template>

<script setup>
  defineProps({
    show: Boolean,
    title: String,
    message: String
  })
</script>

<style>
  .dialog-overlay {
    position: fixed;
    inset: 0;
    background: rgba(0, 0, 0, 0.5);
    display: flex;
    align-items: center;
    justify-content: center;
    z-index: 9999; /* Always on top */
  }

  .dialog-box {
    background: #fff;
    padding: 20px;
    border-radius: 8px;
    width: 320px;
    max-width: 90%;
    text-align: center;
    box-shadow: 0 4px 12px rgba(0, 0, 0, 0.25);
  }

  .dialog-btn {
    margin-top: 15px;
    padding: 8px 16px;
    background: #2563eb;
    color: #fff;
    border: none;
    border-radius: 6px;
    cursor: pointer;
    font-weight: 500;
    transition: background 0.2s ease;
  }

    .dialog-btn:hover {
      background: #1d4ed8;
    }
</style>
