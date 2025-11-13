<template>
  <nav class="navbar">
    <div class="navbar-brand">
      <router-link :to="{ name: 'Home' }" class="app-title">
        Makatizen App Portal
      </router-link>
    </div>

    <div v-if="isAuthenticated" class="user-info">
      <span class="logged-in-as">
        Logged in as: <strong>{{ userRole }}</strong>
      </span>
      <button @click="handleLogout" class="logout-button">
        Logout
        <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="currentColor" class="icon">
          <path fill-rule="evenodd" d="M7.5 3.75A1.5 1.5 0 0 0 6 5.25v13.5a1.5 1.5 0 0 0 1.5 1.5h6a1.5 1.5 0 0 0 1.5-1.5V15a.75.75 0 0 1 1.5 0v3.75a3 3 3 0 0 1-3 3h-6a3 3 0 0 1-3-3V5.25a3 3 0 0 1 3-3h6a3 3 0 0 1 3 3V9a.75.75 0 0 1-1.5 0V5.25a1.5 1.5 0 0 0-1.5-1.5h-6Zm10.662 10.375a.75.75 0 1 0 1.06 1.06l3-3a.75.75 0 0 0 0-1.06l-3-3a.75.75 0 1 0-1.06 1.06l1.72 1.72H13.5a.75.75 0 0 0 0 1.5h5.882l-1.72 1.72Z" clip-rule="evenodd" />
        </svg>
      </button>
    </div>
  </nav>
</template>

<script setup>
  import { ref, computed, onMounted, onUnmounted } from "vue";
  import { useAuthStore } from "@/stores/auth";

  const auth = useAuthStore();
  const username = computed(() => auth.username ?? "User");

  const phTime = ref("Loading...");
  let syncTimer; // fetches every minute
  let tickTimer; // updates every second
  let serverTimeOffset = 0;

  // ✅ Fetch Manila time from worldtimeapi.org and set offset
  async function fetchPhTime() {
    try {
      const res = await fetch("https://worldtimeapi.org/api/timezone/Asia/Manila");
      const data = await res.json();

      if (data?.datetime) {
        const serverNow = new Date(data.datetime);
        const localNow = new Date();
        serverTimeOffset = serverNow - localNow; // difference in ms
      } else {
        phTime.value = "Unavailable";
      }
    } catch (err) {
      phTime.value = "Unavailable";
    }
  }

  // ✅ Update display every second based on last synced time
  function updateLocalClock() {
    const now = new Date(Date.now() + serverTimeOffset);
    phTime.value = now.toLocaleTimeString("en-PH", {
      timeZone: "Asia/Manila",
      hour: "2-digit",
      minute: "2-digit",
      second: "2-digit",
      hour12: true,
    });
  }

  onMounted(() => {
    // Initial sync
    fetchPhTime().then(updateLocalClock);

    // Tick every second locally
    tickTimer = setInterval(updateLocalClock, 1000);

    // Sync with server every minute
    syncTimer = setInterval(fetchPhTime, 60000);
  });

  onUnmounted(() => {
    clearInterval(tickTimer);
    clearInterval(syncTimer);
  });

  defineProps({ title: String });
</script>

<!--<style scoped>
  .header {
    display: grid;
    grid-template-columns: 1fr auto;
    align-items: center;
    background: #fff;
    height: 10vh;
    padding: 0 32px;
    box-shadow: 0 2px 6px rgba(0, 0, 0, 0.05);
  }

  .title {
    font-family: Arial, Helvetica, sans-serif;
    font-size: 1.6em;
    font-weight: 600;
  }

  .right-section {
    display: flex;
    align-items: center;
    gap: 20px;
  }

  .profile-container {
    display: flex;
    align-items: center;
    gap: 10px;
  }

  .user-info {
    display: flex;
    flex-direction: column;
    align-items: flex-end;
    text-align: right;
  }

  .username {
    color: #333;
    font-size: 0.9em;
    font-weight: 600;
    margin: 0;
    font-family: Arial, Helvetica, sans-serif;
  }

  .pst-time {
    color: #0c2884;
    font-size: 0.75em;
    margin: 0;
    font-family: "Courier New", monospace;
  }

  .icon {
    width: 38px;
    height: 38px;
    background-color: #0c2884;
    border-radius: 50%;
  }
</style>-->

<style scoped>
  .navbar {
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: 15px 30px;
    background-color: #006666;
    color: white;
    box-shadow: 0 2px 4px rgba(0, 0, 0, 0.15);
    position: sticky;
    top: 0;
    z-index: 1000;
  }

  .navbar-brand .app-title {
    font-size: 1.5rem;
    font-weight: 700;
    color: white;
    text-decoration: none;
  }

  .user-info {
    display: flex;
    align-items: center;
    gap: 20px;
  }

  .logged-in-as {
    font-size: 0.95rem;
    font-weight: 300;
  }

  .logout-button {
    display: flex;
    align-items: center;
    padding: 8px 15px;
    background-color: #27ae60;
    color: white;
    border: none;
    border-radius: 4px;
    cursor: pointer;
    font-weight: 600;
    transition: background-color 0.3s;
  }

    .logout-button:hover {
      background-color: #1e874b;
    }

  .icon {
    width: 18px;
    height: 18px;
    margin-left: 8px;
    fill: white; /* ensures logout icon is white */
  }
</style>
