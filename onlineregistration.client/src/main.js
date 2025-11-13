import './assets/main.css'
import './components/styles.css';

import { createApp } from 'vue'
import { createPinia } from 'pinia'
import App from './App.vue'
import axios from 'axios'
import router from './router'

const app = createApp(App)

// ✅ create Pinia instance and save it
const pinia = createPinia()
app.use(pinia)

// ✅ add axios globally
app.config.globalProperties.$axios = axios

// ✅ install router after Pinia
app.use(router)

app.mount('#app')
