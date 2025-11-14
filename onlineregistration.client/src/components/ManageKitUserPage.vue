<template>
  <div class="management-content">
    <h1 class="page-title">Manage Kit Users 🛠️</h1>
    <p class="page-description">View, add, edit, or deactivate the personnel responsible for field enrollment kits.</p>

    <!-- ACTION AND SEARCH BAR -->
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
        <!-- Assumes userList is filtered by searchTerm in computed property -->
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
        <tfoot v-if="!users">
          <tr>
            <td colspan="6" class="text-center p-4 text-gray-600">No Kit Users found.</td>
          </tr>
        </tfoot>
      </table>
    </div>

    <!-- ADD/EDIT USER MODAL -->
    <div v-if="showForm" class="form-overlay" @click.self="closeForm">
      <div class="form-popup">
        <h3>{{ editingUser ? 'Edit Kit User Details' : 'Create New Kit User' }}</h3>
        <form @submit.prevent="saveUser">
          <label>Username</label>
          <input v-model="form.username" required />

          <label>Email</label>
          <input type="email" v-model="form.email" required />

          <label v-if="!editingUser">Password</label>
          <input v-if="!editingUser" type="password" v-model="form.password" required />

          <div class="form-actions">
            <button type="submit" class="save-btn">{{ editingUser ? 'Update' : 'Create' }}</button>
            <button type="button" class="cancel-btn" @click="closeForm">Cancel</button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script>
// NOTE: This script is minimal and only exists to satisfy template bindings.
// You should integrate your actual API and state management logic here.
export default {
    data() {
        return {
            searchTerm: '',
            showForm: false,
            editingUser: false,
            // Mock Data Structure
            userList: [
                { id: 101, username: 'kit_user_01', email: 'ku1@makati.gov', isActive: true, lastLoginDate: '2025-10-25' },
                { id: 102, username: 'kit_user_02', email: 'ku2@makati.gov', isActive: false, lastLoginDate: '2025-10-20' },
                { id: 103, username: 'kit_user_03', email: 'ku3@makati.gov', isActive: true, lastLoginDate: '2025-10-30' },
            ],
            form: {
                id: null,
                username: '',
                email: '',
                password: '',
                isActive: true
            },
        };
    },
    methods: {
        openAddForm() {
            this.editingUser = false;
            this.form = { id: null, username: '', email: '', password: '', isActive: true };
            this.showForm = true;
        },
        closeForm() { this.showForm = false; },
        editUser(user) {
            this.editingUser = true;
            this.form = { ...user, password: '' }; // Load data, but clear password
            this.showForm = true;
        },
        saveUser() {
            console.log('Saving user:', this.form);
            this.closeForm();
            // In reality, call API to save/update
        },
        toggleUserStatus(user) {
            console.log(`Toggling status for user ${user.username}`);
            // In reality, call API to update status
        }
    }
}
</script>

<style scoped>
  /* Main Content Area */
  .management-content {
    padding: 20px;
    background-color: #f4f4f4;
    min-height: 100vh;
  }

  .page-title {
    font-size: 2.2em;
    color: #0d47a1; /* Primary Blue */
    margin-bottom: 5px;
    font-weight: 700;
  }

  .page-description {
    margin-bottom: 25px;
    color: #666;
  }

  /* Action Bar (Search and Add Button) */
  .action-bar {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: 25px;
    gap: 15px;
  }

  .search-group {
    position: relative;
    width: 100%;
    max-width: 350px;
  }

  .search-input {
    width: 100%;
    padding: 10px 15px 10px 40px;
    border: 1px solid #ccc;
    border-radius: 6px;
    transition: border-color 0.2s;
  }

    .search-input:focus {
      border-color: #0d47a1;
      outline: none;
    }

  .search-icon {
    position: absolute;
    left: 15px;
    top: 50%;
    transform: translateY(-50%);
    color: #888;
  }

  .add-btn {
    background-color: #4caf50; /* Green */
    color: white;
    border: none;
    padding: 10px 15px;
    border-radius: 6px;
    cursor: pointer;
    font-weight: 600;
    white-space: nowrap; /* Prevents button from wrapping */
    transition: background-color 0.2s;
  }

    .add-btn:hover {
      background-color: #43a047;
    }

  /* Data Table */
  .table-container {
    overflow-x: auto;
    box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
    border-radius: 8px;
    background-color: white;
  }

  .data-table {
    width: 100%;
    border-collapse: collapse;
  }

    .data-table th, .data-table td {
      border: 1px solid #ddd;
      padding: 12px 15px;
      text-align: left;
    }

    .data-table th {
      background-color: #0d47a1; /* Primary Blue */
      color: white;
      text-transform: uppercase;
      font-size: 0.9em;
    }

    .data-table tr:nth-child(even) {
      background-color: #f9f9f9;
    }

    .data-table tr:hover {
      background-color: #f1f1f1;
    }

  /* Status Badges */
  .status-active, .status-inactive {
    padding: 5px 10px;
    border-radius: 12px;
    font-weight: bold;
    font-size: 0.85em;
    display: inline-block;
  }

  .status-active {
    background-color: #e8f5e9; /* Light green */
    color: #388e3c; /* Dark green text */
  }

  .status-inactive {
    background-color: #ffebee; /* Light red */
    color: #d32f2f; /* Dark red text */
  }

  /* Action Buttons in Table */
  .action-cell {
    white-space: nowrap;
  }

  .action-btn {
    border: none;
    padding: 8px 12px;
    border-radius: 5px;
    cursor: pointer;
    font-size: 0.9em;
    font-weight: 600;
    margin-right: 5px;
    transition: background-color 0.2s;
  }

  .edit-btn {
    background-color: #2196f3; /* Blue */
    color: white;
  }

    .edit-btn:hover {
      background-color: #1e88e5;
    }

  .status-btn {
    background-color: #ff9800; /* Orange for status toggle */
    color: white;
  }

    .status-btn:hover {
      background-color: #fb8c00;
    }

  /* --- MODAL/POPUP STYLES (Reused from previous request) --- */
  .form-overlay {
    position: fixed;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    background-color: rgba(0, 0, 0, 0.6);
    display: flex;
    justify-content: center;
    align-items: center;
    z-index: 1000;
  }

  .form-popup {
    background: white;
    padding: 30px;
    border-radius: 10px;
    box-shadow: 0 6px 25px rgba(0, 0, 0, 0.3);
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

  .form-actions {
    margin-top: 25px;
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

  /* Responsive adjustments */
  @media (max-width: 600px) {
    .action-bar {
      flex-direction: column;
      align-items: stretch;
    }

    .search-group {
      max-width: none;
    }

    .data-table th, .data-table td {
      padding: 8px 10px;
      font-size: 0.9em;
    }

      .data-table th:nth-child(4), .data-table td:nth-child(4) { /* Hide Last Login on small screens */
        display: none;
      }
  }
</style>
