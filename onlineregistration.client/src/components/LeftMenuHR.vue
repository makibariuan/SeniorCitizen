<template>
  <div class="left-menu">
    <div class="logo" @click="goTo('/dashboard')">
      <div style="display: grid; grid-template-columns: auto 1fr; align-items: center; gap: 12px;">
        <img src="../assets/makati-logo.png" alt="Makati" />
        <div>
          <div>REPUBLIC OF THE PHILIPPINES</div>
          <p>City of Makati</p>
        </div>
      </div>
    </div>

    <ul>
      <li v-for="item in menuItems"
          :key="item.name"
          class="menu-item"
          :class="{ active: $route.path === item.path }"
          @click="handleMenuClick(item)">
        {{ item.name }}
      </li>

      <li class="logout" @click="logout">Logout</li>
    </ul>
  </div>
</template>

<script setup>
  import { computed, onMounted, watch } from "vue";
  import { useAuthStore } from "@/stores/auth";
  import { useRouter, useRoute } from "vue-router";

  const auth = useAuthStore();
  const router = useRouter();
  const route = useRoute();

  // 🧩 Menu configuration per user type
  const allMenus = {
    2: [ // HR
      { name: "HR Dashboard", path: "/hr" },
      { name: "Employee ID", path: "/employee-id" },
      { name: "Go Back", path: "/dashboard" },
    ],
    3: [ // Super Admin
      { name: "HR Dashboard", path: "/hr" },
      { name: "Employee ID", path: "/employee-id" },
      { name: "Go Back", path: "/dashboard" },
    ],
  };

  // Reactive menu
  const menuItems = computed(() => {
    const type = parseInt(auth.userType ?? -1);
    return allMenus[type] || [];
  });

  // Handle menu item click
  const handleMenuClick = (item) => {
    // If menu has a custom action (like Go Back), run it
    if (item.action && typeof item.action === "function") {
      item.action();
      return;
    }

    // Otherwise, navigate by path
    if (item.path && route.path !== item.path) {
      router.push(item.path);
    }
  };

  // Logout
  const logout = () => {
    auth.logout();
    router.push("/login");
  };

  // Load user info (optional)
  onMounted(async () => {
    if (typeof auth.loadUser === "function") {
      await auth.loadUser();
    }
  });
</script>

<style scoped>
  .left-menu {
    position: fixed;
    top: 0;
    left: 0;
    height: 100%;
    width: 24vw;
    border-right: 1px solid #ddd;
    background: #0c2884;
    display: flex;
    flex-direction: column;
    color: white;
    text-wrap: nowrap;
  }

  .logo {
    align-self: center;
    margin: 22px 0 20px 0;
    font-family: 'Gill Sans', 'Gill Sans MT', Calibri, 'Trebuchet MS', sans-serif;
  }

    .logo:hover {
      cursor: pointer;
    }

    .logo img {
      width: 56px;
      height: auto;
    }

    .logo div div {
      width: 200px;
      align-items: center;
    }

      .logo div div div {
        padding-top: 10px;
        margin: 0;
        font-size: 12px;
        text-decoration: underline;
        text-underline-offset: 5px;
        letter-spacing: 1px;
      }

      .logo div div p {
        margin-top: -8px;
        font-size: 31px;
        letter-spacing: 1px;
        font-weight: lighter;
      }

  .left-menu ul {
    list-style: none;
    padding: 0;
    margin: 0;
    flex: 1;
    display: flex;
    flex-direction: column;
  }

  .left-menu li {
    padding: 15px 30px;
    cursor: pointer;
    transition: background 0.2s;
    text-align: left;
    font-family: Arial, Helvetica, sans-serif;
    font-size: smaller;
  }

    .left-menu li:hover {
      background: #243E90;
    }

    .left-menu li.active {
      background: #3D539D;
      font-weight: bold;
    }

    .left-menu li.menu-item {
      border-bottom: 1px solid #0D205B;
    }

    .left-menu li.logout {
      margin-top: auto;
      color: white;
      border-top: 1px solid #0D205B;
    }
</style>
