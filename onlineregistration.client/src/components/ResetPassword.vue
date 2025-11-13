<template>
  <div class="logo-container">
    <!-- image here -->
    <img src="@/assets/makati-logo.png" alt="Makati Logo" style="width:140px; height:auto; margin-bottom:16px;" />
    <div class="page-title">
      <h2>Welcome to</h2>
      <h1>Makati Onboarding System</h1>
    </div>
  </div>
  <div class="auth-container">
    <h2 v-if="!isReset">Forgot Password</h2>
    <h2 v-else>Reset Password</h2>

    <!-- Forgot Password Form -->
    <div v-if="!isReset" class="form-wrapper">
      <input v-model="forgotEmail"
             type="email"
             placeholder="Enter your email"
             class="auth-input" />
      <button @click="handleForgotPassword" class="auth-button">
        Send Reset Link
      </button>
    </div>

    <!-- Reset Password Form -->
    <div v-else class="form-wrapper">
      <input v-model="newPassword"
             type="password"
             placeholder="New Password"
             class="auth-input" />
      <input v-model="confirmPassword"
             type="password"
             placeholder="Confirm Password"
             class="auth-input" />

      <div class="register-notes">
        <p class="password-title">Your Password Must Have:</p>
        <ul class="password-rules">
          <li><i :class="['fa', hasUppercase ? 'fa-check-circle valid' : 'fa-times-circle invalid']"></i> One uppercase letter</li>
          <li><i :class="['fa', hasLowercase ? 'fa-check-circle valid' : 'fa-times-circle invalid']"></i> One lowercase letter</li>
          <li><i :class="['fa', hasNumber ? 'fa-check-circle valid' : 'fa-times-circle invalid']"></i> At least one number</li>
          <li><i :class="['fa', noSpaces ? 'fa-check-circle valid' : 'fa-times-circle invalid']"></i> No spaces</li>
          <li><i :class="['fa', minLength ? 'fa-check-circle valid' : 'fa-times-circle invalid']"></i> 8 or more characters</li>
        </ul>
      </div>

      <button @click="handleResetPassword"
              class="auth-button"
              :disabled="!isPasswordValid">
        Reset Password
      </button>
    </div>

    <!-- Dialog -->
    <DialogBox :show="showDialog"
               :title="dialogTitle"
               :message="dialogMessage"
               @close="showDialog = false" />
  </div>
</template>

<script setup>
  import { ref, onMounted, computed } from "vue";
  import { useRoute, useRouter } from "vue-router";
  import api from "@/api";
  import DialogBox from "@/components/DialogBox.vue";

  // ---------------- REACTIVE STATE ----------------
  const forgotEmail = ref("");
  const newPassword = ref("");
  const confirmPassword = ref("");
  const token = ref("");
  const email = ref("");
  const isReset = ref(false);

  const showDialog = ref(false);
  const dialogTitle = ref("");
  const dialogMessage = ref("");

  const route = useRoute();
  const router = useRouter();

  // ---------------- PASSWORD VALIDATION ----------------
  const hasUppercase = computed(() => /[A-Z]/.test(newPassword.value));
  const hasLowercase = computed(() => /[a-z]/.test(newPassword.value));
  const hasNumber = computed(() => /[0-9]/.test(newPassword.value));
  const noSpaces = computed(() => !/\s/.test(newPassword.value));
  const minLength = computed(() => newPassword.value.length >= 8);

  const isPasswordValid = computed(() =>
    hasUppercase.value &&
    hasLowercase.value &&
    hasNumber.value &&
    noSpaces.value &&
    minLength.value
  );

  // ---------------- INIT ----------------
  onMounted(() => {
    if (route.query.token && route.query.email) {
      token.value = route.query.token;
      email.value = route.query.email;
      isReset.value = true;
    }
  });

  // ---------------- FORGOT PASSWORD ----------------
  const handleForgotPassword = async () => {
    const emailInput = forgotEmail.value?.trim();
    if (!emailInput) return;

    try {
      await api.post("/auth/forgot-password", { email: emailInput });
      dialogTitle.value = "Success";
      dialogMessage.value = "✅ Check your email for password reset instructions.";
      showDialog.value = true;
      forgotEmail.value = "";
    } catch (err) {
      console.error(err);
      dialogTitle.value = "Error";
      dialogMessage.value = err.response?.data?.message || "Something went wrong.";
      showDialog.value = true;
    }
  };

  // ---------------- RESET PASSWORD ----------------
  const handleResetPassword = async () => {
    if (!isPasswordValid.value) {
      dialogTitle.value = "Password";
      dialogMessage.value = "Please ensure your password meets all the requirements.";
      showDialog.value = true;
      return;
    }

    if (!newPassword.value || !confirmPassword.value) {
      dialogTitle.value = "Error";
      dialogMessage.value = "Please fill in all fields.";
      showDialog.value = true;
      return;
    }

    if (newPassword.value !== confirmPassword.value) {
      dialogTitle.value = "Error";
      dialogMessage.value = "Passwords do not match.";
      showDialog.value = true;
      return;
    }

    try {
      await api.post("/auth/reset-password", {
        email: email.value,
        token: token.value,
        newPassword: newPassword.value,
      });

      dialogTitle.value = "Success";
      dialogMessage.value = "✅ Your password has been reset successfully.";
      showDialog.value = true;

      setTimeout(() => router.push("/login"), 2000);
    } catch (err) {
      console.error(err);
      dialogTitle.value = "Error";
      const msg =
        err.response?.data?.message ||
        err.message ||
        "Incorrect username or password.";

      if (msg.includes("Please confirm your email")) {
        router.push({ name: "ConfirmEmail", query: { notice: "pending" } });
        return;
      }

      dialogMessage.value = msg;
      showDialog.value = true;
    }
  };
</script>

<style scoped>
  html, body, #app {
    height: 100%;
    margin: 0;
  }

  .auth-container {
    min-height: 100vh;
    display: flex;
    flex-direction: column;
    justify-content: center;
    align-items: center;
    padding: 20px;
    background: #f0f2f5;
  }

  .form-wrapper {
    width: 100%;
    max-width: 400px;
    background: #fff;
    padding: 30px;
    border-radius: 12px;
    box-shadow: 0 4px 12px rgba(0,0,0,0.1);
    display: flex;
    flex-direction: column;
  }

  .auth-input {
    width: 100%;
    padding: 10px;
    margin: 10px 0;
    border-radius: 6px;
    border: 1px solid #ccc;
  }

  .auth-button {
    width: 100%;
    padding: 10px;
    background: #4CAF50;
    color: #fff;
    border: none;
    border-radius: 6px;
    cursor: pointer;
    margin-top: 10px;
  }

    .auth-button:hover:not(:disabled) {
      background: #45a049;
    }

    .auth-button:disabled {
      background: #ccc;
      cursor: not-allowed;
    }

  .register-notes {
    margin-top: 15px;
  }

  .password-title {
    font-weight: bold;
    margin-bottom: 5px;
  }

  .password-rules {
    list-style: none;
    padding: 0;
    margin: 0;
  }

    .password-rules li {
      display: flex;
      align-items: center;
      margin-bottom: 5px;
    }

    .password-rules i {
      margin-right: 8px;
    }

  .valid {
    color: green;
  }

  .invalid {
    color: red;
  }
</style>
