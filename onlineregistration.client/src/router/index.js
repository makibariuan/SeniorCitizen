// src/router.js
import { createRouter, createWebHistory } from "vue-router";
import AuthPage from "@/components/AuthPage.vue";
import Dashboard from "@/components/Dashboard.vue";
import PersonalDataSheet from "@/components/PersonalDataSheet.vue";
import Timesheet from "@/components/Timesheet.vue";
import EmailConfirm from "@/components/EmailConfirm.vue";
import ResetPassword from "@/components/ResetPassword.vue";
import DetailsPage from "@/components/EmployeeID.vue";
import DashboardHR from "@/components/DashboardHR.vue";
import EmployeeID from "@/components/EmployeeID.vue";
import { useAuthStore } from "@/stores/auth";

import ManageKitUserPage from "@/components/ManageKitUserPage.vue";
import ManageSystemUserPage from "@/components/ManageSystemUserPage.vue";
import ManageCitizenPage from "@/components/ManageCitizenPage.vue"; 

/*
  userType values:
    0 - Citizen
    1 - Employee
    2 - HR
    3 - Super Admin
*/
const routes = [
  { path: "/", redirect: "/login" },
  { path: "/login", component: AuthPage },
  //{ path: "/confirm-email", name: "ConfirmEmail", component: EmailConfirm },
  //{ path: "/reset-password", name: "ResetPassword", component: ResetPassword },
  //{ path: "/register", component: AuthPage },
  { path: "/dashboard", component: Dashboard, meta: { requiresAuth: true, userRole: [2] } },
  //{ path: "/pds", component: PersonalDataSheet, meta: { requiresAuth: true, userRole: [1, 2, 3] } },
  //{ path: "/ts", component: Timesheet, meta: { requiresAuth: true, userRole: [1, 2, 3] } },
  { path: "/admin", component: DashboardHR, meta: { requiresAuth: true, userRole: [1] } },
  //{ path: "/employee-id", component: EmployeeID, meta: { requiresAuth: true, userRole: [2, 3] } },
  //{ path: "/details", component: DetailsPage, meta: { requiresAuth: true, userRole: [1, 2, 3] } },
  { path: "/manage-kit-users", component: ManageKitUserPage, meta: { requiresAuth: true, userRole: [1] } },
  { path: "/manage-system-users", component: ManageSystemUserPage, meta: { requiresAuth: true, userRole: [1] } },
  { path: "/manage-citizens", component: ManageCitizenPage, meta: { requiresAuth: true, userRole: [1, 2] } },


  { path: "/:pathMatch(.*)*", redirect: "/" }
];

const router = createRouter({
  history: createWebHistory(),
  routes,
});

router.beforeEach((to, from, next) => {
  const auth = useAuthStore();

  // ✅ Token expired → force logout
  if (auth.isTokenExpired()) {
    auth.idleLogoutAction();
    if (to.meta.requiresAuth) return next("/login");
  }

  // ✅ Not logged in but route requires auth → redirect
  if (to.meta.requiresAuth && !auth.isLoggedIn) {
    return next("/login");
  }

  // ✅ Already logged in → block login/register routes
  if ((to.path === "/login" || to.path === "/register") && auth.isLoggedIn) {
    if (auth.userRole == 1)
      return next("/admin");
    else if (auth.userRole == 2)
      return next("/dashboard");
    else
      return next("/login");
  }

  // ✅ Check user type restrictions
  if (to.meta.requiresAuth && to.meta.userRole && auth.isLoggedIn) {
    const userRole = parseInt(auth.userRole); // ensure numeric
    const allowedUserTypes = to.meta.userRole.map((t) => parseInt(t)); // ensure numeric

    if (!allowedUserTypes.includes(userRole)) {
      console.warn(`🚫 Access denied for role ${userRole} on route ${to.path}`);
      if (auth.userRole == 1)
        return next("/admin");
      else if (auth.userRole == 2)
        return next("/dashboard");
      else
        return next("/login");
    }
  }

  next();
});

export default router;
