<template>

  <!-- <link rel="stylesheet" href="styles.css"></link> -->
  <!-- If you need reCAPTCHA, load the script in index.html or use vue-meta/head management -->
  <!-- Use vue-meta or @vueuse/head for head management -->

  <div class="login-wrapper">
    <div class="login-card">
      <div class="left-section">
        <img src="@/assets/makati-logo.png" alt="Makati Logo" class="logo" />
        <h1>Makati Senior Citizen System</h1>
      </div>

      <div class="right-section">
        <img src="@/assets/makati-logo.png" alt="Makati Logo" class="mobile-logo" />
        <h2>Log In</h2>

        <form @submit.prevent="handleLogin" class="login-form">
          <div class="input-group">
            <input v-model="username"
                   type="text"
                   placeholder="Username"
                   required />
          </div>

          <div class="input-group password-group">
            <input v-model="password"
                   :type="showPassword ? 'text' : 'password'"
                   placeholder="Password"
                   required />
            <span class="toggle-password" @click="showPassword = !showPassword">
              <i :class="showPassword ? 'fas fa-eye-slash' : 'fas fa-eye'"></i>
            </span>
          </div>

          <p v-if="error" class="error-message">{{ error }}</p>

          <div class="forgot-password">
            <router-link to="/forgot-password" class="link">Forgot Password?</router-link>
          </div>

          <button type="submit"
                  class="login-btn">
            Login
          </button>
        </form>
      </div>
    </div>
  </div>

  <!-- Success/Error Dialog -->
  <DialogBox :show="showDialog" :title="dialogTitle" :message="dialogMessage" @close="showDialog = false" />

  <!-- Forgot Password Dialog -->
  <!--<DialogBox :show="showForgotDialog" title="Reset Password" @close="showForgotDialog = false">
    <div style="display:flex; flex-direction:column; gap:8px; margin-top:8px;">
      <input v-model="forgotEmail" type="email" placeholder="Enter your email" class="auth-input" />
      <button @click="handleForgotPassword" class="auth-btn">Send Reset Link</button>
    </div>
  </DialogBox>-->


  <!-- Loading Dialog (always on top of everything) -->
  <LoadingDialog :visible="isLoading" />

</template>

<script setup>
  //import Captcha from '@/components/Captcha.vue'
  import { ref, computed, watch } from "vue";
  import { useRouter, useRoute } from "vue-router";
  import api from "@/api";
  import { useAuthStore } from "@/stores/auth";
  import DialogBox from "@/components/DialogBox.vue";
  import LoadingDialog from "@/components/LoadingDialog.vue";
  //import TermsOfService from "@/components/TermsOfService.vue";

  //import { useHead } from '@vueuse/head';

  //useHead({
  //  script: [
  //    {
  //      src: 'https://www.google.com/recaptcha/api.js',
  //      async: true,
  //      defer: true,
  //    },
  //  ],
  //});

  //function onSubmit(token) {
  //  document.getElementById("demo-form").submit();
  //}

  // Router & Auth
  const router = useRouter();
  const route = useRoute();
  const auth = useAuthStore();

  
  // Dialogs
  const showDialog = ref(false);
  const dialogTitle = ref("");
  const dialogMessage = ref("");
  const isLoading = ref(false);


  // Login/Register State
  const username = ref("");
  const password = ref("");


  //const handleForgotPassword = async () => {
  //  if (!forgotEmail.value) return;
  //  try {
  //    isLoading.value = true; // show hourglass
  //    await api.post("/auth/forgot-password", { email: forgotEmail.value });
  //    dialogTitle.value = "Success";
  //    dialogMessage.value = "✅ Check your email for password reset instructions.";
  //    showDialog.value = true;
  //  } catch (err) {
  //    dialogTitle.value = "Error";
  //    dialogMessage.value = err.response?.data?.message || "Something went wrong.";
  //    showDialog.value = true;
  //  } finally {
  //    isLoading.value = false;  // hide hourglass
  //    showForgotDialog.value = false; // close forgot dialog
  //  }
  //};




  // Login Step 1: credentials → send OTP
  const handleLogin = async () => {
    try {
      isLoading.value = true;
      const res = await api.post("/auth/login", {
        username: username.value,
        password: password.value,
      });
      auth.login({ token: res.data.token });
      if (auth.userType != 1) { // ensure only user that are of System will be allowed to login
        dialogTitle.value = "Login Failed";
        let msg = "Invalid Credentials.";

        dialogMessage.value = msg;
        showDialog.value = true;
      } else {
        if (auth.userRole == 1)
          router.push("/admin");
        else
          router.push("/dashboard");
      }
    } catch (err) {
      dialogTitle.value = "Login Failed";
      let msg = "Login failed.";
      if (err.response?.data?.message) {
        msg = err.response.data.message;
      } else if (err.response?.data?.errors) {
        // ASP.NET ModelState errors
        msg = Object.values(err.response.data.errors).flat().join("\n");
      }

      dialogMessage.value = msg;
      showDialog.value = true;
    } finally {
      isLoading.value = false; // hide hourglass
    }
  };



</script>



<!-- <style scoped>
  .logo-container{
    padding-top: 10vw;
    display:flex;
    align-items: center;
    min-height: 100vh;
    width: 50%;
    flex-direction: column;
    position:absolute;
    top: 0;
    left: 0;
  }
  .auth-container {
    display: flex;
    justify-content: center;
    align-items: center;
    min-height: 100vh;
    background: #f5f5f5;
    width: 50%;
    position:absolute;
    top: 0;
    right: 0;
  }

  .page-title h1{
    margin: 0;
    font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
    font-weight: 500;
    font-size: 2.6rem;
    margin-top: -3%;
  }

  .page-title h2{
    margin: 0;
    font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
    font-weight: 520;
    font-size: 1.8rem;
    margin-top: 8px;
  }

  .auth-box {
    background: #fff;
    display: flex;
    flex-direction: column;
    justify-content: center;
    align-items: center;
    padding: 2rem;
    border-radius:10px;
    box-shadow: 0 0 10px rgba(0, 0, 0, 0.2);
  }

  .auth-input-label{
    text-align: left;
    font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
    font-size: 0.9rem;
    width: 100%;
  }

  .auth-title{
    margin: 0 0 2rem;
    font-family: Arial, Helvetica, sans-serif;
    font-weight: 770;
    font-size: 1.55rem;
  }

  /* Inputs and buttons stretch full width of auth-box */
  .auth-input,
  .auth-btn{
    width: 100%;
    /* Remove max-width so they follow auth-box width */
    box-sizing: border-box;
  }

  .auth-input {
    padding: 12px;
    margin: 2px 0 12px;
    border: 1px solid #eee;
    border-radius: 6px;
  }

  .password-wrapper {
    position: relative;
    width: 100%;
  }

    .password-wrapper input {
      padding-right: 60px;
    }

  .register-grid-container {
    display: grid;
    grid-template-columns: 1fr 1fr;
    grid-template-rows: auto auto auto;
    gap: 0 20px
  }

  .register-notes{
    font-size: 0.68rem;
  }

    .register-notes p{
      justify-self: left;
      margin: 0 0 0 15px;
    }

    .register-notes h4{
      justify-self: left;
      margin: 0 0 8px 0;
    }

  .terms-wrapper{
    margin-top: 14px;
    font-size: 0.85rem;
    justify-self: left;
  }

    .terms-wrapper span{
      color: blue;
      font-weight: 500;
      cursor: pointer;
      font-weight: bold;
      
    }

      .terms-wrapper span:hover {
        text-decoration: underline;
      }
    
    .terms-wrapper input{
      margin-right: 8px;
      cursor: pointer;
    }

  .toggle-password {
    position: absolute;
    right: 12px;
    top: 50%;
    transform: translateY(-50%);
    cursor: pointer;
    color: #888;
    font-size: 0.9rem;
    user-select: none;
    pointer-events: all;
  }

  .auth-btn {
    padding: 12px;
    margin: 2rem 0 2rem;
    background: #0c2884;
    color: #fff;
    font-weight: bold;
    border: none;
    border-radius: 30px;
    cursor: pointer;
    transition: background 0.2s;
  }

    .auth-btn:disabled {
      background: #425185;
      cursor: not-allowed;
    }

  .link{
    color: blue;
    font-weight: 500;
    cursor: pointer;
    font-weight: bold;
  }

    .link:hover {
      text-decoration: underline;
    }

  .spacer {
    width: 40vw;
  }
  
  .error-text {
    color: #d32f2f;
    font-size: 0.85rem;
    margin: 4px 0 0;
  }

  .forgot-password {
    cursor: pointer;
    text-align: right;
    width: 100%;
    margin: 4px 0;
    font-size: 0.7rem;
    opacity: 0.6;
  }

    .forgot-password:hover {
      text-decoration: underline;
    }

  input[type="password"]::-ms-reveal,
  input[type="password"]::-ms-clear {
    display: none;
  }

  input[type="password"]::-webkit-credentials-auto-fill-button,
  input[type="password"]::-webkit-password-toggle {
    display: none;
  }

  @media screen and (max-width: 900px) {
    .logo-container {
      display: none;
    }
    .auth-container{
      width: 100%;
      transform: translateX(0), translateY(0);
    }
    .spacer{
      width: 75vw;
    }
  }
</style> -->

<style scoped>
  @import "@/assets/auth.css";
</style>
