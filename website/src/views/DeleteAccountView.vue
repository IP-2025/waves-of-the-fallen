<template>
  <Navbar />
  <main class="container my-5 delete-account-main">
    <div class="row justify-content-center">
      <div class="col-lg-6">
        <h1 class="mb-4 text-danger">Delete Account</h1>
        <p>
          Please enter your email and password to permanently delete your account.
          <br />
          <strong class="text-danger">Warning:</strong> This action cannot be undone.
        </p>
        <form class="delete-account-form p-4 rounded shadow-sm bg-light" @submit.prevent="handleDelete">
          <div class="mb-3">
            <label for="deleteEmail" class="form-label">Email Address</label>
            <input
              type="email"
              class="form-control"
              id="deleteEmail"
              v-model="email"
              placeholder="your@email.com"
              required
            />
          </div>
          <div class="mb-3">
            <label for="deletePassword" class="form-label">Password</label>
            <input
              type="password"
              class="form-control"
              id="deletePassword"
              v-model="password"
              placeholder="Enter your password"
              required
            />
          </div>
          <button type="submit" class="btn btn-danger" :disabled="loading">
            <span v-if="loading" class="spinner-border spinner-border-sm me-2"></span>
            Delete Account
          </button>
        </form>
        <div v-if="message" class="alert mt-4" :class="{'alert-success': success, 'alert-danger': !success}">
          {{ message }}
        </div>
      </div>
    </div>
  </main>
  <Footer />
</template>

<script setup lang="ts">
import { ref } from 'vue'
import Navbar from '@/components/Navbar.vue'
import Footer from '@/components/Footer.vue'
import backendApi from '@/api/backend-api'

const email = ref('')
const password = ref('')
const loading = ref(false)
const message = ref('')
const success = ref(false)

async function handleDelete() {
  loading.value = true
  message.value = ''
  success.value = false

  try {
    await backendApi.deleteAccount(email.value, password.value)
    message.value = 'Your account has been deleted. We are sorry to see you go.'
    success.value = true
    email.value = ''
    password.value = ''
  } catch (error: any) {
    if (error?.response?.data?.message) {
      message.value = error.response.data.message
    } else {
      message.value = 'Failed to delete account. Please check your credentials and try again.'
    }
    success.value = false
  } finally {
    loading.value = false
  }
}
</script>

<style scoped>
.delete-account-main {
  min-height: 60vh;
}
.delete-account-form {
  border: 1px solid #f5c2c7;
}
</style>