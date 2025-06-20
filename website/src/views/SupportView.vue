<template>
<Navbar />
  <main class="container support-main">
    <div class="row justify-content-center">
      <div class="col-lg-8">
        <h1 class="mb-4">Support</h1>
        <p>
          Need help with <strong>Waves of the Fallen</strong>? If
          you have questions, issues, or feedback about our
          multiplayer vampire survivor-like game, please use the
          form below to contact us:
        </p>
        <form
          class="support-form p-4 rounded shadow-sm bg-light"
          action="mailto:wavesofthefallen@gmail.com"
          method="POST"
          enctype="text/plain"
        >
          <div class="mb-3">
            <label for="supportName" class="form-label">Name</label>
            <input
              type="text"
              class="form-control"
              id="supportName"
              name="name"
              placeholder="Your name"
              required
            />
          </div>
          <div class="mb-3">
            <label for="supportEmail" class="form-label">Email Address</label>
            <input
              type="email"
              class="form-control"
              id="supportEmail"
              name="email"
              placeholder="your@email.com"
              required
            />
          </div>
          <div class="mb-3">
            <label for="supportMessage" class="form-label">Message</label>
            <textarea
              class="form-control"
              id="supportMessage"
              name="message"
              rows="5"
              placeholder="How can we help?"
              required
            ></textarea>
          </div>
          <button type="submit" class="btn btn-primary">
            Send
          </button>
        </form>
        <div class="mt-5">
          <h2>Other Ways to Reach Us</h2>
          <ul>
            <li>
              Email:
              <a href="mailto:wavesofthefallen@gmail.com"
                >wavesofthefallen@gmail.com</a
              >
            </li>
            <!-- <li>Discord</strong>: <a href="https://discord.gg/yourgameinvite" target="_blank">Join our Discord server</a></li> -->
          </ul>
        </div>
      </div>
    </div>
  </main>
  <Footer />
</template>

<script setup lang="ts">
import Navbar from '@/components/Navbar.vue'
import Footer from '@/components/Footer.vue'
</script>

<style scoped>
</style>
