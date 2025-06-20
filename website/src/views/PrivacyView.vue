<template>
  <Navbar />
  <main class="container datenschutz-content">
    <h1 class="mb-4">Privacy Policy</h1>
    <p>
      Protecting your personal data is very important to us. We
      process your data exclusively in accordance with legal
      regulations (GDPR, TMG).
    </p>
    <h2>Contacting Us</h2>
    <p>
      If you contact us via the form on the website or by email, your
      provided data will be stored for six months for the purpose of
      processing the request and in case of follow-up questions. We do
      not share this data without your consent.
    </p>
    <h2>Your Rights</h2>
    <ul>
      <li>Right to information, correction, and deletion</li>
      <li>Restriction of processing</li>
      <li>Data portability</li>
      <li>Withdrawal and objection</li>
    </ul>
    <p>
      If you believe that the processing of your data violates data
      protection law or your data protection rights have otherwise
      been violated, you can lodge a complaint with the supervisory
      authority.
    </p>
    <h2>Game Data & Authentication</h2>
    <p>
      As "Waves of the Fallen" is a multiplayer vampire survivor-like
      game, we may collect and process gameplay data such as
      usernames, scores, and multiplayer session information. This
      data is used solely to provide and improve the gaming experience
      and is not shared with third parties.
    </p>
    <h2>Authentication & JWT</h2>
    <p>
      For authentication, "Waves of the Fallen" uses JSON Web Tokens
      (JWTs) that are generated and stored securely on your device or
      mobile phone. The JWT is saved in a file on your device and is
      used to verify your identity when accessing the game and its
      services. This token is never shared with third parties and is
      only used for authentication purposes within the game.
    </p>
    <h2>Contact</h2>
    <p>
      For questions about privacy, you can reach us at:<br />
      <a href="mailto:support@wavesofthefallen.de"
        >wavesofthefallen@gmail.com</a
      >
    </p>
  </main>
  <Footer />
</template>

<script setup lang="ts">
import Navbar from '@/components/Navbar.vue'
import Footer from '@/components/Footer.vue'
</script>

<style scoped>
</style>
