<template>
  <teleport to="body">
    <div v-if="show" class="dialog-overlay" @click.self="close">
      <div class="dialog-box" role="dialog" :aria-labelledby="titleId" ref="dialogRef">
        <header class="dialog-header">
          <h3 :id="titleId">{{ title }}</h3>
          <button class="dialog-close" @click="close" aria-label="Close">✕</button>
        </header>

        <div class="dialog-body">
          <slot />
        </div>
      </div>
    </div>
  </teleport>
</template>

<script setup>
  import { ref, watch, onMounted, onBeforeUnmount } from 'vue';

  const props = defineProps({
    show: { type: Boolean, required: true },
    title: { type: String, default: '' },
  });

  const emit = defineEmits(['update:show', 'close']);

  const titleId = 'dialog-title-' + Math.random().toString(36).slice(2, 9);
  const dialogRef = ref(null);

  const close = () => {
    emit('update:show', false);
    emit('close');
  };

  watch(
    () => props.show,
    (open) => {
      document.body.style.overflow = open ? 'hidden' : '';
      // focus management (optional)
      if (open) {
        setTimeout(() => dialogRef.value?.focus?.(), 0);
      }
    }
  );

  // close on ESC
  const onKey = (e) => {
    if (e.key === 'Escape' && props.show) close();
  };

  onMounted(() => window.addEventListener('keydown', onKey));
  onBeforeUnmount(() => window.removeEventListener('keydown', onKey));
</script>

<style scoped>
  .dialog-overlay {
    position: fixed;
    inset: 0;
    background: rgba(0,0,0,0.45);
    display: flex;
    align-items: center;
    justify-content: center;
    z-index: 1200;
    padding: 24px;
    box-sizing: border-box;
  }

  .dialog-box {
    background: #fff;
    width: min(900px, 95vw);
    padding: 1rem 1.5rem;
    max-height: 86vh;
    overflow: auto;
    border-radius: 10px;
    box-shadow: 0 20px 40px rgba(0,0,0,0.25);
    display: flex;
    flex-direction: column;
    outline: none;
  }

  .dialog-header {
    display: flex;
    align-items: center;
    justify-content: space-between;
    padding: 18px 20px;
    border-bottom: 1px solid #eee;
  }

    .dialog-header h3 {
      margin: 0;
      font-size: 1.125rem;
    }

  .dialog-close {
    background: transparent;
    border: none;
    cursor: pointer;
    font-size: 1.1rem;
    padding: 4px 8px;
  }

  .dialog-body {
    padding: 18px 20px;
  }
</style>
