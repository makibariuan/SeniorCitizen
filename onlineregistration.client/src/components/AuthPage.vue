<template>

  <!-- <link rel="stylesheet" href="styles.css"></link> -->
  <!-- If you need reCAPTCHA, load the script in index.html or use vue-meta/head management -->
  <!-- Use vue-meta or @vueuse/head for head management -->

  <div class="logo-container">
    <!-- image here -->
    <img src="@/assets/makati-logo.png" alt="Makati Logo" style="width:140px; height:auto; margin-bottom:16px;" />
    <div class="page-title">
      <h2>Welcome to</h2>
      <h1>Makati Onboarding System</h1>
    </div>
  </div>
  <div class="auth-container">
    <div class="auth-box">
      <h2 class="auth-title">{{ isLogin ? "Sign In" : "Create Account" }}</h2>

      <!-- Login Form -->
      <div v-if="isLogin && loginStep === 'credentials'">
        <p class="auth-input-label">Email Address</p>
        <input v-model="username" type="email" placeholder="Email Address" class="auth-input" @click="" />
        <p class="auth-input-label">Password</p>
        <div class="password-wrapper">
          <input :type="showPassword ? 'text' : 'password'" v-model="password" placeholder="Password" class="auth-input" />
          <span class="toggle-password" @click="showPassword = !showPassword">
            <i :class="showPassword ? 'fas fa-eye-slash' : 'fas fa-eye'"></i>
          </span>
        </div>

        <p class="forgot-password" @click="openForgotPasswordDialog">Forgot password?</p>
        <button @click="handleLogin" class="auth-btn" :disabled="!canLogin">Sign In</button>

        <p>
          Don't have an account?
          <span class="link" @click="isLogin = false">Create account</span>
        </p>

        <div class="spacer"></div>
      </div>

      <!-- Register Form -->
      <div v-else-if="!isLogin">

        <div class="register-grid-container">
          <div>
            <p class="auth-input-label">Employee ID</p>
            <input v-model="employeeID" type="text" placeholder="Employee ID" class="auth-input" />
          </div>
          <div>
            <p class="auth-input-label">Birthday</p>
            <input v-model="birthday" type="date" placeholder="Birthday" class="auth-input" />
          </div>
          <div>
            <p class="auth-input-label">First Name</p>
            <input v-model="firstName" type="text" placeholder="First Name" class="auth-input" />
          </div>
          <div>
            <p class="auth-input-label">Last Name</p>
            <input v-model="lastName" type="text" placeholder="Last Name" class="auth-input" />
          </div>
          <div>
            <p class="auth-input-label">Email Address (Will be your username)</p>
            <input v-model="email" type="email" placeholder="juandelacruz@makati.com.ph" class="auth-input" />
          </div>
          <div>
            <p class="auth-input-label">Password</p>
            <div class="password-wrapper">
              <input :type="showPassword ? 'text' : 'password'" v-model="password" placeholder="Password" class="auth-input" @input="validatePassword"/>
              <span class="toggle-password" @click="showPassword = !showPassword">
                <i :class="showPassword ? 'fas fa-eye-slash' : 'fas fa-eye'"></i>
              </span>
            </div>
          </div>
          <div>
            <p class="auth-input-label">Confirm Password</p>
            <div class="password-wrapper">
              <input :type="showConfirm ? 'text' : 'password'" v-model="confirmPassword" placeholder="Confirm Password" class="auth-input" />
              <span class="toggle-password" @click="showConfirm = !showConfirm">
                <i :class="showConfirm ? 'fas fa-eye-slash' : 'fas fa-eye'"></i>
              </span>
            </div>
          </div>
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
        </div>

        <div class="terms-wrapper">
          <input type="checkbox" v-model="isTermsAccepted" />
          I accept the
          <span @click="showTerms = true">Terms of Service</span>
        </div>

        <TermsOfService :show="showTerms"
                        @update:show="showTerms = $event"
                        @accepted="isTermsAccepted = true" />

        <Captcha :key="captchaKey" @verified="onCaptchaVerified" />

        <p v-if="passwordMismatch" class="error-text">Passwords do not match</p>
        <button @click="handleRegister" class="auth-btn" :disabled="!canRegister">Sign Up</button>

        <!-- <div class="divider">OR</div> -->
        <!-- <button @click="isLogin = true" class="secondary-btn">Back to Login</button> -->
        <p>
          Already have an account?
          <span class="link" @click="isLogin = true">Sign In</span>
        </p>
        <div style="width: 40vw;"></div>
      </div>
    </div>
  </div>

  <!-- Success/Error Dialog -->
  <DialogBox :show="showDialog" :title="dialogTitle" :message="dialogMessage" @close="showDialog = false" />

  <!-- Forgot Password Dialog -->
  <DialogBox :show="showForgotDialog" title="Reset Password" @close="showForgotDialog = false">
    <div style="display:flex; flex-direction:column; gap:8px; margin-top:8px;">
      <input v-model="forgotEmail" type="email" placeholder="Enter your email" class="auth-input" />
      <button @click="handleForgotPassword" class="auth-btn">Send Reset Link</button>
    </div>
  </DialogBox>

  <!-- OTP Dialog -->
  <DialogBox :show="showOtpDialog" title="OTP Verification" @close="showOtpDialog = false">
    <div style="display:flex; flex-direction:column; gap:8px; margin-top:8px;">
      <p>Enter the 6-digit OTP sent to your email to verify your login.</p>
      <input v-model="otp" type="text" maxlength="6" placeholder="Enter OTP" class="auth-input" />
      <p v-if="otpError" class="error-text">{{ otpError }}</p>
      <button @click="handleVerifyOtp" class="auth-btn" :disabled="!otp">Submit</button>
    </div>
  </DialogBox>


  <!-- Loading Dialog (always on top of everything) -->
  <LoadingDialog :visible="isLoading" />

</template>


<script setup>
  import Captcha from '@/components/Captcha.vue'
  import { ref, computed, watch } from "vue";
  import { useRouter, useRoute } from "vue-router";
  import api from "@/api";
  import { useAuthStore } from "@/stores/auth";
  import DialogBox from "@/components/DialogBox.vue";
  import LoadingDialog from "@/components/LoadingDialog.vue";
  import TermsOfService from "@/components/TermsOfService.vue";

  import { useHead } from '@vueuse/head';

  useHead({
    script: [
      {
        src: 'https://www.google.com/recaptcha/api.js',
        async: true,
        defer: true,
      },
    ],
  });

  function onSubmit(token) {
    document.getElementById("demo-form").submit();
  }

  // Router & Auth
  const router = useRouter();
  const route = useRoute();
  const auth = useAuthStore();

  // captcha verification
  const captchaVerified = ref(false);
  const captchaKey = ref(0);

  // terms of service
  const showTerms = ref(false);
  const isTermsAccepted = ref(false);

  // Login/Register State
  const isLogin = ref(true);
  // const isEmployee = ref(true); // to remove
  const loginStep = ref("credentials"); // not used now; OTP handled via dialog
  const username = ref("");
  const email = ref("");
  const password = ref("");
  const confirmPassword = ref("");
  const otp = ref("");
  const showPassword = ref(false);
  const showConfirm = ref(false);
  const employeeID = ref("");
  const lastName = ref("");  const firstName = ref("");
  const birthday = ref("");

  // Dialogs
  const showDialog = ref(false);
  const dialogTitle = ref("");
  const dialogMessage = ref("");
  const showForgotDialog = ref(false);
  const showOtpDialog = ref(false);
  const forgotEmail = ref("");
  const otpError = ref("");
  const isLoading = ref(false);

  // Computed
  const passwordMismatch = computed(() => confirmPassword.value && password.value !== confirmPassword.value);
  const canLogin = computed(() => username.value && password.value);
  const canRegister = computed(() => email.value && password.value && confirmPassword.value && !passwordMismatch.value);

  // password validation
  // Individual checks
  const hasUppercase = computed(() => /[A-Z]/.test(password.value));
  const hasLowercase = computed(() => /[a-z]/.test(password.value));
  const hasNumber = computed(() => /[0-9]/.test(password.value));
  const noSpaces = computed(() => !/\s/.test(password.value));
  const minLength = computed(() => password.value.length >= 8);

  // Validate as user types
  const validatePassword = () => {
    // Optional: combine all into one validation state
    const allValid =
      hasUppercase.value &&
      hasLowercase.value &&
      hasNumber.value &&
      noSpaces.value &&
      minLength.value;
  };


  // Forgot Password
  const openForgotPasswordDialog = () => {
    forgotEmail.value = username.value || "";
    showForgotDialog.value = true;
  };

  const handleForgotPassword = async () => {
    if (!forgotEmail.value) return;
    try {
      isLoading.value = true; // show hourglass
      await api.post("/auth/forgot-password", { email: forgotEmail.value });
      dialogTitle.value = "Success";
      dialogMessage.value = "✅ Check your email for password reset instructions.";
      showDialog.value = true;
    } catch (err) {
      dialogTitle.value = "Error";
      dialogMessage.value = err.response?.data?.message || "Something went wrong.";
      showDialog.value = true;
    } finally {
      isLoading.value = false;  // hide hourglass
      showForgotDialog.value = false; // close forgot dialog
    }
  };

  watch(isLogin, (newVal) => {
    if (newVal) {
      // Switched to Login
      username.value = "";
      password.value = "";
      otp.value = "";
      forgotEmail.value = "";
      otpError.value = "";
    } else {
      // Switched to Registration
      resetRegistrationForm();
    }
  });

  const onCaptchaVerified = (status) => {
    captchaVerified.value = status;
  };


  const resetRegistrationForm = () => {
    employeeID.value = "";
    birthday.value = "";
    firstName.value = "";
    lastName.value = "";
    email.value = "";
    password.value = "";
    confirmPassword.value = "";
    isTermsAccepted.value = false;
    captchaVerified.value = false;
    showTerms.value = false;
    captchaKey.value++; // refresh CAPTCHA
  };

  const isPasswordValid = computed(() =>
    hasUppercase.value &&
    hasLowercase.value &&
    hasNumber.value &&
    noSpaces.value &&
    minLength.value
  );

  // Register
  const handleRegister = async () => {

    if (!isPasswordValid.value) {
      dialogTitle.value = "Password";
      dialogMessage.value = "Please ensure your password meets all the requirements.";
      showDialog.value = true;
      return;
    }

    if (!isTermsAccepted.value) {
      dialogTitle.value = "Terms of Service";
      dialogMessage.value = "Please accept the Terms of Service before registering.";
      showDialog.value = true;
      return;
    }

    if (!captchaVerified.value) {
      dialogTitle.value = "Captcha Required";
      dialogMessage.value = "Please verify the captcha before signing up.";
      showDialog.value = true;
      return;
    }


    try {
      isLoading.value = true;
      const res = await api.post("/auth/register", {
        username: email.value,
        email: email.value,
        password: password.value,
        employeeid: employeeID.value,
        firstname: firstName.value,
        lastname: lastName.value,
        birthdate: birthday.value
      });
      dialogTitle.value = "Success";
      dialogMessage.value = res.data.message || "Registration successful ✅";
      showDialog.value = true;

      isLogin.value = true;
      resetRegistrationForm();
      username.value = "";
    } catch (err) {
      dialogTitle.value = "Registration Failed";

      let msg = "Registration failed.";
      if (err.response?.data?.message) {
        msg = err.response.data.message;
      } else if (err.response?.data?.errors) {
        // ASP.NET ModelState errors
        msg = Object.values(err.response.data.errors).flat().join("\n");
      }
      captchaKey.value++; // refresh CAPTCHA
      dialogMessage.value = msg;
      showDialog.value = true;
    } finally {
      isLoading.value = false; // hide hourglass
    }
  };

  // Login Step 1: credentials → send OTP
  const handleLogin = async () => {
    try {
      isLoading.value = true;
      await api.post("/auth/login", {
        username: username.value,
        password: password.value,
      });
      showOtpDialog.value = true; // show OTP input dialog
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

  watch(otp, (newVal) => {
    if (!newVal) {
      otpError.value = ""; // clear error if otp is empty
    }
  });

  // Login Step 2: OTP verification
  const handleVerifyOtp = async () => {
    if (!otp.value) return;

    if (otp.value.length !== 6) {
      otpError.value = "OTP must be 6 digits";
      return;
    }
    isLoading.value = true;
    try {
      const res = await api.post("/auth/verify-otp", {
        username: username.value,
        otp: otp.value,
      });

      auth.login({ token: res.data.token });
      showOtpDialog.value = false;  // close OTP dialog
      otpError.value = "";           // reset error
      router.push("/dashboard");

    } catch (err) {
      console.error("OTP verification error:", err);
      otpError.value =
        err.response?.data?.message ||
        err.response?.data?.error ||
        err.message ||
        "Invalid or expired OTP";
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
