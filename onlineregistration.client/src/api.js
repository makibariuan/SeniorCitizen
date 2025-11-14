// src/api.js
import axios from "axios";
import { useAuthStore } from "@/stores/auth";
import router from "@/router";

const api = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL,
//  baseURL: "https://npo-pssic.ddns.net/api/",
});

// Request interceptor
api.interceptors.request.use(
  (config) => {
    const auth = useAuthStore();

    // Skip auth endpoints
    if (!config.url.startsWith("/auth")) {
      // ⬅️ reject instead of throwing Cancel (avoids ERR_NETWORK)
      if (auth.isTokenExpired()) {
        auth.idleLogoutAction();
        return Promise.reject(new Error("Session expired"));
      }

      if (auth.token) {
        config.headers.Authorization = `Bearer ${auth.token}`;
      }
    }

    return config;
  },
  (error) => Promise.reject(error)
);

// Response interceptor
api.interceptors.response.use(
  (response) => response,
  (error) => {
    const auth = useAuthStore();

    if (error.response) {
      const status = error.response.status;
      const message = error.response.data?.message;

      if (status === 401) {
        if (message?.includes("Please confirm your email")) {
          // 🚀 redirect to a dedicated confirmation page
          router.push({ name: "ConfirmEmail" });
          return Promise.reject(new Error(message));
        }

        if (message?.includes("Session expired")) {
          console.warn(message);
          auth.idleLogoutAction();
          router.push("/login");
          return Promise.reject(new Error(message));
        }
      }
    }

    // Handle custom reject from request interceptor
    if (error.message === "Session expired") {
      console.warn(error.message);
      router.push("/login");
      return Promise.reject(error);
    }

    console.error("Unexpected Axios error:", error.message);
    return Promise.reject(error);
  }
);


export default api;
