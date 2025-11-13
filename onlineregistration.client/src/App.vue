<template>
  <div id="app">
    <main class="main-content">
      <router-view />
    </main>

    <!-- Idle / Expired Logout Dialog -->
    <div v-if="showDialog" class="dialog-overlay">
      <div class="dialog-box">
        <h3>{{ dialogTitle }}</h3>
        <p>{{ dialogMessage }}</p>
        <button @click="closeDialog" class="dialog-btn">OK</button>
      </div>
    </div>
  </div>
</template>

<script setup>
  import { ref, onMounted, onBeforeUnmount, watch } from "vue";
  import { useRouter } from "vue-router";
  import { useAuthStore } from "@/stores/auth";

  const router = useRouter();
  const auth = useAuthStore();

  // --- Idle timer setup ---
  const idleTimeout = ref(null);
  const idleLimit = 15 * 60 * 1000; // 15 minutes

  // Dialog state
  const showDialog = ref(false);
  const dialogTitle = ref("");
  const dialogMessage = ref("");

  // --- Reset idle timer ---
  const resetIdleTimer = () => {
    if (idleTimeout.value) clearTimeout(idleTimeout.value);
    idleTimeout.value = setTimeout(() => showIdleDialog(), idleLimit);
  };

  // --- Show idle dialog ---
  const showIdleDialog = () => {
    if (!auth.isLoggedIn) return;

    showDialog.value = true;
    dialogTitle.value = "Session Expired";
    dialogMessage.value =
      "You have been logged out due to inactivity or token expiration.";
  };

  // --- Close dialog and logout ---
  const closeDialog = () => {
    showDialog.value = false;
    auth.idleLogoutAction();
    router.push("/login");
  };

  // --- Activity listeners ---
  const addActivityListeners = () => {
    ["mousemove", "keydown", "click"].forEach((evt) =>
      window.addEventListener(evt, resetIdleTimer)
    );
  };

  const removeActivityListeners = () => {
    ["mousemove", "keydown", "click"].forEach((evt) =>
      window.removeEventListener(evt, resetIdleTimer)
    );
  };

  // --- Watch login state to start/stop idle timer ---
  watch(
    () => auth.isLoggedIn,
    (loggedIn) => {
      if (loggedIn) {
        resetIdleTimer();
        addActivityListeners();
      } else {
        if (idleTimeout.value) clearTimeout(idleTimeout.value);
        removeActivityListeners();
      }
    },
    { immediate: true }
  );

  // --- On mount, check token and setup timers ---
  onMounted(() => {
    if (auth.token) {
      if (!auth.checkSession()) {
        showDialog.value = true;
        dialogTitle.value = "Session Expired";
        dialogMessage.value = "Your session expired. Please log in again.";
        router.push("/login");
        return;
      }

      // Token valid → setup timers
      auth.startAutoLogoutTimer();
      resetIdleTimer();
      addActivityListeners();
    }
  });

  // --- Cleanup ---
  onBeforeUnmount(() => {
    if (idleTimeout.value) clearTimeout(idleTimeout.value);
    removeActivityListeners();
  });
</script>


<style>
  #app {
    font-family: Avenir, Helvetica, Arial, sans-serif;
    text-align: center;
    height: 100%;
    width: 100%;
    display: flex;
    flex-direction: column;
    overflow: hidden;
    /* overflow: auto; */
    background: #e1e6ea;
  }

  .dialog-overlay {
    position: fixed;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    background: rgba(0, 0, 0, 0.4);
    display: flex;
    align-items: center;
    justify-content: center;
  }

  .dialog-box {
    background: white;
    padding: 20px;
    border-radius: 8px;
    width: 300px;
    text-align: center;
    box-shadow: 0 4px 6px rgba(0, 0, 0, 0.2);
  }

  .dialog-btn {
    margin-top: 15px;
    padding: 8px 16px;
    background: #ef9a9a;
    border: none;
    color: white;
    border-radius: 6px;
    cursor: pointer;
  }
</style>
