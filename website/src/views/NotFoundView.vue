<template>
  <main class="container text-center my-5">
    <div class="not-found-content">
      <h1 class="display-1 text-danger mb-4">404</h1>
      <h2 class="mb-3">Page Not Found</h2>
      <p class="lead mb-4">
        Oops! The page you are looking for doesn't exist or has been moved.
      </p>
      <RouterLink to="/" class="btn btn-primary btn-lg">
        Go Back Home
      </RouterLink>
    </div>
  </main>
</template>

<script setup lang="ts">
import { RouterLink } from 'vue-router'
</script>

<style scoped>
.not-found-content {
  padding: 3rem 1rem;
}
</style>