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
  { path: "/confirm-email", name: "ConfirmEmail", component: EmailConfirm },
  { path: "/reset-password", name: "ResetPassword", component: ResetPassword },
  { path: "/register", component: AuthPage },
  { path: "/dashboard", component: Dashboard, meta: { requiresAuth: true, userType: [0, 1, 2, 3] } },
  { path: "/pds", component: PersonalDataSheet, meta: { requiresAuth: true, userType: [1, 2, 3] } },
  { path: "/ts", component: Timesheet, meta: { requiresAuth: true, userType: [1, 2, 3] } },
  { path: "/hr", component: DashboardHR, meta: { requiresAuth: true, userType: [2, 3] } },
  { path: "/employee-id", component: EmployeeID, meta: { requiresAuth: true, userType: [2, 3] } },
  { path: "/details", component: DetailsPage, meta: { requiresAuth: true, userType: [1, 2, 3] } },
  { path: "/:pathMatch(.*)*", redirect: "/" },
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
    return next("/dashboard");
  }

  // ✅ Check user type restrictions
  if (to.meta.requiresAuth && to.meta.userType && auth.isLoggedIn) {
    const userType = parseInt(auth.userType); // ensure numeric
    const allowedUserTypes = to.meta.userType.map((t) => parseInt(t)); // ensure numeric

    if (!allowedUserTypes.includes(userType)) {
      console.warn(`🚫 Access denied for userType ${userType} on route ${to.path}`);
      return next("/dashboard");
    }
  }

  next();
});

export default router;
