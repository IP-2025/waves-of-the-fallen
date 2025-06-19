<template>
  <Navbar />
  <header class="hero-section text-white text-center d-flex align-items-center">
    <div class="container">
      <h1 class="display-3 fw-bold">
        Welcome to Waves of the Fallen
      </h1>
      <p class="lead mb-4">
        A multiplayer vampire survivor-like game. Team up, survive
        endless waves, and become legends!
      </p>
      <RouterLink
        to="/support"
        class="btn btn-lg btn-light fw-semibold px-4 py-2"
      >
        Contact Support
      </RouterLink>
    </div>
  </header>
  <main class="container my-5">
    <section class="row g-4">
      <div class="col-md-4">
        <div class="card h-100 shadow-sm">
          <div class="card-body">
            <h5 class="card-title">About the Game</h5>
            <p class="card-text">
              Waves of the Fallen is a fast-paced, multiplayer
              action game inspired by Vampire Survivors.
              Battle together against relentless hordes,
              unlock new abilities, and compete for high
              scores!
            </p>
          </div>
        </div>
      </div>
      <div class="col-md-4">
        <div class="card h-100 shadow-sm">
          <div class="card-body">
            <h5 class="card-title">Multiplayer Features</h5>
            <p class="card-text">
              Join friends. Coordinate your strategies,
              combine powers, and survive as long as you can.
              Only the strongest teams will endure the night!
            </p>
          </div>
        </div>
      </div>
      <div class="col-md-4">
        <div class="card h-100 shadow-sm">
          <div class="card-body">
            <h5 class="card-title">Get Involved</h5>
            <p class="card-text">
              Sign up, join the community, and help shape the
              future of Waves of the Fallen. Your feedback and
              participation make the game better for everyone!
            </p>
          </div>
        </div>
      </div>
    </section>
  </main>

  <!-- Game Description Section -->
  <section class="row justify-content-center my-5">
    <div class="col-lg-10">
      <div class="p-4 bg-light rounded-4 shadow-sm border">
        <h2 class="mb-3 text-primary fw-bold">Waves of the fallen - Full description</h2>
        <p class="lead">
          Waves of the Fallen is a fast-paced Vampire Survivors-like multiplayer survival game!
        </p>
        <p>
          Waves of the Fallen is a fast-paced Vampire Survivors-like multiplayer survival game!
          Team up with up to 4 players or fight alone and survive wave after wave of enemies. Collect gold, level up your character and weapons, and compete for the highest score!
          The game features simple, colorful graphics and a fun grind-focused gameplay loop — perfect for casual and hardcore players alike.
        </p>
        <h5 class="mt-4 mb-2 text-secondary">Features:</h5>
        <ul>
          <li><strong>Multiplayer</strong> — play solo or with up to 3 friends!</li>
          <li>Survive endless enemy waves</li>
          <li>Collect gold to upgrade characters and weapons</li>
          <li>Global leaderboard — compete for the highest score!</li>
          <li>Simple and fun visual style, accessible for all ages</li>
        </ul>
        <p class="mt-4">
          Whether you're looking for a quick session or a long grind — Waves of the Fallen will keep you coming back for more!
        </p>
        <p class="fw-semibold text-success">
          Download now and start climbing the leaderboards with your friends!
        </p>
      </div>
    </div>
  </section>

  <section class="row justify-content-center my-5">
    <div class="col-lg-10">
      <div class="p-4 bg-light rounded-4 shadow-sm border">
        <h2 class="mb-3 text-primary fw-bold">Waves of the fallen - Short descriptions</h2>
        <h5 class="mt-4 mb-2 text-secondary">Android:</h5>
        <p>
          Survive endless waves with friends! Level up, grind, and climb the leaderboard.
        </p>

        <h5 class="mt-4 mb-2 text-secondary">IOS:</h5>
        <p>
          Survive waves with friends
        </p>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
import { RouterLink } from 'vue-router'
import Navbar from '@/components/Navbar.vue'
</script>

<style scoped>
body {
  height: 100%;
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
.hero-section {
  background: linear-gradient(90deg, #1976d2 0%, #42a5f5 100%);
  min-height: 40vh;
  align-items: center;
  justify-content: center;
  padding: 3rem 0;
}
</style>
