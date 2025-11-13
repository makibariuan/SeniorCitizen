<template>
  <div class="captcha-container">
    <canvas ref="captchaCanvas" width="150" height="50" class="captcha-canvas"></canvas>

    <div class="captcha-controls">
      <input v-model="userInput"
             placeholder="Enter captcha"
             class="captcha-input" />
      <button @click="checkCaptcha" class="captcha-btn">Verify</button>
      <button @click="generateCaptcha" class="refresh-btn">↻</button>
    </div>

    <p v-if="message" :class="{'success': isValid, 'error': !isValid}">
      {{ message }}
    </p>
  </div>
</template>

<script setup>
  import { ref, onMounted } from 'vue'

  const emit = defineEmits(['verified'])

  const captchaCanvas = ref(null)
  const captchaText = ref('')
  const userInput = ref('')
  const message = ref('')
  const isValid = ref(false)

  const generateCaptcha = () => {
    const ctx = captchaCanvas.value.getContext('2d')
    ctx.clearRect(0, 0, 150, 50)

    // Background
    ctx.fillStyle = '#f0f0f0'
    ctx.fillRect(0, 0, 150, 50)

    // Random Captcha
    const chars = 'ABCDEFGHJKLMNPQRSTUVWXYZ23456789'
    captchaText.value = Array.from({ length: 5 }, () =>
      chars.charAt(Math.floor(Math.random() * chars.length))
    ).join('')

    // Draw text
    ctx.font = '28px Arial'
    ctx.fillStyle = '#333'
    ctx.setTransform(1, 0.1, 0.1, 1, 0, 0)
    ctx.fillText(captchaText.value, 20, 35)

    // Noise
    for (let i = 0; i < 4; i++) {
      ctx.strokeStyle = `rgba(0,0,0,${Math.random()})`
      ctx.beginPath()
      ctx.moveTo(Math.random() * 150, Math.random() * 50)
      ctx.lineTo(Math.random() * 150, Math.random() * 50)
      ctx.stroke()
    }

    isValid.value = false
    userInput.value = ''
    message.value = ''
    emit('verified', false)
  }

  const checkCaptcha = () => {
    if (userInput.value.toUpperCase() === captchaText.value.toUpperCase()) {
      message.value = '✅ Captcha verified!'
      isValid.value = true
      emit('verified', true)
    } else {
      message.value = '❌ Incorrect captcha. Try again.'
      isValid.value = false
      emit('verified', false)
      generateCaptcha()
    }
  }

  onMounted(generateCaptcha)
</script>

<style scoped>
  .captcha-container {
    display: flex;
    flex-direction: column;
    align-items: center;
    gap: 8px;
  }

  .captcha-canvas {
    border: 1px solid #ccc;
    border-radius: 4px;
  }

  .captcha-controls {
    display: flex;
    align-items: center;
    gap: 8px;
  }

  .captcha-input {
    padding: 5px 10px;
    border: 1px solid #999;
    border-radius: 4px;
  }

  .captcha-btn,
  .refresh-btn {
    padding: 5px 10px;
    border: none;
    border-radius: 4px;
    cursor: pointer;
    background: #007bff;
    color: white;
  }

  .refresh-btn {
    background: #6c757d;
  }

  .success {
    color: green;
  }

  .error {
    color: red;
  }
</style>
