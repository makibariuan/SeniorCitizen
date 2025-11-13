<template>
  <div class="dashboard-content">
    <h1 class="welcome-title">Administrator Dashboard 🛡️</h1>
    <p class="role-info">
      Welcome back, **{{ getRoleName(userRole) }}**. Use the sections below to manage users and citizen data.
    </p>

    <section v-if="activeView === 'kitUsers' || activeView === 'systemUsers'">
      <h2 class="section-title">{{ activeView === 'kitUsers' ? 'Kit Users' : 'System Users' }}</h2>
      <button class="add-btn" @click="openAddForm">+ Add User</button>

      <template v-if="userList && userList.length">
        <table class="user-table">
          <thead>
            <tr>
              <th>ID</th>
              <th>Username</th>
              <th>Role</th>
              <th>Status</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="user in userList" :key="user.id">
              <td>{{ user.id }}</td>
              <td>{{ user.username }}</td>
              <td>{{ getRoleName(user.userType) }}</td>
              <td>
                <span :class="{'status-active': user.isActive, 'status-inactive': !user.isActive}">
                  {{ user.isActive ? 'Active' : 'Inactive' }}
                </span>
              </td>
              <td>
                <button class="status-btn" @click="toggleUserStatus(user)">
                  {{ user.isActive ? 'Deactivate' : 'Activate' }}
                </button>
                <button class="reset-btn" @click="resetUserPassword(user)">Reset Password</button>
              </td>
            </tr>
          </tbody>
        </table>
      </template>
      <p v-else-if="!loading && !error" class="text-center p-4 text-gray-600">No users found for this category.</p>
    </section>

    <section v-else-if="activeView === 'citizenRecords'">
      <h2 class="section-title">Citizen Records</h2>
      <p class="metric-number">{{ metrics.citizenCount }}</p>
      <p>Total enrolled citizens with biometric and personal records.</p>
      <p class="detail">Last Enrollment: {{ metrics.lastEnrollmentDate || 'N/A' }}</p>
      <button class="action-button success">View Citizen Records</button>
    </section>

    <div v-if="showForm" class="form-overlay" @click.self="closeForm">
      <div class="form-popup">
        <h3>{{ editingUser ? 'Edit User Details' : 'Create New User' }}</h3>
        <form @submit.prevent="saveUser">
          <label>Username</label>
          <input v-model="form.username" required />
          <p v-if="formValidation.username" class="validation-error">{{ formValidation.username }}</p>

          <label>Role</label>
          <select v-model.number="form.userType" required>
            <option :value="1">Super Admin</option>
            <option :value="2">System User</option>
            <option v-if="activeView === 'kitUsers' || form.userType === 3" :value="3">Kit User</option>
          </select>

          <div class="form-actions">
            <button type="submit" class="save-btn">Save</button>
            <button type="button" class="cancel-btn" @click="closeForm">Cancel</button>
          </div>
        </form>
      </div>
    </div>

    <div v-if="showConfirmation" class="form-overlay" @click.self="handleConfirmation(false)">
      <div class="form-popup">
        <h3>Confirm Action</h3>
        <p>{{ confirmationMessage }}</p>
        <div class="form-actions">
          <button class="cancel-btn" @click="handleConfirmation(false)">Cancel</button>
          <button class="save-btn" @click="handleConfirmation(true)">Confirm</button>
        </div>
      </div>
    </div>

    <div v-if="loading" class="loading-overlay">
      <p>Loading data...</p>
      <p>If this persists, check network or authorization.</p>
    </div>

    <div v-if="error" class="error-box">
      <p>{{ error }}</p>
    </div>

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

<!--<style>
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
</style>-->

<style scoped>
  /* Styles remain unchanged */
  .dashboard-content {
    padding: 20px;
    background-color: #f4f4f4;
    min-height: 100vh;
  }

  .welcome-title {
    font-size: 2em;
    color: #0d47a1;
    margin-bottom: 10px;
  }

  .role-info {
    margin-bottom: 25px;
    color: #555;
    font-style: italic;
  }

  .section-title {
    font-size: 1.5em;
    color: #333;
    border-bottom: 2px solid #ccc;
    padding-bottom: 5px;
    margin-top: 30px;
    margin-bottom: 15px;
  }

  .add-btn {
    background-color: #4caf50;
    color: white;
    border: none;
    padding: 10px 15px;
    border-radius: 5px;
    cursor: pointer;
    margin-bottom: 20px;
    transition: background-color 0.2s;
  }

    .add-btn:hover {
      background-color: #43a047;
    }

  .user-table {
    width: 100%;
    border-collapse: collapse;
    box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
    background-color: white;
  }

    .user-table th, .user-table td {
      border: 1px solid #ddd;
      padding: 12px 15px;
      text-align: left;
    }

    .user-table th {
      background-color: #0d47a1;
      color: white;
      text-transform: uppercase;
      font-size: 0.9em;
    }

    .user-table tr:nth-child(even) {
      background-color: #f9f9f9;
    }

    .user-table tr:hover {
      background-color: #f1f1f1;
    }

  /* --- BUTTON STYLES --- */
  .status-btn {
    background-color: #ff9800; /* Orange for status toggle */
    color: white;
    border: none;
    padding: 6px 10px;
    border-radius: 4px;
    cursor: pointer;
    margin-right: 8px;
    transition: background-color 0.2s;
  }

    .status-btn:hover {
      background-color: #fb8c00;
    }

  .reset-btn {
    background-color: #2196f3; /* Blue for reset */
    color: white;
    border: none;
    padding: 6px 10px;
    border-radius: 4px;
    cursor: pointer;
    transition: background-color 0.2s;
  }

    .reset-btn:hover {
      background-color: #1e88e5;
    }

  /* --- STATUS STYLES --- */
  .status-active, .status-inactive {
    padding: 4px 8px;
    border-radius: 4px;
    font-weight: bold;
    font-size: 0.85em;
  }

  .status-active {
    background-color: #e8f5e9; /* Light green */
    color: #388e3c; /* Dark green text */
  }

  .status-inactive {
    background-color: #ffebee; /* Light red */
    color: #d32f2f; /* Dark red text */
  }

  /* Modal/Form Styles */
  .form-overlay {
    position: fixed;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    background-color: rgba(0, 0, 0, 0.5);
    display: flex;
    justify-content: center;
    align-items: center;
    z-index: 1000;
  }

  .form-popup {
    background: white;
    padding: 30px;
    border-radius: 10px;
    box-shadow: 0 4px 20px rgba(0, 0, 0, 0.2);
    width: 90%;
    max-width: 450px;
  }

    .form-popup h3 {
      margin-top: 0;
      color: #0d47a1;
      border-bottom: 1px solid #eee;
      padding-bottom: 10px;
      margin-bottom: 20px;
    }

    .form-popup label {
      display: block;
      margin-top: 10px;
      font-weight: bold;
      color: #333;
    }

    .form-popup input, .form-popup select {
      width: 100%;
      padding: 10px;
      margin-top: 5px;
      border: 1px solid #ccc;
      border-radius: 5px;
      box-sizing: border-box;
    }

  .validation-error {
    color: #d32f2f;
    font-size: 0.85em;
    margin: 5px 0 0;
  }

  .form-actions {
    margin-top: 20px;
    display: flex;
    justify-content: flex-end;
    gap: 10px;
  }

  .save-btn {
    background-color: #0d47a1;
    color: white;
    border: none;
    padding: 10px 15px;
    border-radius: 5px;
    cursor: pointer;
    transition: background-color 0.2s;
  }

    .save-btn:hover {
      background-color: #1976d2;
    }

  .cancel-btn {
    background-color: #bbb;
    color: #333;
    border: none;
    padding: 10px 15px;
    border-radius: 5px;
    cursor: pointer;
    transition: background-color 0.2s;
  }

    .cancel-btn:hover {
      background-color: #999;
    }

  .loading-overlay, .error-box {
    padding: 15px;
    border-radius: 8px;
    position: fixed;
    bottom: 20px;
    right: 20px;
    box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
    z-index: 1001;
    font-weight: bold;
  }

  .loading-overlay {
    background-color: #bbdefb;
    color: #0d47a1;
  }

  .error-box {
    background-color: #ffcdd2;
    color: #b71c1c;
  }

  .metric-number {
    font-size: 3em;
    color: #0d47a1;
    margin: 5px 0;
    font-weight: 700;
  }

  .detail {
    color: #777;
    margin-bottom: 20px;
  }

  .action-button {
    padding: 10px 20px;
    border: none;
    border-radius: 5px;
    color: white;
    font-weight: bold;
    cursor: pointer;
  }

    .action-button.success {
      background-color: #4caf50;
    }
</style>
