<template>
  <div class="layout">
    <LeftMenu @navigate="onNavigate" class="leftMenu" />
    <Header class="header" />

    <div class="content">
      <img src="../assets/makati-logo.png" alt="Makati Logo" />
      <h2 style="font-weight: bold">Hi, {{ firstname }}</h2>
      <h2>Welcome to Human Resources Dashboard</h2>

      <!-- Status Container -->
      <div class="status-container">
        <template v-if="Object.keys(idStats).length">
          <div class="status-card">
            <div class="status-icon">👥</div>
            <div class="status-info">
              <div class="status-number">{{ idStats.employeeCount }}</div>
              <div class="status-label">Total Employees</div>
            </div>
          </div>

          <div class="status-card">
            <div class="status-icon">📅</div>
            <div class="status-info">
              <div class="status-number">{{ idStats.forSchedule }}</div>
              <div class="status-label">For Schedule</div>
            </div>
          </div>

          <div class="status-card">
            <div class="status-icon">📅</div>
            <div class="status-info">
              <div class="status-number">{{ idStats.scheduled }}</div>
              <div class="status-label">For Capture</div>
            </div>
          </div>

          <div class="status-card">
            <div class="status-icon">📸</div>
            <div class="status-info">
              <div class="status-number">{{ idStats.captured }}</div>
              <div class="status-label">Captured</div>
            </div>
          </div>

          <div class="status-card">
            <div class="status-icon">🖨️</div>
            <div class="status-info">
              <div class="status-number">{{ idStats.forPrinting }}</div>
              <div class="status-label">For Printing</div>
            </div>
          </div>

          <!-- New Active Cards Status -->
          <div class="status-card">
            <div class="status-icon">💳</div>
            <div class="status-info">
              <div class="status-number">{{ idStats.activeCards }}</div>
              <div class="status-label">Active Cards</div>
            </div>
          </div>
        </template>
        <template v-else>
          <p>Loading stats...</p>
        </template>
      </div>
    </div>

    <!-- Reusable dialog -->
    <DialogBox :show="showDialog"
               :title="dialogTitle"
               :message="dialogMessage"
               @close="showDialog = false" />

    <!-- Loading Dialog (always on top of everything) -->
    <LoadingDialog :visible="isLoading" />
  </div>
</template>


<script setup>
  import { ref, onMounted, computed } from "vue";
  import LeftMenu from "./LeftMenuHR.vue";
  import Header from "./Header.vue";
  import { useAuthStore } from "@/stores/auth";
  import api from "@/api";
  import DialogBox from "@/components/DialogBox.vue";
  import LoadingDialog from "./LoadingDialog.vue";

  // --- Dialog & Loading ---
  const showDialog = ref(false);
  const dialogTitle = ref("");
  const dialogMessage = ref("");
  const isLoading = ref(false);

  // --- Auth store & user data ---
  const auth = useAuthStore();
  const firstname = computed(() => auth.firstName ?? "User");
  const username = computed(() => auth.username ?? "User");
  const userType = computed(() => Number(auth.userType ?? 1));

  // --- Status stats ---
  const idStats = ref({});

  // --- Navigation state ---
  const current = ref("Dashboard");
  const onNavigate = (item) => {
    current.value = item;
  };

  // --- Load user + stats on mount ---
  onMounted(async () => {
    if (typeof auth.loadUser === "function") {
      await auth.loadUser();
    }
    loadStats();
  });

  // --- Fetch ID stats ---
  async function loadStats() {
    try {
      isLoading.value = true;
      const res = await api.get("/employee/id-stats");
      idStats.value = res.data;
    } catch (err) {
      console.error("AxiosError:", err);
      if (err.response) {
        console.error("Status:", err.response.status);
        console.error("Data:", err.response.data);
      } else if (err.request) {
        console.error("No response received from server", err.request);
      } else {
        console.error("Axios setup error:", err.message);
      }
      // Log out the user on error
      if (typeof auth.logout === "function") {
        auth.logout(); // this should clear user data and redirect to login
      } else {
        showDialog.value = true;
        dialogTitle.value = "Error";
        dialogMessage.value = "Failed to load dashboard stats. Please log in again.";
      }
    } finally {
      isLoading.value = false;
    }
  }
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

  /* Status cards */
  .status-container {
    display: flex;
    justify-content: center;
    gap: 20px;
    margin-top: 40px;
    flex-wrap: wrap;
  }

  .status-card {
    background: white;
    padding: 20px 30px;
    border-radius: 10px;
    display: flex;
    align-items: center;
    gap: 15px;
    box-shadow: 0 4px 8px rgba(0,0,0,0.1);
    min-width: 180px;
    transition: transform 0.2s;
    cursor: default;
  }

    .status-card:hover {
      transform: translateY(-4px);
    }

  .status-icon {
    font-size: 2rem;
  }

  .status-info {
    display: flex;
    flex-direction: column;
    align-items: flex-start;
  }

  .status-number {
    font-size: 1.5rem;
    font-weight: bold;
    color: #2c3e50;
  }

  .status-label {
    font-size: 0.9rem;
    color: #555;
  }
</style>
