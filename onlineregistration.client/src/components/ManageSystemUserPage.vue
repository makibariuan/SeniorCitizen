<template>
  <!--<LeftMenu @navigate="onNavigate" class="leftMenu" />-->
  <Header class="header" />

  <!--Display the contents here-->
  <div class="action-bar">
    <div class="search-group">
      <input type="text" v-model="searchTerm" placeholder="Search by username or ID..." class="search-input" />
      <i class="fas fa-search search-icon"></i>
    </div>
    <button class="add-btn" @click="openAddForm">+ Add New Kit User</button>
  </div>

  <!-- USER DATA TABLE -->
  <div class="table-container">
    <table class="data-table">
      <thead>
        <tr>
          <th>ID</th>
          <th>Username</th>
          <!--<th>Last Login</th>-->
          <th>Status</th>
          <th>Actions</th>
        </tr>
      </thead>
      <!-- Assumes users is filtered by searchTerm in computed property -->
      <tbody>
        <tr v-for="user in users" :key="user.id">
          <td>{{ user.id }}</td>
          <td>{{ user.username }}</td>
          <!--<td>{{ user.lastLoginDate || 'N/A' }}</td>-->
          <td>
            <span :class="{'status-active': user.status, 'status-inactive': !user.status}">
              {{ user.status ? 'Active' : 'Inactive' }}
            </span>
          </td>
          <td class="action-cell">
            <button class="action-btn edit-btn" @click="editUser(user)">Edit</button>
            <button class="action-btn status-btn" @click="toggleUserStatus(user)">
              {{ user.status ? 'Deactivate' : 'Activate' }}
            </button>
          </td>
        </tr>
      </tbody>
      <!--<tfoot v-if="users.value.length">
        <tr>
          <td colspan="6" class="text-center p-4 text-gray-600">No Kit Users found.</td>
        </tr>
      </tfoot>-->
    </table>
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

  <!-- Success/Error Dialog -->
  <DialogBox :show="showDialog" :title="dialogTitle" :message="dialogMessage" @close="showDialog = false" />

  <!-- Loading Dialog (always on top of everything) -->
  <LoadingDialog :visible="isLoading" />

</template>

<script setup>
  import { ref, onMounted, computed } from "vue";
  import { useAuthStore } from "@/stores/auth";

  import api from "@/api";
  import LeftMenu from "./LeftMenuBack.vue";
  import Header from "./Header.vue";
  import DialogBox from "@/components/DialogBox.vue";
  import LoadingDialog from "./LoadingDialog.vue";

  // --- Dialog & Loading ---
  const showDialog = ref(false);
  const dialogTitle = ref("");
  const dialogMessage = ref("");
  const isLoading = ref(false);
  const showConfirmation = ref(false);
  const confirmationMessage = ref("");
  const confirmationAction = ref("");

  // --- Auth store & user data ---
  const auth = useAuthStore();
  const firstname = computed(() => auth.firstName ?? "User");
  const username = computed(() => auth.username ?? "User");
  const userType = computed(() => Number(auth.userType ?? 1));
  const users = ref([]);

  // --- Navigation state ---
  const current = ref("admin");
  const onNavigate = (item) => {
    current.value = item;
  };

  // --- Load user + stats on mount ---
  onMounted(async () => {
    if (typeof auth.loadUser === "function") {
      await auth.loadUser();
    }
    LoadSystemUsers();
  });

  async function LoadSystemUsers() {
    try {
      isLoading.value = true;
      const res = await api.get("/admin/user-list/system"); // <-- change the api url
      users.value = res.data;
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

  function toggleUserStatus(user) {
    const newStatus = !user.status;
    const action = newStatus ? 'Activate' : 'Deactivate';

    this.openConfirmation(
      `Are you sure you want to ${action} user ${user.username}?`,
      () => this.executeToggleUserStatus(user, action)
    );
  }

  async function openConfirmation(message, action) {
    this.confirmationMessage = message;
    this.confirmationAction = action;
    this.showConfirmation = true;
  }

  async function handleConfirmation(confirmed) {
    this.showConfirmation = false;
    if (confirmed && typeof this.confirmationAction === 'function') {
      this.confirmationAction();
    }
    this.confirmationAction = null;
  }

  async function executeToggleUserStatus(user, action) {
    // verify the user to deactivate
    if (username.value == user.username) {
      showDialog.value = true;
      dialogTitle.value = "Error";
      dialogMessage.value = "You cannot deactivate your own account.";
      return;
    }


    try {
      isLoading.value = true;
      const res = await api.post("/admin/toggle-status", {
        UserId: user.id,
      }); // <-- change the api url

      users.value = res.data;
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
      showDialog.value = true;
      dialogTitle.value = "Error";
      dialogMessage.value = "Failed to load dashboard stats. Please log in again.";
    } finally {
      isLoading.value = false;
      LoadSystemUsers();
    }
  }

</script>

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


