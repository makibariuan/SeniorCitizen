<template>
  <div class="layout pds">
    <LeftMenu />
    <Header title="Employee Timesheet" />
    <div class="content">
      <div class="w-[90%] mx-auto">
        <div class="tab-wrapper">
          <div class="tab-content">
            <transition name="fade-slide" mode="out-in">
              <div :key="activeTab" class="form-wrapper">
                <div v-if="activeTab === 'Biometrics Data'">
                  <h2 class="sub-title">Captured biometric logs</h2>

                  <div class="pds-table-wrapper">
                    <div class="pds-table-wrapper">
                      <table class="pds-table">
                        <thead>
                          <tr>
                            <th>Date</th>
                            <th>Time</th>
                            <th>Department</th>
                            <th>Device</th>
                            <th>Mode</th>
                            <th>Status</th>
                          </tr>
                        </thead>
                        <tbody>
                          <tr v-for="app in attendanceRecord" :key="app.dateLog">
                            <td>{{ app.date }}</td>
                            <td>{{ app.time }}</td>
                            <td>{{ app.departmentName }}</td>
                            <td>{{ app.biometricDeviceID }}</td>
                            <td>{{ app.mode }}</td>
                            <td>{{ app.eventType }}</td>
                            <td>
                              <img v-if="app.photo" :src="`data:image/jpeg;base64,${app.photo}`" alt="Photo" width="20" height="30" />
                            </td>
                          </tr>
                        </tbody>
                      </table>
                    </div>

                  </div>
                </div>

                <!--<div v-else-if="activeTab === 'Generate Cards'">
                  <div class="form-wrapper">
                    <h2 class="sub-title">Generate PRINT File</h2>
                    <div class="pds-table-wrapper">

                      <div class="pds-table-wrapper">

                      </div>
                    </div>
                  </div>
                </div>-->
              </div>
            </transition>
          </div>
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
  </div>

</template>


<script setup>
  import LeftMenu from './LeftMenu.vue'
  import Header from './Header.vue'
  import api from "@/api";
  import { ref, onMounted, computed, watch, nextTick } from "vue";
  import { useAuthStore } from "@/stores/auth";
  import DialogBox from "@/components/DialogBox.vue";
  import LoadingDialog from "./LoadingDialog.vue";

  const showDialog = ref(false);
  const dialogTitle = ref("");
  const dialogMessage = ref("");
  const isLoading = ref(false);

  const tabs = ["Biometrics Data"];
  const activeTab = ref("Biometrics Data");

  const auth = useAuthStore();
  const personId = computed(() => auth.userId); // logged in user’s ID
  const employeeId = computed(() => auth.employeeID);
  const personEmail = computed(() => auth.email);
  const firstName = computed(() => auth.firstName);
  const lastName = computed(() => auth.lastName);

  const attendanceRecord = ref([]);


  async function getAttendance() {
    try {
      isLoading.value = true;
      const res = await api.get(`/employee/attendance/${employeeId.value}`);
      attendanceRecord.value = res.data;

      console.log("Loaded pds:", attendanceRecord.value);

    } catch (err) {
      console.error("Failed to load data:", err);
    } finally {
      isLoading.value = false;  // hide hourglass
    }
  }

  onMounted(() => {
    getAttendance();
  });

</script>


<style scoped>

  .delete-btn {
    background-color: #e74c3c;
    color: #fff;
    border: none;
    padding: 6px 12px;
    border-radius: 4px;
    cursor: pointer;
    font-size: 13px;
  }

    .delete-btn:hover {
      background-color: #c0392b;
    }

  /* Layout */
  .pds.layout {
    display: flex;
    /* min-height: 100vh; */
    /* height: 92vh; */
    /* width: 76vw; */
    /* width: 100%;
    height: 100%; */
    flex-direction: column;
  }

  .pds .content {
    flex: 1;
    padding: 15px;
    margin-top: 60px;
    /* margin-top: var(--header-height); */
    /* overflow-y: auto; */
    display: flex;
    flex-direction: column;
    /* background-color: black; */
    /* position: fixed; */
    /* bottom: 0; */
    /* right: 0; */
    /* height: 100%; */
    /* width: 100%; */
    width: calc(100% - var(--left-menu-width));
    height: calc(100% - var(--header-height));
  }

    /* Inputs */
    .pds .content input,
    .pds .content select,
    .pds .content textarea {
      width: 100%;
      padding: 6px 8px;
      border: 1px solid #ccc;
      border-radius: 4px;
      box-sizing: border-box;
      margin-bottom: 0.5rem;
    }

  .pds .form-wrapper { /* MAIN FORM HOLDER OF TABLE AND TITLES */
    display: grid;
    flex-direction: column;
    /* align-items: center; center each control */
    width: 100%;
    padding-left: 20px;
    /* margin: 15px; */
    /* max-width: 65vw; fixed form width */
    /* overflow-x: hidden; */
  }
    /* Inputs and selects */
    .pds .form-wrapper input,
    .pds .form-wrapper select,
    .pds .form-wrapper textarea {
      width: 100%; /* take wrapper width */
      max-width: 400px; /* keep controls uniform */
      /* margin-bottom: 12px; */
    }

  /* .pds .form-wrapper .personal-info-wrapper{
    display: grid;
    grid-template-columns: repeat(4, 1fr);
    grid-template-rows: repeat(35, auto);
    background: lightgrey;
  } */

  .sub-title {
    display: flex;
    /* background-color: lightgrey; */
    width: auto;
    color: #0c2884;
    font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
    font-weight: 700;
    font-size: 1rem;
    border-bottom: 1px solid #f2f2f2;
    padding-top: 20px;
    padding-bottom: 20px;
    /* margin: 15px 0 20px -2.5vw; */
    /* margin-bottom: 10px; */
  }

  /* Buttons */
  .pds .btn {
    display: block;
    /* margin: 10px auto; */
    /* padding: 8px 16px; */
    border-radius: 50px;
    cursor: pointer;
    background: #2563eb;
    color: #fff;
    border: none;
    font-weight: 700;
    transition: background 0.2s ease;
    width: 50%;
    height: 43px;
  }

    .pds .btn:hover {
      background: #1d4ed8;
    }

  .pds .btn-white {
    display: block;
    /* margin: 10px auto; */
    /* padding: 8px 16px; */
    border-radius: 50px;
    cursor: pointer;
    background: white;
    color: #1d4ed8;
    /* border: none; */
    font-weight: 700;
    transition: background 0.2s ease;
    font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
    font-size: 0.9rem;
    border: 1px solid #1d4ed8;
    width: 50%;
    height: 43px;
  }

    .pds .btn-white:hover {
      background: #fafafa;
    }

  /* Tabs */
  .pds .tab-container {
    display: flex;
    justify-content: flex-start;
    gap: 8px;
    /* gap: 125px; */
    /* margin-bottom: 0; remove space */
    margin-bottom: 10px;
    /* border-radius: 100%; */
    border-bottom: none; /* prevent double border */
    /* white-space:nowrap; prevent wrapping */
    border-radius: 999px;
    background: white;
    box-shadow: 0 2px 6px rgba(0, 0, 0, 0.1); /* subtle shadow */
  }

  .pds .tab-btn {
    flex: 1;
    min-width: 120px;
    padding: 10px 0;
    text-align: center;
    position: relative;
    background: none;
    /* border: 1px solid #ccc; */
    /* border-bottom: none; merge with content */
    cursor: pointer;
    /* border-radius: 6px 6px 0 0; rounded only on top */
    border: none;
    outline: none;
    background: none;
    box-shadow: none;
    color: #c0c0c0;
    font-weight: bold;
  }

    .pds .tab-btn.active {
      /* background: #fff; */
      border: 1px solid #ccc;
      border-bottom: none; /* 🔥 removes line under active tab */
      /* border-top: 3px solid #2563eb; */
      font-weight: bold;
      color: black;
      position: relative;
      outline: none;
      box-shadow: none;
      border: none;
      /* z-index: 2; ensures tab is above content */
    }

    .pds .tab-btn:focus {
      outline: none;
    }

    .pds .tab-btn:not(:last-child)::after {
      content: "";
      position: absolute;
      right: 0;
      top: 25%;
      height: 50%;
      width: 1px;
      background: #ccc; /* divider color */
    }

  /* Tab Wrapper */
  .pds .tab-wrapper {
    flex: 1;
    display: flex;
    flex-direction: column;
    /* height: calc(100vh - 160px); adjust header + tabs height */
    height: 78vh;
    box-shadow: 0 2px 6px rgba(0, 0, 0, 0.1);
    border-radius: 12px;
  }

  /* Tab Content */
  .pds .tab-content {
    flex: 1 1 auto;
    min-height: 0;
    overflow-y: auto;
    overflow-x: hidden;
    /* border: 1px solid #ccc; */
    border-top: none;
    padding-bottom: 20px;
    background: #fff;
    border-radius: 0 6px 6px 6px;
    display: flex;
    justify-content: center; /* center horizontally */
    align-items: flex-start; /* align to top */
    border-radius: 12px;
  }

    .pds .tab-content input,
    .pds .tab-content select,
    .pds .tab-content textarea {
      width: 15vw; /* pick your desired fixed size */
      max-width: 100%; /* don't overflow parent */
      margin-bottom: 12px;
    }

    .pds .tab-content form {
      width: 100%;
    }

  /* Wrapper to center table */
  .pds-table-wrapper {
    /* display: flex; */
    justify-content: center; /* center horizontally */
    /* background: lightblue; */
    /* display: fixed; */
    /* margin: 20px 0; */
    /* width: 100%; */

    width: auto;
  }

  /* Table styling */
  .pds-table {
    border-collapse: collapse;
    /* width: 100%; */
    width: auto;
    table-layout: auto;
    /* max-width: 60vw; control max width */
    /* background: #fff; */
    /* background:lightgrey; */
    border: none;
  }

    .pds-table th,
    .pds-table td {
      border: 1px solid #ddd;
      padding: 0 8px 0 8px;
      text-align: left;
      border: none;
    }

    /* Inputs inside table */
    .pds-table input {
      width: 100%;
      box-sizing: border-box;
      padding: 6px 8px;
    }


  .note {
    font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
    color: #ff5656;
    font-size: 0.55rem;
  }

  /* Responsive */
  @media (max-width: 768px) {
    .pds.layout {
      flex-direction: column;
    }

    .pds .tab-content {
      max-width: 100%;
      min-height: auto;
    }
  }

  /* @media only screen and (max-width: 800px) {
    table, thead, tbody, th, td, tr {
      width: 100%;
      border-collapse: collapse;
    }
  } */

</style>

