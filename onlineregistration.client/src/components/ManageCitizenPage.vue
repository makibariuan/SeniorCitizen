<template>
  <LeftMenu @navigate="onNavigate" class="leftMenu" />
  <Header class="header" />

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

  // --- Auth store & user data ---
  const auth = useAuthStore();
  const firstname = computed(() => auth.firstName ?? "User");
  const username = computed(() => auth.username ?? "User");
  const userType = computed(() => Number(auth.userType ?? 1));
  const statisticsData = ref([]);

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
  });

</script>

<style>

</style>
