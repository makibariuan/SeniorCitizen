<template>
  <div class="layout">
    <LeftMenu @navigate="onNavigate" class="leftMenu" />
    <Header class="header" />

    <div class="content">
      <img src="../assets/makati-logo.png" alt="Makati Logo" />
      <h2 style="font-weight: bold">Hi, {{ firstname }}</h2>
      <h2 v-if="userType === 0">Welcome to Makati Citizen Portal</h2>
      <h2 v-else>Welcome to Makati Employee Portal</h2>
    </div>
  </div>
</template>

<script setup>
  import { ref, onMounted, computed } from "vue";
  import LeftMenu from "./LeftMenu.vue";
  import Header from "./Header.vue";
  import { useAuthStore } from "@/stores/auth";

  const auth = useAuthStore();

  // ✅ Make user data reactive
  const firstname = computed(() => auth.firstName ?? "User");
  const username = computed(() => auth.username ?? "User");
  const userType = computed(() => Number(auth.userType ?? 1)); // default = employee

  // ✅ Load user info on mount (if available)
  onMounted(async () => {
    if (typeof auth.loadUser === "function") {
      await auth.loadUser();
    }
  });

  // Local UI state for navigation
  const current = ref("Dashboard");
  const onNavigate = (item) => {
    current.value = item;
  };
</script>

<style>
  .layout {
    display: flex;
    flex-direction: column;
    height: 100vh;
    background: #f2f5fa;
    width: 76vw;
    position: fixed;
    top: 0;
    right: 0;
  }

  .content {
    flex: 1;
    padding: 20px;
    box-sizing: border-box;
    transform: translateY(-8vh);
    text-align: center;
  }

  .leftMenu {
    grid-row: span 5 / span 5;
  }

  .header {
    grid-column: span 4 / span 4;
  }

  h2 {
    font-size: 2rem;
    font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
    color: black;
  }

  img {
    width: 10vw;
    height: auto;
  }
</style>
