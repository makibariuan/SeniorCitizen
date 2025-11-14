// src/stores/auth.js
import { defineStore } from "pinia";

export const useAuthStore = defineStore("auth", {
  state: () => ({
    token: localStorage.getItem("jwt") || null,
    idleLogout: false,
    logoutTimer: null, // keep reference so we don't create multiple timers
  }),

  getters: {
    isLoggedIn: (state) => !!state.token && !state.idleLogout,

    user(state) {
      if (!state.token) return null;
      try {
        const payload = decodeJwt(state.token);
        return {
          id: payload.uid || payload.id || null,
          username: payload.username || "",
          email: payload.email || "",
          firstName: payload.firstname || "",
          lastName: payload.lastname || "",
          //birthdate: payload.birthdate || "",
          //employeeID: payload.employeeID || "",
          userType: payload.usertype || "",
          userRole: payload.userrole || "",
        };
      } catch {
        return null;
      }
    },

    userId() {
      return this.user?.id || null;
    },
    username() {
      return this.user?.username || "";
    },
    email() {
      return this.user?.email || "";
    },
    //birthdate() {
    //  return this.user?.birthdate || "";
    //},
    //employeeID() {
    //  return this.user?.employeeID || "";
    //},
    firstName() {
      return this.user?.firstName || "";
    },
    lastName() {
      return this.user?.lastName || "";
    },
    userType() {
      return this.user?.userType || "";
    },
    userRole() {
      return this.user?.userRole || "";
    },
  },

  actions: {
    login(payload) {
      this.token = payload.token;
      localStorage.setItem("jwt", this.token);
      this.idleLogout = false;
      this.startAutoLogoutTimer();
    },

    logout() {
      this.token = null;
      this.idleLogout = false;
      localStorage.removeItem("jwt");
      if (this.logoutTimer) clearTimeout(this.logoutTimer);
      this.logoutTimer = null;
    },

    idleLogoutAction() {
       this.token = null;
       this.idleLogout = true;
       localStorage.removeItem("jwt");
       if (this.logoutTimer) clearTimeout(this.logoutTimer);
       this.logoutTimer = null;
      // ❌ no router.push() here → avoid circular import
    },

    isTokenExpired() {
       if (!this.token) return true;
       try {
         const payload = decodeJwt(this.token);
         return payload.exp * 1000 < Date.now();
       } catch {
         return true;
       }
    },

    checkSession() {
       if (this.isTokenExpired()) {
         this.idleLogoutAction();
         return false;
       }
       return true;
    },

    startAutoLogoutTimer() {
       if (!this.token) return;
       if (this.logoutTimer) clearTimeout(this.logoutTimer);

       try {
         const payload = decodeJwt(this.token);
         const expireAt = payload.exp * 1000;
         const delay = expireAt - Date.now();

         if (delay <= 0) {
           this.idleLogoutAction();
         } else {
           this.logoutTimer = setTimeout(() => this.idleLogoutAction(), delay);
         }
       } catch {
         this.idleLogoutAction();
       }
    },
  },
});

// --- helper ---
function decodeJwt(token) {
  const base64Url = token.split(".")[1];
  const base64 = base64Url.replace(/-/g, "+").replace(/_/g, "/");
  const padded = base64.padEnd(
    base64.length + (4 - (base64.length % 4)) % 4,
    "="
  );
  return JSON.parse(atob(padded));
}
