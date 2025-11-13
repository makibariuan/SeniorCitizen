<template>
  <div class="confirm-container">
    <div class="confirm-box">
      <h2>Email Confirmation</h2>

      <div v-if="loading">
        ⏳ Loading...
      </div>

      <div v-else>
        <!-- Redirected from login (notice = pending) -->
        <p v-if="notice === 'pending'" class="info">
          📧 Please confirm your email. Check your inbox (or spam). Then login again.
        </p>

        <!-- Success -->
        <p v-else-if="success" class="success">
          ✅ {{ message }}
        </p>

        <!-- Error -->
        <p v-else class="error">
          ❌ {{ message }}
        </p>

        <!-- Resend button only if token expired -->
        <button v-if="expired"
                @click="resendEmail"
                class="confirm-btn resend">
          Resend Confirmation Email
        </button>

        <!-- Always show Go to Login -->
        <button @click="goToLogin" class="confirm-btn">
          Go to Login
        </button>
      </div>
    </div>
  </div>
</template>

<script setup>
  import { ref, onMounted } from "vue";
  import { useRoute, useRouter } from "vue-router";
  import api from "@/api";

  const route = useRoute();
  const router = useRouter();

  const loading = ref(true);
  const success = ref(false);
  const message = ref("");
  const expired = ref(false);
  const notice = route.query.notice; // "pending" from login redirect

  onMounted(async () => {
    const token = route.query.token;
    const email = route.query.email;

    // Case: user redirected from login
    if (notice === "pending" && !token && !email) {
      loading.value = false;
      success.value = false;
      message.value = "Please confirm your email before logging in.";
      return;
    }

    // Case: clicked link in email
    if (token && email) {
      try {
        const res = await api.get("/auth/confirm-email", {
          params: { token, email },
        });
        message.value = res.data.message || "Email confirmed successfully!";
        success.value = true;
      } catch (err) {
        const msg = err.response?.data?.message || "Email confirmation failed.";
        message.value = msg;
        success.value = false;

        if (msg.toLowerCase().includes("expired")) {
          expired.value = true;
        }
      } finally {
        loading.value = false;
      }
    } else {
      // Invalid link
      loading.value = false;
      success.value = false;
      message.value = "Invalid confirmation link.";
    }
  });

  const goToLogin = () => {
    router.push({ path: "/login" });
  };

  const resendEmail = async () => {
    try {
      await api.post("/auth/resend-confirmation", { email: route.query.email });
      message.value = "📧 A new confirmation email has been sent!";
      success.value = true;
      expired.value = false;
    } catch (err) {
      message.value =
        err.response?.data?.message || "Failed to resend confirmation email.";
    }
  };
</script>

<style scoped>
  .confirm-container {
    display: flex;
    align-items: center;
    justify-content: center;
    min-height: 100vh;
    background: #f9fafb;
  }

  .confirm-box {
    background: white;
    padding: 2rem;
    border-radius: 8px;
    text-align: center;
    box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
    max-width: 360px;
  }

  .info {
    color: #1565c0;
  }

  .success {
    color: #2e7d32;
  }

  .error {
    color: #c62828;
  }

  .confirm-btn {
    margin-top: 1.5rem;
    padding: 10px 24px;
    background: #ef9a9a;
    color: white;
    border: none;
    border-radius: 6px;
    cursor: pointer;
  }

    .confirm-btn.resend {
      background: #42a5f5;
      margin-right: 8px;
    }
</style>
