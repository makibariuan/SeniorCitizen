<template>
  <div class="left-menu">
    <!-- <div class="welcome">Welcome, {{ username }}</div> -->
    <div class="logo" @click="goTo('/dashboard')">
      <div style="display: grid; grid-template-columns: auto 1fr; align-items: center; gap: 12px;">
        <img src="../assets/makati-logo.png" alt="Makati" />
        <div>
          <div>
            REPUBLIC OF THE PHILIPPINES
          </div>
          <p>City of Makati</p>
        </div>
      </div>
    </div>
    <ul>
      <li v-for="item in menuItems"
          class="menu-item"
          :key="item.name"
          :class="{ active: $route.path === item.path }"
          @click="goTo(item.path)">
        {{ item.name }}
      </li>
      <li class="logout" @click="logout">Logout</li>
    </ul>
  </div>
</template>

<script setup>
  import { ref, computed, onMounted, watch } from "vue";
  import { useAuthStore } from "@/stores/auth";
  import { useRouter, useRoute } from "vue-router";

  // Pinia store & router
  const auth = useAuthStore();
  const router = useRouter();
  const route = useRoute();

  // 🧩 Menu configuration per user type
  const allMenus = {
    0: [ // Citizen
      { name: "Dashboard", path: "/dashboard" },
      { name: "Social Government Programs", path: "" },
      { name: "Announcements", path: "" },
      { name: "Settings", path: "" },
    ],
    1: [ // Employee
      { name: "Dashboard", path: "/dashboard" },
      { name: "Personal Data Sheet", path: "/pds" },
      //{ name: "Details", path: "/details" },
      { name: "Timesheet", path: "/ts" },
      { name: "Payslip and Compensation", path: "" },
      { name: "Announcements", path: "" },
      { name: "Settings", path: "" },
    ],
    2: [ // HR
      { name: "Dashboard", path: "/dashboard" },
      { name: "Personal Data Sheet", path: "/pds" },
      //{ name: "Details", path: "/details" },
      { name: "Timesheet", path: "/ts" },
      { name: "Human Resources", path: "hr" },
      { name: "Policies and Documentation", path: "" },
      { name: "Announcements", path: "" },
      { name: "Settings", path: "" },
    ],
    3: [ // Super Admin
      { name: "Dashboard", path: "/dashboard" },
      { name: "Personal Data Sheet", path: "/pds" },
      //{ name: "Details", path: "/details" },
      { name: "Timesheet", path: "/ts" },
      { name: "Human Resources", path: "hr" },
      { name: "Policies and Documentation", path: "" },
      { name: "Social Government Programs", path: "" },
      { name: "Payslip and Compensation", path: "" },
      { name: "Announcements", path: "" },
      { name: "Settings", path: "" },
    ],
  };

  // 🧩 Reactive menu (auto-updates based on userType)
  const menuItems = computed(() => {
    const type = parseInt(auth.userType ?? -1);
    return allMenus[type] || [];
  });

  // 👤 Username display
  const username = computed(() => auth.username ?? "User");

  // Load user (optional)
  onMounted(async () => {
    if (typeof auth.loadUser === "function") {
      await auth.loadUser();
    }
  });

  // 🔄 Watch for userType change — update instantly
  watch(
    () => auth.userType,
    (newType, oldType) => {
      console.log(`User type changed from ${oldType} → ${newType}`);
    },
    { immediate: true }
  );

  // Navigation handler
  const goTo = (path) => {
    if (path && route.path !== path) {
      router.push(path);
    }
  };

  // Logout
  const logout = () => {
    auth.logout();
    router.push("/login");
  };
</script>


<style scoped>
  .left-menu {
    position: fixed;
    top: 0;
    left: 0;
    height: 100%;
    width: 24vw;
    /* width: 326px; */
    /* width: 24vw; */
    /* width: 235px; */
    /* padding: 10px; */
    border-right: 1px solid #ddd;
    background: #0c2884;
    display: flex;
    flex-direction: column;
    color: white;
    text-wrap: nowrap;
  }

  .logo {
    /* margin-bottom:20px;
    margin-bottom: 20px; */
    align-self: center;
    margin: 22px 0 20px 0;
    font-family: 'Gill Sans', 'Gill Sans MT', Calibri, 'Trebuchet MS', sans-serif;
  }

    .logo :hover {
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
        /* background: red; */
      }

      .logo div div p {
        margin-top: -8px;
        font-size: 31px;
        letter-spacing: 1px;
        font-weight: lighter;
        /* background: blue; */
      }

  .left-menu ul {
    list-style: none;
    padding: 0;
    margin: 0;
    flex: 1;
    display: flex;
    flex-direction: column;
    /* list-style: none; */
  }

  .left-menu li {
    padding: 15px;
    padding-left: 30px;
    cursor: pointer;
    transition: background 0.2s;
    /* align-content: left; */
    text-align: left;
    /* font-family: sans-serif; */
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
      border-width: thin;
    }

    .left-menu li.logout {
      margin-top: auto;
      color: white;
      /* font-weight: bold; */
      margin-top: auto;
      /* transform: translateY(-100%); */
      border-top: 1px solid #0D205B;
      border-width: thin;
    }
</style>
