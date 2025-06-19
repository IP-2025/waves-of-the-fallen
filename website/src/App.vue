<template>
  <router-view />
</template>

<script setup lang="ts">
</script>

<style>
html, body, #app {
  height: 100%;
  margin: 0;
  padding: 0;
}

#app {
  display: flex;
  flex-direction: column;
  min-height: 100vh;
}

main.container {
  flex: 1 0 auto;
}

.footer {
  flex-shrink: 0;
}
</style>
