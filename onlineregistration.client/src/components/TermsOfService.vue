<template>
  <DialogBox :show="show" title="Terms of Service" @update:show="updateShow">
    <div v-if="loading" class="terms-loading">Loading Terms of Service...</div>

    <div v-else class="terms-content">
      <!-- Only show static header if markdown has no title -->
      <div v-if="!hasTitle" class="terms-header">
        <h2>Makati Onboarding System</h2>
        <p class="subtitle"><em>Last updated: October 2025</em></p>
      </div>

      <div class="terms-body" v-html="marked(termsContent)"></div>

      <div class="terms-actions">
        <button class="btn secondary" @click="close">Close</button>
        <button class="btn primary" @click="acceptTerms">Accept & Close</button>
      </div>
    </div>
  </DialogBox>
</template>

<script setup>
  import { ref, watch } from 'vue'
  import { marked } from 'marked'
  import DialogBox from './DialogBoxToS.vue'

  const props = defineProps({
    show: { type: Boolean, required: true },
    file: { type: String, default: '/terms-of-service.md' },
  })
  const emits = defineEmits(['update:show', 'accepted'])

  const termsContent = ref('')
  const loading = ref(true)
  const hasTitle = ref(false)

  const loadTerms = async () => {
    loading.value = true
    try {
      const r = await fetch(props.file)
      const text = await r.text()
      termsContent.value = text
      // Detect title (Markdown heading)
      hasTitle.value = /^#\s+/m.test(text) || /^##\s+/m.test(text)
    } catch {
      termsContent.value = '⚠️ Unable to load Terms of Service.'
    } finally {
      loading.value = false
    }
  }

  watch(
    () => props.show,
    (open) => {
      if (open && !termsContent.value) loadTerms()
    },
    { immediate: false }
  )

  const updateShow = (val) => emits('update:show', val)
  const close = () => emits('update:show', false)
  const acceptTerms = () => {
    emits('accepted')
    emits('update:show', false)
  }
</script>

<style scoped>
  .terms-content {
    display: flex;
    flex-direction: column;
    gap: 1.25rem;
    font-size: 0.95rem;
    color: #333;
    max-height: 70vh;
    overflow-y: auto;
  }

  .terms-header {
    text-align: center;
    margin-bottom: 0.5rem;
  }

    .terms-header h2 {
      font-size: 1.4rem;
      color: #1d3a8a;
      margin: 0;
    }

  .subtitle {
    font-size: 0.9rem;
    color: #666;
  }

  .terms-body {
    padding: 0 0.5rem;
    line-height: 1.6;
    text-align: left; /* ✅ Force left-aligned Markdown text */
  }

    .terms-body ul {
      list-style: decimal;
      margin-left: 1.5rem;
    }

  .terms-actions {
    display: flex;
    justify-content: flex-end;
    gap: 0.75rem;
    padding-top: 1rem;
    border-top: 1px solid #eee;
  }

  .btn {
    padding: 0.5rem 1.5rem;
    border-radius: 6px;
    font-size: 0.9rem;
    font-weight: 600;
    cursor: pointer;
    transition: all 0.2s ease;
    border: none;
  }

    .btn.primary {
      background-color: #0a2472;
      color: white;
    }

      .btn.primary:hover {
        background-color: #0b2e91;
      }

    .btn.secondary {
      background-color: #e6e9ef;
      color: #333;
    }

      .btn.secondary:hover {
        background-color: #d4d7dc;
      }

  .terms-loading {
    text-align: center;
    padding: 2rem;
    font-style: italic;
  }
</style>
