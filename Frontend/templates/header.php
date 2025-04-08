<?php
if (session_status() === PHP_SESSION_NONE) {
  session_start();
}

include_once($_SERVER['DOCUMENT_ROOT'] . "/H5/Frontend/templates/links.php");
include_once($_SERVER['DOCUMENT_ROOT'] . "/H5/Frontend/includes/auth.php");

// Handle logout
if ($_SERVER["REQUEST_METHOD"] == "POST" && isset($_POST['logout'])) {
  session_destroy();
  header("Location: " . $baseURL . "login");
  exit;
}
?>

<header class="bg-[#282828] text-white">
  <div class="mx-auto flex h-[70px] max-w-screen-xl items-center justify-between px-4 sm:px-6 lg:px-8">
    <!-- Logo -->
    <a class="flex items-center gap-2" href="<?= $baseURL . (is_logged_in() ? 'dashboard' : 'index.php'); ?>">
      <img class="w-10 h-10" src="<?= $baseURL; ?>images/Sentinel-logo.png" alt="Sentinel Logo">
      <span class="font-bold text-xl">Sentinel</span>
    </a>

    <!-- Desktop navigation -->
    <nav class="hidden md:flex items-center gap-6 text-sm">
      <?php if (is_logged_in()) : ?>
        <a class="hover:text-gray-300 transition" href="<?= $baseURL ?>dashboard">Dashboard</a>
        <a class="hover:text-gray-300 transition" href="<?= $baseURL ?>edit-profile">Edit Profile</a>
      <?php endif; ?>
    </nav>

    <!-- Auth buttons -->
    <div class="hidden md:flex items-center gap-4">
      <?php if (!is_logged_in()) : ?>
        <a class="rounded-md bg-teal-600 px-5 py-2.5 text-sm font-medium text-white transition hover:bg-teal-700" href="<?= $baseURL; ?>login">Login</a>
        <a class="rounded-md bg-gray-100 px-5 py-2.5 text-sm font-medium text-teal-600 transition hover:text-teal-600/75" href="<?= $baseURL; ?>signup">Signup</a>
      <?php else : ?>
        <form method="POST" action="<?= htmlspecialchars($_SERVER['PHP_SELF']); ?>">
          <button type="submit" name="logout" class="px-5 py-2.5 bg-red-600 text-white rounded-md text-sm font-medium hover:bg-red-700 transition">Logout</button>
        </form>
      <?php endif; ?>
    </div>

    <!-- Burger menu button -->
    <button id="mobileMenuToggle" class="md:hidden block bg-gray-100 p-2 rounded text-gray-800 hover:text-gray-600">
      <svg xmlns="http://www.w3.org/2000/svg" class="w-6 h-6" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
        <path stroke-linecap="round" stroke-linejoin="round" d="M4 6h16M4 12h16M4 18h16" />
      </svg>
    </button>
  </div>

  <!-- Mobile navigation (hidden by default) -->
  <div id="mobileMenu" class="hidden md:hidden px-4 pb-4">
    <ul class="space-y-2 text-sm mt-4">
      <?php if (is_logged_in()) : ?>
        <li><a href="<?= $baseURL ?>dashboard" class="block py-2 text-white hover:text-teal-300">Dashboard</a></li>
        <li><a href="<?= $baseURL ?>edit-profile" class="block py-2 text-white hover:text-teal-300">Edit Profile</a></li>
        <li>
          <form method="POST">
            <button type="submit" name="logout" class="w-full text-left py-2 text-red-400 hover:text-red-500">Logout</button>
          </form>
        </li>
      <?php else : ?>
        <li><a href="<?= $baseURL ?>login" class="block py-2 text-white hover:text-teal-300">Login</a></li>
        <li><a href="<?= $baseURL ?>signup" class="block py-2 text-white hover:text-teal-300">Signup</a></li>
      <?php endif; ?>
    </ul>
  </div>
</header>

<!-- Mobile menu script -->
<script>
  const toggleBtn = document.getElementById('mobileMenuToggle');
  const mobileMenu = document.getElementById('mobileMenu');

  toggleBtn.addEventListener('click', () => {
    mobileMenu.classList.toggle('hidden');
  });
</script>
