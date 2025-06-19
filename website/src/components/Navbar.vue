<script setup lang="ts">
import { RouterLink } from 'vue-router'
</script>

<template>
  <nav class="navbar navbar-expand-lg navbar-dark bg-primary main-navbar">
    <div class="container">
      <RouterLink class="navbar-brand" to="/">Waves of the Fallen</RouterLink>
      <button
        class="navbar-toggler"
        type="button"
        data-bs-toggle="collapse"
        data-bs-target="#navbarNavDropdown"
        aria-controls="navbarNavDropdown"
        aria-expanded="false"
        aria-label="Toggle navigation"
      >
        <span class="navbar-toggler-icon"></span>
      </button>
      <div class="collapse navbar-collapse" id="navbarNavDropdown">
        <ul class="navbar-nav ms-auto">
          <li class="nav-item">
            <RouterLink class="nav-link" to="/" exact-active-class="active">Home</RouterLink>
          </li>
          <li class="nav-item">
            <RouterLink class="nav-link" to="/support" exact-active-class="active">Support</RouterLink>
          </li>
          <li class="nav-item">
            <RouterLink class="nav-link" to="/terms" exact-active-class="active">Terms</RouterLink>
          </li>
          <li class="nav-item">
            <RouterLink class="nav-link" to="/privacy" exact-active-class="active">Privacy</RouterLink>
          </li>
        </ul>
      </div>
    </div>
  </nav>
</template>

<style scoped>
nav {
  width: 100%;
  font-size: 1rem;
  text-align: center;
  margin-top: 0;
  padding: 1rem 0;
  background-color: #0d6efd; /* Bootstrap primary */
  color: #fff;
  min-height: 56px;
  box-sizing: border-box;
}

nav a.router-link-exact-active {
  color: #fff;
}

nav a.router-link-exact-active:hover {
  background-color: transparent;
}

nav a {
  display: inline-block;
  padding: 0 1.5rem;
  border-left: 1px solid #ffffff33;
  color: #fff;
  text-decoration: none;
  line-height: 2.5rem;
  font-size: 1rem;
}

nav a:first-of-type {
  border: 0;
}


</style>
