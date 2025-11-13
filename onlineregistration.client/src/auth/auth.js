import { useAuthStore } from "@/stores/auth";
import api from "@/api";

// Login with credentials
export async function login(credentials) {
  try {
    const response = await api.post("/auth/login", credentials);
    const { token } = response.data;

    const auth = useAuthStore();
    auth.setToken(token); // store token in memory (and optionally in localStorage if you want persistence)

    return true;
  } catch (err) {
    console.error("Login failed:", err.response?.data?.message || err.message);
    throw err;
  }
}

// Logout current session
export async function logout() {
  const auth = useAuthStore();

  try {
    await api.post("/auth/logout"); // optional
  } catch (err) {
    console.warn("Server logout failed:", err.message);
  }

  auth.clearAuth();
}

// Always load user from the token (decode claims)
export function loadUser() {
  const auth = useAuthStore();
  auth.loadUserFromToken();
  return auth.user;
}
