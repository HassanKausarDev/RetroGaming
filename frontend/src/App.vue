<template>
  <div>
    <nav class="navbar">
      <span class="navbar-brand">🎮 RetroGaming DB</span>
      <div class="nav-links">
        <RouterLink to="/consoles">Consoles</RouterLink>
        <RouterLink to="/manufacturers">Manufacturers</RouterLink>
        <RouterLink to="/map">Map View</RouterLink>
      </div>
    </nav>
    <main class="container">
      <RouterView />
    </main>
  </div>
</template>

<style>
* {
  box-sizing: border-box;
  margin: 0;
  padding: 0;
}

body {
  font-family: system-ui, sans-serif;
  background: #f0f2f5;
  color: #1a1a2e;
}

.navbar {
  background: #1a1a2e;
  color: white;
  padding: 1rem 2rem;
  display: flex;
  align-items: center;
  gap: 2rem;
}

.navbar-brand {
  font-size: 1.2rem;
  font-weight: bold;
  color: white;
}

.nav-links {
  display: flex;
  gap: 1.5rem;
}

.nav-links a {
  color: #aaa;
  text-decoration: none;
  font-size: 0.95rem;
}

.nav-links a.router-link-active {
  color: white;
  font-weight: 600;
}

.container {
  max-width: 1200px;
  margin: 2rem auto;
  padding: 0 1.5rem;
}
</style>